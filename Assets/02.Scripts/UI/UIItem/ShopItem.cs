using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using System;
using Framework.Util;

namespace Framework.UI
{
    [Serializable]
    public class ShopItemProperty
    {
        public float amountYpos;
        public bool isInnerFrameActive;
        public float itemYpos;
        public float iconHeight;
        public bool isNameActive;
        public float textHeight;
    }

    public class ShopItem : MonoBehaviour
    {
        public Image image_Icon;
        public Button button_Buy;

        public GameObject notice;
        public GameObject eventObject;

        public Image image_BackGround;
        public Image image_Button;
        public Image image_Line;
        public Image image_Circle;

        public GameObject objectInnerFrame;

        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_Amount;
        public TextMeshProUGUI text_Price;
        public TextMeshProUGUI text_Timer;

        public GameObject activeObject;
        public GameObject inactiveObject;

        public SerializableDictionary<bool, ShopItemProperty> dic_ShopItemProperty;

        protected IEnumerator tempSequence;

        public MoneyType moneyType { get; protected set; }
        public StorePackageType storePackageType { get; protected set; }
        public StoreType storeType { get; protected set; }

        public int remainTime { get; protected set; }

        public string itemKey { get; protected set; }
        public string code { get; protected set; }
        public float price { get; protected set; }
        public bool isAvailable { get; protected set; }
        
        public int userCeiling { get; protected set; }
        public int maxCeiling { get; protected set; }

        public void Initialize(StoreItem item, StoreType type)
        {
            text_Title.text = LanguageManager.Instance.language switch
            {
                Language.EN => item.nameEn,
                Language.KO => item.nameKo,
                _ => null
            };

            itemKey = CombineCodeAnyType(item.code, item.moneyType);
            code = item.code;
            price = item.price;
            moneyType = item.moneyType.ToEnum<MoneyType>();
            storePackageType = item.itemType.ToEnum<StorePackageType>();
            storeType = type;
            
            userCeiling = item.userCeiling;
            maxCeiling = item.maxCeiling;

            switch (moneyType)
            {
                case MoneyType.TAIKA:
                    text_Price.text = $"<sprite=1>{item.price}";
                    isAvailable = true;
                    break;
                case MoneyType.STIK:
                    eventObject.SetActive(true);
                    text_Price.text = $"<sprite=14>{item.price.ToString("F2")}";
                    ColorUtility.TryParseHtmlString("##FFFCDB", out Color Textcolor);
                    text_Price.color = Textcolor;
                    isAvailable = true;
                    break;
                case MoneyType.ADMOB:
                    text_Price.text = "<sprite=6>Free";
                    ColorUtility.TryParseHtmlString("#FFFAAC", out Color color);
                    text_Price.color = color;
                    //SetItemStatus();
                    SetItemStatus(storePackageType);
                    break;
                case MoneyType.GEM:
                    text_Price.text = $"<sprite=2>{item.price}";
                    isAvailable = true;
                    break;
                default:
                    break;
            }

            switch (storePackageType)
            {
                case StorePackageType.ENERGY:
                    text_Amount.gameObject.SetActive(true);
                    text_Amount.text = $"x{item.receivedItemAmount}";
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.sprite_Energy;
                    SetShopItem(dic_ShopItemProperty[false]);
                    break;
                case StorePackageType.RANDOM_BOX:
                    string[] temp = item.code.Split('_');
                    string randomBoxCode = $"{temp[0]}_{temp[1]}";
                    text_Amount.gameObject.SetActive(false);
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_RandomBoxSprite[randomBoxCode].sprite_Icon;
                    SetShopItem(dic_ShopItemProperty[false]);
                    break;
                case StorePackageType.GEM:
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_gemIcon[code];
                    text_Amount.text = $"x{item.receivedItemAmount}";
                    SetShopItem(dic_ShopItemProperty[true]);
                    break;
            }

            button_Buy.onClick.AddListener(() => OnClick_BuyItemBox());

            notice.SetActive(false);
            SetButtonType(moneyType);
        }

