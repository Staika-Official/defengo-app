using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Framework.Util;
using Framework.GameData.Defense;
using Framework.Network;
using Spine.Unity;
using Spine;
using DG.Tweening;
using Framework.Sound;
using System.Linq;

namespace Framework.UI
{
    [System.Serializable]
    public class UIGroup
    {
        public GameObject ui_Group;
        public Button[] buttons;
    }

    public class InventoryPopup : PopupTemplate
    {
        [SerializeField] private UIGroup[] uiGroups;
        [SerializeField] private CharacterCard[] characterCards;
        [SerializeField] private TextMeshProUGUI text_CharacterName;
        [SerializeField] private TextMeshProUGUI text_Grade;
        [SerializeField] private TextMeshProUGUI text_Infomation;
        [SerializeField] private TextMeshProUGUI text_AttackInfomation;
        public StatusInfo[] statusInfos;
        public StatusInfo fieldStatusInfo;
        public Button button_SetCharacter;
        public ButtonComponent button_ClassUp;
        public ButtonComponent button_ClassUp_Stik;
        [SerializeField] private Button button_Skin;
        public TextMeshProUGUI text_ClassUpPrice;
        [SerializeField] private TextMeshProUGUI text_ClassUpPrice_Stik;
        [SerializeField] private CharacterIndex focusCharacterIndex;
        [SerializeField] private SkeletonGraphic anim_Character;
        [SerializeField] private InventoryItem inventoryItem;
        [SerializeField] private GameObject backGround;
        public bool isLimitedShopOpen;

        [SerializeField] private RectTransform group;

        private bool isPossibleClassUp;
        private bool isPossibleClassUp_Stik;

        [SerializeField] private GameObject stikInfoObj;
        [SerializeField] private TextMeshProUGUI text_HoldingSTik;

        [SerializeField] private GameObject noticeSkin;

        //new dongmin
        [SerializeField] private Reinforcement reinForceMent;


        public override void ActivePopup()
        {

        }

        public override void InActivePopup()
        {
            OnClick_CharacterDetail_Confirm();

            for (int i = 0; i < statusInfos.Length; i++)
            {
                statusInfos[i].gameObject.SetActive(false);
            }

            PopUpSequence(false);
        }

        public override void Initialize()
        {
            PopUpSequence(false);
            button_Close.onClick.AddListener(() => InActivePopup());
            button_SetCharacter.onClick.AddListener(() => OnClick_SetCharacter(focusCharacterIndex));

            button_ClassUp.button.onClick.AddListener(() => OnClick_ClassUp(focusCharacterIndex));
            button_ClassUp_Stik.button.onClick.AddListener(() => OnClick_ClassUp_Stik(focusCharacterIndex));
            button_Skin.onClick.AddListener(() => OnClick_OpenPopupSkin((int)focusCharacterIndex));

            uiGroups[0].buttons[2].onClick.AddListener(() => OnClick_CharacterDetail());
            uiGroups[1].buttons[0].onClick.AddListener(() => OnClick_CharacterDetail_Confirm());

            uiGroups[0].buttons[3].onClick.AddListener(() => reinForceMent.OnClick_StarReinforment());
            uiGroups[0].buttons[4].onClick.AddListener(() => reinForceMent.OnClick_UpgradeReinforment());
            uiGroups[0].buttons[5].onClick.AddListener(() => reinForceMent.OnClick_LevelReinforment());

            for (int i = 0; i < statusInfos.Length; i++)
            {
                statusInfos[i].gameObject.SetActive(false);
            }

            anim_Character.gameObject.SetActive(false);
            noticeSkin.SetActive(false);
        }

        public void SetRelationCharacter(CharacterData characterData)
        {
            for (int i = 0; i < characterCards.Length; i++)
            {
                characterCards[i].gameObject.SetActive(false);
            }

            if (characterData.relationChracterIndexes.Length <= 0) return;

            for (int i = 0; i < characterData.relationChracterIndexes.Length; i++)
            {
                CharacterData data = DataManager.Instance.dic_CharacterData[characterData.relationChracterIndexes[i]];

                characterCards[i].gameObject.SetActive(true);
                characterCards[i].InitializeFriendly(data);
            }
        }

        public void OnClick_SetCharacter(CharacterIndex characterIndex)
        {
            InventoryScreen.Instance.SetUserDec(true, characterIndex);
            InActivePopup();
        }

