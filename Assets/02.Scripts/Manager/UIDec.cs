using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;

namespace Framework.UI
{
    [Serializable]
    public class DecProperty
    {
        public Image image_Portrait;
        public Image image_BackGround;
    }

    [Serializable]
    public class ButtonProperty
    {
        public GameObject[] image_ButtonIcon;
        public Button button_Select;
        public int decIdx;
    }

    public class UIDec : MonoBehaviour
    {
        public MenuType menuType;
        public ButtonProperty[] buttonProperties;
        public CharacterCard[] characterCards;
        public bool isRealease = false;

        private void Start()
        {
            UserSlot userSlot = UserSlotManager.Instance.userSlot;
            int length = userSlot.slots.Length;
            for (int i = 0; i < buttonProperties.Length; i++)
            {
                int idx = buttonProperties[i].decIdx;
                buttonProperties[i].button_Select.onClick.AddListener(() => OnClick_UserSlot(idx));
                if (i < length)
                {
                    if (i == 0)
                    {
                        buttonProperties[i].button_Select.interactable = false;
                        buttonProperties[i].image_ButtonIcon[0].SetActive(true);
                        buttonProperties[i].image_ButtonIcon[1].SetActive(false);
                        buttonProperties[i].image_ButtonIcon[2].SetActive(false);
                    }
                    else
                    {
                        buttonProperties[i].button_Select.interactable = true;
                        buttonProperties[i].image_ButtonIcon[0].SetActive(false);
                        buttonProperties[i].image_ButtonIcon[1].SetActive(true);
                        buttonProperties[i].image_ButtonIcon[2].SetActive(false);
                    }
                }
                else
                {
                    buttonProperties[i].button_Select.interactable = false;
                    buttonProperties[i].image_ButtonIcon[0].SetActive(false);
                    buttonProperties[i].image_ButtonIcon[1].SetActive(false);
                    buttonProperties[i].image_ButtonIcon[2].SetActive(true);
                }
            }
        }

        public void Initialize(MenuType menuType)
        {
            this.menuType = menuType;
            isRealease = false;

            //UserSlot userSlot = UserSlotManager.Instance.userSlot;
            //int length = userSlot.slots.Length;
            //for (int i = 0; i < buttonProperties.Length; i++)
            //{
            //    //int idx = buttonProperties[i].decIdx;
            //    //buttonProperties[i].button_Select.onClick.AddListener(() => OnClick_UserSlot(idx));
            //    if (i < length)
            //    {
            //        if (i == 0)
            //        {
            //            buttonProperties[i].button_Select.interactable = false;
            //            buttonProperties[i].image_ButtonIcon[0].SetActive(true);
            //            buttonProperties[i].image_ButtonIcon[1].SetActive(false);
            //            buttonProperties[i].image_ButtonIcon[2].SetActive(false);
            //        }
            //        else
            //        {
            //            buttonProperties[i].button_Select.interactable = true;
            //            buttonProperties[i].image_ButtonIcon[0].SetActive(false);
            //            buttonProperties[i].image_ButtonIcon[1].SetActive(true);
            //            buttonProperties[i].image_ButtonIcon[2].SetActive(false);
            //        }
            //    }
            //    else
            //    {
            //        buttonProperties[i].button_Select.interactable = false;
            //        buttonProperties[i].image_ButtonIcon[0].SetActive(false);
            //        buttonProperties[i].image_ButtonIcon[1].SetActive(false);
            //        buttonProperties[i].image_ButtonIcon[2].SetActive(true);
            //    }
            //}
        }

        public void SetUserDec(int slotIdx)
        {
            //Debug.Log("Set User Slot n");
            if (isRealease) return;
            isRealease = true;
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            UserSlotManager.Instance.focusIdx = slotIdx;
            UserSlot userSlot = UserSlotManager.Instance.userSlot;
            Slot slot = userSlot.slots[slotIdx - 1];
            int length = userSlot.slots.Length;
            for (int i = 0; i < buttonProperties.Length; i++)
            {
                if (i < length)
                {
                    if (i == slotIdx -1)
                    {
                        buttonProperties[i].button_Select.interactable = false;
                        buttonProperties[i].image_ButtonIcon[0].SetActive(true);
                        buttonProperties[i].image_ButtonIcon[1].SetActive(false);
                        buttonProperties[i].image_ButtonIcon[2].SetActive(false);
                    }
                    else
                    {
                        buttonProperties[i].button_Select.interactable = true;
                        buttonProperties[i].image_ButtonIcon[0].SetActive(false);
                        buttonProperties[i].image_ButtonIcon[1].SetActive(true);
                        buttonProperties[i].image_ButtonIcon[2].SetActive(false);
                    }
                }
                else
                {
                    buttonProperties[i].button_Select.interactable = false;
                    buttonProperties[i].image_ButtonIcon[0].SetActive(false);
                    buttonProperties[i].image_ButtonIcon[1].SetActive(false);
                    buttonProperties[i].image_ButtonIcon[2].SetActive(true);
                }
            }

            for (int i = 0; i < characterCards.Length; i++)
            {
                int characterIdx = slot.slotCharacterIds[i];
                CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)characterIdx];
                CharacterCard characterCard = characterCards[i];
                characterCard.Initialize(data);
            }
        }

        public void OnClick_UserSlot(int slotIdx)
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            UserSlotManager.Instance.focusIdx = slotIdx;
            UserSlot userSlot = UserSlotManager.Instance.userSlot;
            Slot slot = userSlot.slots[slotIdx - 1];
            int length = userSlot.slots.Length;
            for (int i = 0; i < buttonProperties.Length; i++)
            {
                if (i < length)
                {
                    if (i == slotIdx - 1)
                    {
                        buttonProperties[i].button_Select.interactable = false;
                        buttonProperties[i].image_ButtonIcon[0].SetActive(true);
                        buttonProperties[i].image_ButtonIcon[1].SetActive(false);
                        buttonProperties[i].image_ButtonIcon[2].SetActive(false);
                    }
                    else
                    {
                        buttonProperties[i].button_Select.interactable = true;
                        buttonProperties[i].image_ButtonIcon[0].SetActive(false);
                        buttonProperties[i].image_ButtonIcon[1].SetActive(true);
                        buttonProperties[i].image_ButtonIcon[2].SetActive(false);
                    }
                }
                else
                {
                    buttonProperties[i].button_Select.interactable = false;
                    buttonProperties[i].image_ButtonIcon[0].SetActive(false);
                    buttonProperties[i].image_ButtonIcon[1].SetActive(false);
                    buttonProperties[i].image_ButtonIcon[2].SetActive(true);
                }
            }

            for (int i = 0; i < characterCards.Length; i++)
            {
                int characterIdx = slot.slotCharacterIds[i];
                CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)characterIdx];
                CharacterCard characterCard = characterCards[i];
                characterCard.Initialize(data);
            }

            if (menuType == MenuType.INVENTORY)
            {
                //Debug.Log(menuType);
                InventoryScreen.Instance.realeaseAction = () =>
                {
                    isRealease = false;
                };
                InventoryScreen.Instance.SetInventoryItem();
            }
        }

        public void SetUserDec()
        {
            Slot slot = UserSlotManager.Instance.GetSlotFocusIndexData();

            for (int i = 0; i < characterCards.Length; i++)
            {
                int characterIdx = slot.slotCharacterIds[i];
                CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)characterIdx];
                CharacterCard characterCard = characterCards[i];
                characterCard.Initialize(data);
            }
        }
    }
}
