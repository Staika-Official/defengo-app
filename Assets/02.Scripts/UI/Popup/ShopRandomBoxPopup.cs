using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.GameData.Defense;
using Framework.Util;
using System;

namespace Framework.UI
{
    public class ShopRandomBoxPopup : PopupTemplate
    {
        [SerializeField] private Image image_BoxIcon;

        [SerializeField] private Button button_Once;
        [SerializeField] private Button button_Many;
        [SerializeField] private Button button_Skip;
        [SerializeField] private GameObject object_SkipEnable;

        [SerializeField] private TextMeshProUGUI text_Common;
        [SerializeField] private TextMeshProUGUI text_Uncommon;
        [SerializeField] private TextMeshProUGUI text_Rare;
        [SerializeField] private TextMeshProUGUI text_Epic;
        [SerializeField] private TextMeshProUGUI text_Legendary;

        [SerializeField] private RectTransform transform_CommonTitle;
        [SerializeField] private RectTransform transform_UncommonTitle;
        [SerializeField] private RectTransform transform_RareTitle;
        [SerializeField] private RectTransform transform_EpicTitle;
        [SerializeField] private RectTransform transform_LegendaryTitle;

        [SerializeField] private TextMeshProUGUI text_BoxName;

        [SerializeField] private TextMeshProUGUI text_OncePrice;
        [SerializeField] private TextMeshProUGUI text_ManyPrice;

        [SerializeField] private GameObject activeObject;
        [SerializeField] private GameObject inactiveObject;

        [SerializeField] private MoneyType moneyType;
        [SerializeField] private StoreType storeType;

        [SerializeField] private GameObject object_ceiling;
        [SerializeField] private Image image_ceiling;
        [SerializeField] private TextMeshProUGUI text_ceiling;

        private IEnumerator timer;

        private string itemKey;
        private string code;
        private float price;
        private bool isItemAvailable;
        private bool isSkip;
        private RequestBuyItem curRequestBuyItemData = new RequestBuyItem();
        [SerializeField] private SerializableDictionary<bool, float> dic_ShoptitleProperty;

        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            if (timer != null)
            {
                StopCoroutine(timer);
                timer = null;
            }
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            isSkip = SecurePlayerPrefs.GetInt("ShopRandomBoxSkip", 0) == 1;
            object_SkipEnable.SetActive(isSkip);

            button_Close.onClick.AddListener(() =>
            {
                InActivePopup();
            });

            button_Once.onClick.AddListener(() => OnClick_BuyRandomBox(true));
            button_Many.onClick.AddListener(() => OnClick_BuyRandomBox(false));
            button_Skip.onClick.AddListener(() => OnClick_Skip());
        }


        public void SetRandoxBoxInfo(ShopItem item)
        {
            itemKey = item.itemKey;
            code = item.code;
            moneyType = item.moneyType;
            storeType = item.storeType;
            isItemAvailable = item.isAvailable;
            float price = item.price;

            string[] temp = code.Split('_');

            string randomBoxCode = $"{temp[0]}_{temp[1]}";
            image_BoxIcon.sprite = DataManager.Instance.uiPropertyData.dic_RandomBoxSprite[randomBoxCode].sprite_Icon;
            text_BoxName.text = LanguageManager.Instance.GetStringData(randomBoxCode);

            SetEmergeRate(randomBoxCode);

            SetCeiling(item.userCeiling, item.maxCeiling);

            button_Once.gameObject.SetActive(true);
            switch (item.moneyType)
            {
                case MoneyType.TAIKA:
                    break;
                case MoneyType.STIK:
                    this.price = price;
                    button_Once.gameObject.SetActive(false);
                    //button_Once.interactable = true;
                    inactiveObject.SetActive(false);
                    activeObject.SetActive(true);

                    button_Many.gameObject.SetActive(true);
                    text_ManyPrice.text = "<sprite=14>" + price.ToString();
                    //text_OncePrice.text = "<sprite=14>" + (price * 0.1f).ToString();
                    break;
                case MoneyType.ADMOB:
                    button_Many.gameObject.SetActive(false);
                    text_OncePrice.text = "<sprite=6>" + "Free";

                    bool isActive = item.isAvailable;
                    activeObject.SetActive(isActive);
                    inactiveObject.SetActive(!isActive);
                    button_Once.interactable = isActive;
                    if (!isActive)
                    {
                        timer = SetAdmobTimer(item.remainTime);
                        StartCoroutine(timer);
                    }
                    break;
                case MoneyType.GEM:
                    this.price = price;
                    button_Once.interactable = true;
                    inactiveObject.SetActive(false);
                    activeObject.SetActive(true);

                    button_Many.gameObject.SetActive(true);
                    text_ManyPrice.text = "<sprite=2>" + price.ToString();
                    text_OncePrice.text = "<sprite=2>" + ((int)(price * 0.1f)).ToString();

                    break;
                default:
                    break;
            }

            if (item.code.Contains("LEGEND"))
            {
                button_Once.gameObject.SetActive(true);
                button_Many.gameObject.SetActive(false);
                text_OncePrice.text = "<sprite=14>" + price.ToString();
            }
        }

