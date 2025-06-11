using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Framework.UI
{
    public class ButtonTween : MonoBehaviour
    {
        private Button button;

        public float increaseDuration = 0.1f;
        public float decreaseDuration = 0.1f;
        public float maxScale = 1.2f;
        public AnimationCurve animationCurve;

        void Start()
        {
            button = GetComponent<Button>();

            button.onClick.AddListener(() => Tweening());
        }

        public void Tweening()
        {
            if (DOTween.IsTweening(this))
            {
                transform.DOScale(Vector2.one, 0);
                Sequence sequence = DOTween.Sequence()
                    .Append(transform.DOScale(maxScale, increaseDuration))
                    .Append(transform.DOScale(Vector2.one, decreaseDuration))
                    .SetEase(animationCurve);

                sequence.Play();
            }
        }
    }
}

