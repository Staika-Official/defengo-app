using System.Collections;
using System.Collections.Generic;
using Framework.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using Framework.Network;
using Framework.UI;
using Framework.GameData.Defense;

public class ShopItemSkinPackage : ShopItem
{
    [Header("Skin Package")]
    public Image m_ImgSkinLarge;
    public SkinPackageGroup m_SkinPackageGroup;

    private int mSkinID;

    private StoreTableData mTableData;

    public void Initialize(SpecialStoreSkin skin)
    {
        mTableData = IAPManager.Instance.ActiveStoreDataList.Find(a => a.store_group_id == skin.storeGroupId);

        List<RewardTableData> reward_table_list = DataManager.Instance.GetRewardTableDataFromStoreTableData(skin.storeGroupId);

        mSkinID = reward_table_list.Find(a => a.RewardData.Type == RewardType.CHARACTER_SKIN).RewardData.IntValue;
        itemKey = CombineCodeAnyType(skin.storeGroupId.ToString(), skin.moneyType.ToString());
        Product product = IAPManager.Instance.GetProductInfo(mTableData.inapp_tier);

        string price = product.metadata.localizedPrice.ToString();
        string priceInfo = product.metadata.isoCurrencyCode + " " + price;
        text_Price.text = priceInfo;

        CharacterResource resource = DataManager.Instance.CharacterResourceData.dic_Character[mSkinID];
        m_ImgSkinLarge.sprite = resource.SkinPortrait;

        int character_amount = reward_table_list.Find(a => a.RewardData.Type == RewardType.CHARACTER).RewardData.Amount;
        m_SkinPackageGroup.Initialize(mSkinID, character_amount, resource);

        button_Buy.onClick.RemoveAllListeners();
        button_Buy.onClick.AddListener(() =>
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

    public void OnSuccessBuy(ResRewardTemplate response)
    {
        List<RewardData> reward_list = new List<RewardData>();

        response.RewardDataList.ForEach(a => reward_list.Add(a));

        QuestRewardPopup popup = PopupManager.Instance.GetPopUp<QuestRewardPopup>("questReward");
        LobbyManager.Instance.PurchasePopup(false);
        popup.SetRewardSkinPackage(reward_list);
        popup.ActivePopup();

        _ = NetworkManager.Instance.GetSpecialStore((SpecialStoreDTO specialStoreDTO) =>
        {
            ShopScreen.Instance.SpecialStoreItemInit(specialStoreDTO);
        });
    }

    public void OnFailed(string error)
    {

    }
}
