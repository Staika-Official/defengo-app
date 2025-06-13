using System.Collections;
using System.Collections.Generic;
using Framework.Sound;
using UnityEngine;

namespace Framework.UI
{
    public class EditConfigPopup : PopupTemplate
    {
        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                PopUpSequence(false);
            });
        }
    }
}