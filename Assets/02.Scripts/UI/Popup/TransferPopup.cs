using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using Framework.Network;
using TMPro;
using CodeStage.AntiCheat.ObscuredTypes;
using Framework.GameData.Defense;
using System.Globalization;

namespace Framework.UI
{
    public class TransferPopup : PopupTemplate
    {
        public Button button_Transfer;

        public GameObject possibleTransfer;
        public GameObject impossibleTransfer;

        public GameObject possibleCancel;
        public GameObject impossibleCancel;

        public GameObject transferText;
        public GameObject indicator;

        public TMP_InputField inputField_StikValue;
        public TextMeshProUGUI text_StaikaBalance;
        public ObscuredInt staikaBalance;
        public Image image_ButtonBackGround;
        public SerializableDictionary<bool, Color> dic_BackGroundColor;

        private TransferAlert transferAlert;

        public override void ActivePopup()
        {
            PopUpSequence(true);
            inputField_StikValue.text = "";
            inputField_StikValue.Select();

            button_Transfer.interactable = false;

            possibleTransfer.SetActive(false);
            impossibleTransfer.SetActive(true);

            transferText.SetActive(true);
            indicator.SetActive(false);

            //double value = double.Parse(UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString);
            //text_StaikaBalance.text = "Balance : " + string.Format("{0:0.#######.0}", value);
            text_StaikaBalance.text = "Balance : " + UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString;
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() => InActivePopup());
            button_Transfer.onClick.AddListener(() => OnClick_Transfer());
            inputField_StikValue.keyboardType = TouchScreenKeyboardType.DecimalPad;

            inputField_StikValue.onValueChanged.AddListener((value) => OnComplete_Inputfield(value));

            transferAlert = PopupManager.Instance.toastTransferAlert;
        }

        public void OnComplete_Inputfield(string inputfield)
        {
            if (!string.IsNullOrEmpty(inputfield))
            {
                double value = double.Parse(inputfield);

                if (value < 0)
                {
                    //transferAlert.SetFailedAlert("EXCHANGE_FAILED_EXCHANGE", true);
                    return;
                }

                double currentStikValue = double.Parse(UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString);

                //Debug.Log(currentStikValue);

                bool isPossibleTransfer = currentStikValue >= value;

                if (!isPossibleTransfer)
                {
                    inputField_StikValue.text = string.Format("{0:0.#######.0}", currentStikValue);
                    isPossibleTransfer = true;
                }

                button_Transfer.interactable = isPossibleTransfer;
                possibleTransfer.SetActive(isPossibleTransfer);
                impossibleTransfer.SetActive(!isPossibleTransfer);
            }
        }

        public void OnClick_Transfer()
        {
            double value = double.Parse(inputField_StikValue.text);

            if (value < 10)
            {
                transferAlert.gameObject.SetActive(true);
                transferAlert.SetFailedAlert("EXCHANGE_FAILED_EXCHANGE", true);
                return;
            }

            transferText.SetActive(false);
            indicator.SetActive(true);
            button_Transfer.interactable = false;
            possibleCancel.SetActive(false);
            impossibleCancel.SetActive(true);
            _ = NetworkManager.Instance.ExchangeToken(value, OnSuccess, OnFailed);
        }

        public async void OnSuccess()
        {
            InActivePopup();

            await NetworkManager.Instance.GetUserWalletHistory();

            transferText.SetActive(true);
            indicator.SetActive(false);

            WalletScreen.Instance.SetUserWalletBalance();

            double value = double.Parse(inputField_StikValue.text);

            transferAlert.SetSuccessAlert("Transfer_Pending_Desc", value, true);

            transferAlert.gameObject.SetActive(true);

            possibleCancel.SetActive(true);
            impossibleCancel.SetActive(false);
        }

        public void OnFailed(string errorMessage)
        {
            button_Transfer.interactable = true;

            transferText.SetActive(true);
            indicator.SetActive(false);
            transferAlert.SetFailedAlert(errorMessage, true);
            transferAlert.gameObject.SetActive(true);

            possibleCancel.SetActive(true);
            impossibleCancel.SetActive(false);
        }
    }
}