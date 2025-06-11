using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Util;
using TMPro;
using UnityEngine.Events;

namespace Framework.UI
{
    public class SigninConfirmPopup : PopupTemplate
    {
        public TextMeshProUGUI text_NoticeMessage;
        public Button button_confirm;
        public UnityAction Integration;
        public UnityAction Cancel;
        public string jsonData;

        public void SetNoticeMessage(string notice)
        {
            text_NoticeMessage.text = LanguageManager.Instance.GetStringData(notice);
            ActivePopup();
        }

        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public void SocialIntegraion(string json)
        {
            
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                Cancel?.Invoke();
                InActivePopup();
            });

            button_confirm.onClick.AddListener(() =>
            {
                Integration?.Invoke();
                InActivePopup();
            });
        }
    }
}
