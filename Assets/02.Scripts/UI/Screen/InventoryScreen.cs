using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.GameData.Defense;
using UnityEngine.UI;
using System.Linq;
using Framework.Sound;
using Framework.Network;
using UnityEngine.Events;
using TMPro;

namespace Framework.UI
{
    public class InventoryScreen : ScreenTemplate
    {
        public static InventoryScreen Instance;

        public Transform focusTransform;
        public Transform unfocusTransform;
        public GameObject panel_BackGround;
        public UnityAction realeaseAction;

        public UIDec uiDec;

        public InventoryItem[] glacierItems;
        public InventoryItem[] oceanItems;
        public DecChangeItem[] decChangeItems;
        public TextMeshProUGUI text_GlacierCharcterQuantity;
        public TextMeshProUGUI text_OceanCharacterQuantity;

        public bool isDecDataReady = false;

        public CharacterIndex focusCharacterIndex;

        private void Start()
        {
            Instance = this;
        }

        public override void ActiveScreen()
        {
            if (isDecDataReady)
            {
                SetInventoryItem();
            }

            LobbyNavigator.Instance.Notification(MenuType.INVENTORY, false);

            panel_BackGround.SetActive(false);
            LobbyManager.Instance.objectBottomDim.SetActive(true);
        }

        public override void InactiveScreen()
        {
            LobbyManager.Instance.objectBottomDim.SetActive(false);
            CompleteDecChange();
        }

        public void SetUserDec(bool isActive, CharacterIndex characterIndex)
        {
            LobbyManager.Instance.objectBottomDim.SetActive(false);
            bool isSubCharacter = (int)characterIndex >= 1000;
            Transform targetTransform = isActive ? focusTransform : unfocusTransform;
            focusCharacterIndex = characterIndex;
            if (isSubCharacter)
            {
                decChangeItems[5].transform.SetParent(targetTransform);
                decChangeItems[5].SetDecButton(isActive);
            }
            else
            {
                for (int i = 0; i < uiDec.characterCards.Length - 1; i++)
                {
                    decChangeItems[i].transform.SetParent(targetTransform);
                    decChangeItems[i].SetDecButton(isActive);
                }
            }

            // Tweening

            panel_BackGround.SetActive(isActive);
        }

        public void CompleteDecChange()
        {
            LobbyManager.Instance.objectBottomDim.SetActive(true);
            uiDec.SetUserDec(UserSlotManager.Instance.GetFocusIdx());
            SetUserDec(false, CharacterIndex.PENGGO);
            for (int i = 0; i < decChangeItems.Length; i++)
            {
                decChangeItems[i].SetDecButton(false);
            }
        }

        public void DecListInit()
        {
            isDecDataReady = true;
            uiDec.Initialize(menuType);
            uiDec.SetUserDec(UserSlotManager.Instance.GetFocusIdx());
        }

        public override void Initialize()
        {
            SetInventoryItem();

            for (int i = 0; i < decChangeItems.Length; i++)
            {
                decChangeItems[i].SetDecButton(false);
                decChangeItems[i].buttonIdx = i;
                decChangeItems[i].Initialize();
            }
        }

        public void ActivePopUp(CharacterData data)
        {
            InventoryPopup inventoryPopup = PopupManager.Instance.GetPopUp<InventoryPopup>("inventory");
            inventoryPopup.isLimitedShopOpen = false;
            inventoryPopup.SetCharacterData(data);
            inventoryPopup.PopUpSequence(true);

            //inventoryPopup.button_SetCharacter.gameObject.SetActive(true);
            //inventoryPopup.button_ClassUp.gameObject.SetActive(true);
        }

        public void ActiveEventPopUp(CharacterData data)
        {
            InventoryPopup inventoryPopup = PopupManager.Instance.GetPopUp<InventoryPopup>("inventory");
            inventoryPopup.isLimitedShopOpen = true;
            inventoryPopup.SetCharacterData(data);
            inventoryPopup.PopUpSequence(true);

            inventoryPopup.button_SetCharacter.gameObject.SetActive(false);
            inventoryPopup.button_ClassUp.gameObject.SetActive(false);
            inventoryPopup.button_ClassUp_Stik.gameObject.SetActive(false);
        }

