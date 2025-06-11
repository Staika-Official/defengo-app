using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.GameData.Defense;
using TMPro;
using Framework.Util;
using UniRx;
using UniRx.Triggers;

namespace Framework.UI
{
    public class LimitedItem : MonoBehaviour
    {
        public CharacterCard characterCard;
        public CharacterIndex characterIndex;
        public Button button_CharacterInfo;
        public TextMeshProUGUI text_CharacterGrade;
        public TextMeshProUGUI text_CharacterName;

        private void Start()
        {
            button_CharacterInfo.onClick.AddListener(() => OnClick_Button());
        }

        public void Initialize(CharacterIndex characterIndex)
        {
            this.characterIndex = characterIndex;
            CharacterData data = DataManager.Instance.dic_CharacterData[characterIndex];
            
            characterCard.Initialize(data);

            text_CharacterGrade.text = LanguageManager.Instance.GetStringData($"UI_Grade_{(int)data.characterGrade}");
            text_CharacterGrade.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade].color_decGradeText;
            text_CharacterName.text = LanguageManager.Instance.GetStringData(data.characterNameTextKey);
        }

        public void OnClick_Button()
        {
            CharacterData data = DataManager.Instance.dic_CharacterData[characterIndex];
            InventoryScreen.Instance.ActiveEventPopUp(data);
        }
    }
}