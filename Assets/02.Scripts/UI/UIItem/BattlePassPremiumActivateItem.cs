using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Framework.Network;
using Framework.GameData.Defense;
using System;

namespace Framework.UI
{
    public class BattlePassPremiumActivateItem : MonoBehaviour
    {
        public Image image_Reward;
        public TextMeshProUGUI text_RewardCount;

        public GameObject buttonConfirmObject;
        public GameObject effect;
        public GameObject effect_radialFocus;

        public void Initialize(BattlePassPremiumItem data)
        {
            string rewardType = "";

            if (data.rewardType.Contains("GEM"))
            {
                if (data.amount < 100)
                    rewardType = $"{data.rewardType}_SMALL";
                else
                    rewardType = $"{data.rewardType}_LOW";
            }
            else
            {
                rewardType = data.rewardType;
            }

            EffectActivate(data.displayType);

            image_Reward.sprite = DataManager.Instance.uiPropertyData.dic_BattlePassItemIcon[rewardType];

            effect_radialFocus.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            text_RewardCount.text = $"x{data.amount}";

            buttonConfirmObject.SetActive(false);
        }

        public void EffectActivate(string displyType)
        {
            BattlePassDisplayType type = (BattlePassDisplayType)Enum.Parse(typeof(BattlePassDisplayType), displyType);
            if (type == BattlePassDisplayType.HIGHLIGHT)
            {
                effect.SetActive(true);
            }
            else
            {
                effect.SetActive(false);
            }
        }
    }
}
