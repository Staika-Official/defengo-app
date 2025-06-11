using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using System;
using System.Linq;
using Framework.Util;
using UniRx;
using UnityEngine.UI.Extensions;

namespace Framework.UI
{
    [Serializable]
    public class SpecialShopItemProperty
    {
        public float itemYpos;
        public float iconWidth;
        public float iconHeight;
        public TMP_FontAsset font;
    }

    [Serializable]
    public class SpecialShopBadgeProperty
    {
        public float textYpos;
        public Vector2 textsizeDelta;
        public Color color_BadgeText;
    }

    public class ShopItemSpecial : ShopItem
    {
        [Header(("Special"))]
        //special
        public GameObject object_Badge;
        public TextMeshProUGUI text_Badge;
        public TextMeshProUGUI text_SaleRate;
        public TextMeshProUGUI text_Purchased;
        public TextMeshProUGUI text_Limit;

        public Image image_InnerFrame;
        public Image image_HighLight;
        public Image image_base;
        public Image image_Quantity;
        public Gradient2 gradient;

        public RectTransform saleRateTransform;

        public SerializableDictionary<bool, SpecialShopItemProperty> dic_SpecialShopItemProperty;
        public SerializableDictionary<bool, SpecialShopBadgeProperty> dic_SepcialShopBadgeProperty;

        //special
        public int limitedQuantity { get; private set; }
        public int purchasedQuantity { get; private set; }
        public int saleRate { get; private set; }

        public void Initialize(SpecialStoreItem item, StoreType type)
        {
            text_Amount.text = LanguageManager.Instance.language switch
            {
                Language.EN => item.nameEn,
                Language.KO => item.nameKo,
                _ => null
            };

            itemKey = CombineCodeAnyType(item.code, item.moneyType.ToString());
            code = item.code;
            price = item.price;
            isAvailable = item.isAvailable;
            storeType = type;

            limitedQuantity = item.limitedQuantity;
            purchasedQuantity = item.purchasedQuantity;
            saleRate = item.saleRate;
            userCeiling = item.userCeiling;
            maxCeiling = item.maxCeiling;

            moneyType = item.moneyType;
            switch (item.rewardType)
            {
                case "CURRENCY":
                    storePackageType = StorePackageType.GEM;
                    break;

                case "BOX":
                    storePackageType = StorePackageType.RANDOM_BOX;
                    break;

                case "ITEM":
                    storePackageType = StorePackageType.ITEM;
                    break;
            }

            text_Purchased.text = $"{purchasedQuantity}";
            text_Limit.text = $"{limitedQuantity}";

            bool isLimited = limitedQuantity != 0;
            image_Quantity.gameObject.SetActive(isLimited);

            // 

            if (saleRate < 100)
            {
                // if (item.code.Contains("LEGEND"))
                // {
                //     object_Badge.SetActive(true);
                //     text_Badge.gameObject.SetActive(false);

                //     string[] splitCode = item.code.Split('_');
                //     string addtiveCode = $"{splitCode[0]}_{splitCode[1]}";

                //     saleRateTransform.sizeDelta = dic_SepcialShopBadgeProperty[false].textsizeDelta;
                //     saleRateTransform.localPosition = new Vector2(0, dic_SepcialShopBadgeProperty[false].textYpos);
                //     text_SaleRate.color = dic_SepcialShopBadgeProperty[false].color_BadgeText;

                //     text_SaleRate.text = LanguageManager.Instance.GetStringData($"UI_{addtiveCode}_DESC");
                // }
                // else
                // {
                    object_Badge.SetActive(false);
                // }
            }
            else
            {
                object_Badge.SetActive(true);
                text_Badge.gameObject.SetActive(true);

                saleRateTransform.sizeDelta = dic_SepcialShopBadgeProperty[true].textsizeDelta;
                saleRateTransform.localPosition = new Vector2(0, dic_SepcialShopBadgeProperty[true].textYpos);
                text_SaleRate.color = dic_SepcialShopBadgeProperty[true].color_BadgeText;
                text_Badge.text = LanguageManager.Instance.GetStringData("UI_StikToGem_Sale_Rate");
                text_SaleRate.text = $"{saleRate}%";
            }

            switch (moneyType)
            {
                case MoneyType.TAIKA:
                    text_Price.text = $"<sprite=1>{item.price}";
                    break;
                case MoneyType.STIK:
                    text_Price.text = $"<sprite=14>{item.price.ToString("F2")}";
                    ColorUtility.TryParseHtmlString("##FFFCDB", out Color Textcolor);
                    text_Price.color = Textcolor;
                    break;
                case MoneyType.ADMOB:
                    text_Price.text = "<sprite=6>Free";
                    ColorUtility.TryParseHtmlString("#FFFAAC", out Color color);
                    text_Price.color = color;
                    break;
                case MoneyType.GEM:
                    text_Price.text = $"<sprite=2>{item.price}";
                    break;
                default:
                    break;
            }


            switch (storePackageType)
            {
                case StorePackageType.ENERGY:
                    text_Amount.text = $"x{item.receivedItemAmount}";

                    image_Icon.sprite = DataManager.Instance.uiPropertyData.sprite_Energy;
                    SetSpecialShopItem(dic_SpecialShopItemProperty[false]);
                    break;
                case StorePackageType.RANDOM_BOX:
                    string[] splitCode = item.code.Split('_');
                    string randomBoxCode = $"{splitCode[0]}_{splitCode[1]}";
                    text_Amount.text = LanguageManager.Instance.GetStringData(randomBoxCode);

                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_RandomBoxSprite[randomBoxCode].sprite_Icon;
                    SetSpecialShopItem(dic_SpecialShopItemProperty[false]);
                    break;
                case StorePackageType.GEM:
                    string[] gemSplitCode = item.code.Split('_');
                    string gemCode = $"{gemSplitCode[0]}_{gemSplitCode[1]}";
                    text_Amount.text = $"x{item.receivedItemAmount}";

                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_gemIcon[gemCode];
                    SetSpecialShopItem(dic_SpecialShopItemProperty[true]);
                    break;
                case StorePackageType.ITEM:
                    text_Amount.text = $"{LanguageManager.Instance.GetStringData(item.code)} x{item.receivedItemAmount}";

                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_userItemIcon[item.code];
                    SetSpecialShopItem(dic_SpecialShopItemProperty[false]);
                    break;
            }

            button_Buy.onClick.RemoveAllListeners();
            button_Buy.onClick.AddListener(() => OnClick_BuyItemBox());

            SetInnerFrame(code);
            SetButtonType(moneyType);
        }

