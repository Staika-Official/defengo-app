using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Game.Defense;
using UnityEngine;
using UnityEngine.UI;
using Framework.Sound;
using UnityEngine.Events;
using System.Linq;

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
        public GameObject grayStar;
        public GameObject[] stars;
        public bool isFixedSummon;
        public UnityAction SummonFixedCharacter;

        public int testStarGrade;
        public CharacterIndex testCharacterIndex;

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
                GameManager.Instance.BuffSummonCost(0);

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
            UIManager.Instance.text_SummonCharacterCost.text = $"0";

            button_summonCharacter.SetInterectible(true);
            summonCharacterActive.SetActive(true);

            CharacterData characterData = DataManager.Instance.dic_CharacterData[characterIndex];

            image_characterPortrait.sprite = characterData.sprite_ChracterPortrait;
            image_background.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[characterData.characterGrade].color_BackGround;

            if (starGrade == 0)
            {
                grayStar.gameObject.SetActive(true);
                for (int i = 0; i < stars.Length; i++)
                {
                    stars[i].gameObject.SetActive(false);
                }
                fiveStar.gameObject.SetActive(false);
            }
            else if (starGrade == 5)
            {
                for (int i = 0; i < stars.Length; i++)
                {
                    stars[i].gameObject.SetActive(false);
                }
                grayStar.gameObject.SetActive(false);
                fiveStar.gameObject.SetActive(true);
            }
            else
            {
                grayStar.gameObject.SetActive(false);
                fiveStar.gameObject.SetActive(false);
                for (int i = 0; i < starGrade; i++)
                {
                    if (i < stars.Length)
                        stars[i].gameObject.SetActive(true);
                }
                for (int i = starGrade; i < stars.Length; i++)
                {
                    stars[i].gameObject.SetActive(false);
                }
            }

            SetPossibleSummon(GridManager.Instance.IsPossibleSummon());

            SummonFixedCharacter = () =>
            {
                GameManager.Instance.SummonFixedCharacter(characterIndex, starGrade);
            };
        }

        public void SetPossibleSummon(bool isPossible)
        {
            if (isFixedSummon)
            {
                if (GridManager.Instance.IsPossibleSummon())
                {
                    button_summonCharacter.SetInterectible(true);
                    summonCharacterActive.SetActive(true);
                }
                else
                {
                    button_summonCharacter.SetInterectible(false);
                    summonCharacterActive.SetActive(false);
                }
            }
            else
            {
                button_summonCharacter.SetInterectible(isPossible);
                summonCharacterActive.SetActive(isPossible);
            }
        }
    }
}
