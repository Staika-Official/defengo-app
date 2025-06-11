using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.RemoteConfig;
using System.Threading.Tasks;
using System;
using Framework.Util;
using Framework.GameData.Defense;
using Google;
using Firebase.Extensions;
using Framework.Network;
using Framework.Login;
using System.Net;

[Serializable]
public class MaintenaceDetail
{
    public int maintenanceGrade;
    public string titleEn;
    public string titleKr;
    public string descriptionKr;
    public string descriptionEn;
    public string fromDateTime;
    public string toDateTime;

}

[Serializable]
public class MinimumVersionInfo
{
    public string aosVersion;
    public string iosVersion;
    public bool isPatch;
    public bool isUpdate;
}

[Serializable]
public class ForceChangeServer
{
    public string aosVersion;
    public string iosVersion;
}

public class FirebaseInitializer : MonoBehaviour
{
    private FirebaseApp app;

    public static FirebaseInitializer Instance;
    private string versionInfo;
    private string versionKey;
    private string maintenanceDetailKey;
    private string forceChangeServerKey;
    private string whiteListIPKey;
    private List<string> whiteListIP = new List<string>();
    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            versionInfo = Application.version;

#if !UNITY_EDITOR
            GetFCMToken();
#endif
        }
    }

    public void GetGameVersion()
    {
        InitState();
        FetchDataAsync();
    }

    private void InitState()
    {
        Debug.Log("Application State : " + NetworkManager.Instance.applicationState);

        maintenanceDetailKey = NetworkManager.Instance.applicationState switch
        {
            ApplicationState.DEV => "maintenanceDetailStage",
            ApplicationState.STAGE => "maintenanceDetailStage",
            ApplicationState.PRODUCTION => "maintenanceDetail",
            _ => ""
        };

        versionKey = NetworkManager.Instance.applicationState switch
        {
            ApplicationState.DEV => "stageMinimumVersion",
            ApplicationState.STAGE => "stageMinimumVersion",
            ApplicationState.PRODUCTION => "minimumVersion",
            _ => "stageMinimumVersion"
        };

        forceChangeServerKey = "forceChangeServer";

        whiteListIPKey = NetworkManager.Instance.applicationState switch
        {
            ApplicationState.DEV => "whiteListIP",
            ApplicationState.STAGE => "whiteListIP",
            ApplicationState.PRODUCTION => "whiteListIP",
            _ => "whiteListIP"
        };
    }

    private async void FetchDataAsync()
    {
        Debug.Log("Fetching data...");
        System.Threading.Tasks.Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        await fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        // Fetch successful. Parameter values must be activated to use.

        remoteConfig.ActivateAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");

            IDictionary<string, ConfigValue> values = remoteConfig.AllValues;

            values.TryGetValue(whiteListIPKey, out ConfigValue whiteListIPValue);
            if (whiteListIPValue.StringValue != null)
            {
                whiteListIP = new List<string>(whiteListIPValue.StringValue.Split(';'));
            }
            

            values.TryGetValue(maintenanceDetailKey, out ConfigValue maintenanceDatail);
            if (MaintenanceGradeCheck(maintenanceDatail.StringValue) == false) // 점검중
                return;

            values.TryGetValue(versionKey, out ConfigValue value);
            if (VersionCheck(value.StringValue) == false) // 업데이트
                return;
                
            // 강제 서버 이동 순서 중요함
            values.TryGetValue(forceChangeServerKey, out ConfigValue forceChangeServer);
            ForceServerChange(forceChangeServer.StringValue);
            
            LoginManager.Instance.CheckToken();
        });
    }

    private bool MaintenanceGradeCheck(string data)
    {
        MaintenaceDetail maintenaceDetail = JsonUtility.FromJson<MaintenaceDetail>(data);

        switch (maintenaceDetail.maintenanceGrade)
        {
            case 1:
                if (CheckWhiteListIP() == false)
                {
                    LoginManager.Instance.MaintenanceNotice(maintenaceDetail);
                    return false;
                }
                break;
            default:
                break;
        }
        return true;
    }

    private bool CheckWhiteListIP()
    {
        string externalip = new WebClient().DownloadString("https://api.ipify.org");
        
        return whiteListIP.Contains(externalip);
    }

    private bool VersionCheck(string data)
    {
        if (!string.IsNullOrEmpty(data))
        {
            MinimumVersionInfo info = JsonUtility.FromJson<MinimumVersionInfo>(data);

            string platformVersion = Application.platform switch
            {
                RuntimePlatform.Android => info.aosVersion,
                RuntimePlatform.IPhonePlayer => info.iosVersion,
                _ => info.aosVersion
            };

            int temp = CompareVersion(versionInfo, platformVersion);
            //Debug.Log(temp);
            bool isCurrentVersion = temp == 1 || temp == 0;

            if (!isCurrentVersion)
            {
                if (info.isUpdate)
                {
                    LoginManager.Instance.updatePopup.PopUpSequence(true);
                    return false;
                }
                else
                {
                    Debug.Log("Patch");
                }
            }

            return true;
        }

        return false;
    }

    private void ForceServerChange(string data)
    {
        if (!string.IsNullOrEmpty(data))
        {
            ForceChangeServer info = JsonUtility.FromJson<ForceChangeServer>(data);

            string platformVersion = Application.platform switch
            {
                RuntimePlatform.Android => info.aosVersion,
                RuntimePlatform.IPhonePlayer => info.iosVersion,
                _ => info.aosVersion
            };

            int temp = CompareVersion(versionInfo, platformVersion);
            //Debug.Log(temp);
            bool isCurrentVersion = temp == 0; // 같은 것 만

            if (isCurrentVersion)
            {
                NetworkManager.Instance.applicationState = ApplicationState.STAGE;
            }
        }
    }

    /// <summary>
    /// Version State : currentVersion 
    /// </summary>
    /// <param name="currentVersion"></param>
    /// <param name="minimumVersion"></param>
    /// <returns></returns>
    public int CompareVersion(string currentVersion, string minimumVersion)
    {
        string[] v1a = currentVersion.Split('.', StringSplitOptions.RemoveEmptyEntries);
        string[] v2a = minimumVersion.Split('.', StringSplitOptions.RemoveEmptyEntries);

        if (v1a.Length == 0 || v2a.Length == 0) return 100;
        int maxLength = v1a.Length > v2a.Length ? v1a.Length : v2a.Length;

        for (int i = 0; i < maxLength; i++)
        {
            int v1i = 0, v2i = 0;
            if (v1a.Length > i && !int.TryParse(v1a[i], out v1i)) return 100;
            if (v2a.Length > i && !int.TryParse(v2a[i], out v2i)) return 100;
            if (v1i != v2i) return v1i.CompareTo(v2i);
        }
        return 0;
    }

    public void GetFCMToken()
    {
        Debug.Log("Get FCM TOKEN");
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;

                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
            }
            else
            {
                Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });

        StartCoroutine(GameStart());
    }

    public IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1f);
        SceneLoadManager.Instance.SwitchingScene(1);
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
        SecurePlayerPrefs.SetString("fcmToken", token.Token);
        Debug.Log("[Get FCM Token]");
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        if (e.Message.Notification.Title == "MAINTENANCE")
        {
            if (UserInfoManager.Instance.provider == "GOOGLE")
            {
                GoogleSignIn.DefaultInstance.SignOut();
            }
            _ = NetworkManager.Instance.Logout();
        }

        if (e.Message.Notification.Title == "QUIT")
        {
            Application.Quit();
        }
    }
}

