using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;


namespace Framework.UI
{

    public class CustomToggle : MonoBehaviour
    {
        public bool onToggle;

        [Header("TMP text Color")]
        public Color offColor;
        public Color onColor;

        [Header("TMP text Position")]
        [Space(2)]
        public float off_TMP_Position_L;
        public float off_TMP_Position_R;
        public float on_TMP_Position_L;
        public float on_TMP_Position_R;
        

        [Header("Handle Position")]
        [Space(2)]
        public float handlePosition_L;
        public float handlePosition_R;

        [Space(10)]
        [Header("Objs")]
        public Button Btn_L;
        public Button Btn_R;
        public TMP_Text TMP_L;
        public TMP_Text TMP_R;
        public GameObject handle;
        public Material offSharedMat;
        public Material onSharedMat;

        public UnityAction<bool> onCompleteToggle;
        public UnityAction<PageState> onCompleteShopToggle;

        //private void OnEnable()
        //{
        //    Debug.Log("Init Toggle");
        //    Init();
        //}

        public void Initialize()
        {
            DefaultSet();

            Btn_L.onClick.AddListener(() =>
            {
                SetOnOff(false);
            });

            Btn_R.onClick.AddListener(() =>
            {
                SetOnOff(true);
            });
        }

        public void DefaultSet()
        {
            onToggle = false;
            TMP_L.transform.DOLocalMoveX(on_TMP_Position_L, 0f);
            TMP_L.DOColor(onColor, 0f);
            handle.transform.DOLocalMoveX(handlePosition_L, 0f);
            TMP_R.transform.DOLocalMoveX(off_TMP_Position_R, 0f);
            TMP_R.DOColor(offColor, 0f);
            TMP_L.fontSharedMaterial = onSharedMat;
            TMP_R.fontSharedMaterial = offSharedMat;
        }

        public void SetToggle(PageState pageState)
        {
            switch (pageState)
            {
                case PageState.SHOP:
                    handle.transform.DOLocalMoveX(handlePosition_R, 0.2f).SetEase(Ease.OutQuad).SetAutoKill(true);
                    TMP_R.transform.DOLocalMoveX(on_TMP_Position_R, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true)
                        .SetDelay(0.1f);
                    TMP_R.fontSharedMaterial = onSharedMat;
                    TMP_R.DOColor(onColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);

                    TMP_L.transform.DOLocalMoveX(off_TMP_Position_L, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true)
                        .SetDelay(0.1f);
                    TMP_L.fontSharedMaterial = offSharedMat;
                    TMP_L.DOColor(offColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    break;
                case PageState.GEM:
                    handle.transform.DOLocalMoveX(handlePosition_L, 0.2f).SetEase(Ease.OutQuad).SetAutoKill(true);
                    TMP_L.transform.DOLocalMoveX(on_TMP_Position_L, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true)
                        .SetDelay(0.1f);
                    TMP_L.fontSharedMaterial = onSharedMat;
                    TMP_L.DOColor(onColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);

                    TMP_R.transform.DOLocalMoveX(off_TMP_Position_R, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true)
                        .SetDelay(0.1f);
                    TMP_R.fontSharedMaterial = offSharedMat;
                    TMP_R.DOColor(offColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    break;
                default:
                    break;
            }

            onCompleteShopToggle?.Invoke(pageState);
        }

        public void SetOnOff(bool isLeftOn)
        {
            if (isLeftOn)
            {
                handle.transform.DOLocalMoveX(handlePosition_R, 0.2f).SetEase(Ease.OutQuad).SetAutoKill(true);
                TMP_R.transform.DOLocalMoveX(on_TMP_Position_R, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                TMP_R.fontSharedMaterial = onSharedMat;
                TMP_R.DOColor(onColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);

                TMP_L.transform.DOLocalMoveX(off_TMP_Position_L, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                TMP_L.fontSharedMaterial = offSharedMat;
                TMP_L.DOColor(offColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
            }
            else
            {
                handle.transform.DOLocalMoveX(handlePosition_L, 0.2f).SetEase(Ease.OutQuad).SetAutoKill(true);
                TMP_L.transform.DOLocalMoveX(on_TMP_Position_L, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                TMP_L.fontSharedMaterial = onSharedMat;
                TMP_L.DOColor(onColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);

                TMP_R.transform.DOLocalMoveX(off_TMP_Position_R, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                TMP_R.fontSharedMaterial = offSharedMat;
                TMP_R.DOColor(offColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
            }

            onCompleteToggle?.Invoke(!isLeftOn);

            //if (onoff != true) // false
            //{
            //    handle.transform.DOLocalMoveX(handlePosition_L, 0.2f).SetEase(Ease.OutQuad).SetAutoKill(true);
            //    TMP_L.transform.DOLocalMoveX(handlePosition_L, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
            //    TMP_L.fontSharedMaterial = onSharedMat;
            //    TMP_L.DOColor(onColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);

            //    TMP_R.transform.DOLocalMoveX(off_TMP_Position_R, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
            //    TMP_R.fontSharedMaterial = offSharedMat;
            //    TMP_R.DOColor(offColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);


            //}

            //if (onoff != false) // true
            //{
            //    handle.transform.DOLocalMoveX(handlePosition_R, 0.2f).SetEase(Ease.OutQuad).SetAutoKill(true);
            //    TMP_R.transform.DOLocalMoveX(handlePosition_R, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
            //    TMP_R.fontSharedMaterial = onSharedMat;
            //    TMP_R.DOColor(onColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);

            //    TMP_L.transform.DOLocalMoveX(off_TMP_Position_L, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
            //    TMP_L.fontSharedMaterial = offSharedMat;
            //    TMP_L.DOColor(offColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
            //}
        }
    }
}