using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using Framework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastMessageItem : MonoBehaviour
{
    public Image image_Toast;
    public TextMeshProUGUI text_Toast;
    public RectMask2D mask_Toast;

    public Sequence sequence_Toast;
    public void Initialize()
    {
        Sequence lastSequence = DOTween.Sequence()
            .SetAutoKill(false)
            .OnRewind(() =>
            {
                text_Toast.gameObject.SetActive(false);
                image_Toast.fillOrigin = (int)Image.OriginHorizontal.Right;
                image_Toast.fillAmount = 1f;
                Debug.Log("22");
            })
            .Append(image_Toast.DOFillAmount(0f, 3.2f))
            .OnComplete(() =>
            {
                //LobbyManager.Instance.toastMessage.Count--;
                //LobbyManager.Instance.toastMessage.ReturnMessage(this);
            });
        
        Sequence middleSequence = DOTween.Sequence()
            .SetAutoKill(false)
            .OnRewind(() =>
            {
                text_Toast.gameObject.SetActive(true);
            })
            .Append(text_Toast.rectTransform.DOPunchScale(Vector3.one * 2, 0.4f, 3, 2f))
            .OnComplete(() =>
            {
                lastSequence.Restart();
            });

        sequence_Toast = DOTween.Sequence()
            .OnStart(() =>
            {
                //LobbyManager.Instance.toastMessage.Count++;
            })
            .SetAutoKill(false)
            .OnRewind(() =>
            {
                text_Toast.gameObject.SetActive(false);
                image_Toast.fillOrigin = (int)Image.OriginHorizontal.Left;
                image_Toast.fillAmount = 0.1f;
                Debug.Log("11");
                Debug.Log(image_Toast.fillAmount);
            })
            .Append(DOTween.To(() => image_Toast.fillAmount, x => image_Toast.fillAmount = x, 0.5f, 2f).From())
            .OnComplete(() =>
            {
                middleSequence.Restart();
            });

       
    }

    public void SequenceStart()
    {
        sequence_Toast.Restart();
    }
    
}
