using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Framework.UI
{
    public class AlertPopup : PopupTemplate
    {
        public TextMeshProUGUI text_AlertMessage;

        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                InActivePopup();
            });
        }

        public void SetMessage(string message)
        {
            text_AlertMessage.text = message;
            ActivePopup();
        }
    }
}