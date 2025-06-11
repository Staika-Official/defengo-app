using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;


namespace Framework.UI
{

    public class CustomToggleSpecial : MonoBehaviour
    {
        [Header("TMP text Color")]
        public Color offColor;
        public Color onColor;

        [Header("TMP text Position")]
        [Space(2)]
        public float off_TMP_Position_L;
        public float off_TMP_Position_M;
        public float off_TMP_Position_R;
        public float on_TMP_Position_L;
        public float on_TMP_Position_M;
        public float on_TMP_Position_R;


        [Header("Handle Position")]
        [Space(2)]
        public float handlePosition_L;
        public float handlePosition_M;
        public float handlePosition_R;

        [Space(10)]
        [Header("Objs")]
        public Button Btn_L;
        public Button Btn_M;
        public Button Btn_R;
        public TMP_Text TMP_L;
        public TMP_Text TMP_M;
        public TMP_Text TMP_R;
        public GameObject handle;
        public Material offSharedMat;
        public Material onSharedMat;

        public UnityAction<PageState> onCompleteToggle;

        public void Initialize()
        {
            DefaultSet();

            Btn_L.onClick.AddListener(() =>
            {
                SetToggle(PageState.SPECIAL);
            });

            Btn_M.onClick.AddListener(() =>
            {
                SetToggle(PageState.SHOP);
            });

            Btn_R.onClick.AddListener(() =>
            {
                SetToggle(PageState.GEM);
            });
        }

        public void DefaultSet()
        {
            handle.transform.DOLocalMoveX(handlePosition_M, 0f);

            //left
            TMP_L.transform.DOLocalMoveX(off_TMP_Position_L, 0f);
            TMP_L.DOColor(offColor, 0f);

            //middle
            TMP_M.transform.DOLocalMoveX(on_TMP_Position_M, 0f);
            TMP_M.DOColor(onColor, 0f);

            //right
            TMP_R.transform.DOLocalMoveX(off_TMP_Position_R, 0f);
            TMP_R.DOColor(offColor, 0f);

            TMP_L.fontSharedMaterial = offSharedMat;
            TMP_M.fontSharedMaterial = onSharedMat;
            TMP_R.fontSharedMaterial = offSharedMat;
        }

        public void SetToggle(PageState state)
        {
            switch (state)
            {
                case PageState.SHOP:
                    //텍스트 머티리얼
                    TMP_L.fontSharedMaterial = offSharedMat;
                    TMP_M.fontSharedMaterial = onSharedMat;
                    TMP_R.fontSharedMaterial = offSharedMat;

                    //텍스트 이동
                    TMP_L.transform.DOLocalMoveX(off_TMP_Position_L, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    TMP_M.transform.DOLocalMoveX(on_TMP_Position_M, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    TMP_R.transform.DOLocalMoveX(off_TMP_Position_R, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);

                    //텍스트 색상
                    TMP_L.DOColor(offColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    TMP_M.DOColor(onColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    TMP_R.DOColor(offColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);

                    //핸들 이동
                    handle.transform.DOLocalMoveX(handlePosition_M, 0.2f).SetEase(Ease.OutQuad).SetAutoKill(true);
                    break;
                case PageState.GEM:
                    //텍스트 머티리얼
                    TMP_L.fontSharedMaterial = offSharedMat;
                    TMP_M.fontSharedMaterial = offSharedMat;
                    TMP_R.fontSharedMaterial = onSharedMat;

                    //텍스트 이동
                    TMP_L.transform.DOLocalMoveX(off_TMP_Position_L, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    TMP_M.transform.DOLocalMoveX(off_TMP_Position_M, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    TMP_R.transform.DOLocalMoveX(on_TMP_Position_R, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);

                    //텍스트 색상
                    TMP_L.DOColor(offColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    TMP_M.DOColor(offColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    TMP_R.DOColor(onColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);

                    //핸들 이동
                    handle.transform.DOLocalMoveX(handlePosition_R, 0.2f).SetEase(Ease.OutQuad).SetAutoKill(true);
                    break;
                case PageState.SPECIAL:
                    //텍스트 머티리얼
                    TMP_L.fontSharedMaterial = onSharedMat;
                    TMP_M.fontSharedMaterial = offSharedMat;
                    TMP_R.fontSharedMaterial = offSharedMat;

                    //텍스트 이동
                    TMP_L.transform.DOLocalMoveX(on_TMP_Position_L, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    TMP_M.transform.DOLocalMoveX(off_TMP_Position_M, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    TMP_R.transform.DOLocalMoveX(off_TMP_Position_R, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);

                    //텍스트 색상
                    TMP_L.DOColor(onColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    TMP_M.DOColor(offColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);
                    TMP_R.DOColor(offColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true).SetDelay(0.1f);

                    //핸들 이동
                    handle.transform.DOLocalMoveX(handlePosition_L, 0.2f).SetEase(Ease.OutQuad).SetAutoKill(true);
                    break;
            }

            onCompleteToggle?.Invoke(state);
        }
    }
}