using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Google;
using UnityEngine.Events;
namespace Framework.Login
{
    public class GoogleSignInManager : MonoBehaviour
    {
        public static GoogleSignInManager Instance;
        public UnityAction<string, string> OnSuccessLogin;

#if UNITY_IOS
        public readonly string webClientId = "701023223608-9scd334cpgr6bnuanc7h8prbi6gg3ol5.apps.googleusercontent.com";
#elif UNITY_ANDROID
        public readonly string webClientId = "701023223608-trjqpv0g24ahpe2i7qf78dsvlm6b8dh5.apps.googleusercontent.com";
                                             //"492530739614-vv1a3l374hdl0ktce7qutuobk7catpsh.apps.googleusercontent.com";
#elif !UNITY_IOS && !UNITY_ANDROID
        public readonly string webClientId = "496812139168-up33dsm6t0g1mfk3fv6ten8mvaj4sua9.apps.googleusercontent.com";
#endif
        private GoogleSignInConfiguration configuration;
        
        private bool isLogin = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            //InitConfig();
        }

        public void InitConfig()
        {
            configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true
            };
        }

        public void OnClick_GoogleLogin()
        {
            if(isLogin) return;

            isLogin = true;
            
            InitConfig();
            
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            GoogleSignIn.Configuration.RequestEmail = true;

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
              OnAuthenticationFinished);
        }

        public void OnSignOut()
        {
            GoogleSignIn.DefaultInstance.SignOut();
        }

        internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
        {
            if (task.IsFaulted)
            {
                using IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    Debug.Log("[Login Error]");
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    OnSignOut();
                    isLogin = false;
                }
                else
                {
                    Debug.Log("[Task is Faulted]");
                    OnSignOut();
                    isLogin = false;
                }
            }
            else if (task.IsCanceled)
            {
                Debug.Log("[Task is Canceled]");
                OnSignOut();
                isLogin = false;
            }
            else
            {
                Debug.Log("[Login Success]");
                string token = task.Result.IdToken;
                Framework.GameData.Defense.UserInfoManager.Instance.provider = "GOOGLE";
                OnSuccessLogin?.Invoke(token, "GOOGLE");
                isLogin = false;
            }
        }
    }
}