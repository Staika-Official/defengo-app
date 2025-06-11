using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Sound;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Framework.UI
{
    public class SkinPopupItem : MonoBehaviour
    {
        public Image m_Image;
        public Image m_Background;
        public Image m_Frame;
        public Image m_Inner;
        public Gradient2 m_Gradient;
        public GameObject m_Lock;
        public GameObject m_Equip;
        public GameObject m_Select;
        public Button m_Button;

        public int SkinID { get; private set; }

        private SkinPopup mPopup;


        public void Initialize(SkinPopup popup)
        {
            mPopup = popup;

            m_Button.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                mPopup.SelectItem(SkinID);
            });
        }

        public void SetData(int skin_id, SkinGradeType type, Sprite portrait, bool is_owned, bool is_equip)
        {
            gameObject.SetActive(true);

            SkinID = skin_id;
            m_Image.sprite = portrait;
            m_Lock.SetActive(type != SkinGradeType.NONE && !is_owned);
            m_Equip.SetActive(is_equip);
            m_Select.SetActive(false);

            SkinColor skin_color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[type];
            m_Background.color = skin_color.color_Background;
            m_Frame.color = skin_color.color_Frame;
            m_Inner.color = skin_color.color_Inner;
            m_Gradient.enabled = skin_color.is_Active_Grdient;
            if (skin_color.is_Active_Grdient)
                m_Gradient.EffectGradient = skin_color.color_Inner_Grdient;
        }

        public void SelectItem(bool is_select)
        {
            m_Select.SetActive(is_select);
        }
    }
}