        public void OnClick_CharacterDetail()
        {
            CharacterData data = DataManager.Instance.dic_CharacterData[focusCharacterIndex];
            SetStatusInfo(data);

            uiGroups[0].ui_Group.SetActive(false);
            for (int i = 0; i < uiGroups[0].buttons.Length; i++)
            {
                uiGroups[0].buttons[i].gameObject.SetActive(false);
            }

            uiGroups[1].ui_Group.SetActive(true);
            uiGroups[1].buttons[0].gameObject.SetActive(true);

            string attackInfo = LanguageManager.Instance.GetStringData(DataManager.Instance.dic_CharacterData[focusCharacterIndex].characterAttackDescTextKey);

            text_AttackInfomation.text = attackInfo;
        }

        public void SetStatusInfo(CharacterData data)
        {
            bool isOcean = data.characterType == CharacterType.SUB;
            if (isOcean)
                fieldStatusInfo.FieldBuffInitiailize(data.fieldBuffType, data);

            reinForceMent.Initialize(data);

            for (int i = 0; i < data.statusTypes.Length; i++)
            {
                statusInfos[i].Initialize(data.statusTypes[i], data);
            }
        }

        public void OnClick_CharacterDetail_Confirm()
        {
            uiGroups[1].ui_Group.SetActive(false);
            uiGroups[1].buttons[0].gameObject.SetActive(false);

            uiGroups[0].ui_Group.SetActive(true);
            for (int i = 0; i < uiGroups[0].buttons.Length; i++)
            {
                uiGroups[0].buttons[i].gameObject.SetActive(true);
            }

            CharacterData data = DataManager.Instance.dic_CharacterData[focusCharacterIndex];

            //new dongmin
            SetStatusInfo(data);

            if (isLimitedShopOpen)
            {
                button_ClassUp.gameObject.SetActive(false);
                button_ClassUp_Stik.gameObject.SetActive(false);
                button_SetCharacter.gameObject.SetActive(false);
                stikInfoObj.SetActive(false);
            }
            else
            {
                if (data.characterClassLevel == CharacterClassManager.Instance.GetMaxClassLevel())
                {
                    //true
                    button_ClassUp.gameObject.SetActive(false);
                    button_ClassUp_Stik.gameObject.SetActive(false);
                    stikInfoObj.SetActive(false);
                }
                else if (data.characterClassLevel == 0)
                {
                    button_ClassUp.gameObject.SetActive(false);
                    button_ClassUp_Stik.gameObject.SetActive(false);
                    button_SetCharacter.gameObject.SetActive(false);
                    stikInfoObj.SetActive(false);
                }
            }
        }

        private void OnClick_ClassUp(CharacterIndex characterIndex)
        {
            if (isPossibleClassUp)
            {
                button_ClassUp.SetInterectible(false);
                button_ClassUp_Stik.SetInterectible(false);

                MoneyType moneyType = MoneyType.GEM;
                ReqCharacterClassUp data = new()
                {
                    characterId = (int)characterIndex,
                    paidType = moneyType.ToString()
                };

                _ = NetworkManager.Instance.ClassUpCharacter(moneyType, data, SuccessClassUp, FailedClassUp);
            }
            else
            {
                Debug.Log("Impossible ClassUp");
                SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                popup.SetNoticeMessage("UI_Not_Enough_Currency");
            }
        }
        
        private void OnClick_ClassUp_Stik(CharacterIndex characterIndex)
        {
            if (isPossibleClassUp_Stik)
            {
                button_ClassUp.SetInterectible(false);
                button_ClassUp_Stik.SetInterectible(false);

                MoneyType moneyType = MoneyType.STIK;
                ReqCharacterClassUp data = new()
                {
                    characterId = (int)characterIndex,
                    paidType = moneyType.ToString()
                };

                _ = NetworkManager.Instance.ClassUpCharacter(moneyType, data, SuccessClassUp, FailedClassUp);
            }
            else
            {
                Debug.Log("Impossible ClassUp");
                SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                popup.SetNoticeMessage("UI_Not_Enough_Currency");
            }
        }

        public IEnumerator CharacterClassUpAnim()
        {
            TrackEntry entry = anim_Character.AnimationState.SetAnimation(0, "LvUp", false);
            yield return new WaitForSpineAnimationComplete(entry);

            CharacterData data = DataManager.Instance.dic_CharacterData[focusCharacterIndex];
            bool isOcean = data.characterType == CharacterType.SUB;
            string animKey = isOcean ? "Idle_Inventory" : "Idle";
            anim_Character.AnimationState.SetAnimation(0, animKey, true);
        }

