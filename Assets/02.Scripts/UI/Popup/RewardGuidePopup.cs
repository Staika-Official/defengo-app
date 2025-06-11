using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.GameData.Defense;
using Framework.Util;

namespace Framework.UI
{
    public class RewardGuidePopup : PopupTemplate
    {
        [SerializeField] private TextMeshProUGUI text_Title;
        [SerializeField] private TextMeshProUGUI text_Desc;
        [SerializeField] private List<RewardGroup> list_RewardGroup = new();
        [SerializeField] private Queue<RewardGroup> queue_RewardGroup = new();
        [SerializeField] private RectTransform rect_RewardViewPort;
        [SerializeField] private ScrollRect scrollRect;
        
        //private readonly int naturalIndex = 6;
        //private readonly int rateIndex = 3;
        private readonly int rewardMaxIndex = 9;

        public void OpenPopup(LeaderBoardType type)
        {
            RewardLeaderBoard data;
            if (type == LeaderBoardType.WAVE_DAILY)
            {
                data = UserInfoManager.Instance.dailyRewardBoard;
                text_Title.text = LanguageManager.Instance.GetStringData("UI_Daily_Rank_Title");
                text_Desc.text = LanguageManager.Instance.GetStringData("UI_bestWaveDaliyDesc");
            }
            else
            {
                data = UserInfoManager.Instance.weeklyRewardBoard;
                text_Title.text = LanguageManager.Instance.GetStringData("UI_Weekly_Rank_Title");
                text_Desc.text = LanguageManager.Instance.GetStringData("UI_bestWaveDesc");
            }

            RefreshWeeklyRewardGroup();

            if (data.rewardLeaderBoard.Length <= rewardMaxIndex)
                scrollRect.vertical = false;
            else
                scrollRect.vertical = true;

            for (int i = 0; i < data.rewardLeaderBoard.Length; ++i)
            {
                string key = data.rewardLeaderBoard[i].rankType;
                //if (!CheckCreateRewardGroup(key)) continue;

                RewardGroup group = GetWeeklyRewardGroup();
                group.transform.SetParent(rect_RewardViewPort);
                group.transform.localScale = Vector2.one;
                group.gameObject.SetActive(true);

                group.Initialize(data.rewardLeaderBoard[i]);
            }

            queue_RewardGroup.Clear();

            ActivePopup();
        }

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
                InActivePopup();
            });
        }
        
        public RewardGroup GetWeeklyRewardGroup()
        {
            if (queue_RewardGroup.Count == 0)
            {
                GameObject obj = Instantiate(list_RewardGroup[0].gameObject);
                RewardGroup group = obj.GetComponent<RewardGroup>();
                list_RewardGroup.Add(group);
                return group;
            }
            else
            {
                return queue_RewardGroup.Dequeue();
            }
        }

        public void RefreshWeeklyRewardGroup()
        {
            for (int i = 0; i < list_RewardGroup.Count; ++i)
            {
                list_RewardGroup[i].gameObject.SetActive(false);
                queue_RewardGroup.Enqueue(list_RewardGroup[i]);
            }
        }

        // public bool CheckCreateRewardGroup(string key)
        // {
        //     //todo 중요 : 위클리 리워드보상이 늘어나면 여기서 인덱스 값 추가해줘서 체크 해줘야함
        //     //보상 리워드 길이가 리터럴로 선언되어있습니다.
        //     //naturalIndex = NATURAL에 해당하는 인덱스
        //     //rateIndex = RATE에 해당하는 인덱스

        //     bool validation = false;

        //     if (key.Contains("NATURAL"))
        //     {
        //         for (int i = 0; i < naturalIndex; ++i)
        //         {
        //             string natureKey = $"NATURAL_{i + 1}";
                
        //             if (natureKey == key)
        //             {
        //                 validation = true;
        //                 break;
        //             }
        //         }
        //     }
        //     else
        //     {
        //         for (int j = 0; j < rateIndex; ++j)
        //         {
        //             string rateKey = $"RATE_{j + 1}";

        //             if (rateKey == key)
        //             {
        //                 validation = true;
        //                 break;
        //             }
        //         }
        //     }

        //     return validation;
        // }
    }
}
