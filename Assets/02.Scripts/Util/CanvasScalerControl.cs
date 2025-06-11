using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerControl : MonoBehaviour
{
    private void Awake()
    {
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        float match = Screen.height / Screen.width <= 1.5f ? 1.0f : 0f;

        scaler.matchWidthOrHeight = match;
    }
}
