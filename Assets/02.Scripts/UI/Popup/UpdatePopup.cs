using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.UI;

namespace Framework.Login
{
    public class UpdatePopup : PopupTemplate
    {
        public Button button_Confirm;

        //#if UNITY_ANDROID
        //    public string storeLink = "https://play.google.com/store/apps/details?id=io.staika.game.defengo&pli=1";
        //#elif UNITY_IOS
        //    public string storeLink = "https://apps.apple.com/us/app/defengo/id1671930331";
        //#endif

        public string storeLink;

        public override void ActivePopup()
        {

        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    storeLink = "https://apps.apple.com/us/app/defengo/id1671930331";
                    break;
                case RuntimePlatform.Android:
                    storeLink = "https://play.google.com/store/apps/details?id=io.staika.game.defengo&pli=1";
                    break;
                default:
                    storeLink = "https://play.google.com/store/apps/details?id=io.staika.game.defengo&pli=1";
                    break;
            }

            button_Confirm.onClick.AddListener(() =>
            {
                Application.OpenURL(storeLink);
            });

            button_Close.onClick.AddListener(() =>
            {
                PopUpSequence(false);
            });
        }
    }
}
