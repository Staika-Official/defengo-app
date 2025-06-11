using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.GameData.Defense;
using UnityEngine.UI;
using Framework.Sound;
using Framework.Network;
using TMPro;
using System.Globalization;
using SimpleJSON;
using Framework.Util;

namespace Framework.UI
{
    public enum HistoryType
    {
        TAIKA,
        STAIKA
    }

    public enum ExchangeType
    {
        STIK_TO_TIK,
        TIK_TO_STIK
    }

    public class WalletScreen : ScreenTemplate
    {
        public static WalletScreen Instance;
        public Button button_PurchaseDetails;
        public Button button_StaikaDetails;

        #region Token
        [Header("Token")]
        public Button button_stikToTik;
        public Button button_tikToStik;
        public Button button_gemSwap;
        public Button button_transferStikWallet;
        public Button button_staikaWebWallet;
        public Button button_PenggoWallet;
        public TextMeshProUGUI text_taikaBalance;
        public TextMeshProUGUI text_stikBalance;
        public ExchangeType exchangeType;

        public GameObject taikaGroup;
        public GameObject staikaGroup;

        private void Start()
        {
            Instance = this;
        }

        public void OnClick_TokenSwap(ExchangeType exchangeType)
        {
            this.exchangeType = exchangeType;
            string tokenSymbol = exchangeType switch
            {
                ExchangeType.STIK_TO_TIK => "STIK_TO_TIK",
                ExchangeType.TIK_TO_STIK => "TIK_TO_STIK",
                _ => "STIK_TO_TIK"
            };

            GetTokenProductList(tokenSymbol);
        }
        #endregion


        public override void ActiveScreen()
        {
            SetUserWalletBalance();
        }

        public void SetUserWalletBalance()
        {
            text_taikaBalance.text = UserInfoManager.Instance.userTikWalletHistory.balance.uiAmountString;

            //string stikBalance = UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString;
            text_stikBalance.text = UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString;
            //text_stikBalance.text = UserInfoManager.Instance.GetUserStikAmount();

            //stikBalance.Remove(stikBalance.IndexOf('.') + 9);
        }

        public override void InactiveScreen()
        {

        }

        public override void Initialize()
        {
            if (UserInfoManager.Instance.IsBlockRegion)
            {
                button_staikaWebWallet.gameObject.SetActive(false);
                button_tikToStik.gameObject.SetActive(false);
                staikaGroup.SetActive(false);
                RectTransform rect = button_gemSwap.transform as RectTransform;
                rect.anchoredPosition = new(-120.3f, 55);
            }

            button_staikaWebWallet.onClick.AddListener(() => OnClick_WebWallet());
            button_stikToTik.onClick.AddListener(() => OnClick_TokenSwap(ExchangeType.STIK_TO_TIK));
            button_tikToStik.onClick.AddListener(() => OnClick_TokenSwap(ExchangeType.TIK_TO_STIK));
            button_transferStikWallet.onClick.AddListener(() =>
            {
                if (UserInfoManager.Instance.userState.userStatus == "GUEST")
                {
                    SystemNoticePopup systemNoticePopup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                    systemNoticePopup.SetNoticeMessage("MSG_Permission_Denied");
                    return;
                }

                TransferPopup popup = PopupManager.Instance.GetPopUp<TransferPopup>("transfer");
                popup.ActivePopup();
            });

            button_PurchaseDetails.onClick.AddListener(() => OnClick_PurchaseDetail(HistoryType.TAIKA));
            button_StaikaDetails.onClick.AddListener(() => OnClick_PurchaseDetail(HistoryType.STAIKA));

            button_gemSwap.onClick.AddListener(() =>
            {
                LobbyNavigator.Instance.OnClick_Screen(MenuType.SHOP);
                ShopScreen.Instance.TogglePage(PageState.GEM);
            });
        }

        public void OnClick_PurchaseDetail(HistoryType historyType)
        {
            WalletHistoryPopup walletHistoryPopup = PopupManager.Instance.GetPopUp<WalletHistoryPopup>("walletHistory");
            walletHistoryPopup.historyType = historyType;
            walletHistoryPopup.PopUpSequence(true);
            walletHistoryPopup.ActivePopup();
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
        }

        public async void GetTokenProductList(string tokenSymbol)
        {
            await NetworkManager.Instance.GetTokenProductList(SetProductList, Failed, tokenSymbol);
        }

        public void Failed()
        {
            Debug.Log("Get Token Product Failed");
        }

        public void SetProductList(string data)
        {
            //Debug.Log(data);
            //            Debug.Log(data);

            JSONNode json = JSONNode.Parse(data);

            SwapPopup popup = PopupManager.Instance.GetPopUp<SwapPopup>("swapToTik");
            popup.SetTokenExchangeInfo(json, exchangeType);
        }

        public void OnClick_WebWallet()
        {
            if (UserInfoManager.Instance.userState.userStatus == "GUEST")
            {
                SystemNoticePopup systemNoticePopup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                systemNoticePopup.SetNoticeMessage("MSG_Permission_Denied");
                return;
            }

            string lanKey = LanguageManager.Instance.GetLanguageKey();
            string walletUrl = NetworkManager.Instance.applicationState switch
            {
                ApplicationState.DEV => $"https://wallet.stage.staika.io/?hl={lanKey}",
                ApplicationState.STAGE => $"https://wallet.stage.staika.io/?hl={lanKey}",
                ApplicationState.PRODUCTION => $"https://wallet.staika.io/?hl={lanKey}",
                _ => $"https://wallet.stage.staika.io/?hl={lanKey}"
            };

            Webview.Instance.ShowWebViewPopup(walletUrl);
        }

        public void SetUserWalletHistory()
        {
            _ = NetworkManager.Instance.GetUserWalletHistory();
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}
