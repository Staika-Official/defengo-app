using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Framework.Network;
using Framework.GameData.Defense;

namespace Framework.UI
{
    public class WeeklyRewardItem : MonoBehaviour
    {
        [SerializeField] private Image image_Icon;
        [SerializeField] private TextMeshProUGUI text_Amount;

        public void Initialize(LeaderBoardReward reward)
        {
            text_Amount.text = $"x{reward.rewardValue}";
            image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_weeklyRewardSymbol[reward.rewardType];
        }
    }
}