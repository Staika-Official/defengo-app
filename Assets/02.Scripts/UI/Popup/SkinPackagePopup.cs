using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using Framework.UI;
using Framework.Util;
using Spine;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Framework.UI
{
    public class SkinPackagePopup : PopupTemplate
    {
        public TextMeshProUGUI m_TextTitle;
        public TextMeshProUGUI m_TextPeriod;
        public TextMeshProUGUI m_TextPrice;

        public Image m_SkinLarge;
        public VideoPlayer m_VideoPlayer;

        public SkinPackageGroup m_SkinPackageGroup;

        public Button m_Buy;

        private int mSkinID = 0;
        private bool isOpened = false;

        private StoreTableData mTableData;
        private SpecialStoreSkin mSkinData;
        private List<RewardTableData> mRewardTableList;

        private UnityAction mCallback = null;

        public override void ActivePopup()
        {
            if (isOpened)
                return;

            PopUpSequence(true);
            m_VideoPlayer.gameObject.SetActive(true);
            m_VideoPlayer.StepForward();
            m_VideoPlayer.Play();
            isOpened = true;
        }

        public override void InActivePopup()
        {
            m_VideoPlayer.Stop();
            m_VideoPlayer.gameObject.SetActive(false);
            PopUpSequence(false);

            mCallback?.Invoke();
        }

        public override void Initialize()
        {
            m_VideoPlayer.gameObject.SetActive(false);

            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InActivePopup();
            });

            m_Buy.onClick.AddListener(() =>
            {
                if (DataManager.Instance.userCharacters.skins.Contains(mSkinID))
                {
                    int gem_amount = DataManager.Instance.SkinTableDataList.Find(a => a.id == mSkinID).price;

                    SystemNoticePopup systemPopup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                    systemPopup.SetYesNoPopupMessage(string.Format(LanguageManager.Instance.GetStringData("MSG_Skin_Owned_Already"), gem_amount), OnSkinPackageBuy);
                }
                else
                    OnSkinPackageBuy();
            });
        }

        public void SetUI(SpecialStore storeInfo, StoreTableData table_data, UnityAction callback = null)
        {
            mCallback = callback;
            mTableData = table_data;
            mRewardTableList = DataManager.Instance.GetRewardTableDataFromStoreTableData(storeInfo.skin.storeGroupId);

            mSkinID = mRewardTableList.Find(a => a.RewardData.Type == RewardType.CHARACTER_SKIN).RewardData.IntValue;
            int character_amount = mRewardTableList.Find(a => a.RewardData.Type == RewardType.CHARACTER).RewardData.Amount;
            CharacterResource resource = DataManager.Instance.CharacterResourceData.dic_Character[mSkinID];

            System.TimeSpan duration = GameUtil.RemainTime(storeInfo.toDate);

            m_SkinLarge.sprite = resource.SkinPortrait;
            m_SkinPackageGroup.Initialize(mSkinID, character_amount, resource);
            m_TextTitle.text = LanguageManager.Instance.GetStringData(string.Format("UI_Skin_{0}", mSkinID));

            string period = string.Format(LanguageManager.Instance.GetStringData("UI_Special_Shop_Time"), duration.Days.ToString());
            m_TextPeriod.text = period;

            Product product = IAPManager.Instance.GetProductInfo(mTableData.inapp_tier);
            string price = product.metadata.localizedPrice.ToString();
            string priceInfo = product.metadata.isoCurrencyCode + " " + price;
            m_TextPrice.text = priceInfo;
        }

        private void OnSkinPackageBuy()
        {
            LobbyManager.Instance.PurchasePopup(true);

            IAPManager.Instance.iapBuyType = IapBuyType.SKIN;

            NetworkManager.Instance.onSuccessWallet = (amount) =>
            {
                RequestBuyItem data = new()
                {
                    clientId = "DEFENGO",
                    code = mTableData.code,
                    repurchase = false,
                    userId = UserInfoManager.Instance.userId
                };

                _ = NetworkManager.Instance.PostUserSkinPackageBuy(data, OnSuccessBuy, OnFailed);
            };

            IAPManager.Instance.PurchaseProduct(mTableData.inapp_tier);
        }

        private void OnSuccessBuy(ResRewardTemplate response)
        {
            List<RewardData> reward_list = new List<RewardData>();

            response.RewardDataList.ForEach(a => reward_list.Add(a));

            QuestRewardPopup popup = PopupManager.Instance.GetPopUp<QuestRewardPopup>("questReward");
            LobbyManager.Instance.PurchasePopup(false);
            popup.SetRewardSkinPackage(reward_list);
            popup.ActivePopup();
            InActivePopup();
        }

        private void OnFailed(string error)
        {

        }
    }
}

