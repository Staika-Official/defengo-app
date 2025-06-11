using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.Util;
using Framework.UI;
using Framework.GameData.Defense;
using Beebyte.Obfuscator;
using Google;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace Framework.Login
{
    public enum AdminAccount
    {
        ROBERT,
        JUN,
        ADMIN
    }

    public class Admin
    {
        public string username;
        public string password;
        public string clientId;
    }

    public enum Provider
    {
        APPLE,
        GOOGLE,
        GUEST
    }

    public class LoginManager : MonoBehaviour
    {
        public static LoginManager Instance;

        public ButtonComponent button_AppleLogin;
        public ButtonComponent button_GoogleLogin;
        public ButtonComponent button_GuestLogin;

        private delegate void SuccessLogin(string token, string provide);

        private static SuccessLogin OnSuccessLogin;

        public bool isLogin = false;
        [SerializeField] private AdminAccount adminAccount;

        public JoinPopup joinPopup;
        [SerializeField] private RestorePopup restorePopup;
        [SerializeField] private NoticePopup noticePopup;
        [SerializeField] private SystemNoticePopup systemNoticePopup;

        [SerializeField] private GameObject socialLoginGroup;
        [SerializeField] private GameObject loadingGroup;
        [SerializeField] private TextMeshProUGUI text_LoadingMessage;
        [SerializeField] private GameObject splashPanel;

        [SerializeField] private Image image_progressBar;
        [SerializeField] private TextMeshProUGUI text_progress;
        [SerializeField] private TextMeshProUGUI text_VerText;
        public UpdatePopup updatePopup;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            switch (SystemInfo.deviceType)
            {
                case DeviceType.Unknown:
                    break;
                case DeviceType.Handheld:
                    break;
                case DeviceType.Console:
                    break;
                case DeviceType.Desktop:
                    Application.Quit();
                    break;
            }

#if UNITY_IOS
            button_AppleLogin.onPointerUp = () =>
            {
                OnClick_SocialLogin(Provider.APPLE);
            };

#elif UNITY_ANDROID
            button_AppleLogin.gameObject.SetActive(false);
#endif

            button_GoogleLogin.onPointerUp = () => { OnClick_SocialLogin(Provider.GOOGLE); };

            button_GuestLogin.onPointerUp = OnClick_GuestLogin;

            image_progressBar.fillAmount = 0;
            text_progress.text = "0 %";


            if (!joinPopup.gameObject.activeSelf)
            {
                joinPopup.gameObject.SetActive(true);
                joinPopup.PopUpSequence(false);
            }

            if (!updatePopup.gameObject.activeSelf)
            {
                updatePopup.gameObject.SetActive(true);
                updatePopup.PopUpSequence(false);
            }

            if (!systemNoticePopup.gameObject.activeSelf)
            {
                systemNoticePopup.gameObject.SetActive(true);
                systemNoticePopup.PopUpSequence(false);
            }

            if (!restorePopup.gameObject.activeSelf)
            {
                restorePopup.gameObject.SetActive(true);
                restorePopup.PopUpSequence(false);
            }

            if (!noticePopup.gameObject.activeSelf)
            {
                noticePopup.gameObject.SetActive(true);
                noticePopup.InActivePopup();
            }

            joinPopup.Initialize();
            updatePopup.Initialize();
            restorePopup.Initialize();
            noticePopup.Initialize();
            systemNoticePopup.Initialize();


            FirebaseInitializer.Instance.GetGameVersion();

            button_GuestLogin.gameObject.SetActive(true);
            socialLoginGroup.SetActive(false);
            text_VerText.text = $"Ver {Application.version}";
        }

        public void MaintenanceNotice(MaintenaceDetail data)
        {
            noticePopup.SetNoticePopup(data);
        }

        public void CheckToken()
        {
            NetworkManager.Instance.SetBaseUrl();

            Debug.Log("[Check Token]");

            string accessToken = SecurePlayerPrefs.GetString("accessToken");
            bool isFirst = string.IsNullOrEmpty(accessToken);
            SetLoginScreen(isFirst);
        }

        private void ProgressSequence(float progress)
        {
            image_progressBar.fillAmount = progress;
            text_progress.text = $"{(int)(progress * 100)} %";
        }

        public void RestorePopup()
        {
            restorePopup.PopUpSequence(true);
        }

        public void SetLoginScreen(bool isFirst)
        {
#if UNITY_IOS
            // Check the user's consent status.
            // If the status is undetermined, display the request request:
            var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                ATTrackingStatusBinding.RequestAuthorizationTracking();
            }
#endif

            if (isFirst)
            {
                socialLoginGroup.SetActive(true);
                loadingGroup.SetActive(false);
            }
            else
            {
                loadingGroup.SetActive(true);

                int idx = Random.Range(1, 23);

                text_LoadingMessage.text = LanguageManager.Instance.GetStringData($"UI_Loading_{idx}");

                DataManager.onProgress += ProgressSequence;
                socialLoginGroup.SetActive(false);

                if (!isLogin)
                {
                    isLogin = true;
                    _ = NetworkManager.Instance.GetUserResourceByTokenModule(() =>
                    {
                        _ = NetworkManager.Instance.GetUserGameData();
                    });

                    NetworkManager.Instance.OnFailedLoadUserData = SetErrorNotice;
                }
                else
                {
                    _ = NetworkManager.Instance.GetUserGameData();
                }
            }
        }

        private void SetErrorNotice(string data)
        {
            systemNoticePopup.SetNoticeMessage(data);
        }

        private void OnClick_GuestLogin()
        {
            string deviceInfo = Application.platform == RuntimePlatform.IPhonePlayer ? "IOS" : "AOS";
            isLogin = true;

            SignInGuestDTO signInGuestDTO = new()
            {
                clientId = "DEFENGO",
                fcmToken = SecurePlayerPrefs.GetString("fcmToken"),
                deviceId = SystemInfo.deviceUniqueIdentifier,
                deviceModel = SystemInfo.deviceModel,
                platform = deviceInfo,
                appVersion = Application.version,
                osVersion = SystemInfo.operatingSystem
            };

            string socialLoginData = JsonUtility.ToJson(signInGuestDTO);
            _ = NetworkManager.Instance.GuestLoginProcess(socialLoginData, false, SetErrorNotice);
        }

        [SkipRename]
        private void OnClick_SocialLogin(Provider provider)
        {
//            Debug.Log("OnClick_SocialLogin");
#if UNITY_EDITOR
            OnSuccessLogin = (_token, _provider) =>
            {
                isLogin = true;

                Admin data = new();
                switch (adminAccount)
                {
                    case AdminAccount.ROBERT:
                        data.clientId = "DEFENGO";
                        data.username = "defengotest2@gmail.com";
                        data.password = "admin";
                        break;
                    case AdminAccount.JUN:
                        data.clientId = "DEFENGO";
                        data.username = "jun";
                        data.password = "user";
                        break;
                    case AdminAccount.ADMIN:
                        data.clientId = "DEFENGO";
                        data.username = "admin";
                        data.password = "admin";
                        break;
                }

                string json = JsonUtility.ToJson(data);
                _ = NetworkManager.Instance.LoginProcess(json, true, SetErrorNotice);
            };
#endif
            isLogin = true;
//            Debug.Log("Is Login True !!!!!!!!");
            switch (provider)
            {
                case Provider.APPLE:
#if UNITY_EDITOR
                    OnSuccessLogin("asd", "asd");
#elif !UNITY_EDITOR && !UNITY_ANDROID
                        AppleLoginManager.Instance.OnSuccessLogin = (_token, _provider) =>
                        {
                            SetLoginData(_token, _provider);
                        };
                    AppleLoginManager.Instance.OnClick_AppleLogin();
#endif
                    break;
                case Provider.GOOGLE:
#if UNITY_EDITOR
                    OnSuccessLogin("asd", "GOOGLE");
#elif !UNITY_EDITOR
                    bool isConfig = GoogleSignIn.Configuration == null;

                    GoogleSignInManager.Instance.OnSuccessLogin = (_token, _provider) =>
                    {
                        SetLoginData(_token, _provider);
                    };
                    GoogleSignInManager.Instance.OnClick_GoogleLogin();
#endif
                    break;
            }
        }

        private void SetLoginData(string _token, string _provider)
        {
            Debug.Log("Set Login Data");
            string deviceInfo = Application.platform == RuntimePlatform.IPhonePlayer ? "IOS" : "AOS";
            SignInSocialDTO signInData = new()
            {
                clientId = "DEFENGO",
                fcmToken = SecurePlayerPrefs.GetString("fcmToken"),
                deviceId = SystemInfo.deviceUniqueIdentifier,
                deviceModel = SystemInfo.deviceModel,
                platform = deviceInfo,
                appVersion = Application.version,
                osVersion = SystemInfo.operatingSystem,
                //appVersion = Application.version,
                // clientId = "DEFENGO",
                // deviceId = SystemInfo.deviceUniqueIdentifier,
                // deviceModel = SystemInfo.deviceModel,
                // fcmToken = SecurePlayerPrefs.GetString("fcmToken"),
                // platform = deviceInfo,
                provider = _provider,
                token = _token,
                forceLogin = true
            };

            string socialLoginData = JsonUtility.ToJson(signInData);
            Debug.Log("[social Login Data] : " + socialLoginData);
            _ = NetworkManager.Instance.LoginProcess(socialLoginData, false, SetErrorNotice);
        }

        public void OnDestroy()
        {
            Instance = null;
        }

        public void OnMainTenance()
        {
            button_AppleLogin.SetInterectible(false);
            button_GoogleLogin.SetInterectible(false);
        }
    }
}