using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Util;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;

namespace Framework.UI
{
    public class DecChangePopup : PopupTemplate
    {
        public ChanageDecButton[] changeDecButtons;
        public ChanageDecButton oceanDecButton;
        public Image image_FocusCharacterPortrait;
        public Image[] image_BackGround;
        public TextMeshProUGUI text_CharacterName;
        public CharacterIndex focusCharacterIndex;

        public override void ActivePopup()
        {
            Slot slot = UserSlotManager.Instance.GetSlotFocusIndexData();

            for (int i = 0; i < changeDecButtons.Length; i++)
            {
                int characterIdx = slot.slotCharacterIds[i];
                CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)characterIdx];
                ChanageDecButton chanageDecButton = changeDecButtons[i];

                chanageDecButton.image_Portrait.sprite = data.sprite_ChracterPortrait;
                
                string colorValue = DataManager.Instance.decButtonColorInfos[(int)data.characterGrade].backGroundColor;
                ColorUtility.TryParseHtmlString(colorValue, out Color color);
                chanageDecButton.image_BackGround.color = color;
            }
        }

        public void SetDecChange(CharacterData data)
        {
            focusCharacterIndex = data.characterIndex;
            string name = LanguageManager.Instance.GetStringData(data.characterNameTextKey);
            text_CharacterName.text = name;
            image_FocusCharacterPortrait.sprite = data.sprite_ChracterPortrait;
            int idx = (int)data.characterGrade;
            string innerColorData = DataManager.Instance.decButtonColorInfos[idx - 1].innerDecoColor;
            string outColorData = DataManager.Instance.decButtonColorInfos[idx - 1].backGroundColor;
            ColorUtility.TryParseHtmlString(innerColorData, out Color innerColor);
            ColorUtility.TryParseHtmlString(outColorData, out Color outColor);
            image_BackGround[0].color = innerColor;
            image_BackGround[1].color = outColor;
        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
                PopUpSequence(false);
            });
            for (int i = 0; i < changeDecButtons.Length; i++)
            {
                int clickIdx = i;
                changeDecButtons[i].button_DecChange.onClick.AddListener(() => OnClick_Set(clickIdx, focusCharacterIndex));
            }
        }
   
        public void OnClick_Set(int buttonIdx, CharacterIndex characterIndex)
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            Slot slot = UserSlotManager.Instance.GetSlotFocusIndexData();

            CharacterIndex slotIndex = (CharacterIndex)slot.slotCharacterIds[buttonIdx];
            if (slotIndex == characterIndex)
            {
                return;
            }
            else
            {
                bool isSameThing = false;

                for (int i = 0; i < changeDecButtons.Length; i++)
                {
                    CharacterIndex slotCharacter = (CharacterIndex)slot.slotCharacterIds[i];
                    if (slotCharacter == characterIndex)
                    {
                        Debug.Log($"slotCharacter == characterIndex /// {slotCharacter} / {characterIndex}");
                        isSameThing = true;
                        int temp = slot.slotCharacterIds[buttonIdx];
                        slot.slotCharacterIds[buttonIdx] = (int)characterIndex;
                        slot.slotCharacterIds[i] = temp;
                        break;
                    }
                }

                if (!isSameThing)
                {
                    slot.slotCharacterIds[buttonIdx] = (int)characterIndex;
                }
                UserSlotManager.Instance.SetChangeSlot(slot, InventoryScreen.Instance.SetInventoryItem);

                for (int i = 0; i < changeDecButtons.Length; i++)
                {
                    int characterIdx = slot.slotCharacterIds[i];
                    CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)characterIdx];
                    ChanageDecButton chanageDecButton = changeDecButtons[i];

                    chanageDecButton.image_Portrait.sprite = data.sprite_ChracterPortrait;                    
                }
            }

            InventoryScreen.Instance.uiDec.SetUserDec();
            
            //PopUpSequence(false);
            //PopupManager.Instance.GetPopUp<DecChangePopup>("decChange").PopUpSequence(false);
        }

    }
}
