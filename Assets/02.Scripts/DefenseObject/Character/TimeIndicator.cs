using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

namespace Framework.Game.Defense
{
    public class TimeIndicator : MonoBehaviour
    {
        public Character targetCharacter;
        public bool isDrag = false;
        public Image image_Indicator;

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => targetCharacter.isDrag)
                .Subscribe(_ => transform.position = targetCharacter.transform.localPosition);
        }

        public void FocusCharacter(bool isFocus)
        {
            if (isFocus)
            {
                transform.SetParent(UIManager.GetDynamicFocusTransform());
            }
            else
            {
                transform.SetParent(UIManager.GetDynamicCanvasTransform());
            }
        }

        public void SetPosition()
        {
            transform.position = targetCharacter.transform.localPosition;
        }

        public void SetIndicator(float value)
        {
            image_Indicator.fillAmount = value;
        }

        public void Initialize(Character character)
        {
            targetCharacter = character;

            transform.SetParent(UIManager.GetDynamicCanvasTransform());
            image_Indicator.fillAmount = 0;

            transform.localScale = Vector2.one;
            transform.position = targetCharacter.transform.localPosition;
        }

        public void SetWorldPosition()
        {
            Vector2 setVector = new(targetCharacter.transform.localPosition.x, targetCharacter.transform.localPosition.y);

            Vector2 pos = Camera.main.WorldToScreenPoint(setVector);

            transform.position = pos;
        }

        public Vector2 GetWorldPosition()
        {
            Vector2 setVector = new(targetCharacter.transform.localPosition.x, targetCharacter.transform.localPosition.y);

            Vector2 pos = Camera.main.WorldToScreenPoint(setVector);

            return pos;
        }
    }
}
