using System;
using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class SpecialShopGroup : MonoBehaviour
    {
        public SpecialStorePackageType specialStorePackageType;
        public RectTransform rect_BackGround;
        public RectTransform rect_Group;

        public ShopItem specialShopPackageSrc;

        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_Period;

        public List<ShopItem> SpecialShopItemList { get { return specialShopItems; } }
        private List<ShopItem> specialShopItems = new();
        public Queue<ShopItem> queueShopItems = new();

        public int groupAmount { get; set; }
        public int periodDay { get; set; }

        public void Initialize()
        {
            text_Title.text = LanguageManager.Instance.GetStringData("SpecialStore_ITEM");
            // string period = string.Format(LanguageManager.Instance.GetStringData("UI_Special_Shop_Time"), duration.Days.ToString());

            //남은 기간으로 카테고리 소팅해줘야하기 위함
            // periodDay = duration.Days;
            // text_Period.text = period;

            specialShopPackageSrc.gameObject.SetActive(false);
        }

        public bool SetGroupItemCheck(SpecialStoreDTO storeInfo, bool isBlockRegion)
        {
            RefreshSpecialShopItem();

            int createCount = 0;
            // if (storeInfo.type == SpecialStorePackageType.ITEM)
            // {
                for (int i = 0; i < storeInfo.specialStore.Length; ++i)
                {
                    if (isBlockRegion && storeInfo.specialStore[i].moneyType == MoneyType.STIK)
                    {
                        continue;
                    }

                    ShopItemSpecial item = GetSpecialShopItem() as ShopItemSpecial;
                    item.Initialize(storeInfo.specialStore[i], StoreType.SPECIAL);

                    item.gameObject.SetActive(true);
                    specialShopItems.Add(item);
                    ++createCount;
                }
            // }
            // else if (storeInfo.type == SpecialStorePackageType.SKIN)
            // {
            //     ShopItemSkinPackage item = GetSpecialShopItem() as ShopItemSkinPackage;
            //     item.Initialize(storeInfo.skin);

            //     item.gameObject.SetActive(true);
            //     specialShopItems.Add(item);
            //     ++createCount;
            // }


            //현재 상품의 갯수로 렉트 사이즈 조절하기위함
            groupAmount = specialShopItems.Count;

            if (createCount == 0)
            {
                return false;
            }

            return true;
        }

        public void RefreshSpecialShopItem()
        {
            for (int i = 0; i < specialShopItems.Count; ++i)
            {
                specialShopItems[i].gameObject.SetActive(false);
                queueShopItems.Enqueue(specialShopItems[i]);
            }

            specialShopItems.Clear();
        }

        public ShopItem GetSpecialShopItem()
        {
            if (queueShopItems.Count == 0)
            {
                specialShopPackageSrc.gameObject.SetActive(true);
                ShopItem shopItem = Instantiate(specialShopPackageSrc, rect_Group);
                shopItem.transform.localScale = Vector3.one;
                specialShopPackageSrc.gameObject.SetActive(false);

                return shopItem;
            }
            else
            {
                return queueShopItems.Dequeue();
            }
        }
    }
}
