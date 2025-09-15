using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Game.Defense;
using UnityEngine;
using UnityEngine.UI;
using Framework.Sound;
using UnityEngine.Events;

namespace Framework.UI
{
    public class SummonButton : MonoBehaviour
    {
        public ButtonComponent button_summonCharacter;
        public Image image_characterPortrait;
        public Image image_background;
        public GameObject summonCharacterActive;
        public GameObject fixedCharacterInfo;
        public GameObject normalSummonGroup;
        public GameObject fiveStar;
        public GameObject[] stars;
        public bool isFixedSummon;
        public UnityAction SummonFixedCharacter;

        public void Initilize()
        {
            button_summonCharacter.onPointerUp = OnClick_SummonCharacter;
        }

        public void OnClick_SummonCharacter()
        {
            if (isFixedSummon)
            {
                //GameManager.Instance.SummonFixedCharacter();
                SummonFixedCharacter?.Invoke();
                normalSummonGroup.SetActive(true);
                fixedCharacterInfo.SetActive(false);
                isFixedSummon = false;

                GameManager.Instance.CalcPossibleCostAction();
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_SUMMON);
                if (GameManager.Instance.isTestMode)
                {
                    GameManager.Instance.characterSpawner.SummonCharacter();
                }
                else
                {
                    GameManager.Instance.SummonCharacter();
                }

                GameManager.Instance.CalcPossibleCostAction();
            }
        }

        public void SetFixedCharacter(CharacterIndex characterIndex, int starGrade)
        {
            Debug.Log("Set Fixed Character");
            Debug.Log("CharacterIndex : " + characterIndex);
            Debug.Log("star Grade : " + starGrade);
            isFixedSummon = true;
            fixedCharacterInfo.SetActive(true);
            normalSummonGroup.SetActive(false);

            button_summonCharacter.SetInterectible(true);
            summonCharacterActive.SetActive(true);

            CharacterData characterData = DataManager.Instance.dic_CharacterData[characterIndex];

            image_characterPortrait.sprite = characterData.sprite_ChracterPortrait;
            image_background.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[characterData.characterGrade].color_BackGround;

            if (starGrade < 5)
            {
                for (int i = 0; i < stars.Length; i++)
                {
                    stars[i].SetActive(i < starGrade);
                }
            }
            else 
            {
                
            }
            
            
            SummonFixedCharacter = () =>
            {
                GameManager.Instance.SummonFixedCharacter(characterIndex, starGrade);
            };
        }

        public void SetPossibleSummon(bool isPossible)
        {
            if (isFixedSummon) return;

            button_summonCharacter.SetInterectible(isPossible);
            summonCharacterActive.SetActive(isPossible);
        }
    }
}