        public void SetInventoryItem()
        {
            Dictionary<CharacterIndex, CharacterData> data = DataManager.Instance.dic_CharacterData;

            foreach (var item in data)
            {
                data[item.Key].isUsed = false;
            }

            List<CharacterData> usedList = new();
            List<CharacterData> unUsedList = new();
            List<CharacterData> unOwnedList = new();

            int[] slotInfo = UserSlotManager.Instance.GetSlotFocusIndexData().slotCharacterIds;
            int totalGlacierCharacterCount = 0;
            int totalOceanCharacterCount = 0;
            int unownedGlacierCharacterCount = 0;
            int unownedOceanCharacterCount = 0;

            for (int i = 0; i < slotInfo.Length; i++)
            {
                CharacterIndex idx = (CharacterIndex)slotInfo[i];
                usedList.Add(data[idx]);
                data[idx].isUsed = true;
            }

            foreach (var item in data)
            {
                if(data[item.Key].characterClassLevel > 0)
                {
                    if (data[item.Key].isUsed) continue;

                    data[item.Key].isUsed = false;
                    unUsedList.Add(data[item.Key]);
                }
                else
                {
                    unOwnedList.Add(data[item.Key]);

                    if(data[item.Key].characterType == CharacterType.SUB)
                    {
                        unownedOceanCharacterCount++;
                    }
                    else
                    {
                        unownedGlacierCharacterCount++;
                    }
                }
            }

            usedList.AddRange(unUsedList);
            usedList.AddRange(unOwnedList);

            int glacierIdx = 0;
            int oceanIdx = 0;

            for (int i = 0; i < usedList.Count; i++)
            {
                bool isOceanCharacter = usedList[i].characterType == CharacterType.SUB;
                
                if (isOceanCharacter)
                {
                    oceanItems[oceanIdx].Initialize(usedList[i], true);
                    totalOceanCharacterCount++;
                    oceanIdx++;
                }
                else
                {
                    glacierItems[glacierIdx].Initialize(usedList[i], true);
                    totalGlacierCharacterCount++;
                    glacierIdx++;
                }
            }

            text_GlacierCharcterQuantity.text = $"{totalGlacierCharacterCount - unownedGlacierCharacterCount}/{totalGlacierCharacterCount}";
            text_OceanCharacterQuantity.text = $"{totalOceanCharacterCount - unownedOceanCharacterCount}/{totalOceanCharacterCount}";
            DataManager.Instance.isDecDataReady = true;
            DecListInit();
            realeaseAction?.Invoke();
        }

        public InventoryItem GetInventoryItem(CharacterIndex characterIndex)
        {
            InventoryItem[] items = (int)characterIndex >= 1000 ? oceanItems : glacierItems;

            int idx = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if(items[i].characterIndex == characterIndex)
                {
                    idx = i;
                }
            }
            return items[idx];
        }

        public void OnDestroy()
        {
            Instance = null;
        }



        public void SetChangeDec(int buttonIdx)
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            Slot slot = UserSlotManager.Instance.GetSlotFocusIndexData();

            //Debug.Log(buttonIdx);
            CharacterIndex slotIndex = (CharacterIndex)slot.slotCharacterIds[buttonIdx];

            if(slotIndex == focusCharacterIndex)
            {
                return;
            }
            else
            {
                bool isSameThing = false;
                if((int)focusCharacterIndex < 1000)
                {
                    for (int i = 0; i < decChangeItems.Length - 1; i++)
                    {
                        CharacterIndex slotCharacter = (CharacterIndex)slot.slotCharacterIds[i];
                        if(slotCharacter == focusCharacterIndex)
                        {
                            isSameThing = true;
                            int temp = slot.slotCharacterIds[buttonIdx];
                            slot.slotCharacterIds[buttonIdx] = (int)focusCharacterIndex;
                            slot.slotCharacterIds[i] = temp;
                            break;
                        }
                    }
                }

                if(!isSameThing)
                {
                    //Debug.Log(isSameThing);
                    slot.slotCharacterIds[buttonIdx] = (int)focusCharacterIndex;
                }

                UserSlotManager.Instance.SetChangeSlot(slot, SetInventoryItem);

                CompleteDecChange();
            }

        }
    }
}
