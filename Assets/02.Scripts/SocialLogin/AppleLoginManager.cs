using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using UnityEngine.Events;
using Framework.UI;


namespace Framework.Login
{
    public class AppleLoginManager : MonoBehaviour
    {
        public UnityAction<string, string> OnSuccessLogin;

        //#if UNITY_IOS
        private IAppleAuthManager _appleAuthManager;

        public static AppleLoginManager Instance;

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
#if UNITY_IOS
            if (AppleAuthManager.IsCurrentPlatformSupported)
            {
                var deserializer = new PayloadDeserializer();
                this._appleAuthManager = new AppleAuthManager(deserializer);
            }
#endif
        }

        private void Update()
        {
#if UNITY_IOS
            if (this._appleAuthManager != null)
            {
                this._appleAuthManager.Update();
            }
#endif
        }

        public void OnClick_AppleLogin()
        {
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

            this._appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    var appleIdCredential = credential as IAppleIDCredential;
                    string identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);
                    string authCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode, 0, appleIdCredential.AuthorizationCode.Length);

                    //if(string.IsNullOrEmpty(appleIdCredential.Email) || appleIdCredential.Email.Contains("@privaterelay.appleid.com"))
                    //{
                    //    SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                    //    popup.SetNoticeMessage("UI_Email_Masking");

                    //    Debug.Log($"Anonymous AppleEmail token :{identityToken}, authCode :{authCode}");
                    //}
                    //else
                    //{
                    OnSuccessLogin?.Invoke(authCode, "APPLE");
                    //}

                },
                error =>
                {
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                });
        }
        //#endif
    }


}