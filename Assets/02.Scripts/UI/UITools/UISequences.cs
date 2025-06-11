using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using DG.Tweening;
using System;
using Framework.GameData.Defense;

namespace Framework.UI
{
    public class UISequences : MonoBehaviour
    {
        public SequenceType sequenceType = SequenceType.NONE;
        //[HideInInspector]
        public OpenPopup openPopup;
        //[HideInInspector]
        public OpenTopHeader openTopHeader;

        public Sequence Open_TopHeader()
        {
            return DOTween.Sequence()
                .Append(gameObject.transform.DOLocalMove(openTopHeader.movePos, openTopHeader.duration)
                .From()
                .SetEase(openTopHeader.ease));
        }
        public Sequence Open_Popup()
        {
            return DOTween.Sequence()
                .Append(gameObject.transform.DOScale(openPopup.endScale, openPopup.duration)
                .SetEase(openPopup.scaleCurve));
        }

        public Sequence Open_Button()
        {
            return DOTween.Sequence()
                .Append(gameObject.transform.DOScale(openPopup.endScale, openPopup.duration)
                .SetEase(openPopup.scaleCurve));
        }

        public void ExcuteSeqeunce()
        {
            Sequence sequence = SequenceManager.Instance.GetUISequence(sequenceType, this);
            bool isTweening = sequence.IsPlaying();
            if(!isTweening) sequence.Play();
        }
    }
}
