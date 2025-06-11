using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Sound;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.GameData.Defense;
using Framework.Util;
using UnityEngine.Events;
using DG.Tweening;

namespace Framework.UI
{
    [System.Serializable]
    public class SetIcon
    {
        public Sprite sprite_Icon;
        public Sprite sprite_SecondIcon;
    }

    public class EarnPopup : PopupTemplate
    {
        public UnityAction OnConfirm;
        public SerializableDictionary<InboxType, SetIcon> dic_SetIcon;
        public ButtonComponent button_Confirm;
        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_Amount;
        public TextMeshProUGUI text_ItemInfo;
        public GameObject chestObject;

        public Image image_ChestIcon;
        public Image image_Icon;
        public Image image_SecondIcon;

        public Transform trans_AttractorStart;

        public override void ActivePopup()
        {
            // button_Close.transform.DOScale(Vector2.zero, 0);
            // StartCoroutine(ActivateButton());
        }

        public override void InActivePopup()
        {

        }

        public void SetAmountValue(InboxType rewardType, string amountValue, int quantity)
        {
            SetIcon setIcon = dic_SetIcon[rewardType];

            image_SecondIcon.sprite = setIcon.sprite_SecondIcon;

            string amountString = "";

            switch (rewardType)
            {
                case InboxType.MAIL:
                    break;
                case InboxType.RANDOM_BOX:
                    chestObject.SetActive(true);
                    image_Icon.gameObject.SetActive(false);
                    image_ChestIcon.sprite = setIcon.sprite_Icon;
                    break;
                case InboxType.ENERGY:
                    chestObject.SetActive(false);
                    image_Icon.gameObject.SetActive(true);
                    image_Icon.sprite = setIcon.sprite_Icon;
                    text_ItemInfo.text = LanguageManager.Instance.GetStringData("UI_Energy");
                    amountString = $"<sprite=7>{amountValue}";
                    OnConfirm = () =>
                    {
                        AttractorManager.Instance.SetAttractor("energy", trans_AttractorStart, LobbyManager.Instance.GetEnergyValue);
                        _ = NetworkManager.Instance.GetUserWalletInfo(SetGemValue, "PTIK");
                        OnConfirm = null;
                    };
                    break;
                case InboxType.TIK:
                    chestObject.SetActive(false);
                    image_Icon.gameObject.SetActive(true);
                    image_Icon.sprite = setIcon.sprite_Icon;
                    text_ItemInfo.text = LanguageManager.Instance.GetStringData("UI_Taika");
                    OnConfirm = () =>
                    {
                        AttractorManager.Instance.SetAttractor("tik", trans_AttractorStart, OnCompleteGetGem);
                        _ = NetworkManager.Instance.GetUserWalletHistory();
                        OnConfirm = null;
                    };
                    amountString = $"<sprite=1>{amountValue}";

                    break;
                case InboxType.STIK:
                    chestObject.SetActive(false);
                    image_Icon.gameObject.SetActive(true);
                    image_Icon.sprite = setIcon.sprite_Icon;
                    text_ItemInfo.text = LanguageManager.Instance.GetStringData("UI_Staika");
                    amountString = $"<sprite=14>{amountValue}";
                    _ = NetworkManager.Instance.GetUserWalletHistory();
                    break;
                case InboxType.GEM:
                    chestObject.SetActive(false);
                    image_Icon.gameObject.SetActive(true);
                    image_Icon.sprite = setIcon.sprite_Icon;
                    text_ItemInfo.text = LanguageManager.Instance.GetStringData("UI_Gem");
                    amountString = $"<sprite=2>{amountValue}";
                    OnConfirm = () =>
                    {
                        AttractorManager.Instance.SetAttractor("gem", trans_AttractorStart, OnCompleteGetGem);
                        OnConfirm = null;
                    };
                    break;
                case InboxType.GO:
                    break;
                case InboxType.HATCHING_ORB:

                    //값 받아와서 합치면 UI_HATCHING_ORB_LEGEND가 나옴
                    string uiStringData = "UI_" + amountValue;

                    //박스 오브젝트 비활성화
                    chestObject.SetActive(false);
                    //아이템 이미지 활성화
                    image_Icon.gameObject.SetActive(true);

                    //아이템 이미지 셋
                    image_SecondIcon.sprite = DataManager.Instance.uiPropertyData.dic_HatchingStoneSprite[amountValue].sprite_Icon;
                    image_Icon.sprite = DataManager.Instance.uiPropertyData.dic_HatchingStoneSprite[amountValue].sprite_Icon;

                    //아이템 정보 셋
                    text_ItemInfo.text = LanguageManager.Instance.GetStringData(uiStringData);

                    //아이템 타입을 매개변수로 넣어줘서 타입에 맞는 스트링포맷을 들고옴
                    amountString = ShopScreen.Instance.hatchingStoneChange.GetAmonutHatchingStoneIcon(amountValue, quantity);

                    //확인버튼 클릭 시 부화석 정보 다시 셋
                    OnConfirm = () =>
                    {
                        AttractorManager.Instance.SetHatchingAttractor("hatching", amountValue, trans_AttractorStart);
                        _ = NetworkManager.Instance.GetHatchingStone();
                        OnConfirm = null;
                    };
                    break;
                default:
                    break;
            }
            text_Amount.text = amountString;

            PopUpSequence(true);

        }

