using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gpm.WebView;
using DG.Tweening;
using System.Web;
using Framework.UI;

namespace Framework.Util
{
    public class Webview : MonoBehaviour
    {
        // Start is called before the first frame update
        public static Webview Instance;

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void ShowNoticePopup(string url)
        {
            GpmWebView.ShowUrl(
              url,
              new GpmWebViewRequest.Configuration()
              {
                  style = GpmWebViewStyle.POPUP,
                  size = new GpmWebViewRequest.Size
                  {
                      hasValue = true,
                      width = (int)(Screen.width * 0.8f),
                      height = (int)(Screen.height * 0.8f)
                  },
                  position = new GpmWebViewRequest.Position
                  {
                      hasValue = true,
                      x = 0,
                      y = 0
                  },

                  orientation = GpmOrientation.UNSPECIFIED,
                  isClearCookie = true,
                  isClearCache = true,
                  isNavigationBarVisible = true,
                  isCloseButtonVisible = true,
#if UNITY_IOS
                    contentMode = GpmWebViewContentMode.RECOMMENDED,
                  isMaskViewVisible = false,
#endif
                },
                null, null);
        }

        public void ShowWebViewPopupExit(string url)
        {
            GpmWebView.ShowUrl(
                url,
                new GpmWebViewRequest.Configuration()
                {
                    //style = GpmWebViewStyle.FULLSCREEN,
                    style = GpmWebViewStyle.POPUP,
                    position = new GpmWebViewRequest.Position
                    {
                        hasValue = true,
                        x = (int)(Screen.width * 0.1f),
                        y = (int)(Screen.height * 0.1f)
                    },
                    size = new GpmWebViewRequest.Size
                    {
                        hasValue = true,
                        width = (int)(Screen.width * 0.8f),
                        height = (int)(Screen.height * 0.8f)
                    },

                    //                    orientation = GpmOrientation.UNSPECIFIED,
                    //                    isClearCookie = true,
                    //                    isClearCache = true,
                    //                    isNavigationBarVisible = true,
                    //                    isCloseButtonVisible = true,
                    //                    isBackButtonVisible = true,
                    //                    backgroundColor = "#000000",
                    //                    navigationBarColor = "#000000",
                    //#if UNITY_IOS
                    //                    contentMode = GpmWebViewContentMode.RECOMMENDED,
                    //                    isMaskViewVisible = true,
                    //#endif
                    //style = GpmWebViewStyle.FULLSCREEN,
                    //orientation = GpmOrientation.UNSPECIFIED,
                    //isClearCookie = true,
                    //isClearCache = true,
                    //backgroundColor = "#FFFFFF",
                    //isNavigationBarVisible = true,
                    //navigationBarColor = "#4B96E6",
                    //title = "The page title.",
                    //isBackButtonVisible = true,
                    //isForwardButtonVisible = true,
                    //isCloseButtonVisible = true,
                    //supportMultipleWindows = true,
#if UNITY_IOS
                    contentMode = GpmWebViewContentMode.MOBILE
#endif
                },
                OnCallback,
                new List<string>()
                {
                    "defengo"
                });
        }

        public void ShowWebViewPopup(string url)
        {
            GpmWebView.ShowUrl(
                url,
                new GpmWebViewRequest.Configuration()
                {
#if UNITY_IOS
                    style = GpmWebViewStyle.POPUP,
                    size = new GpmWebViewRequest.Size
                    {
                        hasValue = true,
                        width = (int)(Screen.width),
                        height = (int)(Screen.height)
                    },
                    position = new GpmWebViewRequest.Position
                    {
                        hasValue = true,
                        x = 0,
                        y = 0
                    },

#elif UNITY_ANDROID
                    style = GpmWebViewStyle.FULLSCREEN,
#endif
                    orientation = GpmOrientation.PORTRAIT,
                    isClearCookie = true,
                    isClearCache = true,
                    isNavigationBarVisible = false,
                    isCloseButtonVisible = false,
#if UNITY_IOS
                    contentMode = GpmWebViewContentMode.RECOMMENDED,
                    isMaskViewVisible = false,
#endif
            },
                OnCallback,
                new List<string>()
                {
                    "defengo"
                });
        }

