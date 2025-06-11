using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Framework.Game.Defense;
using Framework.GameData.Defense;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Framework.UI
{

    public class Reinforcement : MonoBehaviour
    {
        public GameObject[] reinForceMentObjects;

        public Image[] buttonBackGroundImages;

        public GameObject[] starObjects;
        public TextMeshProUGUI text_Upgrade;
        public TextMeshProUGUI text_LevelUP;

        public ObscuredInt starValue;
        public ObscuredInt upGradeValue;
        public ObscuredInt levelUpValue;

        public readonly ObscuredInt DefalutStarValue = 0;
        public readonly ObscuredInt DefalutUpgradeValue = 1;
        public ObscuredInt DefalutlevelUpValue;

        public readonly ObscuredInt maxStarValue = 5;
        public readonly ObscuredInt maxUpgradeValue = 10;
        public readonly ObscuredInt maxlevelUpValue = 15;

        public Color buttonDefalutColor;
        public Color buttonChangeColor;

        public CharacterIndex focusCharacterIndex;

        public int MaxCheckForDefalut(ObscuredInt Soruce, ObscuredInt defalut, ObscuredInt max) => (Soruce > max) ? defalut : Soruce;

        public void Initialize(CharacterData characterData)
        {
            for (int i = 0; i < reinForceMentObjects.Length; ++i)
            {
                reinForceMentObjects[i].SetActive(false);
            }

            for(int i =0; i < buttonBackGroundImages.Length; ++i)
            {
                buttonBackGroundImages[i].color = buttonDefalutColor;
            }

            bool isOcean = characterData.characterType == CharacterType.SUB;
            bool isInActive = characterData.characterClassLevel == 0;
            int classLevel = isInActive ? (int)characterData.characterGrade + 1 : characterData.characterClassLevel;

            DefalutlevelUpValue = classLevel;

            starValue = DefalutStarValue;
            upGradeValue = DefalutUpgradeValue;
            levelUpValue = DefalutlevelUpValue;
            
            focusCharacterIndex = characterData.characterIndex;

            if (isOcean)
                OceanInitailize();
            else if (!isOcean && characterData.isPossibleUpgrade)
                GlacierInitailize();
            else
                TempGlacierInitailize();

            text_Upgrade.text = $"Lv.{upGradeValue}";
            text_LevelUP.text = $"Lv.{levelUpValue}";
        }

        public void TempGlacierInitailize()
        {
            reinForceMentObjects[(int)ReinforcementType.STAR].SetActive(true);
            reinForceMentObjects[(int)ReinforcementType.LEVELUP].SetActive(true);

            DefalutStar();
            starObjects[starValue].SetActive(true);
        }


        public void OceanInitailize()
        {
            reinForceMentObjects[(int)ReinforcementType.LEVELUP].SetActive(true);
        }


        public void GlacierInitailize()
        {
            for (int i = 0; i < reinForceMentObjects.Length; ++i)
            {
                reinForceMentObjects[i].SetActive(true);
            }

            DefalutStar();

            starObjects[starValue].SetActive(true);
        }

        public void DefalutStar()
        {
            for (int i = 0; i < starObjects.Length; ++i)
            {
                starObjects[i].SetActive(false);
            }
        }


        public void OnClick_StarReinforment()
        {
            bool isUsed = false;

            ++starValue;

            DefalutStar();

            starValue = MaxCheckForDefalut(starValue, DefalutStarValue, maxStarValue);

            starObjects[starValue].SetActive(true);

            if (starValue != DefalutStarValue)
            {
                buttonBackGroundImages[(int)ReinforcementType.STAR].color = buttonChangeColor;
                isUsed = true;
            }
            else
            {
                buttonBackGroundImages[(int)ReinforcementType.STAR].color = buttonDefalutColor;
            }

            StatusInfoUpdate(isUsed, false, false);
        }

        public void OnClick_UpgradeReinforment()
        {
            bool isUsed = false;

            ++upGradeValue;

            upGradeValue = MaxCheckForDefalut(upGradeValue, DefalutUpgradeValue, maxUpgradeValue);

            text_Upgrade.text = $"Lv.{upGradeValue}";

            if (upGradeValue != DefalutUpgradeValue)
            {
                buttonBackGroundImages[(int)ReinforcementType.UPGRADE].color = buttonChangeColor;
                isUsed = true;
            }
            else
            {
                buttonBackGroundImages[(int)ReinforcementType.UPGRADE].color = buttonDefalutColor;
            }

            StatusInfoUpdate(false, isUsed, false);
        }

        public void OnClick_LevelReinforment()
        {
            bool isUsed = false;

            ++levelUpValue;

            levelUpValue = MaxCheckForDefalut(levelUpValue, DefalutlevelUpValue, maxlevelUpValue);

            text_LevelUP.text = $"Lv.{levelUpValue}";

            if (levelUpValue != DefalutlevelUpValue)
            {
                buttonBackGroundImages[(int)ReinforcementType.LEVELUP].color = buttonChangeColor;
                isUsed = true;
            }
            else
            {
                buttonBackGroundImages[(int)ReinforcementType.LEVELUP].color = buttonDefalutColor;
            }

            StatusInfoUpdate(false, false, isUsed);
        }

        public void StatusInfoUpdate(bool isStar, bool isUpgrade, bool isLevelUp)
        {
            CharacterData data = DataManager.Instance.dic_CharacterData[focusCharacterIndex];

            StatusInfo[] statusInfos = PopupManager.Instance.GetPopUp<InventoryPopup>("inventory").statusInfos;
            StatusInfo fieldStatusInfo = PopupManager.Instance.GetPopUp<InventoryPopup>("inventory").fieldStatusInfo;

            IncreaseStruct SendIncreaseStruct = new IncreaseStruct(starValue, upGradeValue, 0, levelUpValue, isStar, isUpgrade, isLevelUp);

            bool isOcean = data.characterType == CharacterType.SUB;

            if (isOcean)
            {
                fieldStatusInfo.ReinForceMentFieldBuffInitialize(data.fieldBuffType, data, levelUpValue);
            }
            else
            {
                for (int i = 0; i < data.statusTypes.Length; ++i)
                {
                    statusInfos[i].ReinforcementInitalize(data.statusTypes[i], data, SendIncreaseStruct);
                }
            }
        }



    }
}