        public void SuccessClassUp(UserClassUpResponse resData, MoneyType moneyType)
        {
            //D_gem_classup
            AdjustInitializer.TrackEvent("2qy8uo");

            SoundManager.Instance.PlaySound(SoundKey.SF_UPGRADE);
            CharacterIndex characterIndex = (CharacterIndex)resData.characterId;
            CharacterData data = DataManager.Instance.dic_CharacterData[characterIndex];
            data.characterClassLevel = resData.classLevel;
            data.characterQuantity = resData.remainQuantity;
            inventoryItem.SetQuantity(data);
            InventoryScreen.Instance.GetInventoryItem(characterIndex).SetQuantity(data);

            inventoryItem.animation_FX.Play();
            StartCoroutine(CharacterClassUpAnim());

            //new dongmin

            bool isOcean = data.characterType == CharacterType.SUB;

            if (isOcean)
            {
                fieldStatusInfo.SetLevelUp();
                fieldStatusInfo.FieldBuffInitiailize(data.fieldBuffType, data);
            }

            reinForceMent.Initialize(data);

            for (int i = 0; i < data.statusTypes.Length; i++)
            {
                statusInfos[i].SetLevelUp();
                statusInfos[i].ClassUpSequence(data, data.statusTypes[i]);
            }
            
            if (moneyType == MoneyType.STIK)
                GetWalletStik();
            else if (moneyType == MoneyType.GEM)
                GetUserWallet();
        }

        private void GetUserWallet()
        {
            _ = NetworkManager.Instance.GetUserWalletInfo(SuccessWallet, "PTIK");
        }

        private void SuccessWallet(int gem)
        {
            UserInfoManager.Instance.gemValue = gem;
            LobbyManager.Instance.text_Gem.text = $"{gem}";

            SetClassUpButton();
            InventoryScreen.Instance.CompleteDecChange();
            InventoryScreen.Instance.uiDec.SetUserDec();
        }

        private async void GetWalletStik()
        {
            UserWalletHistory userStikWallethistory = await NetworkManager.Instance.SendToServerAsync<UserWalletHistory>(string.Format(Url.getUserWalletHistory, "STIK"), SendType.GET, TokenType.ACCESSTOKEN);
            UserInfoManager.Instance.userStikWalletHistory = userStikWallethistory;
            text_HoldingSTik.text = userStikWallethistory.balance.uiAmountString;

            SetClassUpButton();
            InventoryScreen.Instance.CompleteDecChange();
            InventoryScreen.Instance.uiDec.SetUserDec();
        }

        public void FailedClassUp()
        {
            Debug.Log("Failed ClassUp");
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetNoticeMessage("SYSTEM_ERROR");


            CharacterData data = DataManager.Instance.dic_CharacterData[focusCharacterIndex];

            inventoryItem.SetQuantity(data);
        }

        private void SetClassUpButton()
        {
            CharacterData data = DataManager.Instance.dic_CharacterData[focusCharacterIndex];

            if (data.characterClassLevel == 0)
            {

            }
            else if (data.characterClassLevel >= CharacterClassManager.Instance.GetMaxClassLevel())
            {
                //false
                button_ClassUp.gameObject.SetActive(false);
                button_ClassUp_Stik.gameObject.SetActive(false);
                stikInfoObj.SetActive(false);
            }
            else
            {
                // Gem Button
                button_ClassUp.gameObject.SetActive(true);
                int price = CharacterClassManager.Instance.GetClassUpGemPrice(data.characterClassLevel);
                int userTik = UserInfoManager.Instance.gemValue;
                //int characterRequiredQuantityIndex = data.characterClassLevel - 1 - (int)data.characterGrade;
                int classUpQuantity = CharacterClassManager.Instance.GetClassUpInfo(data.characterGrade, data.characterClassLevel);

                bool isEnoughTaika = userTik >= price;
                bool isEnoughCharacterQuantity = data.characterQuantity >= classUpQuantity;

                text_ClassUpPrice.text = $"<sprite=2>{price}";

                if (isEnoughTaika && isEnoughCharacterQuantity)
                {
                    isPossibleClassUp = true;
                    button_ClassUp.SetInterectible(true);;
                }
                else
                {
                    button_ClassUp.SetInterectible(false);
                    isPossibleClassUp = false;
                }

                // Stik Button
                if (!UserInfoManager.Instance.IsBlockRegion)
                {
                    stikInfoObj.SetActive(true);
                    text_HoldingSTik.text = UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString;

                    button_ClassUp_Stik.gameObject.SetActive(true);

                    double stikPrice = CharacterClassManager.Instance.GetClassUpStikPrice(data.characterClassLevel);
                    text_ClassUpPrice_Stik.text = $"<sprite=14>{stikPrice}";

                    double userStik = double.Parse(UserInfoManager.Instance.userStikWalletHistory.balance.uiAmountString);
                    bool isEnoughStik = stikPrice <= userStik;

                    if (isEnoughStik && isEnoughCharacterQuantity)
                    {
                        isPossibleClassUp_Stik = true;
                        button_ClassUp_Stik.SetInterectible(true);
                    }
                    else
                    {
                        button_ClassUp_Stik.SetInterectible(false);
                        isPossibleClassUp_Stik = false;
                    }
                }
                else
                {
                    stikInfoObj.SetActive(false);
                    button_ClassUp_Stik.gameObject.SetActive(false);
                }
            }
        }

