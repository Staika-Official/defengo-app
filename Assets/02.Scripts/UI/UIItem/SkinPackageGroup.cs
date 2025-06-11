using System.Collections;
using System.Collections.Generic;
using Framework.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Framework.GameData.Defense;

public class SkinPackageGroup : MonoBehaviour
{
    public Image m_Character;
    public Image m_CharacterFrame;
    public Image m_CharacterBackGround;
    public TextMeshProUGUI m_CharacterAmount;

    public Image m_Skin;

    public Image m_Profile;
    public Image m_ProfileGrade;
    public GameObject m_PlusProfile;



    public void Initialize(int skin_id, int character_amount, CharacterResource resource)
    {
        SkinTableData skin_data = DataManager.Instance.SkinTableDataList.Find(a => a.id == skin_id);
        if (skin_data == null)
            return;

        CharacterData character_data = DataManager.Instance.dic_CharacterData[(CharacterIndex)skin_data.character_id];
        if (character_data == null)
            return;

        CharacterGrade grade = character_data.characterGrade;
        CharacterCardInfo info = DataManager.Instance.uiPropertyData.dic_CharacterCardInfo[grade];

        m_Character.sprite = character_data.sprite_ChracterPortrait;
        m_CharacterFrame.sprite = info.sprite_Frame;
        m_CharacterBackGround.color = info.color_BackGround;
        m_CharacterAmount.text = $"x{character_amount}";
        m_Skin.sprite = resource.SkinPortrait;


        if (resource.ChracterPortrait != null)
        {
            m_PlusProfile.SetActive(true);
            m_Profile.gameObject.SetActive(true);

            m_Profile.sprite = resource.ChracterPortrait;
            m_ProfileGrade.color = DataManager.Instance.uiPropertyData.dic_CharacterSkinColor[skin_data.skin_grade_type].color_Profile;
        }
        else
        {
            m_PlusProfile.SetActive(false);
            m_Profile.gameObject.SetActive(false);
        }
    }
}
