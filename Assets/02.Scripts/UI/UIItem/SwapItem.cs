using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SimpleJSON;
using Framework.Network;
using Framework.GameData.Defense;
using System.Globalization;


namespace Framework.UI
{
    public class SwapItem : MonoBehaviour
    {
        public Image image_Button;
        public Button button_Swap;
        public TextMeshProUGUI text_FromTokenBalance;
        public TextMeshProUGUI text_ToTokenBalance;
        public JSONNode json;
        public string title;

        private void Start()
        {
            button_Swap.onClick.AddListener(() => OnClick_Swap());
        }

        public void Initialize(JSONNode json, ExchangeType exchangeType)
        {
            string toTokenKey = "";
            string fromTokenKey = "";
            this.json = json;
            string from = "";

            switch (exchangeType)
            {
                case ExchangeType.STIK_TO_TIK:

                    //Debug.Log("Swap Item In Case");

                    toTokenKey = "<sprite=1>";
                    fromTokenKey = "<sprite=14>";
                    title = "STIK_TO_TIK";
                    image_Button.color = DataManager.Instance.uiPropertyData.dic_walletPropertyColor["background_staika"];

                    //double value = double.Parse($"{json["fromUiAmountString"]}".Replace('"', ' '));
                    //from = value.ToString();
                    from = $"{json["fromUiAmountString"]}".Replace('"', ' ');
                    decimal tempFee = decimal.Parse($"{json["uiFeeString"]}".Replace('"', ' '), new CultureInfo("en-US"));

                    decimal tempToToken = decimal.Parse($"{json["toUiAmountString"]}".Replace('"', ' '), new CultureInfo("en-US"));

                    text_FromTokenBalance.text = $"{toTokenKey}{tempToToken - tempFee}";
                    //Debug.Log("From Token Balance : " + $"{toTokenKey}{tempToToken - tempFee}");
                        //$"{toTokenKey}{json["toUiAmountString"]}".Replace('"', ' ');
                    text_ToTokenBalance.text = $"{fromTokenKey}{from}";

                    //Debug.Log("To Token Balance : " + $"{fromTokenKey}{from}");
                    break;
                case ExchangeType.TIK_TO_STIK:

                    toTokenKey = "<sprite=14>";
                    fromTokenKey = "<sprite=1>";
                    title = "TIK_TO_STIK";
                    image_Button.color = DataManager.Instance.uiPropertyData.dic_walletPropertyColor["background_taika"];
                    decimal fee = decimal.Parse($"{json["uiFeeString"]}".Replace('"', ' '), new CultureInfo("en-US"));
                    decimal fromToken = decimal.Parse($"{json["fromUiAmountString"]}".Replace('"', ' '), new CultureInfo("en-US"));

                    from = $"{fee + fromToken}";

                    text_FromTokenBalance.text = $"{toTokenKey}{json["toUiAmountString"]}".Replace('"', ' ');
                    //Debug.Log("From Token Balance : " + $"{toTokenKey}{json["toUiAmountString"]}".Replace('"', ' '));
                    text_ToTokenBalance.text = $"{fromTokenKey}{from}";
                    //Debug.Log("To Token Balance : " + $"{fromTokenKey}{from}");
                    break;
            }
        }

        public void OnClick_Swap()
        {
            PopupManager.Instance.GetPopUp<SwapPopup>("swapToTik").SwapToken(json, title);
        }
    }
}