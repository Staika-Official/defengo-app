using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using UnityEngine.UI.Extensions;

namespace Framework.UI
{
    public class InventoryItem : MonoBehaviour
    {
        public GameObject objectNewCharacter;
        public Image image_CharacterPortrait;
        public Image image_Frame;
        public Image image_Shadow;
        public Image image_UpperShadow;
        public Image image_UpperFrameShadow;
        public Image image_LowerFrameShadow;
        public Gradient2 gradient;
        public bool isMax;
        public int characterQuantity;
        public int characterRequiredQuantity;
        public int level;
        public TextMeshProUGUI text_Level;
        public TextMeshProUGUI text_requiredQuantity;
        public TextMeshProUGUI text_MaxFillRequiredQuantity;
        public GameObject maxFill;
        public ParticleSystem _ParticleComp;
        public Animation animation_FX;
        public GameObject image_NFT;
        public GameObject image_EquipIcon;
        public Button button_CharacterInfo;
        public CharacterIndex characterIndex;
        public Slider slider_CharacterExp;
        public GameObject[] activeObjects;
        public GameObject maxText;
        public GameObject emptyText;
        public GameObject newCharacter;

        private void Start()
        {
            button_CharacterInfo.onClick.AddListener(() => OnClick_CharacterInfo());
        }

        public void Initialize(CharacterData data, bool isScreenDisplay)
        {
            objectNewCharacter.SetActive(data.isNewInventory);
            data.isNewInventory = false;
            image_CharacterPortrait.sprite = data.sprite_ChracterPortrait;
            text_Level.text = $"LV.{data.characterClassLevel}";
            text_Level.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade].color_LevelText;
            characterIndex = data.characterIndex;
            CharacterCardInfo cardInfo = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade];
            image_Frame.sprite = cardInfo.sprite_Frame;
            image_NFT.SetActive(false);
            image_EquipIcon.SetActive(false);
            
            bool isActive = data.characterClassLevel > 0;

            if (!isActive)
            {
                Color color = DataManager.Instance.uiPropertyData.TxtColor["InventoryItem_Frame_Color_NotOwned"];
                image_LowerFrameShadow.color = color;
                image_UpperFrameShadow.color = color;

                emptyText.SetActive(true);
                image_CharacterPortrait.color = DataManager.Instance.uiPropertyData.TxtColor["DisableDec_Image - CharacterPortrait"];
                image_Frame.color = DataManager.Instance.uiPropertyData.TxtColor["DisableDec_Image - Frame"];
                image_Shadow.color = DataManager.Instance.uiPropertyData.TxtColor["DisableDec_Image - BackgroundShadow"];

                image_UpperShadow.color = cardInfo.disableShadowColor;
                slider_CharacterExp.gameObject.SetActive(false);
                text_Level.gameObject.SetActive(false);
                maxText.SetActive(false);
                gradient.enabled = false;
            }
            else
            {
                emptyText.SetActive(false);
                bool isMaxLevel = data.characterClassLevel == CharacterClassManager.Instance.GetMaxClassLevel();
                gradient.enabled = isMaxLevel;
                string colorKey = isMaxLevel ? "InventoryItem_Frame_Color_MaxLevel" : "InventoryItem_Frame_Color_Owned";

                Color color = DataManager.Instance.uiPropertyData.TxtColor[colorKey];
                image_LowerFrameShadow.color = color;
                image_UpperFrameShadow.color = color;

                text_Level.gameObject.SetActive(!isMaxLevel);
                slider_CharacterExp.gameObject.SetActive(!isMaxLevel);
                maxText.SetActive(isMaxLevel);

                image_CharacterPortrait.color = Color.white;
                image_Frame.color = Color.white;
                image_Shadow.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade].color_BackGround;

                image_UpperShadow.color = cardInfo.disableShadowColor;
            }

            SetQuantity(data);

            if(isScreenDisplay)
            {
                image_EquipIcon.SetActive(data.isUsed);
            }
        }

        public void SetQuantity(CharacterData data)
        {
            text_Level.text = $"LV.{data.characterClassLevel}";

            if(data.characterClassLevel >= CharacterClassManager.Instance.GetMaxClassLevel())
            {
                gradient.enabled = true;
                string colorKey = "InventoryItem_Frame_Color_MaxLevel";

                Color color = DataManager.Instance.uiPropertyData.TxtColor[colorKey];
                image_LowerFrameShadow.color = color;
                image_UpperFrameShadow.color = color;

                text_Level.gameObject.SetActive(false);
                slider_CharacterExp.gameObject.SetActive(false);
                maxText.SetActive(true);
            }
            else
            {
                characterRequiredQuantity = CharacterClassManager.Instance.GetClassUpInfo(data.characterGrade, data.characterClassLevel);
                
                characterQuantity = data.characterQuantity;
            }

            string requiredQuantity = $"{characterQuantity}/{characterRequiredQuantity}";

            if (characterQuantity >= characterRequiredQuantity && characterQuantity != 0 && characterRequiredQuantity != 0)
            {

                maxFill.SetActive(true);
                text_MaxFillRequiredQuantity.text = requiredQuantity;
            }
            else
            {
                maxFill.SetActive(false);

                float value = (float)characterQuantity / (float)characterRequiredQuantity;
                text_requiredQuantity.text = requiredQuantity;
                slider_CharacterExp.value = value;
            }
        }

        public void SetClassUp()
        {

        }

        public void OnClick_CharacterInfo()
        {
            CharacterData data = DataManager.Instance.dic_CharacterData[characterIndex];
            InventoryScreen.Instance.ActivePopUp(data);
        }

        public void PlayPS()
        {
            _ParticleComp.Play();
        }

    }
}