        private void OnCallback(
        GpmWebViewCallback.CallbackType callbackType,
        string data,
        GpmWebViewError error)
        {
            Debug.Log("OnCallback: " + callbackType);
            switch (callbackType)
            {
                case GpmWebViewCallback.CallbackType.Open:
                    if (error != null)
                    {
                        Debug.LogFormat("Fail to open WebView. Error:{0}", error);
                    }
                    break;
                case GpmWebViewCallback.CallbackType.Close:
                    if (error != null)
                    {
                        Debug.LogFormat("Fail to close WebView. Error:{0}", error);
                    }
                    break;
                case GpmWebViewCallback.CallbackType.PageStarted:
                    if (string.IsNullOrEmpty(data) == false)
                    {
                        Debug.LogFormat("PageStarted Url : {0}", data);
                    }
                    break;
                case GpmWebViewCallback.CallbackType.PageLoad:
                    if (string.IsNullOrEmpty(data) == false)
                    {
                        Debug.LogFormat("Loaded Page:{0}", data);
                        //GpmWebView.ExecuteJavaScript($"test();");
                    }
                    break;
                case GpmWebViewCallback.CallbackType.MultiWindowOpen:
                    Debug.Log("MultiWindowOpen");
                    break;
                case GpmWebViewCallback.CallbackType.MultiWindowClose:
                    Debug.Log("MultiWindowClose");
                    break;
                case GpmWebViewCallback.CallbackType.Scheme:
                    if (error == null)
                    {
                        if (data.Contains("REQUEST_TOKEN") == true)
                        {
                            Debug.Log("Requesttoken");
                            string token = SecurePlayerPrefs.GetString("accessToken");
                            GpmWebView.ExecuteJavaScript($"setToken('{token}');");
                        }
                        else if (data.Contains("CLOSE_WEBVIEW") == true)
                        {
                            WalletScreen.Instance.SetUserWalletBalance();

                            GpmWebView.Close();
                        }
                        else if (data.Contains("COPY_ADDRESS") == true)
                        {
                            Debug.Log("Copy Address");
                            string parsedUrl = data.Split('?')[1];
                            var paramCollection = HttpUtility.ParseQueryString(parsedUrl);
                            string copyAddress = paramCollection["address"];
                            Debug.Log(copyAddress);
                            GUIUtility.systemCopyBuffer = copyAddress;
                            
                        }
                        else if(data.Contains("ESCAPE_TO_BROWSER")== true)
                        {
                            Debug.Log("Escape Browser");
                            string[] parsedUrls = data.Split('?');
                            var paramCollection = HttpUtility.ParseQueryString(parsedUrls[1]);
                            var paramCollections = HttpUtility.ParseQueryString(parsedUrls[2]);
                            string webUrl = paramCollection["address"];
                            string cluster = paramCollections["cluster"];

                            Debug.Log(cluster);
                            Debug.Log(webUrl);
                            Application.OpenURL(webUrl + "?cluster=" + cluster);
                        }
                        else if(data.Contains("ESCAPE_TO_STAIKA") == true)
                        {
                            Debug.Log("Escape Staika");
                            Debug.Log(data);
                            string[] parsedUrls = data.Split('?');
                            var paramCollection = HttpUtility.ParseQueryString(parsedUrls[1]);
                            var paramCollections = HttpUtility.ParseQueryString(parsedUrls[2]);
                            string webUrl = paramCollection["address"];
                            string route = paramCollections["route"];

                            Debug.Log(route);
                            Debug.Log(webUrl);
                            Application.OpenURL(webUrl + "?route=" + route);
                        }
                    }
                    else
                    {
                        Debug.Log(string.Format("Fail to custom scheme. Error:{0}", error));
                    }
                    break;
                case GpmWebViewCallback.CallbackType.GoBack:
                    Debug.Log("GoBack");
                    break;
                case GpmWebViewCallback.CallbackType.GoForward:
                    Debug.Log("GoForward");
                    break;
                case GpmWebViewCallback.CallbackType.ExecuteJavascript:
                    Debug.LogFormat("ExecuteJavascript data : {0}, error : {1}", data, error);
                    break;
#if UNITY_ANDROID
                case GpmWebViewCallback.CallbackType.BackButtonClose:
                    Debug.Log("BackButtonClose");
                    break;
#endif
            }

        }
    }
}