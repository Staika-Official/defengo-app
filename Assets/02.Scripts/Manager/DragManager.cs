using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

namespace Framework.Game.Defense
{
    public class DragManager : MonoBehaviour
    {
        public Transform getTransform;
        public Character currentObject;
        public bool isTouched = false;
        public UnityAction onRelease;
        public bool isSellPos;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isTouched) return;
                isTouched = true;
                Vector3 mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                RaycastHit2D hit = Physics2D.Raycast(mousePos, transform.forward, 20f);

                if (hit)
                {
                    getTransform = hit.transform;
                    currentObject = hit.transform.GetComponent<Character>();
                    currentObject.SetTouch();

                    if (!currentObject.isDraggable) return;

                    UIManager.Instance.sellPopup.gameObject.SetActive(true);
                    UIManager.Instance.sellPopup.selectedCharacter = currentObject;
                    UIManager.Instance.sellPopup.SetSellPrice(currentObject);
                    UIManager.Instance.sellPanel.SetActive(true);
                    UIManager.Instance.canvasGroup_ButtonGroup.alpha = 0;

                }
                else
                {
                    getTransform = null;
                    currentObject = null;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (getTransform == null && currentObject == null) return;
                if (!currentObject.isDraggable) return;

                Vector3 mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                getTransform.transform.position = new Vector2(mousePos.x, mousePos.y);
                isSellPos = UIManager.Instance.sellPopup.isMouseOver;

                if(currentObject.isDestroyed)
                {
                    currentObject.FocusCharacter(false);
                    getTransform = null;
                    currentObject = null;
                    isTouched = false;
                    UIManager.Instance.sellPopup.gameObject.SetActive(false);
                    UIManager.Instance.sellPanel.SetActive(false);
                    UIManager.Instance.canvasGroup_ButtonGroup.alpha = 1;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if(currentObject == null)
                {
                    isTouched = false;
                    return;
                }

                if (isSellPos)
                {
                    UIManager.Instance.sellPopup.SellCharacter();
                    //멀티 컨트롤 조작 의심
                    UIManager.Instance.sellPopup.OnMouseOver();
                }

                if (currentObject != null)
                {
                    currentObject.SetRelease();
                    currentObject = null;
                }

                if(getTransform != null)
                {
                    getTransform = null;
                }
                isTouched = false;
                UIManager.Instance.sellPopup.gameObject.SetActive(false);
                UIManager.Instance.sellPanel.SetActive(false);
                UIManager.Instance.canvasGroup_ButtonGroup.alpha = 1;
            }
        }
    }
}
