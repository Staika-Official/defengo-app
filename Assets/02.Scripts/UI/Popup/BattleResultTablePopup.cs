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

            data = data
            .OrderByDescending(x => x.waveCount)
            .ThenByDescending(x => x.monsterKilled)
            .ToList();

            for (int i = 0; i < data.Count; i++)
            {
                data[i].rank = i + 1;
                battleResultTableItems[i].Initialize(data[i]);
            }

            openAnimation.Play();
        }

        public override void InActivePopup()
        {
            if (NetworkConnect.Instance.isFriendlyMatch)
            {
                Debug.Log("OnClick Home");
                SceneLoadManager.onCompleteLoadScene = () =>
                {
                    SoundManager.Instance.PlaySound(SoundKey.BGM_LOBBY);
                };
                NetworkConnect.Instance.runner.Shutdown();
                SceneLoadManager.Instance.SwitchingScene(2);

                GameManager.Instance.objectPoolManager.AllClear();
            }
            else
            {
                UIManager.Instance.battleResultPromotePopup.ActivePopup();
            }
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
            InActivePopup();
        }
    }
}
