using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.GameData.Defense;
using TMPro;
using Fusion;
using Framework.Network;
using Framework.Util;

namespace Framework.UI
{

    public class MatchMakingPopup : PopupTemplate
    {
        // public Dictionary<PlayerRef, NetworkUserInfo> dic_PlayerData = new();
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

            bool isFriendlyMatch = NetworkConnect.Instance.isFriendlyMatch;
            int maxPlayerCount = NetworkConnect.Instance.maxPlayerCount;

            for (int i = 1; i < matchingUserInfoItems.Count; i++)
            {
                if (i < maxPlayerCount)
                {
                    matchingUserInfoItems[i].button_Invite.gameObject.SetActive(false);
                    matchingUserInfoItems[i].matching.SetActive(true);
                    matchingUserInfoItems[i].locked.SetActive(false);
                }
                else
                {
                    matchingUserInfoItems[i].button_Invite.gameObject.SetActive(false);
                    matchingUserInfoItems[i].matching.SetActive(false);
                    matchingUserInfoItems[i].locked.SetActive(true);
                }
            }
        }

        public override void InActivePopup()
        {
            button_Close.gameObject.SetActive(false);
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
            button_Close.gameObject.SetActive(false);
            StopCoroutine(timer);
            timer = null;
            NetworkConnect.Instance.ShutDown();
            PopUpSequence(false);
        }

        public void UpdateUserInfo()
        {
            int idx = 1;
            bool isFriendlyMatch = NetworkConnect.Instance.isFriendlyMatch;
            int maxPlayerCount = NetworkConnect.Instance.maxPlayerCount;

            foreach (var item in NetworkConnect.Instance.dic_PlayerData.Values)
            {
                int slotIndex = (item.playerIdx == NetworkConnect.Instance.playerIdx) ? 0 : idx;

                if (slotIndex < matchingUserInfoItems.Count)
                {
                    matchingUserInfoItems[slotIndex].UserInfoInitialize(item);
                }
                else
                {
                    Debug.LogWarning(
                        $"[UpdateUserInfo] Not enough slots for playerIdx={item.playerIdx}. " +
                        $"slotIndex={slotIndex}, totalSlots={matchingUserInfoItems.Count}"
                    );
                }

                if (slotIndex != 0) idx++;
            }

            // Fill empty slots or lock slots if beyond max players
            for (int i = NetworkConnect.Instance.dic_PlayerData.Count; i < matchingUserInfoItems.Count; i++)
            {
                if (i < maxPlayerCount)
                {
                    matchingUserInfoItems[i].button_Invite.gameObject.SetActive(isFriendlyMatch);
                    matchingUserInfoItems[i].matching.SetActive(!isFriendlyMatch);
                    matchingUserInfoItems[i].locked.SetActive(false);
                }
                else
                {
                    matchingUserInfoItems[i].button_Invite.gameObject.SetActive(false);
                    matchingUserInfoItems[i].matching.SetActive(false);
                    matchingUserInfoItems[i].locked.SetActive(true);
                }
            }

            button_Close.gameObject.SetActive(true);
        }

        public IEnumerator SetTimeCount()
        {
            while (true)
            {
                if (NetworkConnect.Instance.dic_PlayerData.Count <= 1)
                {
                    text_Matching.text = LanguageManager.Instance.GetStringData("UI_Matching_Time");
                    text_TimeCount.text = $"{timeCount / 60:00} : {timeCount % 60:00}";
                }
                else
                {
                    text_Matching.text = LanguageManager.Instance.GetStringData("UI_Starting_Time");
                }
                yield return new WaitForSeconds(1);
                timeCount++;
            }
        }
    }
}
