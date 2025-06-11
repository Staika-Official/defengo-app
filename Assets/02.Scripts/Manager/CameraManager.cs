using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Util
{
    public class CameraManager : MonoBehaviour
    {
        public float horizontalResolution = 1080;

        void Start()
        {
            float currentAspect = (float)Screen.width / (float)Screen.height;
            bool isMobile = currentAspect < 0.6;
            horizontalResolution = isMobile ? 1200 : 1920;
            Camera.main.orthographicSize = horizontalResolution / currentAspect / 230;
        }
    }
}
