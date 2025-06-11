using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Util
{
    public class SceneLoadManager : MonoBehaviour
    {
        public static SceneLoadManager Instance;
        public delegate void OnCompleteLoadScene();
        public static OnCompleteLoadScene onCompleteLoadScene;

        private void Start()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        public int GetCurrentScene()
        {
            int sceneIdx = SceneManager.GetActiveScene().buildIndex;
            return sceneIdx;
        }

        public void SwitchingScene(int sceneIdx)
        {
            StartCoroutine(SwitchingSceneAsync(sceneIdx));
        }

        public IEnumerator SwitchingSceneAsync(int sceneIndex)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneIndex);
            while (!async.isDone)
            {
                yield return null;
            }
            onCompleteLoadScene?.Invoke();
        }
    }
}
