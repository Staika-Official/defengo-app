using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Framework.Sound;
using Framework.GameData.Defense;
using System;

namespace Framework.UI
{
    public class CrystalItem : MonoBehaviour
    {
        public TextMeshProUGUI text_CrystalAmount;
        public TextMeshProUGUI text_Gem;

        public Image image_Cristal;
        //이미지 몇개 더 추가 예정
        public Button button_gem;

        public int gemAmount;
        public int crystalAmount;

        public void Start()
        {
            button_gem.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                CrystalPopup popup = PopupManager.Instance.GetPopUp<CrystalPopup> ("crystal");
                popup.SetCrystalPopup(gemAmount, crystalAmount);
            });
        }

        public void Initialize(int cristal, int gem)
        {
            image_Cristal.sprite = DataManager.Instance.uiPropertyData.dic_BattlePassCrystal[(BattlePassCrystalType)cristal];
            crystalAmount = cristal;
            gemAmount = gem;

            text_CrystalAmount.text = $"x{crystalAmount}";
            text_Gem.text = $"<sprite=2>{gemAmount}"; 
        }
    }
}
