using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastMessage : MonoBehaviour
{
    public ToastMessageItem image_prefab;
    
    public Queue<ToastMessageItem> queue_Messages = new();

    public Transform trans_ActiveParent;
    public Transform trans_InActiveParent;

    [HideInInspector] 
    public int Count = 0;
    
    [HideInInspector] 
    public bool isMessageCreate; 
    
    public void Initialize()
    {
        RefreshMessage();
    }
    
    public void RefreshMessage()
    {
        queue_Messages.Clear();
    }

    public void ReturnMessage(ToastMessageItem toastMessage)
    {
        toastMessage.transform.SetParent(trans_InActiveParent);
        toastMessage.transform.localPosition = Vector2.zero;
        toastMessage.gameObject.SetActive(false);
        queue_Messages.Enqueue(toastMessage);
    }
    
    public ToastMessageItem GetMessage()
    {
        if (queue_Messages.Count == 0)
        {
            ToastMessageItem toastMessage = Instantiate(image_prefab,trans_ActiveParent, false).GetComponent<ToastMessageItem>();
            toastMessage.transform.localScale = Vector2.one;
            toastMessage.Initialize();
            return toastMessage;
        }
        else
        {
            ToastMessageItem toastMessage = queue_Messages.Dequeue();
            toastMessage.transform.SetParent(trans_ActiveParent);
            return toastMessage;
        }
    }
    
    public void Sequence(string text)
    {
        if (isMessageCreate) return;

        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }
        
        Color color = Color.black;
        color.a = 0.6f;
        
        ToastMessageItem toastMessage = GetMessage();
        toastMessage.gameObject.SetActive(true);
        toastMessage.image_Toast.color = color;

        TextMeshProUGUI tmp = toastMessage.text_Toast;
        tmp.text = text;

        toastMessage.SequenceStart();
        isMessageCreate = Count >= 5;
    }
}