        private void SetEmergeRate(string code)
        {
            EmergeRate emergeRate = UserInfoManager.Instance.dic_RandomboxEmergeRate[code];

            bool isLegend = code.Contains("LEGEND");

            transform_CommonTitle.gameObject.SetActive(!isLegend);
            transform_UncommonTitle.gameObject.SetActive(!isLegend);
            transform_RareTitle.gameObject.SetActive(!isLegend);
            transform_EpicTitle.gameObject.SetActive(!isLegend);

            if (!isLegend)
            {
                text_Common.text = $"{emergeRate.normal * 100}%";
                text_Uncommon.text = $"{emergeRate.rare * 100}%";
                text_Rare.text = $"{emergeRate.unique * 100}%";
                text_Epic.text = $"{emergeRate.epic * 100}%";
            }

            text_Legendary.text = $"{emergeRate.legend * 100}%";

            transform_LegendaryTitle.anchoredPosition = new Vector3(transform_LegendaryTitle.anchoredPosition.x, dic_ShoptitleProperty[isLegend], 0f);
        }

        private void SetCeiling(int user, int max)
        {
            if (max == 0)
            {
                object_ceiling.SetActive(false);
                return;
            }

            object_ceiling.SetActive(true);
            text_ceiling.text = $"{user}/{max}";
            image_ceiling.fillAmount = (float)user / (float)max;
        }

        public void OnClick_BuyRandomBox(bool isOnce)
        {
            if (storeType == StoreType.SPECIAL && !isItemAvailable)
            {
                Failed_Limited();
                return;
            }

            ButtonInteractable(false);
            Debug.Log("randombox");
            switch (moneyType)
            {
                case MoneyType.TAIKA:
                    double tikPrice = isOnce ? (this.price * 0.1f) : this.price;
                    double myTik = double.Parse(UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString);
                    if (tikPrice > myTik)
                    {
                        Failed_Enough();
                        return;
                    }

                    string[] split = code.Split('_');
                    string tikBoxCode = TranslateStoreRandomBoxCode(isOnce, split);

                    curRequestBuyItemData.Set(UserInfoManager.Instance.userId, "DEFENGO", tikBoxCode, false);

                    _ = NetworkManager.Instance.RequestBuyItem(curRequestBuyItemData, Success, Failed);
                    break;
                case MoneyType.STIK:
                    double stikPrice = isOnce ? (this.price * 0.1f) : this.price;
                    double myStik = double.Parse(UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString);
                    if (stikPrice > myStik)
                    {
                        Failed_Enough();
                        return;
                    }

                    string[] splitArray = code.Split('_');
                    string boxCode = TranslateStoreRandomBoxCode(isOnce, splitArray);

                    curRequestBuyItemData.Set(UserInfoManager.Instance.userId, "DEFENGO", boxCode, false);

                    _ = NetworkManager.Instance.RequestBuyItem(curRequestBuyItemData, Success, Failed);
                    break;
                case MoneyType.ADMOB:
                    AdmobManager.Instance.ShowRewardedAd();

                    AdmobManager.onSuccessWatchAd = () =>
                    {
                        curRequestBuyItemData.Clear();
                        
                        _ = NetworkManager.Instance.RegistUserAdValid("BOX_NORMAL_AD_1", "random-box", AdSuccess, Failed_Enough);
                        PopUpSequence(false);
                    };
                    break;
                case MoneyType.GEM:
                    float price = isOnce ? (int)(this.price * 0.1f) : this.price;

                    if (price > UserInfoManager.Instance.gemValue)
                    {
                        Failed_Enough();
                        return;
                    }

                    string[] codeSplitArray = code.Split('_');
                    string randomBoxCode = TranslateStoreRandomBoxCode(isOnce, codeSplitArray);

                    curRequestBuyItemData.Set(UserInfoManager.Instance.userId, "DEFENGO", randomBoxCode, false);

                    _ = NetworkManager.Instance.RequestBuyItem(curRequestBuyItemData, Success, Failed);
                    break;
                default:
                    break;
            }
        }

        private void OnClick_Skip()
        {
            isSkip = !isSkip;
            object_SkipEnable.SetActive(isSkip);
            SecurePlayerPrefs.SetInt("ShopRandomBoxSkip", isSkip ? 1 : 0);
        }

        private string TranslateStoreRandomBoxCode(bool isOnce, string[] randomBoxCode)
        {
            string boxCode = "";

            switch (storeType)
            {
                case StoreType.NORMAL:
                    if (moneyType == MoneyType.STIK)
                    {
                        boxCode = isOnce ? $"{randomBoxCode[0]}_{randomBoxCode[1]}_1_STIK" : $"{randomBoxCode[0]}_{randomBoxCode[1]}_10_STIK";
                    }
                    else
                    {
                        boxCode = isOnce ? $"{randomBoxCode[0]}_{randomBoxCode[1]}_1" : $"{randomBoxCode[0]}_{randomBoxCode[1]}_10";
                    }
                    break;
                case StoreType.SPECIAL:
                    boxCode = isOnce ? $"{randomBoxCode[0]}_{randomBoxCode[1]}_SPECIAL_1" : $"{randomBoxCode[0]}_{randomBoxCode[1]}_SPECIAL_10";
                    break;
                default:
                    break;
            }

            return boxCode;
        }

