using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Util;
using Framework.Game.Defense;
using Framework.Network;

namespace Framework.UI
{
    public class RandomResult : MonoBehaviour
    {
        public Image image_Frame;
        public Image image_CharacterPortrait;
        public GameObject newCharacter;
        public GameObject epicEffect;

        public TextMeshProUGUI text_Grade;
        public TextMeshProUGUI text_CharacterName;
        public TextMeshProUGUI text_ItemAmount;

        public Animator animator;

        public void Initialize(CharacterData data)
        {
            CharacterCardInfo cardInfo = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade];
            string grade = LanguageManager.Instance.GetStringData($"UI_Grade_{(int)data.characterGrade}");
            text_Grade.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade].color_decGradeText;
            text_Grade.text = grade;
            text_ItemAmount.text = "x1";
            text_CharacterName.text = LanguageManager.Instance.GetStringData($"UI_Character_{(int)data.characterIndex}");  
            image_Frame.sprite = cardInfo.sprite_Frame;
            image_CharacterPortrait.sprite = data.sprite_ChracterPortrait;
            newCharacter.SetActive(false);

            bool isEpicGrade = (int)data.characterGrade >= 6;
            epicEffect.SetActive(isEpicGrade);

            if(data.isNew)
            {
                newCharacter.SetActive(true);
                data.isNew = false;
            }

            animator.SetTrigger("CardEnter");
        }

        public void Initialize(UserItem data, int amount)
        {
            HatchingStoneCardInfo cardInfo = DataManager.Instance.uiPropertyData.dic_HatchingCardInfo[data.item];

            text_Grade.color = DataManager.Instance.uiPropertyData.dic_HatchingCardInfo[data.item].color_GradeText;
            text_Grade.text = LanguageManager.Instance.GetStringData($"UI_GRADE_{data.item}");
            text_ItemAmount.text = $"x{amount}";
            text_CharacterName.text = LanguageManager.Instance.GetStringData($"UI_{data.item}");
            image_Frame.sprite = cardInfo.sprite_Frame;
            image_CharacterPortrait.sprite = DataManager.Instance.uiPropertyData.dic_HatchingStoneSprite[data.item].sprite_Icon;
            newCharacter.SetActive(false);

            bool isEpicGrade = data.item.Contains("EPIC") || data.item.Contains("LEGEND");

            epicEffect.SetActive(isEpicGrade);
            animator.SetTrigger("CardEnter");
        }
    }
}
