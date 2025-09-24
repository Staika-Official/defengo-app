using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

public class CustomListButtons : MonoBehaviour
{
    public int selectedTab;

    [Header("TMP text Color")]
    public Color offColor;
    public Color onColor;

    [Header("TMP text Position")]
    [Space(2)]
    public List<float> off_TMP_Positions;
    public List<float> on_TMP_Positions;


    [Header("Handle Position")]
    [Space(2)]
    public List<float> handle_Positions;

    [Space(10)]
    [Header("Objs")]
    public List<Button> buttons;
    public List<TMP_Text> TMPs;
    public GameObject handle;
    public Material offSharedMat;
    public Material onSharedMat;

    public UnityAction<int> onCompleteSelect;


    public void Initialize()
    {
        DefaultSet();

        foreach (var btn in buttons)
        {
            btn.onClick.AddListener(() =>
            {
                SelectTab(buttons.IndexOf(btn));
            });
        }
    }

    public void DefaultSet()
    {
        for (int i = 0; i < TMPs.Count; i++)
        {
            if (i == selectedTab)
            {
                TMPs[i].transform.DOLocalMoveX(on_TMP_Positions[i], 0f);
                TMPs[i].DOColor(onColor, 0f);
                TMPs[i].fontSharedMaterial = onSharedMat;
                handle.transform.DOLocalMoveX(handle_Positions[i], 0f);
            }
            else
            {
                TMPs[i].transform.DOLocalMoveX(off_TMP_Positions[i], 0f);
                TMPs[i].DOColor(offColor, 0f);
                TMPs[i].fontSharedMaterial = offSharedMat;
            }
        }
    }

    public void SelectTab(int tab)
    {
        Debug.Log($"{gameObject.name} Select tab: tab");
        selectedTab = tab;
        for (int i = 0; i < TMPs.Count; i++)
        {
            if (i == selectedTab)
            {
                handle.transform.DOLocalMoveX(handle_Positions[i], 0.2f).SetAutoKill(true);
                TMPs[i].transform.DOLocalMoveX(on_TMP_Positions[i], 0.1f).SetAutoKill(true).SetDelay(0.1f);
                TMPs[i].DOColor(onColor, 0.1f).SetAutoKill(true).SetEase(Ease.OutQuad).SetDelay(0.1f);
                TMPs[i].fontSharedMaterial = onSharedMat;
            }
            else
            {
                TMPs[i].transform.DOLocalMoveX(off_TMP_Positions[i], 0.1f).SetAutoKill(true);
                TMPs[i].DOColor(offColor, 0.1f).SetEase(Ease.OutQuad).SetAutoKill(true);
                TMPs[i].fontSharedMaterial = offSharedMat;
            }
        }

        onCompleteSelect?.Invoke(tab);
    }
}
