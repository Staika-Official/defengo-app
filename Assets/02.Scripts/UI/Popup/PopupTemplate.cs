using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Framework.UI
{
    public abstract class PopupTemplate : MonoBehaviour
    {
        public Ease easeType;

        public UISequences uiSequences;

        public CanvasGroup canvasGroup;
        public Button button_Close;
        public UnityAction PauseAction;

        public void PopUpSequence(bool isActive)
        {
            Debug.Log($"{gameObject.name} sequence: {isActive}");

            float alpha = isActive ? 1 : 0;
            canvasGroup.alpha = alpha;
            canvasGroup.blocksRaycasts = isActive;
            canvasGroup.interactable = isActive;

            if (uiSequences != null)
            {
                uiSequences.ExcuteSeqeunce();
            }

            if(PopupManager.Instance != null)
            {
                transform.SetParent(PopupManager.GetPopupRect(isActive));
            }

            PauseAction?.Invoke();
        }

        public void InactiveSequence()
        {

        }

        public abstract void Initialize();
        public abstract void ActivePopup();
        public abstract void InActivePopup();
    }
}
