using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Framework.UI
{
    [Serializable]
    public class OpenPopup
    {
        public AnimationCurve scaleCurve;
        public float startScale;
        public float endScale;
        public float duration;
    }

    [Serializable]
    public class OpenTopHeader
    {
        public Ease ease;
        public Vector3 movePos;
        public Vector3 startPos;
        public float duration;
    }

    public enum SequenceType
    {
        NONE,
        OPEN_POPUP,
        CLOSE_POPUP,
        TOP_HEADER,
        CLICK_UP,
        OPEN_SPAWNTIMER,
        APPEAR_BUTTON
    }


    public class SequenceManager : MonoBehaviour
    {
        public static SequenceManager Instance;
        public Sequence Sequence;
        public AnimationCurve _ClickUpCurve;

        void Start()
        {
            if(Instance is null)
            {
                Instance = this;
            }
        }


        public Sequence GetUISequence(SequenceType sequenceType, UISequences uISequences)
        {
            Sequence sequence = sequenceType switch
            {
                SequenceType.OPEN_POPUP => DOTween.Sequence()
                .Append(uISequences.transform.DOScale(uISequences.openPopup.startScale, 0))
                .Append(uISequences.transform.DOScale(uISequences.openPopup.endScale, uISequences.openPopup.duration)
                .SetEase(uISequences.openPopup.scaleCurve)),
                SequenceType.CLOSE_POPUP => DOTween.Sequence()
                .Append(uISequences.transform.DOScale(uISequences.openPopup.endScale, uISequences.openPopup.duration)
                .SetEase(uISequences.openPopup.scaleCurve)),
                SequenceType.TOP_HEADER => DOTween.Sequence()
                .Append(uISequences.transform.DOLocalMove(uISequences.openTopHeader.movePos, uISequences.openTopHeader.duration)
                .From()
                .SetEase(uISequences.openTopHeader.ease)),
                SequenceType.CLICK_UP => DOTween.Sequence()
                .Append(uISequences.transform.DOScale(Vector2.one, 0)).SetAutoKill(true)
                .Append(uISequences.transform.DOScale(1.1f,0.6f).SetEase(_ClickUpCurve).SetAutoKill(true)),
                SequenceType.OPEN_SPAWNTIMER => DOTween.Sequence()
                .Append(uISequences.transform.DOScale(0.4f,0.7f).SetAutoKill(true).From().SetEase(Ease.OutBounce)),
                SequenceType.APPEAR_BUTTON => throw new System.NotImplementedException(),
                _ => throw new System.NotImplementedException(),
            };
            return sequence;
        }
    }
}