        private void SetInnerFrame(string code)
        {
            string[] splitCode = code.Split('_');
            string InnerCode = $"{splitCode[0]}_{splitCode[1]}";
            UIPropertyData propertyData = DataManager.Instance.uiPropertyData;

            if (propertyData.dic_ShopInnerFrameColor.ContainsKey(InnerCode))
            {
                gradient.enabled = true;
                image_InnerFrame.color = propertyData.dic_ShopInnerFrameColor[InnerCode].color_InnerFrame;
                image_HighLight.color = propertyData.dic_ShopInnerFrameColor[InnerCode].color_HighLight;
                image_base.color = propertyData.dic_ShopInnerFrameColor[InnerCode].color_Base;
                image_Quantity.color = propertyData.dic_ShopInnerFrameColor[InnerCode].color_Quantity;
                gradient.EffectGradient = propertyData.dic_ShopInnerFrameColor[InnerCode].color_Grdient;
                gradient.Offset = propertyData.dic_ShopInnerFrameColor[InnerCode].grdientOffset;
                gradient.Zoom = propertyData.dic_ShopInnerFrameColor[InnerCode].grdientZoom;
            }
            else
            {
                string defaultCode = "Defalut";
                gradient.enabled = false;
                image_InnerFrame.color = propertyData.dic_ShopInnerFrameColor[defaultCode].color_InnerFrame;
                image_HighLight.color = propertyData.dic_ShopInnerFrameColor[defaultCode].color_HighLight;
                image_base.color = propertyData.dic_ShopInnerFrameColor[defaultCode].color_Base;
                image_Quantity.color = propertyData.dic_ShopInnerFrameColor[defaultCode].color_Quantity;
                gradient.EffectGradient = propertyData.dic_ShopInnerFrameColor[defaultCode].color_Grdient;
                gradient.Offset = propertyData.dic_ShopInnerFrameColor[defaultCode].grdientOffset;
                gradient.Zoom = propertyData.dic_ShopInnerFrameColor[defaultCode].grdientZoom;
            }
        }

        private void SetSpecialShopItem(SpecialShopItemProperty shopItemProperty)
        {
            image_Icon.rectTransform.anchoredPosition = new Vector3(image_Icon.rectTransform.localPosition.x, shopItemProperty.itemYpos, 0);
            text_Amount.font = shopItemProperty.font;

            //ui쪽에서 다른상자들과 다르게 레전드 상자이미지 크기가 다르다해서 따로 처리
            if (code.Contains("LEGEND"))
            {
                image_Icon.rectTransform.sizeDelta = new Vector2(shopItemProperty.iconWidth, 200f);
            }
            else
            {
                image_Icon.rectTransform.sizeDelta = new Vector2(shopItemProperty.iconWidth, shopItemProperty.iconHeight);
            }

        }
    }
}

