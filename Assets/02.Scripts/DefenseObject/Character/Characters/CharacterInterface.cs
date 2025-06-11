using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

namespace Framework.Game.Defense
{
    public class CharacterInterface : MonoBehaviour
    {
        public GameObject[] stars;
        public GameObject gradeStar;

        public Character targetCharacter;
        public bool isDrag = false;

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isDrag)
                .Subscribe(_ => transform.position = targetCharacter.transform.localPosition);
        }

        public void FocusCharacter(bool isFocus)
        {
            if(isFocus)
            {
                transform.SetParent(UIManager.GetDynamicFocusTransform());
            }
            else
            {
                transform.SetParent(UIManager.GetDynamicCanvasTransform());
            }
        }

        public void Initialize()
        {
            transform.SetParent(UIManager.GetDynamicCanvasTransform());

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].SetActive(false);
            }

            transform.localScale = Vector2.one;
            transform.position = targetCharacter.transform.localPosition;
        }

        public void SetTargetCharacterPosition()
        {
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

        public void UpdateValue(int starGradeValue)
        {
            if(starGradeValue > 4)
            {
                for (int i = 0; i < 3; i++)
                {
                    stars[i].SetActive(false);
                    gradeStar.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < starGradeValue; i++)
                {
                    stars[i].SetActive(true);
                    gradeStar.SetActive(false);
                }
            }
        }
    }
}
