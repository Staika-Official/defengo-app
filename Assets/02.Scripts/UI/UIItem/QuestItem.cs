using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Network;
using TMPro;
using UnityEngine.UI;
using Framework.GameData.Defense;
using Framework.Sound;
using Framework.Util;

namespace Framework.UI
{
    public class QuestItem : MonoBehaviour
    {
        public Image image_Icon;

        public Image image_BackGround;
        public Image image_MainFrame;
        public Image[] image_Lines;
        public Image image_Reward;
        
        public ButtonComponent button_Reward;
        
        public GameObject object_Clear;
        public GameObject object_Notice;

        public TextMeshProUGUI text_RewardCount;
        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_Achieve;

        public RectTransform rect_Star;

        public Slider slider_ItemGuage;

        public QuestType questType;
        public QuestRewardStateType rewardState;
        //public QuestMissionType missionType;
        public string missionType;
        
        public int attainAmount;
        public int acquireCount;

        public RandomRewardType rewardType;
        
        private void Start()
        {
            button_Reward.onPointerUp = () =>
            {
                OnClick_Reward();
            };
        }
        
        public void Initialize(UserDailyQuests data, QuestType type, bool isReward = false)
        {
            button_Reward.SetInterectible(false);
            
            attainAmount = data.attainAmount;
            
            //test
            // attainAmount = 188542;
            // attainAmount = 999;
            
            acquireCount = Mathf.Min(data.acquireCount, attainAmount);

            questType = type;

            image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_RewardItemIcon["DAILYEXP"];
            
            string missionTitle = LanguageManager.Instance.GetStringData($"UI_MISSION_{data.type}");

            text_Title.text = string.Format(missionTitle, Calculator.TruncateValue(attainAmount));
            
            //test
            // string missionTemp = LanguageManager.Instance.GetStringData($"UI_MISSION_{TempFunction(data.type)}");
            // text_Title.text = string.Format(missionTemp, data.attainAmount);
            
            text_RewardCount.text = $"{data.rewardAmount.ToString()}";

            rewardType = RandomRewardType.DAILYEXP;
            
            //missionType = (QuestMissionType)Enum.Parse(typeof(QuestMissionType), data.type);
            missionType = data.type;
            
            rewardState = (QuestRewardStateType)Enum.Parse(typeof(QuestRewardStateType), data.rewardStatus);

            slider_ItemGuage.value = (float)acquireCount / attainAmount;
            
            bool isRewardCheck = false;
            bool isCompleteCheck = false;

            switch (rewardState)
            {
                case QuestRewardStateType.NONE:
                    break;
                case QuestRewardStateType.AVAILABLE:
                    isRewardCheck = true;
                    if (!isReward)
                    {
                        button_Reward.SetInterectible(true);
                    }
                    break;
                case QuestRewardStateType.REWARDED:
                    isCompleteCheck = true;
                    break;
                default:
                    break;
            }
            

            SetFrameColor(rewardState);
            RewardCheck(isRewardCheck);
            ClearCheck(isCompleteCheck);
        }

        #region Test
        // public string TempFunction(string type)
        // {
        //     string temp = "";
        //     
        //     if (type.Contains("LOGIN"))
        //     {
        //         temp = "CLASSUP";
        //     }
        //     else if (type.Contains("BOSS"))
        //     {
        //         temp = "SUMMON";
        //     }
        //     else if (type.Contains("INAPP"))
        //     {
        //         temp = "MERGE";
        //     }
        //     else if (type.Contains("MONSTER"))
        //     {
        //         temp = "HIGHGRADE";
        //     }
        //     else if (type.Contains("PLAY"))
        //     {
        //         temp = "UPGRADE";
        //     }
        //     else if (type.Contains("AD"))
        //     {
        //         temp = "GEM_SPEND";
        //     } 
        //     else if (type.Contains("MISSION"))
        //     {
        //         temp = "GEM_SWAP";
        //     }
        //     else if (type.Contains("WAVE"))
        //     {
        //         temp = "GEM_SWAP";
        //     }
        //
        //     if (string.IsNullOrEmpty(temp))
        //     {
        //         Debug.Log(type);    
        //     }
        //     
        //     return temp;
        // }
        #endregion
       