        public void SetShopItem(ShopItemProperty shopItemProperty)
        {
            image_Icon.rectTransform.sizeDelta = new Vector2(shopItemProperty.iconHeight, shopItemProperty.iconHeight);
            text_Amount.rectTransform.sizeDelta = new Vector2(text_Amount.rectTransform.sizeDelta.x, shopItemProperty.textHeight);
            text_Amount.rectTransform.anchoredPosition = new Vector3(text_Amount.rectTransform.localPosition.x, shopItemProperty.amountYpos, 0);
            objectInnerFrame.SetActive(shopItemProperty.isInnerFrameActive);
            image_Icon.rectTransform.anchoredPosition = new Vector3(image_Icon.rectTransform.localPosition.x, shopItemProperty.itemYpos, 0);
            text_Title.gameObject.SetActive(shopItemProperty.isNameActive);
        }

        public void SetButtonType(MoneyType moneyType)
        {
            ShopPackgeInfo shopPackageInfo = DataManager.Instance.uiPropertyData.dic_ShopPackageInfo[moneyType];

            image_BackGround.color = shopPackageInfo.color_BackGround;
            image_Button.color = shopPackageInfo.color_Button;
            image_Circle.color = shopPackageInfo.color_Line;
            image_Line.color = shopPackageInfo.color_Line;
        }

        public void SetItemStatus(StorePackageType storePackageType)
        {
            switch (storePackageType)
            {
                case StorePackageType.ENERGY:
                    if (tempSequence != null)
                    {
                        StopCoroutine(tempSequence);
                    }
                    LobbyManager.Instance.GetEnergyValue();

                    break;
                case StorePackageType.RANDOM_BOX:


                    break;
            }

            _ = NetworkManager.Instance.UserAdValidInfo(storePackageType.ToString(), OnSuccessGetAdmobInfo, OnFailedGetAdmobInfo);
        }

        public void SetItemStatus()
        {
            if (tempSequence != null)
            {
                StopCoroutine(tempSequence);
            }
            LobbyManager.Instance.GetEnergyValue();

            if (moneyType == MoneyType.ADMOB)
            {
                _ = NetworkManager.Instance.UserAdValidInfo(storePackageType.ToString(), OnSuccessGetAdmobInfo, OnFailedGetAdmobInfo);
            }
        }

        public void AddUserCeiling(int count)
        {
            if (maxCeiling == 0)
                return;

            userCeiling += count;
            if (userCeiling >= maxCeiling)
            { userCeiling -= maxCeiling; }
        }

        public string CombineCodeAnyType(string code, string moneyType)
        {
            string combineCode = code + moneyType;

            return combineCode;
        }

        public void OnClick_BuyItemBox()
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            ShopScreen.Instance.BuyItemBox(storePackageType, itemKey);
        }


        public void OnSuccessGetAdmobInfo(UserAdValidInfo data)
        {
            this.isAvailable = data.isAvailable;

            if (isAvailable)
            {
                inactiveObject.SetActive(false);
                activeObject.SetActive(true);
                LobbyNavigator.Instance.Notification(MenuType.SHOP, true);
                notice.SetActive(true);

                button_Buy.interactable = true;
            }
            else if (data.remainCoolTime > 0)
            {
                notice.SetActive(false);
                inactiveObject.SetActive(true);
                LobbyNavigator.Instance.Notification(MenuType.SHOP, false);
                activeObject.SetActive(false);

                if (tempSequence != null)
                {
                    StopCoroutine(tempSequence);
                }

                tempSequence = SetAdmobTimer(data.remainCoolTime);
                StartCoroutine(tempSequence);
            }
            else
            {
                inactiveObject.SetActive(true);
                activeObject.SetActive(false);

                button_Buy.interactable = false;
            }
        }

        public void OnFailedGetAdmobInfo()
        {
            Debug.Log("Failed Store Item");
        }

        public IEnumerator SetAdmobTimer(int timer)
        {
            remainTime = timer;

            for (int i = 0; i < timer; i++)
            {
                text_Timer.text = $"{remainTime / 60:00} : {remainTime % 60:00}";
                yield return new WaitForSeconds(1f);
                remainTime--;
            }

            inactiveObject.SetActive(false);
            activeObject.SetActive(true);
            //SetItemStatus();
            SetItemStatus(storePackageType);
        }

    }
}

