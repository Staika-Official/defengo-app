using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;
using Framework.Network;
using UnityEngine.UI;

namespace Framework.UI
{
    public class RestorePopup : PopupTemplate
    {
        public Button button_Confirm;
        
        public override void ActivePopup()
        {
            
        }

        public override void InActivePopup()
        {
            
        }

        public override void Initialize()
        {
            button_Confirm.onClick.AddListener(() =>
            {
                OnClick_Confirm();
            });

            button_Close.onClick.AddListener(() =>
            {
                PopUpSequence(false);
            });
        }

        public void OnClick_Confirm()
        {
            _ = NetworkManager.Instance.RestoreAccountV2(Success, Failed);
        }

        public void Success()
        {
            PopUpSequence(false);
        }

        public void Failed(TerminationStatusInfo data)
        {
            PopUpSequence(false);
            string errorCode = data.errorCode;
        }
    }
}