        public void SetCharacterData(CharacterData data)
        {
            anim_Character.skeletonDataAsset = data.anim;
            anim_Character.gameObject.SetActive(true);

            anim_Character.SkeletonDataAsset.Clear();
            anim_Character.AnimationState.ClearTracks();


            List<Skin> skin_list = anim_Character.skeletonDataAsset.GetAnimationStateData().SkeletonData.Skins.ToList();
            anim_Character.initialSkinName = skin_list.Count > 1 ? skin_list[1].Name : skin_list[0].Name;

            anim_Character.Initialize(true);

            text_Grade.color = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[data.characterGrade].color_decGradeText;
            text_Grade.text = LanguageManager.Instance.GetStringData($"UI_Grade_{(int)data.characterGrade}");

            bool isOcean = data.characterType == CharacterType.SUB;
            string animKey = isOcean ? "Idle_Inventory" : "Idle";
            fieldStatusInfo.gameObject.SetActive(isOcean);
            float yValue = isOcean ? -130f : 0;

            if (isOcean)
            {
                fieldStatusInfo.FieldBuffInitiailize(data.fieldBuffType, data);
            }

            group.DOLocalMoveY(yValue, 0);
            anim_Character.AnimationState.SetAnimation(0, animKey, true);

            focusCharacterIndex = data.characterIndex;

            backGround.SetActive(!isOcean);

            inventoryItem.Initialize(data, false);

            //new dongmin
            reinForceMent.Initialize(data);

            string name = LanguageManager.Instance.GetStringData(data.characterNameTextKey);
            text_CharacterName.text = name;
            string info = LanguageManager.Instance.GetStringData(data.characterDescTextKey);
            text_Infomation.text = info;
            string attackInfo = LanguageManager.Instance.GetStringData(data.characterAttackDescTextKey);
            text_AttackInfomation.text = attackInfo;

            if (data.characterClassLevel < CharacterClassManager.Instance.GetMaxClassLevel())
            {
                int price = CharacterClassManager.Instance.GetClassUpGemPrice(data.characterClassLevel);
                text_ClassUpPrice.text = $"<sprite=2>{price}";
                // todo : price 계산해서 넣어야 함
                text_ClassUpPrice_Stik.text = $"<sprite=14>{price}";
            }

            bool isActive = data.characterClassLevel != 0;

            button_ClassUp.gameObject.SetActive(isActive);
            button_ClassUp_Stik.gameObject.SetActive(isActive);
            button_SetCharacter.gameObject.SetActive(isActive);
            stikInfoObj.SetActive(isActive);

            SetRelationCharacter(data);
            SetClassUpButton();

            for (int i = 0; i < statusInfos.Length; i++)
            {
                statusInfos[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < data.statusTypes.Length; i++)
            {
                statusInfos[i].gameObject.SetActive(true);
                statusInfos[i].Initialize(data.statusTypes[i], data);
            }
        }

        private void OnClick_OpenPopupSkin(int character_id)
        {
            SkinPopup popup = PopupManager.Instance.GetPopUp<SkinPopup>("skin");
            popup.ActivePopup();

            popup.SetUI(character_id);
        }
    }


}
