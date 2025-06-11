using System.Collections;
using System.Collections.Generic;
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
    public class SkinBuyPopup : PopupTemplate
    {
        public TextMeshProUGUI m_Title;
        public TextMeshProUGUI m_BuyDesc;
        public TextMeshProUGUI m_BuyPrice;
        public SkeletonGraphic m_Anim;
        public Button m_BtnBuy;
        public Button m_BtnComfirm;

        private SkinTableData mSkinTable;

        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InActivePopup();
            });

            m_BtnBuy.onClick.AddListener((() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                if (UserInfoManager.Instance.gemValue < mSkinTable.price)
                {
                    SystemNoticePopup notice = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
                    notice.SetNoticeMessage("UI_Not_Enough_Currency");
                    return;
                }

                ReqSkinBuyData data = new ReqSkinBuyData
                {
                    characterId = mSkinTable.character_id,
                    skinId = mSkinTable.id
                };

                _ = NetworkManager.Instance.PostUserSkinBuy(data, SuccessBuy, FailBuy);
            }
            ));

            m_BtnComfirm.onClick.AddListener((() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                InActivePopup();
            }
            ));

            m_Anim.gameObject.SetActive(false);
        }

        public void SetUI(SkinTableData skin_data, CharacterType character_type)
        {
            mSkinTable = skin_data;

            m_Title.text = LanguageManager.Instance.GetStringData("UI_Skin_Purchase");
            m_BuyDesc.text = LanguageManager.Instance.GetStringData("UI_Skin_PurchaseDesc");

            m_Anim.Init(DataManager.Instance.CharacterResourceData.dic_Character[skin_data.id].skeletonDataAsset, character_type);
            m_BuyPrice.text = LanguageManager.Instance.GetPriceStringData(skin_data.price_type, skin_data.price);

            m_BtnBuy.gameObject.SetActive(true);
            m_BtnComfirm.gameObject.SetActive(false);

            ActivePopup();
        }

        private void SuccessBuy(ResUserSkins response)
        {
            m_Title.text = LanguageManager.Instance.GetStringData("UI_Skin_Get");
            m_BuyDesc.text = LanguageManager.Instance.GetStringData("UI_Skin_GetDesc");

            m_BtnBuy.gameObject.SetActive(false);
            m_BtnComfirm.gameObject.SetActive(true);

            DataManager.Instance.userCharacters.skins.Add(mSkinTable.id);
            DataManager.Instance.userCharacters.equippedSkins.Add(mSkinTable.id);

            SkinPopup popup = PopupManager.Instance.GetPopUp<SkinPopup>("skin");
            popup.OnSuccessSkinChange(response);

            LobbyManager.Instance.UpdateWalletInfo(mSkinTable.price_type);
        }

        private void FailBuy(string message)
        {

        }
    }
}