        public void Initialize(UserAchievements data, QuestType type, bool isReward = false)
        {
            button_Reward.SetInterectible(false);
            
            attainAmount = data.attainAmount;
            acquireCount = Mathf.Min(data.acquireCount, attainAmount);

            questType = type;
            
            //아이콘 및 보상받기
            image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_RewardItemIcon[data.reward];
            
            string missionTitle = LanguageManager.Instance.GetStringData($"UI_MISSION_{data.type}");
            text_Title.text = string.Format(missionTitle, Calculator.TruncateValue(data.attainAmount));
            
            text_RewardCount.text = $"{data.rewardAmount.ToString()}";

            rewardType = (RandomRewardType)Enum.Parse(typeof(RandomRewardType), data.reward);

            //missionType = (QuestMissionType)Enum.Parse(typeof(QuestMissionType), data.type);
            missionType = data.type;
            
            rewardState = (QuestRewardStateType)Enum.Parse(typeof(QuestRewardStateType), data.rewardStatus);

            slider_ItemGuage.value = (float)acquireCount / attainAmount;
            
            bool isRewardCheck = false;
            bool isCompleteCheck = false;

            switch (rewardState)
            {
                case QuestRewardStateType.NONE:
                    break;
                case QuestRewardStateType.AVAILABLE:
                    isRewardCheck = true;
                    if (!isReward)
                    {
                        button_Reward.SetInterectible(true);
                    }
                    break;
                case QuestRewardStateType.REWARDED:
                    isCompleteCheck = true;
                    break;
                default:
                    break;
            }
            
            SetFrameColor(rewardState);
            RewardCheck(isRewardCheck);
            ClearCheck(isCompleteCheck);
        }

        public void SetFrameColor(QuestRewardStateType type)
        {
            image_BackGround.color = DataManager.Instance.uiPropertyData.dic_QuestItemColor[type].color_BackGround;
            image_MainFrame.color = DataManager.Instance.uiPropertyData.dic_QuestItemColor[type].color_MainFrame;

            for (int i = 0; i < image_Lines.Length; ++i)
            {
                image_Lines[i].color = DataManager.Instance.uiPropertyData.dic_QuestItemColor[type].color_Line;
            }
            
            image_Reward.color = DataManager.Instance.uiPropertyData.dic_QuestItemColor[type].color_Reward;
        }

        public void ClearCheck(bool isClear)
        {
            object_Clear.SetActive(isClear);

            if (!isClear)
            {
                text_Achieve.text = Calculator.TruncateValue(acquireCount) + " / " + Calculator.TruncateValue(attainAmount);
            }

            text_Achieve.gameObject.SetActive(!isClear);
        }

        public void RewardCheck(bool isReward)
        {
            object_Notice.SetActive(isReward);
        }

        public void OnClick_Reward()
        {
            //리워드 받고 석세스가 된다면 다시 리프레쉬해서 
            //interactable 초기해주기 때문에 석세스에서 설정안해줌
            //failed에서만 처리

            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            QuestPopup popup = PopupManager.Instance.GetPopUp<QuestPopup>("quest");
            popup.SetQuestItemButtonInteractable(false);
            
            //Debug.Log($"OnClick Quest Interactable {button_Reward.isInterectible}");
            
            switch (questType)
            {
                case QuestType.DAILY:
                    QuestReward();
                    break;
                case QuestType.ACHIEVEMENT:
                    AchievementReward();
                    break;
                default:
                    break;
            }
        }
        
        public void QuestReward()
        {
            _ = NetworkManager.Instance.GetUserDailyQuestExp(missionType, SuccessQuest, Failed);
            
            //test
            //SuccessQuest("test");
        }

        public void SuccessQuest(string json)
        {
            QuestPopup popup = PopupManager.Instance.GetPopUp<QuestPopup>("quest");
            popup.QuestReward(json, this);
            
            //test
            //popup.QuestReward(this);
            //button_Reward.SetInterectible(true);
        }
        
        public void AchievementReward()
        {
            //NonTest
            _ = NetworkManager.Instance.GetUserAchievementReward(missionType, SuccessAchievement, Failed);
            
            //Test
            //QuestPopup popup = PopupManager.Instance.GetPopUp<QuestPopup>("quest");
            //popup.AchievementReward(popup.testData.text, this);
        }
        
        public void SuccessAchievement(string json)
        {
            //Debug.Log("Success Achievement");
            QuestPopup popup = PopupManager.Instance.GetPopUp<QuestPopup>("quest");
            popup.AchievementReward(json, this);
        }
        
        public void Failed(string errorData)
        {
            QuestPopup questPopup = PopupManager.Instance.GetPopUp<QuestPopup>("quest");
            questPopup.SetQuestItemButtonInteractable(true);
            
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");

            try
            {
                ServerErrorMessage error = JsonUtility.FromJson<ServerErrorMessage>(errorData);

                switch (error.errorCode)
                {
                    case "INSUFFICIENT_CONDITION":
                        //달성 조건이 충족하지 못할때, 보상을 받을 수 없는 상태.
                        popup.SetNoticeMessage("UI_Insufficient_Condition");
                        break;
                    case "FAILED_ACHIEVEMENT":
                        //만렙 이상의 보상요청 또는 보상 실패.
                        popup.SetNoticeMessage("UI_Failed_Achievement");
                        break;
                    case "ALREADY_REWARDED":
                        //일일퀘스트 이미 경험치를 받은 상태.
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
