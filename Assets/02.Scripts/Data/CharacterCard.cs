using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.GameData.Defense;
using TMPro;
using DG.Tweening;

namespace Framework.UI
{
    public class CharacterCard : MonoBehaviour
    {
        public Image image_Portrait;
        public Image image_Frame;
        public Image image_BackGround;
        public Image image_Shadow;
        public GameObject maxObject;
        
        public TextMeshProUGUI text_Level;
        
        public void Initialize(CharacterData characterData)
        {
            CharacterGrade grade = characterData.characterGrade;
            CharacterCardInfo info = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[grade];

            image_Portrait.sprite = characterData.sprite_ChracterPortrait;
            image_Frame.sprite = info.sprite_Frame;
            image_BackGround.color = info.color_BackGround;
            image_Shadow.color = info.color_Shadow;
            text_Level.text = $"Lv.{characterData.characterClassLevel}";

            text_Level.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[grade].color_LevelText;
        }

        public void InitilaizeEvent(CharacterData characterData)
        {
            CharacterGrade grade = characterData.characterGrade;
            CharacterCardInfo info = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[grade];

            image_Portrait.sprite = characterData.sprite_ChracterPortrait;
            image_Frame.sprite = info.sprite_Frame;
            image_BackGround.color = info.color_BackGround;
            image_Shadow.color = info.color_Shadow;
            text_Level.text = $"Lv.{characterData.characterClassLevel}";

            text_Level.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[grade].color_LevelText;
        }

        public void InitializeFriendly(CharacterData data)
        {
            CharacterGrade grade = data.characterGrade;
            CharacterCardInfo info = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[grade];

            image_Portrait.sprite = data.sprite_ChracterPortrait;
            image_Frame.sprite = info.sprite_Frame;
            image_BackGround.color = info.color_BackGround;
            image_Shadow.color = info.color_Shadow;
        }
        
        public void InitializeItem(int characterIndex)
        {
            CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)characterIndex];
            
            CharacterGrade grade = data.characterGrade;
            CharacterCardInfo info = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[grade];

            image_Portrait.sprite = data.sprite_ChracterPortrait;
            image_Frame.sprite = info.sprite_Frame;
            image_BackGround.color = info.color_BackGround;
            image_Shadow.color = info.color_Shadow;
            text_Level.text = "x1";
        }

        public void InitializeWithArrow(CharacterData characterData)
        {
            CharacterGrade grade = characterData.characterGrade;
            CharacterCardInfo info = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[grade];

            image_Portrait.sprite = characterData.sprite_ChracterPortrait;
            image_Frame.sprite = info.sprite_Frame;
            image_BackGround.color = info.color_BackGround;
            image_Shadow.color = info.color_Shadow;
            text_Level.text = $"Lv.{characterData.characterClassLevel}";

            if (characterData.characterClassLevel == CharacterClassManager.Instance.GetMaxClassLevel())
            {
                maxObject.SetActive(false);
            }
            else
            {
                int levelUpQuantity = CharacterClassManager.Instance.GetClassUpInfo(characterData.characterGrade, characterData.characterClassLevel);

                bool isActive = characterData.characterQuantity >= levelUpQuantity;
                maxObject.SetActive(isActive);
            }

            text_Level.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[grade].color_LevelText;
        }
    }
}