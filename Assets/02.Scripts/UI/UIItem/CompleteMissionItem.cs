using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Network;
using Framework.Sound;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class CompleteMissionItem : MonoBehaviour
    {
        public Animator animator_Box;

        public Button button_Reward;
        
        public GameObject object_ClosedItem;
        public GameObject object_OpenItem;

        //public GameObject object_Notice;
        public GameObject object_Effect;
        
        public bool isAvailable;
        public bool isRewarded;
        public int myExp;
        public readonly string animKey = "missionComplete";

        private void Start()
        {
            button_Reward.onClick.AddListener(() =>
            {
                OnClick_MissionReward();
            });
        }
        
        public void Initialize(bool isReward, int boxExp, int userExp)
        {
            // 일퀘 rewardAmount sum한 값이 상자 경험치보다 낮으면 false
            // 일퀘 rewardAmount sum한 값이 상자 경험치보다 높고 재화를 받은 상태 true
            // 일퀘 rewardAmount sum한 값이 상자 경험치보다 높고 재화를 안받은 상태가 false
            
            this.myExp = boxExp;
            
            animator_Box.Rebind();
            this.isRewarded = isReward;
            
            if (myExp <= userExp)
            {
                if (!isRewarded)
                {
                    animator_Box.SetTrigger(animKey);
                }
                
                isAvailable = isRewarded;
                //object_Notice.SetActive(!isRewarded);
                object_Effect.SetActive(!isRewarded);
            }
            else
            {
                //경험치가 낮다면 무조건 false
                isAvailable = !isRewarded;
                //object_Notice.SetActive(isRewarded);
                object_Effect.SetActive(isRewarded);
            }
            
            //경험치와 상관없이 true false만 체크해주면 됌
            object_ClosedItem.SetActive(!isRewarded);
            object_OpenItem.SetActive(isRewarded);
        }
        
        public void OnClick_MissionReward()
        {
            if (this.isAvailable) return;
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            isAvailable = !isAvailable;
            
            _ = NetworkManager.Instance.GetUserDailyCompleteMissionReward(myExp, Success, Failed);

            // List<UserItem> tempitems = new();
            //
            // UserItem temp1 = new()
            // {
            //     item = "HATCHING_ORB_UNIQUE",
            //     quantity = 10
            // };
            //
            // UserItem temp2 = new()
            // {
            //     item = "HATCHING_ORB_EPIC",
            //     quantity = 5
            // };
            //
            // UserItem temp3 = new()
            // {
            //     item = "HATCHING_ORB_LEGEND",
            //     quantity = 100
            // };
            //
            // UserItem temp4 = new()
            // {
            //     item = "HATCHING_ORB_RARE",
            //     quantity = 100
            // };
            //
            // tempitems.Add(temp1);
            // tempitems.Add(temp2);
            // tempitems.Add(temp3);
            // tempitems.Add(temp4);
            //
            // QuestRewardPopup popup = PopupManager.Instance.GetPopUp<QuestRewardPopup>("questReward");
            // popup.CompleteMissionReward(tempitems);
        }

        public void Success(string json)
        {
            List<UserItem> data = JsonConvert.DeserializeObject<List<UserItem>>(json);
            QuestRewardPopup rewardPopup = PopupManager.Instance.GetPopUp<QuestRewardPopup>("questReward");
            rewardPopup.CompleteMissionReward(data);
            
            QuestPopup questPopup = PopupManager.Instance.GetPopUp<QuestPopup>("quest");
            questPopup.GetQuestData(false);
        }

        public void Failed(string errorData)
        {
            isAvailable = !isAvailable;
            
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");

            try
            {
                ServerErrorMessage error = JsonUtility.FromJson<ServerErrorMessage>(errorData);

                switch (error.errorCode)
                {
                    case "INSUFFICIENT_CONDITION":
                        //달성 조건이 충족하지 못할때, 보상을 받을 수 없는 상태
                        popup.SetNoticeMessage("UI_Insufficient_Condition");
                        break;
                    case "FAILED_DAILY_QUEST":
                        //지정 경험치 이외의 값을 요청한 경우(20, 40, 60, 80, 100)
                        popup.SetNoticeMessage("UI_Failed_DailyQuest");
                        break;
                    case "ALREADY_REWARDED":
                        //이미 해당 경험치를 보상 받은 경우
                        popup.SetNoticeMessage("UI_Already_Rewarded");
                        break;
                    case "TOO_MANY_REQUESTS":
                        //짧은 시간 내에 일정 기준의 연속적인 호출
                        popup.SetNoticeMessage("MSG_TOO_MANY_REQUESTS");
                        break;
                    default:
                        //네트워크 환경 불안정
                        popup.SetNoticeMessage("UI_Network_Unstable");
                        break;
                }
            }
            catch
            {
                popup.SetNoticeMessage("UI_Network_Unstable");
            }
        }
    }
}
