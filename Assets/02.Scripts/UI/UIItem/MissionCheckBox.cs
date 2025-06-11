using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Util;

namespace Framework.Game.Defense
{
    public class MissionCheckBox : MonoBehaviour
    {
        public TextMeshProUGUI text_Mission;
        public GameObject checkSign;
        public int reward;
        public TextMeshProUGUI text_RewardValue;
        public TextMeshProUGUI text_waterAmount;
        public Image waterSymbol;
        //RectTransform textMissionRect;
        
        public void Initialize(MissionCheck data)
        {
            text_Mission.text = LanguageManager.Instance.GetStringData(data.descriptionKey);
            //text_Mission.TryGetComponent<RectTransform>(out RectTransform rect);
            //textMissionRect = rect;
            reward = data.rewardValue;
            text_RewardValue.text = data.rewardValue.ToString();
            checkSign.SetActive(false);
        }

        public void MissionComplete()
        {
            checkSign.SetActive(true);
            text_Mission.rectTransform.anchoredPosition += new Vector2(58, 0);
            //textMissionRect.anchoredPosition += new Vector2(58, 0);
            text_Mission.color = new Color32(15,147,93,255);
            text_waterAmount.color = new Color32(192,255,163,83);
            waterSymbol.color = new Color32(192,255,156,130);
            GameManager.Instance.ChangeGem(reward);
        }
    }
}
