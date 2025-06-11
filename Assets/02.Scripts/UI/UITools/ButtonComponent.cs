using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

namespace Framework.UI
{
    public class ButtonComponent : MonoBehaviour
    {
        public Button button;
        [SerializeField] private UISequences uISequences;
        public UnityAction onPointerUp;

        [SerializeField] private bool isInterectible = true;
        [SerializeField] private GameObject disableObject;

        private void Start()
        {
            button.OnPointerDownAsObservable()
            .Where(_ => isInterectible)
                .Subscribe(_ =>
                {
                    if (uISequences != null)
                    {
                        uISequences.ExcuteSeqeunce();
                    }
                });

            button.OnPointerUpAsObservable()
                .Where(_ => isInterectible)
                .Subscribe(_ =>
                {
                    onPointerUp?.Invoke();
                });
        }

        public void SetInterectible(bool isInterectible, bool isDisableObj = true)
        {
            button.interactable = isInterectible;
            this.isInterectible = isInterectible;
            
            if(isDisableObj && disableObject != null)
            {
                disableObject.SetActive(!isInterectible);
            }
        }
    }
}