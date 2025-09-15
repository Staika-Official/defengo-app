using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.Util;
using Framework.Game.Defense;
using Framework.Sound;

namespace Framework.UI
{
    public class BattleResultTablePopup : PopupTemplate
    {
        public Animation openAnimation;
        public BattleResultTableItem[] battleResultTableItems;

        public override void ActivePopup()
        {
            PopUpSequence(true);

            List<NetworkBattleData> data = NetworkConnect.Instance.dic_PlayerData.Values.ToList();

            data.Sort((NetworkBattleData a, NetworkBattleData b) => b.waveCount.CompareTo(a.waveCount));

            for (int i = 0; i < data.Count; i++)
            {
                battleResultTableItems[i].Initialize(data[i]);
            }

            openAnimation.Play();
        }

        public override void InActivePopup()
        {
            Debug.Log("End Game");
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            for (int i = 0; i < battleResultTableItems.Length; i++)
            {
                battleResultTableItems[i].LockInitialize();
            }

            Debug.Log("Add Listener");

            button_Close.onClick.AddListener(() =>
            {
                Debug.Log("Add Listener");
                OnClick_Home();
            });
        }

        public void OnClick_Home()
        {
            Debug.Log("OnClick Home");
            NetworkConnect.Instance.runner.Shutdown();
            SceneLoadManager.onCompleteLoadScene = () =>
            {
                SoundManager.Instance.PlaySound(SoundKey.BGM_LOBBY);
            };
            NetworkConnect.Instance.runner.Shutdown();
            SceneLoadManager.Instance.SwitchingScene(2);

            GameManager.Instance.objectPoolManager.AllClear();
        }
    }
}
