using UnityEngine;
using TMPro;
using Framework.Network;

namespace Framework.UI
{
    public class RewardGroup : MonoBehaviour
    {
        [SerializeField] private RewardItem[] rewardItems;
        [SerializeField] private TextMeshProUGUI text_rate;

        [SerializeField] private GameObject object_RankGold;
        [SerializeField] private GameObject object_RankSliver;
        [SerializeField] private GameObject object_RankBronze;
        [SerializeField] private GameObject object_RankText;                
        
        public void Initialize(LeaderBoardRewardList rewardList)
        {
            SetRankIcon(rewardList.rankType);
            
            for (int i = 0; i < rewardItems.Length; i++)
            {
                rewardItems[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < rewardList.rewards.Length; i++)
            {
                rewardItems[i].gameObject.SetActive(true);
                rewardItems[i].Initialize(rewardList.rewards[i]);
            }
        }

        public void SetRankIcon(string rankType)
        {
            object_RankGold.SetActive(false);
            object_RankSliver.SetActive(false);
            object_RankBronze.SetActive(false);
            object_RankText.SetActive(false);

            if (rankType.Contains("NATURAL_1"))
            {
                object_RankGold.SetActive(true);
            }
            else if (rankType.Contains("NATURAL_2"))
            {
                object_RankSliver.SetActive(true);
            }
            else if (rankType.Contains("NATURAL_3"))
            {
                object_RankBronze.SetActive(true);
            }
            else
            {
                object_RankText.SetActive(true);
                text_rate.text = GetRewardRankRate(rankType);
            }
        }

        public string GetRewardRankRate(string key)
        {
            //중요 : 위클리 리워드보상이 늘어나면 여기서 키값 추가해줘서 값 보여줘야함
            
            string rankText = key switch
            {
                "NATURAL_1" => "1",
                "NATURAL_2" => "2",
                "NATURAL_3" => "3",
                "NATURAL_4" => "~10",
                "NATURAL_5" => "~100",
                "NATURAL_6" => "~200",
                "NATURAL_7" => "~300",
                "RATE_1" => "~5%",
                "RATE_2" => "~20%",
                "RATE_3" => "~50%",
                _ => ""
            };

            return rankText;
        }
        
        
    }
}
