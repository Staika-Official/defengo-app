using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Framework.Util;
using Framework.GameData.Defense;
using DG.Tweening;

namespace Framework.UI
{
    public class DecChangeItem : MonoBehaviour
    {
        public Button button_OnSelect;
        public int buttonIdx;

        public float rotateValue;
        public float rotateDuration;

        public DG.Tweening.Sequence sequence;
        
        public void SetDecButton(bool isActive)
        {
            button_OnSelect.interactable = isActive;

            TweenButton(isActive);
        }

        public void Initialize()
        {
            button_OnSelect.onClick.AddListener(() =>
            {
                InventoryScreen.Instance.SetChangeDec(buttonIdx);
            });
        }

        public void TweenButton(bool isActive)
        {
            if(isActive)
            {
                sequence = DOTween.Sequence()
                        .Append(transform.DORotate(new Vector3(0, 0, rotateValue), rotateDuration, RotateMode.Fast))
                        .Append(transform.DORotate(new Vector3(0, 0, -1 * rotateValue), rotateDuration, RotateMode.Fast))
                        .SetLoops(-1);

                sequence.Play();
            }
            else
            {
                sequence.Kill();
                transform.DORotate(Vector3.zero, 0, RotateMode.Fast);
            }
        }
    }
}
