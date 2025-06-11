using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Util;
using DG.Tweening;

namespace Framework.UI
{
    public class EventRewardPopup : PopupTemplate
    {
        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            StartCoroutine(AttractorSequence());
            PopUpSequence(false);
        }

        public IEnumerator AttractorSequence()
        {
            yield return new WaitForSeconds(0);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                AttractorManager.Instance.SetAttractor("stik", button_Close.transform);
                InActivePopup();
            });
        }
    }
}