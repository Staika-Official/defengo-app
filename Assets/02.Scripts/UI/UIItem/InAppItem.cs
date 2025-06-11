using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using Framework.GameData.Defense;
using Framework.Network;
using UnityEngine.Purchasing;

namespace Framework.UI
{
    public class InAppItem : MonoBehaviour
    {
        public string productId;
        public TextMeshProUGUI text_Price;
        public Button button_InAppPurchase;

        public void Initialize(string productId)
        {
            this.productId = productId;

            Product product = IAPManager.Instance.GetProductInfo(productId);
            // Debug.Log("Localized Title  : " + product.metadata.localizedTitle);
            // Debug.Log("isoCurrencyCode : " + product.metadata.isoCurrencyCode);
            CultureInfo culture = GetCultureInfoFromISOCurrencyCode(product.metadata.isoCurrencyCode);
            string price = product.metadata.localizedPrice.ToString();

            //string priceInfo = RegionInfo.CurrentRegion.ISOCurrencySymbol + " " + price;
            string priceInfo = product.metadata.isoCurrencyCode + " " + price;
            text_Price.text = priceInfo;

            button_InAppPurchase.onClick.AddListener(() =>
            {
                LobbyManager.Instance.PurchasePopup(true);
                
                IAPManager.Instance.iapBuyType = IapBuyType.GEM;

                NetworkManager.Instance.onSuccessWallet = (amount) =>
                {
                    InAppPurchasePopup popup = PopupManager.Instance.GetPopUp<InAppPurchasePopup>("inAppPurchase");
                    LobbyManager.Instance.PurchasePopup(false);
                    popup.SuccessInAppPurchase(amount);
                };
                
                IAPManager.Instance.PurchaseProduct(productId);
            });
        }

        public static CultureInfo GetCultureInfoFromISOCurrencyCode(string code)
        {
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo ri = new(ci.LCID);
                if (ri.ISOCurrencySymbol == code)
                    return ci;
            }
            return null;
        }

    }
}

