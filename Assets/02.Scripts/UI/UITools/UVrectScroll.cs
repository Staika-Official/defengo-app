using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UVrectScroll : MonoBehaviour
{
   
    public float Xspeed;
    public float Yspeed;

    RawImage rawImg;
    Rect uvRect;


    void Start()
    {
        this.gameObject.transform.TryGetComponent<RawImage>(out RawImage rImg);
        rawImg = rImg;
        uvRect = rawImg.uvRect;
    }

    void Update()
    {
        if (Xspeed > 0) uvRect.x += Time.deltaTime * Xspeed;
        if (Yspeed > 0) uvRect.y += Time.deltaTime * Yspeed;
        rawImg.uvRect = uvRect;
    }
}
