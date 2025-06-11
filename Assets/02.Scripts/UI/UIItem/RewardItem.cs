using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.GameData.Defense;

namespace Framework.UI
{
    public class RewardItem : MonoBehaviour
    {
        public TextMeshProUGUI text_amount;
        public Image image_rewardSymbol;

        public void Initialize(LeaderBoardReward weeklyReward)
        {
            string amount = $"x{weeklyReward.rewardValue}";
            text_amount.text = amount;
            image_rewardSymbol.sprite = DataManager.Instance.uiPropertyData.dic_weeklyRewardSymbol[weeklyReward.rewardType];
        }
    }
}