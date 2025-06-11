using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using Framework.UI;
using Framework.Util;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class SkinPopup : PopupTemplate
    {
        public SkeletonGraphic m_Skeleton;
        public TextMeshProUGUI m_SkinName;

        //UI구성상 스킨은 최대2개가 나올예정
        public SkinPopupItem[] m_SkinItems;

        public Button m_BtnChange;
        public GameObject m_BtnChangeDisable;
        public Button m_BtnPurchase;
        public TextMeshProUGUI m_Price;
        public GameObject m_BtnPurchaseDisable;
        public TextMeshProUGUI m_PriceDisable;


        private CharacterData mCharacterData;

        private List<int> mOwnedSkins;
        private List<SkinTableData> mSkinDataList = new List<SkinTableData>();
        private SkinTableData mSelectSkinData;


        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            mCharacterData = null;
            mSelectSkinData = null;
            mSkinDataList.Clear();

            PopUpSequence(false);
        }

        public override void Initialize()
        {
            m_Skeleton.gameObject.SetActive(false);

            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InActivePopup();
            });

            m_BtnChange.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                _ = NetworkManager.Instance.PatchUserSkinChange(mSelectSkinData.character_id, mSelectSkinData.id, OnSuccessSkinChange, OnFailedSkinChange);
            });

            m_BtnPurchase.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                if (UserInfoManager.Instance.gemValue < mSelectSkinData.price)
                {
                    SystemNoticePopup notice = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                    notice.SetNoticeMessage("UI_Not_Enough_Currency");
                    return;
                }

                SkinBuyPopup popup = PopupManager.Instance.GetPopUp<SkinBuyPopup>("skinBuy");
                popup.SetUI(mSelectSkinData, mCharacterData.characterType);

                InActivePopup();
            });


            foreach (SkinPopupItem item in m_SkinItems)
                item.Initialize(this);
        }

        public void SetUI(int character_id)
        {
            mCharacterData = DataManager.Instance.dic_CharacterData[(CharacterIndex)character_id];
            mSkinDataList = DataManager.Instance.SkinTableDataList.FindAll(a => a.character_id == character_id).OrderBy(a => a.skin_grade_type).ToList();
            mOwnedSkins = DataManager.Instance.userCharacters.skins;
            CharacterScriptableObject resource = DataManager.Instance.CharacterResourceData;

            for (int i = 0; i < m_SkinItems.Length; i++)
            {
                if (i < mSkinDataList.Count)
                {
                    int skin_id = mSkinDataList[i].id;
                    bool is_equip = mCharacterData.selectSkinID == skin_id;
                    SkinTableData skin_data = mSkinDataList.Find(a => a.id == skin_id);

                    m_SkinItems[i].SetData(skin_id, skin_data.skin_grade_type, resource.dic_Character[skin_id].ChracterPortrait, mOwnedSkins.Contains(skin_id), is_equip);

                    if (is_equip)
                        SelectItem(skin_id);
                }
                else
                    m_SkinItems[i].gameObject.SetActive(false);

            }

        }

        public void SelectItem(int skin_id)
        {
            if (mSelectSkinData != null && mSelectSkinData.id == skin_id)
                return;

            mSelectSkinData = mSkinDataList.Find(a => a.id == skin_id);
            m_Skeleton.Init(DataManager.Instance.CharacterResourceData.dic_Character[skin_id].skeletonDataAsset, mCharacterData.characterType);



            m_SkinName.text = LanguageManager.Instance.GetStringData($"UI_Skin_{skin_id}");

            foreach (SkinPopupItem item in m_SkinItems)
            {
                item.SelectItem(item.SkinID == skin_id);
            }
            if (mCharacterData.isOwned == false)
            {
                bool is_purchase = mSelectSkinData.skin_grade_type != SkinGradeType.NONE;
                m_BtnPurchaseDisable.SetActive(is_purchase);
                m_BtnChangeDisable.SetActive(!is_purchase);

                m_BtnPurchase.gameObject.SetActive(false);
                m_BtnChange.gameObject.SetActive(false);
            }
            else if (mSelectSkinData.skin_grade_type != SkinGradeType.NONE && mOwnedSkins.Contains(skin_id) == false)
            {
                m_BtnPurchase.gameObject.SetActive(true);
                m_Price.text = LanguageManager.Instance.GetPriceStringData(mSelectSkinData.price_type, mSelectSkinData.price);
                m_PriceDisable.text = m_Price.text;

                m_BtnPurchaseDisable.SetActive(false);
                m_BtnChange.gameObject.SetActive(false);
                m_BtnChangeDisable.SetActive(false);
            }
            else
            {
                m_BtnPurchase.gameObject.SetActive(false);
                m_BtnPurchaseDisable.SetActive(false);

                bool is_disable = mCharacterData.selectSkinID == skin_id;
                m_BtnChange.gameObject.SetActive(!is_disable);
                m_BtnChangeDisable.SetActive(is_disable);
            }
        }

        public void OnSuccessSkinChange(ResUserSkins response)
        {
            ResUserSkinData res_skin_data = response.skins.ToList().Find(a => a.equipped);
            if (res_skin_data != null)
                DataManager.Instance.dic_CharacterData[(CharacterIndex)response.characterId].SetChangeSkin(res_skin_data.skinId);
            else
            {
                SkinTableData data = mSkinDataList.Find(a => a.skin_grade_type == SkinGradeType.NONE);
                DataManager.Instance.dic_CharacterData[(CharacterIndex)response.characterId].SetChangeSkin(data.id);
            }

            InventoryPopup popup = PopupManager.Instance.GetPopUp<InventoryPopup>("inventory");
            popup.SetCharacterData(DataManager.Instance.dic_CharacterData[(CharacterIndex)response.characterId]);

            InActivePopup();
        }

        private void OnFailedSkinChange(string errorcode)
        {
            switch (errorcode)
            {
                case "FAILED_EQUIPPED_SKIN":
                    SystemNoticePopup notice = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                    notice.SetNoticeMessage("NOT_OWNED");
                    break;
            }

            InActivePopup();
        }
    }
}

