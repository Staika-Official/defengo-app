#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Game.Defense;
using UnityEditor;

namespace Framework.UI
{
    public class CustomEditor : EditorWindow
    {
        [MenuItem("Custom Particle/Particle")]
        static void DoSomething()
        {
            Debug.Log("Created Particle Object");
            GameObject obj = new("Base Particle");
            obj.AddComponent<ObjectParticle>();
        }

        [MenuItem("UI/Resize")]
        public static void Resize()
        {
            float currentAspect = (float)Screen.width / (float)Screen.height;
            Debug.Log(Screen.width);
            Debug.Log(Screen.height);
            Debug.Log(currentAspect);
            bool isMobile = currentAspect < 0.6;
            float horizontalResolution = isMobile ? 1200 : 1920;
            Camera.main.orthographicSize = horizontalResolution / currentAspect / 200;
        }
    }
}
#endif
