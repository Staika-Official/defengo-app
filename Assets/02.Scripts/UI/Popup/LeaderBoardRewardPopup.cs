using UnityEngine;
using TMPro;
using Framework.Network;
using Framework.GameData.Defense;
using UnityEngine.Events;
using Framework.Util;

namespace Framework.UI
{
    public class LeaderBoardRewardPopup : PopupTemplate
    {
        [SerializeField] private TextMeshProUGUI text_Title;
        [SerializeField] private TextMeshProUGUI text_Description;
        [SerializeField] private RewardItem[] rewardItems;
        public UnityAction OnCompletePopup;

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
                OnClick_Confirm();
            });
        }



        public void SetRewardList(LeaderBoardType type, string key)
        {
            for (int i = 0; i < rewardItems.Length; i++)
            {
                rewardItems[i].gameObject.SetActive(false);
            }

            LeaderBoardRewardList weeklyRewardList;
            string typeStringKey;
            if (type == LeaderBoardType.WAVE_DAILY)
            {
                weeklyRewardList = UserInfoManager.Instance.GetDailyRankInfoList(key);
                typeStringKey = "Daily";
            }
            else
            {
                
                weeklyRewardList = UserInfoManager.Instance.GetWeeklyRankInfoList(key);
                typeStringKey = "Weekly";
            }

            text_Title.text = LanguageManager.Instance.GetStringData($"UI_{typeStringKey}_Reward");
            string descKey = $"UI_{typeStringKey}_Profile_Desc_1";

            string desc = LanguageManager.Instance.GetStringData(descKey);

            if (key.Contains("NATURAL"))
            {
                string rankText = key switch
                {
                    "NATURAL_1" => string.Format(LanguageManager.Instance.GetStringData($"UI_{typeStringKey}_Rank_1st"), "1"),
                    "NATURAL_2" => string.Format(LanguageManager.Instance.GetStringData($"UI_{typeStringKey}_Ranker_Guide"), "2"),
                    "NATURAL_3" => string.Format(LanguageManager.Instance.GetStringData($"UI_{typeStringKey}_Ranker_Guide"), "3"),
                    "NATURAL_4" => string.Format(LanguageManager.Instance.GetStringData($"UI_{typeStringKey}_Ranker_Guide"), "10"),
                    "NATURAL_5" => string.Format(LanguageManager.Instance.GetStringData($"UI_{typeStringKey}_Ranker_Guide"), "100"),
                    "NATURAL_6" => string.Format(LanguageManager.Instance.GetStringData($"UI_{typeStringKey}_Ranker_Guide"), "200"),
                    "NATURAL_7" => string.Format(LanguageManager.Instance.GetStringData($"UI_{typeStringKey}_Ranker_Guide"), "300"),
                    _ => ""
                };

                if (!key.Contains("4") && !key.Contains("5") && !key.Contains("6") && !key.Contains("7"))
                {
                    string rankProfile = LanguageManager.Instance.GetStringData($"UI_{typeStringKey}_Profile_Desc_2");
                    desc = string.Format(desc, rankText) + " " + rankProfile;

                    if (key != "NATURAL_1")
                    {
                        text_Description.text = desc;
                    }
                    else
                    {
                        text_Description.text = rankText + " " + rankProfile;
                    }
                }
                else
                {
                    desc = string.Format(desc, rankText);
                    text_Description.text = desc;
                }
            }
            else
            {
                string rateText = key switch
                {
                    "RATE_1" => "TOP 5%",
                    "RATE_2" => "TOP 20%",
                    "RATE_3" => "TOP 50%",
                    _ => ""
                };

                text_Description.text = string.Format(desc, rateText);
            }

            for (int i = 0; i < weeklyRewardList.rewards.Length; i++)
            {
                rewardItems[i].gameObject.SetActive(true);
                rewardItems[i].Initialize(weeklyRewardList.rewards[i]);
            }

            ActivePopup();
        }

        public void OnClick_Confirm()
        {
            OnCompletePopup?.Invoke();
            InActivePopup();
        }
    }
}
