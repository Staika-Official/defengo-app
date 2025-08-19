using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Sound;
using Framework.Network;
using System;
using DG.Tweening;

namespace Framework.UI
{
    public class FriendPopup : PopupTemplate
    {
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
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                PopUpSequence(false);
            });

            // _ = NetworkManager.Instance.GetTotalUserProfileDetail(UserInfoManager.Instance.userId, Success, Failed);
        }
        void Success()
        {

        }
        void Failed()
        {

        }
    }
}
