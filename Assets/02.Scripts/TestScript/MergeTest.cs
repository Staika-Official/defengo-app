using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeTest : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isTouched = false;
    public Vector2 startPos;
    public List<Collider2D> collisions;

    public void SetTouch()
    {
        isTouched = true;
        startPos = transform.position;
    }

    public void SetRelease()
    {
        if(collisions.Count > 0)
        {
            float temp = 10.0f;
            int idx = 0;
            for (int i = 0; i < collisions.Count; i++)
            {
                Vector2 myPos = transform.position;
                Vector2 objPos = collisions[i].transform.position;

                Vector2 vec = myPos - objPos;

                float dis = Vector2.SqrMagnitude(vec);

                if(temp >= dis)
                {
                    temp = dis;
                    idx = i;
                }
            }

            transform.position = collisions[idx].transform.position;
        }
        else
        {
            transform.position = startPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTouched) return;
        collisions.Add(collision);
        //Debug.Log("asd");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isTouched) return;
        collisions.Remove(collision);
        //Debug.Log("asd");
    }
}
