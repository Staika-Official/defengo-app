using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using Gpm.Common.ThirdParty.LitJson;

namespace Framework.Util
{
    public class DataLoadManager : MonoBehaviour
    {
        public static DataLoadManager Instance;

        void Start()
        {
            if (Instance is null)
            {
                Instance = this;
            }
        }

        //Todo : Dec Data / Wave Data / Tile Data / Config Data
        public async Task<T> GetDataAsync<T>(string dataKey) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(dataKey);
            await handle.Task;
            T data = handle.Result;
            Addressables.Release(handle);
            return data;
        }

        public async Task<bool> IsDownloadResource(string key)
        {
            AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(key);
            await handle.Task;

            bool isDownloadCompleted = handle.Result == 0;
            return isDownloadCompleted;
        }

        public async Task<T> GetDataAsyncBinary<T>(string dataKey)
        {

#if UNITY_EDITOR
            TextAsset asset = (TextAsset)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/07.TextAsset/01.Table/" + dataKey + ".json", typeof(TextAsset));
            if (asset != null)
                return JsonMapper.ToObject<T>(asset.text);
#endif

            T data;
            AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(dataKey);
            await handle.Task;

            byte[] bytes = System.Convert.FromBase64String(handle.Result.text);
            string decode_string = System.Text.Encoding.UTF8.GetString(bytes);
            data = JsonMapper.ToObject<T>(decode_string);

            return data;
        }
    }
}
