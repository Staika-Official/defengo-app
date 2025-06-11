using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Framework.UI
{
    public class IndicateManager : MonoBehaviour
    {
        public TextMeshProUGUI text_Percent;
        public Image image_LoadingBar;

        public void SetIndicateValue(float percent)
        {
            image_LoadingBar.fillAmount = percent * 0.01f;
            text_Percent.text = $"{(int)percent}%";
        }
       
    }
}
