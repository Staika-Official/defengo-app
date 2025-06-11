using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using DG.Tweening;
using Framework.GameData.Defense;
using System.Globalization;
using SimpleJSON;
using CodeStage.AntiCheat.ObscuredTypes;
using Framework.Util;
using System;

namespace Framework.UI
{
    public class SwapPopup : PopupTemplate
    {
        public TextMeshProUGUI text_DisplayFromTokenName;
        public TextMeshProUGUI text_FromTokenName;
        public TextMeshProUGUI text_FromBalance;

        public TextMeshProUGUI text_DisplayToTokenName;
        public TextMeshProUGUI text_ToTokenName;
        public TextMeshProUGUI text_ToTokenBalance;

        public TextMeshProUGUI text_FeeInfo;
        public TextMeshProUGUI text_Fee;


        public Transform btn_bg;
        public Transform arrow_img;
        public Image image_Dim;
        public RectTransform rect_Popup;

        public SwapItem[] swapItems;

        public ObscuredDecimal krw;
        public ObscuredDecimal usd;

        public float startY = 0;
        public float endY = 0;


        private GameObject toastSwapAlert;
        private TransferAlert toastTransferAlert;

        public override void ActivePopup()
        {
            PopUpSequence(true);
            image_Dim.DOFade(0, 0).OnComplete(() => image_Dim.DOFade(0.98f, 0.2f));
            rect_Popup.DOLocalMoveY(-3250f, 0.2f).SetAutoKill(true).SetEase(Ease.OutQuad).From();
            btn_bg.transform.DOScale(0f, 0.3f).SetAutoKill(true).SetDelay(0.25f).SetEase(Ease.OutBack).From();
            arrow_img.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).SetDelay(0.25f).SetAutoKill(true).SetEase(Ease.OutExpo).From();
        }

        public override void InActivePopup()
        {
            image_Dim.DOFade(0f, 0f);
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() => InActivePopup());
            toastTransferAlert = PopupManager.Instance.toastTransferAlert;
            toastSwapAlert = PopupManager.Instance.toastSwapAlert;
        }

        public void SwapToken(JSONNode json, string title)
        {

            bool isTik = json["fromTokenSymbol"] == "TIK";
            bool isValidationBalance = false;
            double amount = isTik ? UserInfoManager.Instance.userTikWalletHistory.balance.amount : UserInfoManager.Instance.userStikWalletHistory.balance.amount;
            if (isTik)
            {
                double tikAmount = UserInfoManager.Instance.userTikWalletHistory.balance.amount;
                double tikPrice = double.Parse(json["fromUiAmountString"]) + double.Parse(json["uiFeeString"]);
                isValidationBalance = tikAmount >= tikPrice;
            }
            else
            {
                CultureInfo cultureInfo = new("en-US");
                double stikAmount = UserInfoManager.Instance.userStikWalletHistory.balance.amount;
                string stikUi = json["fromUiAmountString"];
                stikUi = stikUi.Replace(".", "");
                double stikPrice = double.Parse(stikUi);
                isValidationBalance = stikAmount >= stikPrice;
            }

            if (isValidationBalance)
            {
                SwapToken swapToken = new()
                {
                    userId = int.Parse(UserInfoManager.Instance.userId),
                    title = title,
                    fromTokenSymbol = json["fromTokenSymbol"],
                    fromUiAmount = json["fromUiAmountString"],
                    toTokenSymbol = json["toTokenSymbol"],
                    toUiAmount = json["toUiAmountString"],
                    feeTokenSymbol = "TIK",
                    feeUiAmount = json["uiFeeString"],
                    priceKRW = double.Parse(krw.ToString()),
                    priceUSD = double.Parse(usd.ToString())
                };

                string data = JsonUtility.ToJson(swapToken);

                _ = NetworkManager.Instance.SwapToken(data, OnSuccess, OnFailed);
                toastSwapAlert.SetActive(true);
            }
            else
            {
                OnFailed("");
            }

        }

        public async void OnSuccess()
        {
            await NetworkManager.Instance.GetUserWalletHistory();

            WalletScreen.Instance.SetUserWalletBalance();

            InActivePopup();

            toastSwapAlert.SetActive(false);

            toastTransferAlert.gameObject.SetActive(true);
            toastTransferAlert.SetSwapAlert("EXCHANGE_SUCCESS", true);
        }

        public void OnFailed(string failedMessage)
        {
            toastSwapAlert.SetActive(false);
            toastTransferAlert.gameObject.SetActive(true);
            toastTransferAlert.SetFailedAlert("EXCHANGE_FAILED", true);
        }

        public void SetTokenExchangeInfo(JSONNode json, ExchangeType exchangeType)
        {
            decimal krw = decimal.Parse(json["quotes"]["priceKRW"], new CultureInfo("en-US"));
            this.krw = krw;
            decimal usd = decimal.Parse(json["quotes"]["priceUSD"], new CultureInfo("en-US"));
            this.usd = usd;

            Color taikaColor = DataManager.Instance.uiPropertyData.dic_walletPropertyColor["text_taika"];
            Color staikaColor = DataManager.Instance.uiPropertyData.dic_walletPropertyColor["text_staika"];

            string feeRate = "";

            switch (exchangeType)
            {
                case ExchangeType.STIK_TO_TIK:
                    text_FromBalance.text = UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString;//.ToString("N3", new CultureInfo("en-US"));
                    text_DisplayFromTokenName.text = "<sprite=14>Staika";
                    text_DisplayToTokenName.text = "<sprite=1>Taika";
                    text_ToTokenBalance.text = UserInfoManager.Instance.userTikWalletHistory.balance.uiAmountString;
                    text_FromTokenName.text = "<sprite=14>Staika";
                    text_FromTokenName.color = staikaColor;
                    text_ToTokenName.text = "<sprite=1>Taika";
                    text_ToTokenName.color = taikaColor;
                    text_DisplayToTokenName.color = taikaColor;
                    text_DisplayFromTokenName.color = staikaColor;
                    feeRate = "10%";
                    break;
                case ExchangeType.TIK_TO_STIK:
                    text_FromBalance.text = UserInfoManager.Instance.userTikWalletHistory.balance.uiAmountString;
                    text_ToTokenBalance.text = UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString;
                    text_DisplayFromTokenName.text = "<sprite=1>Taika";
                    text_DisplayFromTokenName.color = taikaColor;
                    text_DisplayToTokenName.text = "<sprite=14>Staika";
                    text_DisplayToTokenName.color = staikaColor;
                    text_FromTokenName.text = "<sprite=1>Taika";
                    text_FromTokenName.color = taikaColor;
                    text_ToTokenName.text = "<sprite=14>Staika";
                    text_ToTokenName.color = staikaColor;
                    feeRate = "20%";
                    break;
                default:
                    break;
            }

            for (int i = 0; i < json["products"].Count; i++)
            {
                swapItems[i].Initialize(json["products"][i], exchangeType);
            }

            decimal balance = krw;

            string balanceText = $"<sprite=14>1 <sprite=17><sprite=1>{balance}";
            text_FeeInfo.text = balanceText;

            string now = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            string feeText = string.Format(LanguageManager.Instance.GetStringData("TIK_TO_STIK_DESC"), now, feeRate);

            text_Fee.text = feeText;

            ActivePopup();
        }
    }
}