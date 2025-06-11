using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Framework.GameData.Defense;
using Framework.UI;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.ObscuredTypes;
using Framework.Network;

namespace Framework.Game.Defense
{
    public class DecButton : MonoBehaviour
    {
        public string characterName;

        public CharacterGrade characterGrade;
        public ButtonComponent button_Dec;
        public TextMeshProUGUI text_LevelValue;
        public TextMeshProUGUI text_CostValue;
        public TextMeshProUGUI text_InactiveCostValue;

        public GameObject[] costValueObject;

        public Image image_Character;
        public Image image_CharacterDarker;
        public Image image_Frame;
        public Image image_BackGround;
        public Image image_BackGroundDarker;
        public GameObject locked;

        public float targetScale;
        public float duration;
        public Ease setEase;

        public ObscuredInt levelValue;
        public ObscuredInt costValue;
        public CharacterIndex characterIndex;
        public bool isPossibleUpgrade;
        public bool isUniqueUpgrade;
        private void Awake()
        {
            ObscuredCheatingDetector.StartDetection(OnCheterDetected);
        }

        private void OnCheterDetected()
        {
            _ = NetworkManager.Instance.AbusingRecord("Cheat Upgrade");
        }

        
        public void Initialize(CharacterData data)
        {
            characterName = data.name;
            characterGrade = data.characterGrade;
            characterIndex = data.characterIndex;
            isPossibleUpgrade = data.isPossibleUpgrade;
            isUniqueUpgrade = data.isUniqueUpgrade;
            levelValue = 1;
            text_LevelValue.text = levelValue.ToString();
            costValue = GetCostValue(characterIndex, isUniqueUpgrade);
            text_CostValue.text = costValue.ToString();
            text_InactiveCostValue.text = costValue.ToString();
            SetCharacterGrade(characterGrade);

            Sprite characterPortrait = data.sprite_ChracterPortrait;
            image_Character.sprite = characterPortrait;
            image_CharacterDarker.sprite = characterPortrait;

            if (data.isPossibleUpgrade)
            {
                button_Dec.onPointerUp = () =>
                {
                    OnClick_UpgradeButton();
                };
                text_CostValue.gameObject.SetActive(true);
                text_InactiveCostValue.gameObject.SetActive(true);
                locked.SetActive(false);
            }
            else
            {
                text_CostValue.gameObject.SetActive(false);
                text_InactiveCostValue.gameObject.SetActive(false);
                locked.SetActive(true);

                float yPos = isPossibleUpgrade ? -70 + 210 : -82 + 210;
                float scaleXY = isPossibleUpgrade ? 1 : 0.95f;

                button_Dec.SetInterectible(false);

                image_CharacterDarker.gameObject.SetActive(true);

                costValueObject[0].SetActive(false);
                costValueObject[1].SetActive(false);
                image_BackGroundDarker.gameObject.SetActive(true);

                transform.localPosition = new Vector2(transform.localPosition.x, yPos);
                transform.localScale = new Vector2(scaleXY, scaleXY);
            }
        }

        public void UpgradeFailed(bool isReset)
        {
            UpgradeType type = isReset ? UpgradeType.RESET : UpgradeType.FAILED;
            
            if (isReset)
            {
                int tempCost = costValue;
                int tempLevel = levelValue;

                levelValue = 1;
                costValue = GetCostValue(characterIndex, isUniqueUpgrade);

                UIManager.Instance.ChangeValueSequnce(costValue, tempCost, text_CostValue);
                UIManager.Instance.ChangeValueSequnce(levelValue, tempLevel, text_LevelValue);

                UIManager.Instance.ChangeValueSequnce(costValue, tempCost, text_InactiveCostValue);
            }
            
            UIManager.Instance.SetUpgradeAnim(characterIndex, levelValue, type);
        }
        
        public void IncreaseCost()
        {
            int tempCost = costValue;
            int tempLevel = levelValue;
            
            levelValue++;
            costValue = CalcCostValue(tempCost, tempLevel);
            
            UIManager.Instance.ChangeValueSequnce(costValue, tempCost, text_CostValue);
            UIManager.Instance.ChangeValueSequnce(levelValue, tempLevel, text_LevelValue);

            UIManager.Instance.ChangeValueSequnce(costValue, tempCost, text_InactiveCostValue);
            UIManager.Instance.SetUpgradeAnim(characterIndex, levelValue, UpgradeType.SUCCESS);
        }

        public int CalcCostValue(int costValue, int levelValue)
        {
            float tempCostValue = costValue + levelValue;
            
            switch (characterIndex)
            {
                case CharacterIndex.HAMMERING :
                    CharacterData characterData = DataManager.Instance.dic_CharacterData[characterIndex];
                    tempCostValue = Mathf.Floor((0.5f * Mathf.Pow(this.levelValue, 2) - 0.5f * this.levelValue + ConfigData.GEM_UPGRADE_FIRST)
                                                * (characterData.characterUniqueValue[3] - (characterData.characterClassLevel - 1) 
                                                    * characterData.classUpFactor[1]));
                    break;
            }
            
            return (int)tempCostValue;
        }
        
        public int GetCostValue(CharacterIndex characterIndex, bool isUniqueUpgrade)
        {
            float tempCostValue = ConfigData.GEM_UPGRADE_FIRST;
            
            if (isUniqueUpgrade)
            {
                switch (characterIndex)
                {
                    case CharacterIndex.HAMMERING :
                        CharacterData characterData = DataManager.Instance.dic_CharacterData[characterIndex];
                        tempCostValue = Mathf.Floor((0.5f * Mathf.Pow(levelValue, 2) - 0.5f * levelValue + ConfigData.GEM_UPGRADE_FIRST)
                            * (characterData.characterUniqueValue[3] - (characterData.characterClassLevel - 1) 
                                * characterData.classUpFactor[1]));
                        break;
                }
            }

            return (int)tempCostValue;
        }

        public void CalcPossibleCost(int gem)
        {
            if (isPossibleUpgrade)
            {
                bool isPossible = costValue <= gem && GameManager.Instance.missionManager.dic_CharacterList[characterIndex].starGradeList.Count > 0;

                float yPos = isPossible ? -70 + 210 : -82 + 210;
                float scaleXY = isPossible ? 1 : 0.95f;

                button_Dec.SetInterectible(isPossible);

                image_CharacterDarker.gameObject.SetActive(!isPossible);

                costValueObject[0].SetActive(isPossible);
                costValueObject[1].SetActive(!isPossible);
                image_BackGroundDarker.gameObject.SetActive(!isPossible);

                transform.localPosition = new Vector2(transform.localPosition.x, yPos);
                transform.localScale = new Vector2(scaleXY, scaleXY);
            }
        }

        public void SetCharacterGrade(CharacterGrade characterGrade)
        {
            CharacterCardInfo data = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[characterGrade];
            image_Frame.sprite = data.sprite_Frame;
            image_BackGround.color = data.color_BackGround;
            image_BackGroundDarker.color = data.disableShadowColor;
        }

        public void OnClick_UpgradeButton()
        {
            if (isUniqueUpgrade)
            {
                GameManager.Instance.CheckUpgradeLevel(characterIndex, costValue, levelValue, IncreaseCost);
            }
            else
            {
                GameManager.Instance.UpgradeLevel(characterIndex, costValue, IncreaseCost);
            }
            
            //TweeningButton();
        }

        public void TweeningButton()
        {
            transform.DOScale(targetScale, duration).OnComplete(() => transform.DOScale(1, duration).SetEase(setEase));
        }
    }
}
