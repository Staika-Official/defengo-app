using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;

namespace Framework.UI
{
    public class TokenInfo : MonoBehaviour
    {
        public DisplayTokenType displayTokenType;

        public TextMeshProUGUI text_balance;
        public Button button_stikToTik;
        public Button button_tikToStik;
        public Button button_tikToGem;
        public Button button_withdrawal;

        public void Initialize(DisplayTokenType diplayTokenType)
        {
            this.displayTokenType = diplayTokenType;
            switch (diplayTokenType)
            {
                case DisplayTokenType.TAIKA:
                    break;
                case DisplayTokenType.STAIKA:
                    break;
                default:
                    break;
            }
        }
    }
}