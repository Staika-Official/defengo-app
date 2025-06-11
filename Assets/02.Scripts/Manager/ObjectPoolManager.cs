using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Framework.GameData.Defense;
using System.Linq;
using Framework.Network;
using Framework.Game.Defense;

namespace Framework.Util
{
    [System.Serializable]
    public class PoolingObject
    {
        public GameObject poolObject;
        public string objectName;
    }

    public class ObjectPoolManager : MonoBehaviour
    {
        public delegate void AssetLoad();
        public static AssetLoad OnCompleteAssetLoad;
        public Transform poolContainer;
        public Transform inactivePoolContainer;
        public Dictionary<string, Queue> objectQueueDic = new();
        public Dictionary<string, GameObject> poolingObjectPool = new();

        public List<Queue> queues = new();

        public List<string> poolObjectList;

        void Start()
        {
            SetObjectPool();
        }

        public async void SetObjectPool()
        {
            TextAsset asset = await DataLoadManager.Instance.GetDataAsync<TextAsset>("AssetListData");

            AssetList assetList = JsonUtility.FromJson<AssetList>(asset.text);

            poolObjectList = assetList.assetName.ToList();


            if (!UserInfoManager.Instance.userState.finishedTutorial)
            {
                Dictionary<string, CharacterData> dic_charater = GameManager.Instance.characterSpawner.characterDataDic;
                foreach (KeyValuePair<string, CharacterData> data in dic_charater)
                    AddCharacterObjectPoll(data.Value);

                CharacterData subData = GameManager.Instance.characterSpawner.subCharacter;
                AddCharacterObjectPoll(subData);

            }
            else
            {
                Slot slot = UserSlotManager.Instance.GetSlotFocusIndexData();

                foreach (int character_id in slot.slotCharacterIds)
                {
                    CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)character_id];
                    if (data != null)
                        AddCharacterObjectPoll(data);
                }
            }

            for (int i = 0; i < poolObjectList.Count; i++)
            {
                GameObject obj = await DataLoadManager.Instance.GetDataAsync<GameObject>(poolObjectList[i]);
                if (obj == null)
                    continue;

                PoolingObject poolingObject = new()
                {
                    poolObject = obj,
                    objectName = poolObjectList[i]
                };

                poolingObjectPool.Add(poolingObject.objectName, poolingObject.poolObject);
                Queue queue = new();
                objectQueueDic.Add(poolingObject.objectName, queue);
            }

            OnCompleteAssetLoad?.Invoke();
        }

        public T CreateNewObject<T>(string objName) where T : MonoBehaviour
        {
            T obj = Instantiate(poolingObjectPool[objName], transform).GetComponent<T>();
            obj.gameObject.SetActive(false);
            return obj;
        }

        public T GetObject<T>(string objName) where T : MonoBehaviour
        {
            if (objectQueueDic[objName].Count > 0)
            {
                T obj = objectQueueDic[objName].Dequeue() as T;
                obj.transform.SetParent(poolContainer);
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            {
                T obj = CreateNewObject<T>(objName);
                obj.transform.SetParent(poolContainer);
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        public void AllClear()
        {
            //Instance.objectQueueDic.Clear();

            //Instance.poolingObjectPool.Clear();
        }

        public void ReturnObject<T>(T obj, string objName) where T : MonoBehaviour
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(inactivePoolContainer);
            objectQueueDic[objName].Enqueue(obj);
        }

        private void AddCharacterObjectPoll(CharacterData data)
        {
            if (poolObjectList.Exists(a => a == data.name) == false)
                poolObjectList.Add(data.name);

            if (string.IsNullOrEmpty(data.projectileName) == false && data.projectileName != "projectile" && poolObjectList.Exists(a => a == data.projectileName) == false)
                poolObjectList.Add(data.projectileName);
        }
    }
}
