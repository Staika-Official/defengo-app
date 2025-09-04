using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Sound;
using Framework.Network;
using Framework.Util;
using Framework.GameData.Defense;
using Google;
using Framework.Login;

namespace Framework.UI
{
    public class SettingPopup : PopupTemplate
    {
        public ScrollRect scrollRect;
        public Slider slider_SFXVolume;
        public Slider slider_BGMVolume;
        public TextMeshProUGUI text_SFXVolume;
        public TextMeshProUGUI text_BGMVolume;
        public TextMeshProUGUI text_VerText;

        public ButtonComponent button_CopyUID;
        public TextMeshProUGUI text_UID;

        public DiscordEvent discordEvent;

        public Button button_Korean;
        public Button button_English;
        public Button button_LogOut;
        public Button button_TermsOfUse;
        public Button button_Termination;
        public Button button_Exit;
        public Button button_Discord;
        public Button button_Google;
        public Button button_Apple;

        public RectTransform rect_AccountGroup;
        public GameObject koreanGroup;
        public GameObject englishGroup;
        public GameObject integrationGroup;
        public GameObject warningMessage;
        public RectTransform rect_ScrollView;



        public override void ActivePopup()
        {
            discordEvent.SetEventInfo(UserInfoManager.Instance.discordConnect);
            rect_ScrollView.anchoredPosition = Vector2.zero;
            bool isSocialLogin = UserInfoManager.Instance.provider == "GUEST";

            // scrollRect.enabled = !isSocialLogin;
            // integrationGroup.SetActive(!isSocialLogin);
            SetGuestLoginProperty(isSocialLogin);
            PopUpSequence(true);
        }

        public void SetGuestLoginProperty(bool isGuest)
        {
            if (isGuest)
            {
                rect_AccountGroup.sizeDelta = new Vector2(808, 640);
                button_Termination.gameObject.SetActive(true);

                RectTransform rect = button_Termination.transform as RectTransform;
                rect.anchoredPosition = new Vector2(190, -545f);

                text_VerText.rectTransform.anchoredPosition = new Vector2(0, -1530);
            }
            else
            {
                rect_AccountGroup.sizeDelta = new Vector2(808, 260);
                button_Termination.gameObject.SetActive(true);
                RectTransform rect = button_Termination.transform as RectTransform;
                rect.anchoredPosition = new Vector2(190, -188f);

                text_VerText.rectTransform.anchoredPosition = new Vector2(0, -1150);
            }
            integrationGroup.SetActive(isGuest);
            warningMessage.SetActive(isGuest);
        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            text_VerText.text = $"Ver {Application.version}";

            float sfxVolume = SoundManager.Instance.audio_SFX.volume;
            float bgmVolume = SoundManager.Instance.audio_BGM.volume;

            slider_SFXVolume.value = sfxVolume;
            slider_BGMVolume.value = bgmVolume;

            text_BGMVolume.text = $"{(int)(bgmVolume * 100)}%";
            text_SFXVolume.text = $"{(int)(sfxVolume * 100)}%";

            slider_SFXVolume.onValueChanged.AddListener((sfxVol) => OnValueChangeSFX(sfxVol));
            slider_BGMVolume.onValueChanged.AddListener((bgmVol) => OnValueChangeBGM(bgmVol));

            text_UID.text = UserInfoManager.Instance.userId;

            bool isEnglish = LanguageManager.Instance.nationalKey == "en";

            koreanGroup.SetActive(!isEnglish);
            englishGroup.SetActive(isEnglish);

            bool isApple = Application.platform == RuntimePlatform.IPhonePlayer;

            button_Apple.gameObject.SetActive(isApple);

            button_Close.onClick.AddListener(() => OnClick_Close());
            button_LogOut.onClick.AddListener(() => OnClick_Logout());

            button_Apple.onClick.AddListener(() => OnClick_SocialLogin(Provider.APPLE));
            button_Google.onClick.AddListener(() => OnClick_SocialLogin(Provider.GOOGLE));

            button_TermsOfUse.onClick.AddListener(() => PopupManager.Instance.GetPopUp<TermsPopup>("terms").ActivePopup());
            button_Termination.onClick.AddListener(() => PopupManager.Instance.GetPopUp<WithdrawalPopup>("withdrawal").PopUpSequence(true));

            button_CopyUID.button.onClick.AddListener(() =>
            {
                GUIUtility.systemCopyBuffer = text_UID.text;
                SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                popup.SetNoticeText(LanguageManager.Instance.GetStringData("UI_Success"));
            });
        }

        public void OnClick_Logout()
        {
            if (UserInfoManager.Instance.provider.Contains("GOOGLE"))
            {
                Debug.Log("Google Sign Out");
                GoogleSignIn.DefaultInstance.SignOut();
            }
            else
            {

            }
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            _ = NetworkManager.Instance.Logout();
        }

