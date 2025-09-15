using System.Collections;
using System.Collections.Generic;
using Framework.Network;
using Framework.Sound;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.UI
{
    public class StartUpBattleModePopup : PopupTemplate
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
                NetworkConnect networkConnect = Instantiate(HomeScreen.Instance.networkPrefab).GetComponent<NetworkConnect>();
                networkConnect.ConnectToLobby();
                MatchMakingPopup popup = PopupManager.Instance.GetPopUp<MatchMakingPopup>("matchMaking");
                popup.ActivePopup();
                DontDestroyOnLoad(networkConnect);
            };
        }
    }
}