using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using TMPro;
using System;
using Framework.Util;
using UnityEngine.UI;
using System.Globalization;

namespace Framework.UI
{
    public class WalletHistoryPopup : PopupTemplate
    {
        public ScrollRect scrollRect;
        public RectTransform rect_Scroll;
        public TextMeshProUGUI text_Balance;
        public TextMeshProUGUI text_Header;
        public Image image_CoinSymbol;
        public GameObject itemBox;
        public GameObject groupByDate;
        public List<GameObject> itemBoxes = new();
        public HistoryType historyType;
        
        public Dictionary<string, List<Transaction>> dic_DateGroup = new();

        public override void ActivePopup()
        {
            rect_Scroll.anchoredPosition = Vector2.zero;
            string headerKey = "";
            string symbolKey = "";            
            switch (historyType)
            {
                case HistoryType.TAIKA:
                    //text_Balance.text = UserInfoManager.Instance.userTikWalletHistory.balance.amount.ToString("N0", new CultureInfo("en-US"));
                    text_Balance.text = UserInfoManager.Instance.userTikWalletHistory.balance.uiAmountString;
                    text_Header.color = DataManager.Instance.uiPropertyData.dic_walletPropertyColor["text_taika"];
                    symbolKey = "<sprite=1>";
                    //headerKey = "UI_Taika";
                    headerKey = "Taika";
                    break;
                case HistoryType.STAIKA:
                    //headerKey = "UI_Staika";
                    headerKey = "Staika";
                    symbolKey = "<sprite=14>";
                    //double value = double.Parse(UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString);
                    text_Header.color = DataManager.Instance.uiPropertyData.dic_walletPropertyColor["text_staika"];
                    text_Balance.text = UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString;
                    //string.Format("{0:0.#######0}", value);
                    break;
                default:
                    break;
            }


            //text_Header.text = symbolKey + LanguageManager.Instance.GetStringData(headerKey);
            text_Header.text = symbolKey + headerKey;
            SetGroupByDateTime(historyType);
        }

        public override void InActivePopup()
        {
            for (int i = 0; i < itemBoxes.Count; i++)
            {
                Destroy(itemBoxes[i]);
            }

            itemBoxes.Clear();
            dic_DateGroup.Clear();

            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() => OnClick_Close());

            rect_Scroll.anchoredPosition = Vector2.zero;
        }

        public void SetGroupByDateTime(HistoryType historyType)
        {
            Transaction[] datas = historyType switch
            {
                HistoryType.STAIKA => UserInfoManager.Instance.userStikWalletHistory.transactions,
                HistoryType.TAIKA => UserInfoManager.Instance.userTikWalletHistory.transactions,
                _ => null
            };

            for (int i = 0; i < datas.Length; i++)
            {
                string date = Convert.ToDateTime(datas[i].createdDate).ToString("yyyy-MM-dd");

                bool isThing = dic_DateGroup.ContainsKey(date);

                if(isThing)
                {
                    dic_DateGroup[date].Add(datas[i]);
                }
                else
                {
                    List<Transaction> list = new();
                    list.Add(datas[i]);
                    dic_DateGroup.Add(date, list);
                }
            }

            float offsetY = 0.0f;

            int dayCount = dic_DateGroup.Count;
            int dataCount = datas.Length;

            rect_Scroll.sizeDelta = new Vector2(824, (dataCount * 142) + (dayCount * 80) + ((dataCount + dayCount) * 8));

            foreach (var item in dic_DateGroup)
            {
                GroupByDate obj = Instantiate(groupByDate).GetComponent<GroupByDate>();
                itemBoxes.Add(obj.gameObject);
                obj.transform.SetParent(rect_Scroll);
                obj.transform.localScale = Vector2.one;
                obj.transform.localPosition = new Vector2(0, offsetY);
                string dateInfo = item.Key.Replace('-', '.');
                obj.text_DateInfo.text = dateInfo;

                offsetY -= 138;

                for (int i = 0; i < dic_DateGroup[item.Key].Count; i++)
                {
                    PaymentItem paymentItem = Instantiate(itemBox).GetComponent<PaymentItem>();
                    itemBoxes.Add(paymentItem.gameObject);
                    paymentItem.transform.SetParent(rect_Scroll);
                    paymentItem.transform.localScale = Vector2.one;
                    paymentItem.transform.localPosition = new Vector2(0, offsetY);
                    paymentItem.SetWalletData(dic_DateGroup[item.Key][i], historyType);
                    offsetY -= 138;
                }
            }
        }

        public void OnClick_Close()
        {
            InActivePopup();
        }
    }
}
