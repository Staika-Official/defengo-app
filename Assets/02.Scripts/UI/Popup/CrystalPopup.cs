using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Framework.GameData.Defense;
using Framework.Util;
using Framework.Sound;

namespace Framework.UI
{
    public class CrystalPopup : PopupTemplate
    {
        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_Info;
        public TextMeshProUGUI text_Gem;
        public TextMeshProUGUI text_Crystal;

        public Image image_Cristal;

        public Button button_BuyItem;

        public int gemAmount;
        public int crystalAmount;

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
            button_BuyItem.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnClick_BuyCrystal();
            });

            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnClick_Close();
            });
        }

        public void SetCrystalPopup(int gemAmount, int crystalAmount)
        {
            //크리스탈 이미지를 얻어오고 스케일 조정을 위한타입
            BattlePassCrystalType type = (BattlePassCrystalType)crystalAmount;

            //타입에 맞는 이미지 변경
            image_Cristal.sprite = DataManager.Instance.uiPropertyData.dic_BattlePassCrystal[type];
            //제목 텍스트
            text_Title.text = LanguageManager.Instance.GetStringData("UI_Crystal_Pass_Title");
            //설명 텍스트
            text_Info.text = LanguageManager.Instance.GetStringData("UI_Crystal_Pass_Desc");;
            //젬 텍스트
            text_Gem.text = $"<sprite=2>{gemAmount}";
            //갯수 텍스트
            text_Crystal.text = $"x{crystalAmount}";

            this.gemAmount = gemAmount;
            this.crystalAmount = crystalAmount;

            ActivePopup();
        }

        public void OnClick_BuyCrystal()
        {
            BattlePassCrystalBuyPopup popup = PopupManager.Instance.GetPopUp<BattlePassCrystalBuyPopup>("battlePassCrystalBuy");
            popup.GemPayMent(gemAmount, crystalAmount);
            InActivePopup();
        }

        public void OnClick_Close()
        {
            InActivePopup();
        }
    }
}
