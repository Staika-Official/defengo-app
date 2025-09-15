using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.GameData.Defense;
using TMPro;
using Fusion;
using Framework.Network;

namespace Framework.UI
{

    public class MatchMakingPopup : PopupTemplate
    {
        public Dictionary<PlayerRef, NetworkUserInfo> dic_PlayerData = new();
        public List<MatchingUserInfoItem> matchingUserInfoItems = new();
        public TextMeshProUGUI text_Matching;
        public TextMeshProUGUI text_TimeCount;
        public int timeCount;
        public IEnumerator timer;
        public override void ActivePopup()
        {
            timeCount = 0;
            timer = SetTimeCount();
            StartCoroutine(timer);
            Debug.Log("Matching Start");
            PopUpSequence(true);

            NetworkBattleData playerData = new NetworkBattleData()
            {
                profileId = UserInfoManager.Instance.userState.equippedProfileId,
                nickname = UserInfoManager.Instance.nickname,
                decList = UserSlotManager.Instance.GetSlotFocusIndexData().slotCharacterIds
            };
            matchingUserInfoItems[0].UserInfoInitialize(playerData);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                Shutdown();
            });
        }

        public async void Shutdown()
        {
            StopCoroutine(timer);
            timer = null;
            await NetworkConnect.Instance.runner.Shutdown();
            PopUpSequence(false);
        }

        public void UpdateUserInfo()
        {
            int idx = 1;

            foreach (var item in NetworkConnect.Instance.dic_PlayerData)
            {
                if (item.Value.playerIdx == NetworkConnect.Instance.playerIdx)
                {
                    matchingUserInfoItems[0].UserInfoInitialize(item.Value);
                }
                else
                {
                    matchingUserInfoItems[idx].UserInfoInitialize(item.Value);
                    idx++;
                }
            }
        }

        public IEnumerator SetTimeCount()
        {
            while (true)
            {
                text_TimeCount.text = $"{timeCount / 60:00} : {timeCount % 60:00}";
                yield return new WaitForSeconds(1);
                timeCount++;
            }
        }


    }
}
