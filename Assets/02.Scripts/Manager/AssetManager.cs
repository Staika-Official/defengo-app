using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Framework.Util
{
    public class AssetManager : MonoBehaviour
    {
        public static AssetManager Instance;
        public delegate void DownloadComplete();
        public static DownloadComplete OnComplete_Download;

        public AsyncOperationHandle handle;

        private void Start()
        {
            if(Instance is null)
            {
                Instance = this;
            }
        }

        public async void AssetDownload()
        {
            string[] addressableDataLabels = new string[] { "Glacier", "TextData", "Sound" };
            //StartCoroutine(CoPercent());
            for (int i = 0; i < addressableDataLabels.Length; i++)
            {
                handle = Addressables.DownloadDependenciesAsync(addressableDataLabels[i]);
                await handle.Task;
            }

            OnComplete_Download?.Invoke();
        }


        public IEnumerator CoPercent()
        {
            while (true)
            {
                string percent = handle.PercentComplete.ToString();
                Debug.Log(percent);
                if(handle.PercentComplete == 1)
                {
                    break;
                }
                yield return null;
            }
        }
    }
}