        private void GetUserWallet()
        {
            _ = NetworkManager.Instance.GetUserWalletHistory(SuccessTikWallet, SuccessStikWallet);
            _ = NetworkManager.Instance.GetUserWalletInfo(SuccessPtikWallet, "PTIK");
        }

        private void SuccessPtikWallet(int gem)
        {
            UserInfoManager.Instance.gemValue = gem;
            LobbyManager.Instance.text_Gem.text = $"{gem}";
        }
        private void SuccessStikWallet()
        {
            double stikBalance = Double.Parse(UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString);
            LobbyManager.Instance.text_Stik.text = $"{stikBalance:0.####}";
        }
        private void SuccessTikWallet()
        {
            LobbyManager.Instance.text_Taika.text = UserInfoManager.Instance.userTikWalletHistory.balance.uiAmountString;
        }
        private void SuccessSpecialInit(SpecialStoreDTO specialStoreDTO)
        {
            ShopScreen.Instance.SpecialStoreItemInit(specialStoreDTO);
        }

        private void ButtonInteractable(bool isInteractable)
        {
            button_Once.interactable = isInteractable;
            button_Many.interactable = isInteractable;
        }

        private void Success(ResRewardTemplate data)
        {
            InActivePopup();
            RandomBoxPopup popup = PopupManager.Instance.GetPopUp<RandomBoxPopup>("randomBoxOpen");

            popup.ActivePopup();
            string[] temp = code.Split('_');
            string boxCode = $"{temp[0]}_{temp[1]}";
            popup.RandomBoxOpen(data, boxCode, isSkip, itemKey);
            
            ShopScreen.Instance.dic_Item[itemKey].AddUserCeiling(data.RewardDataList.Count);
            ShopScreen.Instance.dic_Item[itemKey].SetItemStatus();
            
            GetUserWallet();

            if (storeType == StoreType.SPECIAL)
            {
                _ = NetworkManager.Instance.GetSpecialStore(SuccessSpecialInit);
            }

            ButtonInteractable(true);
        }

        private void AdSuccess(RandomBoxIndex data)
        {
            InActivePopup();
            RandomBoxPopup popup = PopupManager.Instance.GetPopUp<RandomBoxPopup>("randomBoxOpen");

            popup.ActivePopup();
            string[] temp = code.Split('_');
            string boxCode = $"{temp[0]}_{temp[1]}";
            popup.RandomBoxOpen(data, boxCode, isSkip);

            ShopScreen.Instance.dic_Item[itemKey].AddUserCeiling(data.rewardList.Length);
            ShopScreen.Instance.dic_Item[itemKey].SetItemStatus();
            
            GetUserWallet();

            if (storeType == StoreType.SPECIAL)
            {
                _ = NetworkManager.Instance.GetSpecialStore(SuccessSpecialInit);
            }

            ButtonInteractable(true);

            // D_ad_randombox
            AdjustInitializer.TrackEvent("5hb5ha");
        }

        private void Failed_Enough()
        {
            ButtonInteractable(true);

            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage("UI_Not_Enough_Currency");
            InActivePopup();
        }

        private void Failed_Limited()
        {
            ButtonInteractable(true);

            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage("MSG_limited_Count");
            InActivePopup();
        }

        private void Failed(string errorData)
        {
            ButtonInteractable(true);

            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");

            try
            {
                ServerErrorMessage error = JsonUtility.FromJson<ServerErrorMessage>(errorData);

                switch (error.errorCode)
                {
                    case "INSUFFICIENT_BALANCE":
                        popup.SetNoticeMessage("UI_Not_Enough_Currency");
                        break;
                    case "EXCEED_LIMITED_QUANTITY":
                        popup.SetNoticeMessage("MSG_limited_Count");
                        break;
                    case "TOO_MANY_REQUESTS":
                        popup.SetNoticeMessage("MSG_TOO_MANY_REQUESTS");
                        break;
                    default:
                        popup.SetNoticeMessage("UI_Not_Enough_Currency");
                        break;
                }
            }
            catch
            {
                popup.SetNoticeMessage("UI_Network_Unstable");
            }

            InActivePopup();
        }

        public void FinishSeqeunce()
        {
            InActivePopup();
        }
        private IEnumerator SetAdmobTimer(int timer)
        {
            int remainTime = timer;

            for (int i = 0; i < timer; i++)
            {
                text_OncePrice.text = $"{remainTime / 60:00} : {remainTime % 60:00}";
                yield return new WaitForSeconds(1f);
                remainTime--;
            }

            button_Once.interactable = true;
            inactiveObject.SetActive(false);
            activeObject.SetActive(true);
        }
    }
}
