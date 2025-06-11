using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Network;
using UnityEngine.Events;

namespace Framework.GameData.Defense
{
    public class UserSlotManager : MonoBehaviour
    {
        public static UserSlotManager Instance;
        public UserSlot userSlot;

        public int focusIdx;

        private void Start()
        {
            if(Instance is null)
            {
                Instance = this;
                focusIdx = 1;
            }
        }

        public int GetFocusIdx()
        {
            return focusIdx;
        }

        public Slot GetSlotIndexData(int idx)
        {
            Slot slot = userSlot.slots[idx - 1];
            focusIdx = idx;
            return slot;
        }

        public void SetChangeSlot(Slot slot, UnityAction Success)
        {
            userSlot.slots[focusIdx - 1] = slot;
            SendUserSlotData(Success);
        }

        public Slot GetSlotFocusIndexData()
        {
            Slot slot = userSlot.slots[focusIdx - 1];
            return slot;
        }

        public void UserSlotData(UserSlot userSlot)
        {
            this.userSlot = userSlot;
        }

        public void SendUserSlotData(UnityAction Success)
        {
            Dictionary<string, object> data = new()
            {
                { "userId", UserInfoManager.Instance.userId},
                { "slotNumber", focusIdx},
                { "characterIds", userSlot.slots[focusIdx - 1].slotCharacterIds}
            };

            NetworkManager.Instance.SetUserSlotData(data, Success);
        }
    }
}
