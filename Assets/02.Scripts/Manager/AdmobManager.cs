using UnityEngine;
using GoogleMobileAds.Api;
using Framework.Sound;
using Framework.UI;
using GoogleMobileAds.Mediation.UnityAds.Api;

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager Instance;
    public RewardedAd rewardedAd;

    public delegate void OnSuccessWatchAd();
    public static OnSuccessWatchAd onSuccessWatchAd;

    public delegate void OnSuccessFilledAd();
    public static OnSuccessFilledAd onSuccessFilledAd;

    public string adUnitId = "";

    private void Start()
    {
#if UNITY_IOS
        //Key
        //adUnitId = "ca-app-pub-7166239305649170/4879003536";
        //미디에이션 Key
        adUnitId = "ca-app-pub-3467340260114612/5880860486";
        
#elif UNITY_ANDROID
        //Key
        //adUnitId = "ca-app-pub-7166239305649170/8810509680";

        //미디에이션 Key
        adUnitId = "ca-app-pub-3467340260114612/1520031381";
#else
        adUnitId = "asd";
#endif

        if (Instance == null)
        {
            Instance = this;

            MobileAds.Initialize((initStatus) =>
            {
                PrintStatus("AdMob 초기화 완료");
                RequestAndLoadRewardedAd();
            });

            UnityAds.SetConsentMetaData("gdpr.consent", true);
            UnityAds.SetConsentMetaData("privacy.consent", true);

            MobileAds.RaiseAdEventsOnUnityMainThread = true;
        }
    }
    
    public void RequestAndLoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest();

        // create new rewarded ad instance
        RewardedAd.Load(adUnitId, adRequest,
            (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null || ad == null)
                {
                    PrintStatus("Rewarded ad failed to load: " + (loadError?.GetMessage() ?? "Unknown error"));
                    Invoke(nameof(RequestAndLoadRewardedAd), 5f); // 5초 후 재시도
                    return;
                }

                PrintStatus("Rewarded ad loaded.");
                onSuccessFilledAd?.Invoke();
                rewardedAd = ad;

                ad.OnAdFullScreenContentOpened += () =>
                {
                    SoundManager.Instance.audio_BGM.mute = true;
                    SoundManager.Instance.audio_SFX.mute = true;
                    PrintStatus("Rewarded ad opening.");
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    RequestAndLoadRewardedAd();
                };
                ad.OnAdImpressionRecorded += () =>
                {
                    PrintStatus("Rewarded ad recorded an impression.");
                };
                ad.OnAdClicked += () =>
                {
                    PrintStatus("Rewarded ad recorded a click.");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    PrintStatus("Rewarded ad failed to show with error: " +
                               error.GetMessage());
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Rewarded ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);
                    PrintStatus(msg);

                    string data = adValue.CurrencyCode;
                    PrintStatus(data);

                    PrintStatus("Get Response Id : " + ad.GetResponseInfo().GetResponseId());
                    
                    // Request Api
                };
            });
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                PrintStatus("Rewarded ad granted a reward: " + reward.Amount);
                SoundManager.Instance.audio_BGM.mute = false;
                SoundManager.Instance.audio_SFX.mute = false;
                onSuccessWatchAd?.Invoke();
            });
        }
        else
        {            
            PrintStatus("Rewarded ad is not ready yet.");

            // UI 메시지 추가 가능
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage("MSG_Exhausted_ADS");

            RequestAndLoadRewardedAd();
        }
    }    

    private void PrintStatus(string message)
    {
        Debug.Log(message);
    }
}
