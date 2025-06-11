using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;

public class InitSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        StartCoroutine(GameStart());
#endif
    }

    public IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1f);
        SceneLoadManager.Instance.SwitchingScene(1);
    }

}
