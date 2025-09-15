using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using Framework.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.UI
{
    public class StartUpSingleModePopup : PopupTemplate
    {
        public ButtonComponent button_Play;

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
                InActivePopup();
            });
            button_Play.onPointerUp += () =>
            {
                button_Play.onPointerUp = null;
                SoundManager.Instance.PlaySound(SoundKey.SF_GAMEPLAY);
                HomeScreen.Instance.RequestStartGame();
            };
        }


        public void OnCompleteGameStart()
        {
            SceneLoadManager.Instance.SwitchingScene(3);
        }

        public void ReleasePlayButton(bool isInterectable)
        {
            button_Play.SetInterectible(isInterectable);
        }
    }
}
