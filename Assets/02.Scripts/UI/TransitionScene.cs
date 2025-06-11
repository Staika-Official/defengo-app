using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionScene : MonoBehaviour
{
    
    Image bg;
    Material halftoneMat;
    public float dotScale;
    public bool dotDirectionToggle;



    private void Awake()
    {
        bg = GetComponent<Image>();
        halftoneMat = bg.material;
      
    }

    

    private void OnEnable()
    {
        StartCoroutine(DotScaleChangeValue());
        if(dotDirectionToggle is false)
        {
            dotScale = 0;
        }
        if(dotDirectionToggle is true)
        {
            dotScale = 7;
        }
        
      
    }

    IEnumerator DotScaleChangeValue()
    {
        while (true)
        {
            if (dotDirectionToggle is false)
            {
                halftoneMat.SetFloat("_X_directionToggleLeft", 0);
                halftoneMat.SetFloat("_X_directionToggleRight", 1);
            }
            if(dotDirectionToggle is true)
            {
                halftoneMat.SetFloat("_X_directionToggleLeft", 1);
                halftoneMat.SetFloat("_X_directionToggleRight", 0);
            }
            yield return new WaitForEndOfFrame();
            halftoneMat.SetFloat("_DotScale", dotScale);
        }
    }
}
