using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;

namespace Framework.UI
{
    public abstract class ScreenTemplate : MonoBehaviour
    {
        public bool isActivated;
        public CanvasGroup canvasGroup;
        public MenuType menuType;

        public void ScreenSequence(bool isActive)
        {
            if(isActivated)
            {
                InactiveScreen();
            }

            isActivated = isActive;
            float alpha = isActive ? 1 : 0;
            canvasGroup.alpha = alpha;
            canvasGroup.blocksRaycasts = isActive;
            canvasGroup.interactable = isActive;
            if(isActive) ActiveScreen();
        }

        public void InitializePopup(PopupTemplate popup)
        {
            if (!popup.gameObject.activeSelf)
            {
                popup.gameObject.SetActive(true);
            }

            popup.PopUpSequence(false);
            popup.Initialize();
        }

        public abstract void Initialize();
        public abstract void ActiveScreen();
        public abstract void InactiveScreen();
    }
}
