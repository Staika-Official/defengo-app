using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ArrowFX : MonoBehaviour
{
    public Image[] arrows;
    public float delayTime;
    public Color endColor;

    public void ArrowFxTTween(Image[] arrows)
    {
        delayTime = 0;

        foreach (Image item in arrows)
        {
            item.DOColor(endColor, 0.5f).SetEase(Ease.InOutQuad).SetDelay(delayTime).SetLoops(-1, LoopType.Yoyo);
            delayTime += 0.2f;
        }
    }

    private void OnEnable()
    {
        ArrowFxTTween(arrows);
    }
}