        public void OnClick_SocialLogin(Provider provider)
        {
            if (UserInfoManager.Instance.userState.userStatus == "SOCIAL")
            {
                Debug.Log("Social Email is Already exist");
                return;
            }

            switch (provider)
            {
                case Provider.APPLE:
#if UNITY_IOS
                    AppleLoginManager.Instance.OnSuccessLogin = (_token, _provider) =>
                    {
                        SetLoginData(_token, _provider);
                    };
                    AppleLoginManager.Instance.OnClick_AppleLogin();
#endif
                    break;
                case Provider.GOOGLE:
                    bool isConfig = GoogleSignIn.Configuration == null;
                    GoogleSignInManager.Instance.OnSuccessLogin = (_token, _provider) =>
                    {
                        Debug.Log("Google Sign in Callback");
                        SetLoginData(_token, _provider);
                    };
                    GoogleSignInManager.Instance.OnClick_GoogleLogin();
                    break;
                case Provider.GUEST:
                    break;
                default:
                    break;
            }
        }

        public void OnClick_Close()
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            PopUpSequence(false);
        }

        public void OnValueChangeBGM(float value)
        {
            SoundManager.Instance.audio_BGM.volume = value;
            text_BGMVolume.text = $"{(int)(value * 100)}%";
            SecurePlayerPrefs.SetFloat("bgmVolume", slider_BGMVolume.value);
        }

        public void OnValueChangeSFX(float value)
        {
            SoundManager.Instance.audio_SFX.volume = value;
            text_SFXVolume.text = $"{(int)(value * 100)}%";
            SecurePlayerPrefs.SetFloat("sfxVolume", slider_SFXVolume.value);
        }

        public void SetLoginData(string _token, string _provider)
        {
            string deviceInfo = Application.platform == RuntimePlatform.IPhonePlayer ? "IOS" : "AOS";
            GuestIntegration guestIntegration = new()
            {
                clientId = "DEFENGO",
                provider = _provider,
                platform = deviceInfo,
                token = _token
            };

            string socialLoginData = JsonUtility.ToJson(guestIntegration);

            _ = NetworkManager.Instance.SocialIntegrationCheck(socialLoginData, _token, _provider, SocialIntegrationSuccess, SocialIntegrationFailed);
        }

        public void SocialIntegrationSuccess(string json, string _token, string _provider)
        {
            //Debug.Log(json);
            GuestIntegrationCheck guestIntegrationCheck = JsonUtility.FromJson<GuestIntegrationCheck>(json);
            string deviceInfo = Application.platform == RuntimePlatform.IPhonePlayer ? "IOS" : "AOS";
            if (guestIntegrationCheck.isAvailable)
            {
                SigninConfirmPopup signinConfirm = PopupManager.Instance.GetPopUp<SigninConfirmPopup>("signinConfirm");
                signinConfirm.SetNoticeMessage("MSG_Switching_Accouunt");

                signinConfirm.Integration = () =>
                {
                    SignInSocialDTO signInData = new()
                    {
                        appVersion = Application.version,
                        clientId = "DEFENGO",
                        deviceId = SystemInfo.deviceUniqueIdentifier,
                        deviceModel = SystemInfo.deviceModel,
                        fcmToken = SecurePlayerPrefs.GetString("fcmToken"),
                        platform = deviceInfo,
                        provider = _provider,
                        token = _token,
                    };
                    string data = JsonUtility.ToJson(signInData);
                    _ = NetworkManager.Instance.SocialIntegrationProcess(data, IntegrationSuccess, IntegrationFailed);
                };

                signinConfirm.Cancel = () =>
                {
                    if (UserInfoManager.Instance.provider.Contains("GOOGLE"))
                    {
                        GoogleSignInManager.Instance.OnSignOut();
                    }
                };
            }
            else
            {
                SocialIntegrationFailed("MSG_Switching_Accouunt_Fail_1");

                if (UserInfoManager.Instance.provider.Contains("GOOGLE"))
                {
                    GoogleSignInManager.Instance.OnSignOut();
                }
            }
        }

        public void IntegrationSuccess()
        {
            SystemNoticePopup systemNoticePopup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            systemNoticePopup.SetNoticeMessage("MSG_Switching_Account_Success");
        }

        public void IntegrationFailed(string message)
        {
            SystemNoticePopup systemNoticePopup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            systemNoticePopup.SetNoticeMessage("MSG_Switching_Accouunt_Fail_2");
        }

        public void SocialIntegrationFailed(string failed)
        {
            SystemNoticePopup systemNoticePopup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            systemNoticePopup.SetNoticeMessage(failed);
        }
    }
}