        public void SetAmountValue(string amountValue, string itemCode, Sprite sprite, StorePackageType storePackageType)
        {
            chestObject.SetActive(false);

            image_Icon.sprite = sprite;

            text_ItemInfo.text = LanguageManager.Instance.GetStringData(itemCode);

            string amount = amountValue;
            string text = "";
            switch (storePackageType)
            {
                case StorePackageType.ENERGY:
                    text = $"<sprite=7>{amount}";
                    OnConfirm = () =>
                    {
                        AttractorManager.Instance.SetAttractor("energy", trans_AttractorStart, LobbyManager.Instance.GetEnergyValue);
                        _ = NetworkManager.Instance.GetUserWalletInfo(SetGemValue, "PTIK");
                        OnConfirm = null;
                    };
                    break;
                case StorePackageType.RANDOM_BOX:
                    break;
                case StorePackageType.GEM:
                    text = $"<sprite=2>{amount}";
                    OnConfirm = () =>
                    {
                        AttractorManager.Instance.SetAttractor("gem", trans_AttractorStart, OnCompleteGetGem);
                        OnConfirm = null;
                    };
                    break;
                case StorePackageType.ITEM:
                    {
                        text = $"<sprite=29>{amount}";

                        string attractorKey = "";
                        switch (itemCode)
                        {
                            case "ROULETTE_TICKET":
                                attractorKey = "rouletteTicket";
                                break;
                        }
                        
                        OnConfirm = () =>
                        {
                            AttractorManager.Instance.SetAttractor(attractorKey, trans_AttractorStart);
                            OnConfirm = null;
                        };
                    }
                    break;
                default:
                    break;
            }

            text_Amount.text = text;
        }

        public void OnCompleteGetGem()
        {
            _ = NetworkManager.Instance.GetUserWalletInfo(SetGemValueSequence, "PTIK");
        }

        public void SetGemValueSequence(int gemValue)
        {
            int temp = UserInfoManager.Instance.gemValue;
            UserInfoManager.Instance.gemValue = gemValue;
            LobbyManager.Instance.ChangeValueGem(gemValue, temp);
        }

        public void SetGemValue(int gem)
        {
            UserInfoManager.Instance.gemValue = gem;
            LobbyManager.Instance.text_Gem.text = $"{gem}";
        }

        // public IEnumerator ActivateButton()
        // {
        //     yield return new WaitForSeconds(1);
        //     button_Close.transform.DOScale(Vector2.one, 0.3f);
        // }

        public void OnClick_GetItem()
        {

        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                OnConfirm?.Invoke();
                PopUpSequence(false);
            });
        }
    }
}
