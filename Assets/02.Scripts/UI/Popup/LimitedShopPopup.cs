using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Util;
using Framework.Network;
using System;
using Framework.GameData.Defense;
using Spine.Unity;
using UnityEngine.Localization.Components;

namespace Framework.UI
{
    public class LimitedShopPopup : PopupTemplate
    {
        [SerializeField] private TextMeshProUGUI text_Gem;
        [SerializeField] private TextMeshProUGUI text_Period;
        [SerializeField] private TextMeshProUGUI text_userCeiling;
        [SerializeField] private TextMeshProUGUI text_maxCeiling;

        [SerializeField] private CharacterCard ceilingCard;
        [SerializeField] private LimitedItem[] limitedItems;

        [SerializeField] private Button button_Skip;
        [SerializeField] private GameObject object_SkipEnable;

        [SerializeField] private ButtonComponent button_Once;
        [SerializeField] private ButtonComponent button_Many;
        [SerializeField] private TextMeshProUGUI text_OncePrice;
        [SerializeField] private TextMeshProUGUI text_ManyPrice;

        [SerializeField] private Slider slider_Ceiling;
        [Header("UISet"), SerializeField] private LimitedShopUIItem limitedShopUIItem;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private SkeletonGraphic characterSpine;
        [SerializeField] private LocalizeStringEvent localizeDesc;

        private LimitedStoreItem limitedStoreItemData;
        private bool isSkip;

        public override void ActivePopup()
        {
            SetLimitedStoreItem();
            SetBuyButton();
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() => InActivePopup());

            button_Once.button.onClick.AddListener(() =>
                {
                    button_Once.SetInterectible(false);
                    OnClick_BuyItem(true);
                });
            button_Many.button.onClick.AddListener(() =>
                {
                    button_Many.SetInterectible(false);
                    OnClick_BuyItem(false);
                });
            button_Skip.onClick.AddListener(() => OnClick_Skip());

            SetupUI();
        }

        [ContextMenu("Setup UI")]
        private void SetupUI()
        {
            if (limitedShopUIItem != null)
            {
                if (backgroundImage) backgroundImage.sprite = limitedShopUIItem.backgroundImage;
                if (characterSpine)
                {
                    characterSpine.skeletonDataAsset = limitedShopUIItem.characterSkeletonData;
                    characterSpine.Initialize(true);
                }
                if (localizeDesc) localizeDesc.StringReference.TableEntryReference = limitedShopUIItem.descLocalizeKey;
            }
        }

        private void SetLimitedStoreItem()
        {
            isSkip = SecurePlayerPrefs.GetInt("ShopRandomBoxSkip", 0) == 1;
            object_SkipEnable.SetActive(isSkip);
            limitedStoreItemData = DataManager.Instance.limitedStoreItem;

            ceilingCard.InitializeItem(limitedStoreItemData.rewardCharacters[0]);

            for (int i = 0; i < limitedItems.Length; i++)
            {
                if (limitedStoreItemData.rewardCharacters.Length > i)
                {
                    limitedItems[i].gameObject.SetActive(true);
                    limitedItems[i].Initialize((CharacterIndex)limitedStoreItemData.rewardCharacters[i]);
                }
                else
                    limitedItems[i].gameObject.SetActive(false);
            }

            text_Gem.text = $"{UserInfoManager.Instance.gemValue}";

            slider_Ceiling.maxValue = limitedStoreItemData.maxCeiling;
            slider_Ceiling.value = limitedStoreItemData.userCeiling;
            text_userCeiling.text = $"{limitedStoreItemData.userCeiling}";
            text_maxCeiling.text = string.Format(LanguageManager.Instance.GetStringData("UI_PitySystem"), limitedStoreItemData.maxCeiling);

            SetPeriod();
        }

        private void SetBuyButton()
        {
            float price = limitedStoreItemData.price;

            text_OncePrice.text = $"<sprite=2>{(int)(price * 0.1f)}";
            text_ManyPrice.text = $"<sprite=2>{(int)price}";

            button_Once.SetInterectible(UserInfoManager.Instance.gemValue >= price * 0.1f);
            button_Many.SetInterectible(UserInfoManager.Instance.gemValue >= price);
        }

        private void OnClick_BuyItem(bool isOnce)
        {
            float price = isOnce ? (limitedStoreItemData.price * 0.1f) : limitedStoreItemData.price;

            if (UserInfoManager.Instance.gemValue < price)
            {
                Failed_Enough();
                return;
            }

            string randomBoxCode = isOnce ? limitedStoreItemData.code.Replace("10", "1") : limitedStoreItemData.code;

            RequestBuyItem data = new()
            {
                clientId = "DEFENGO",
                code = randomBoxCode,
                repurchase = false,
                userId = UserInfoManager.Instance.userId
            };

            _ = NetworkManager.Instance.RequestBuyItem(data, Success, Failed);
        }

        private void Success(ResRewardTemplate data)
        {
            AdjustInitializer.TrackEvent("mvdjf0");
            RandomBoxPopup popup = PopupManager.Instance.GetPopUp<RandomBoxPopup>("randomBoxOpen");

            popup.ActivePopup();

            string boxCode = "BOX_LIMITED";
            popup.RandomBoxOpen(data, boxCode, isSkip);

            AddUserCeiling(data.RewardDataList.Count);

            GetUserWallet();
        }

        private void OnClick_Skip()
        {
            isSkip = !isSkip;
            object_SkipEnable.SetActive(isSkip);
            SecurePlayerPrefs.SetInt("ShopRandomBoxSkip", isSkip ? 1 : 0);
        }

        private void Failed_Enough()
        {
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage("UI_Not_Enough_Currency");
        }

        private void Failed(string errorData)
        {
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
            
            SetBuyButton();
        }

        private void GetUserWallet()
        {
            _ = NetworkManager.Instance.GetUserWalletHistory();
            _ = NetworkManager.Instance.GetUserWalletInfo(SuccessWallet, "PTIK");
        }

        private void SuccessWallet(int gem)
        {
            UserInfoManager.Instance.gemValue = gem;
            LobbyManager.Instance.text_Gem.text = $"{gem}";
            text_Gem.text = $"{gem}";

            SetLimitedStoreItem();
            SetBuyButton();
        }

        private void SetPeriod()
        {
            DateTime endTime = DateTime.Parse(DataManager.Instance.limitedStoreItem.toDate);
            string periodTimeSpan = $"{endTime:yy-MM-dd}";
            string period = string.Format(LanguageManager.Instance.GetStringData("UI_Event_Store_1_Time"), periodTimeSpan);

            text_Period.text = period;
        }

        private void AddUserCeiling(int count)
        {
            if (DataManager.Instance.limitedStoreItem.maxCeiling == 0)
                return;

            DataManager.Instance.limitedStoreItem.userCeiling += count;
            if (DataManager.Instance.limitedStoreItem.userCeiling >= DataManager.Instance.limitedStoreItem.maxCeiling)
            { DataManager.Instance.limitedStoreItem.userCeiling -= DataManager.Instance.limitedStoreItem.maxCeiling; }
        }

    }
}
