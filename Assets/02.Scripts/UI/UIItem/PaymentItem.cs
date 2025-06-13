using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.Util;
using Framework.GameData.Defense;

namespace Framework.UI
{
    public class PaymentItem : MonoBehaviour
    {
        public TextMeshProUGUI text_ItemName;
        public TextMeshProUGUI text_Price;
        public TextMeshProUGUI text_Date;
        public Image image_Icon;

        public void Initialize(UserStoreItem data)
        {
            text_ItemName.text = LanguageManager.Instance.GetStringData(data.code);
            text_Price.text = $"{data.paidAmount}";
            text_Date.text = Convert.ToDateTime(data.createdDate).ToString("HH:mm");
        }

        public void SetWalletData(Transaction data, HistoryType historyType)
        {
            text_ItemName.text = LanguageManager.Instance.GetStringData(data.content);
            text_Date.text = Convert.ToDateTime(data.createdDate).ToString("HH:mm");

            try
            {
                image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_WalletHistoryIcons[data.content ?? ""];
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log(e);
                image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_WalletHistoryIcons["WELCOME"];
            }

            Color color = DataManager.Instance.uiPropertyData.dic_WalletTextColor[data.type];
            text_Price.color = color;

            string mark = data.type == "IN" ? "+" : "-";

            switch (historyType)
            {
                case HistoryType.TAIKA:
                    text_Price.text = $"{mark}{data.uiAmountString}";
                    break;
                case HistoryType.STAIKA:
                    string stikValue = data.uiAmountString;
                    bool isPoint = stikValue.Contains('.');

                    if (isPoint)
                    {
                        if (data.type == "IN")
                        {
                            text_Price.text = mark + data.uiAmountString.Remove(data.uiAmountString.IndexOf('.') + 5);
                        }
                        else
                        {
                            text_Price.text = $"{mark}{data.uiAmountString}";
                        }
                    }
                    else
                    {
                        text_Price.text = $"{mark}{data.uiAmountString}";
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
