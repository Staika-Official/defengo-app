using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class InventoryHatchingItem : MonoBehaviour
    {
        public Image image_Hatch_Icon;
        public Image image_Frame;
        public Image image_BackGround_Shdow;
        public Image image_Shadow;

        public void Initialize(UserItem item)
        {
            HatchingStoneCardInfo stoneCardInfo = DataManager.Instance.uiPropertyData.dic_HatchingCardInfo[item.item];

            image_Hatch_Icon.sprite = DataManager.Instance.uiPropertyData.dic_HatchingStoneSprite[item.item].sprite_Icon;

            image_BackGround_Shdow.color = stoneCardInfo.color_BackGround;
            image_Shadow.color = stoneCardInfo.color_Shadow;
        }
    }
}
