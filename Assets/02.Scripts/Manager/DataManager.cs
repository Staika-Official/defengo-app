using System.Collections.Generic;
using System.Linq;
using Framework.Network;
using Framework.Sound;
using Framework.UI;
using Framework.Util;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Framework.GameData.Defense
{
    [System.Serializable]
    public class UserProfileData
    {
        public bool isOwned;
        public ProfileType profileType;
        public int profileIndex;
        public Sprite sprite_image;
        public CharacterGrade characterGrade;
    }

    public class DataManager : MonoBehaviour
    {
        public SerializableDictionary<int, UserProfileData> dic_userProfileData = new();
        public Dictionary<CharacterIndex, CharacterData> dic_CharacterData = new();

        //부화석 정보들
        public Dictionary<string, UserItem> dic_hatchData = new();

        public static DataManager Instance;

        public delegate void DataLoad();
        public static DataLoad OnCompleteDataLoad;

        public DecButtonColorInfo[] decButtonColorInfos;

        public UIPropertyData uiPropertyData;
        public CharacterScriptableObject CharacterResourceData;
        public RewardSpritetableObject RewardSpriteResourceData;

        public bool isDecDataReady = false;

        public UserCharacters userCharacters;
        public StoreItemDTO storeItemDTO;
        public SpecialStoreDTO specialStoreDTO;
        public LimitedStoreItem limitedStoreItem;

        public delegate void OnProgress(float progress);
        public static OnProgress onProgress;

        public bool isFirst = false;

        //TableData
        public List<SkinTableData> SkinTableDataList;
        public List<StoreTableData> StoreTableDataList;
        public List<LimitedShopCeilingTableData> LimitedShopCeilingTableDataList;
        public List<RewardTableData> RewardTableDataList;
        public Dictionary<string, List<int>> RouletteTableData = new();

        private void Start()
        {
            if (Instance is null)
            {
                Instance = this;
                isDecDataReady = false;
            }

            AssetManager.OnComplete_Download = () =>
            {
                LoadTableDataList();
                LoadCharacterData();
            };
        }

        public void LoadHatchingStoneItemData(HatchingItemData itemdata)
        {
            dic_hatchData.Clear();

            //딕셔너리로 둔 이유
            //값을 꺼내올 때 저번에는 리스트로 했을 때 value안에있는 string값을 인덱스로 변환시켜서 꺼내와야했다
            //값을 가져올때 계속 스트링을 인덱스로 변환시키기엔 성능상 손해같아서 차라리 딕셔너리를 키값으로 둠

            for (int i = 0; i < itemdata.userItems.Length; ++i)
            {
                dic_hatchData.Add(itemdata.userItems[i].item, itemdata.userItems[i]);
            }

            dic_hatchData = dic_hatchData.OrderBy(x => GetHatchingGrade(x.Key)).ToDictionary(x => x.Key, x => x.Value);
        }

        public CharacterGrade GetHatchingGrade(string str)
        {
            CharacterGrade gradeIndex;

            gradeIndex = str switch
            {
                "HATCHING_ORB_COMMON" => CharacterGrade.COMMON,
                "HATCHING_ORB_UNCOMMON" => CharacterGrade.UNCOMMON,
                "HATCHING_ORB_RARE" => CharacterGrade.RARE,
                "HATCHING_ORB_EPIC" => CharacterGrade.EPIC,
                "HATCHING_ORB_LEGENDARY" => CharacterGrade.LEGENDARY,
                _ => CharacterGrade.COMMON
            };

            return gradeIndex;
        }

        private async void LoadTableDataList()
        {
            if (isFirst)
                return;

            SkinTableDataList = await DataLoadManager.Instance.GetDataAsyncBinary<List<SkinTableData>>("SkinData");
            StoreTableDataList = await DataLoadManager.Instance.GetDataAsyncBinary<List<StoreTableData>>("StoreData");
            LimitedShopCeilingTableDataList = await DataLoadManager.Instance.GetDataAsyncBinary<List<LimitedShopCeilingTableData>>("LimitedShop_CeilingData");
            RewardTableDataList = await DataLoadManager.Instance.GetDataAsyncBinary<List<RewardTableData>>("RewardData");
            //StoreTable 로드 이후
            IAPManager.Instance.Initialize();
        }



        public async void LoadCharacterData()
        {
            if (!isFirst)
            {
                System.Array soundKeys = System.Enum.GetValues(typeof(SoundKey));
                System.Array assetKeys = System.Enum.GetValues(typeof(CharacterIndex));

                int allLength = soundKeys.Length + assetKeys.Length;
                int currentLength = 0;

                for (int i = 0; i < soundKeys.Length; i++)
                {
                    currentLength++;

                    SoundData data = await DataLoadManager.Instance.GetDataAsync<SoundData>(soundKeys.GetValue(i).ToString());
                    SoundManager.Instance.dic_SoundData.Add((SoundKey)soundKeys.GetValue(i), data);
                    float progress = (float)currentLength / (float)allLength;
                    onProgress?.Invoke(progress);
                }

                for (int i = 0; i < assetKeys.Length; i++)
                {
                    currentLength++;
                    CharacterData data = await DataLoadManager.Instance.GetDataAsync<CharacterData>($"Character_{(int)assetKeys.GetValue(i)}");
                    data.isUsed = false;
                    dic_CharacterData.Add(data.characterIndex, data);
                    data.characterClassLevel = 0;
                    data.characterQuantity = 0;

                    //character skin
                    
                }

                GetCharacterFromServer();

                foreach (var key in dic_CharacterData.Keys)
                {
                    var data = dic_CharacterData[key];
                    List<SkinTableData> skin_list = SkinTableDataList.FindAll(a => a.character_id == (int)data.characterIndex).OrderBy(a => a.skin_grade_type).ToList();
                    foreach (SkinTableData skin in skin_list)
                    {
                        if (skin.skin_grade_type == SkinGradeType.NONE)
                            data.SetChangeSkin(skin.id);
                        else if (userCharacters.equippedSkins != null && userCharacters.equippedSkins.Contains(skin.id))
                            data.SetChangeSkin(skin.id);
                    }

                    float progress = (float)currentLength / (float)allLength;
                    onProgress?.Invoke(progress);
                }

                SetUserCharacterListNew();
            }

            onProgress?.Invoke(1);
            onProgress = null;
            isFirst = true;
            SceneLoadManager.Instance.SwitchingScene(2);
        }

        public void SetUserCharacterListNew()
        {
            for (int i = 0; i < userCharacters.characters.Count; i++)
            {
                Network.CharacterInfo characterInfo = userCharacters.characters[i];
                CharacterData data = dic_CharacterData[(CharacterIndex)characterInfo.characterId];

                data.characterClassLevel = characterInfo.classLevel;
                data.characterQuantity = characterInfo.quantity;
            }
        }

        public List<RewardTableData> GetRewardTableDataFromStoreTableData(int store_group_id)
        {
            List<StoreTableData> store_tableList = StoreTableDataList.FindAll(a => a.store_group_id == store_group_id);

            List<RewardTableData> reward_tableList = new();

            foreach (var store_tableData in store_tableList)
            {
                List<RewardTableData> data_list = RewardTableDataList.FindAll(a => a.reward_group == store_tableData.reward_group_id);

                reward_tableList.AddRange(data_list);
            }

            return reward_tableList;
        }

        public async void GetCharacterFromServer()
        {
            var json = await NetworkManager.Instance.GetCharacterFromServer();
            if (json != null)
            {
                JArray array = JArray.Parse(json);
                foreach (JObject item in array)
                {
                    var characterData = dic_CharacterData[(CharacterIndex)item["characterIndex"].Value<int>()];
                    characterData.OverrideFromJson(item);
                }
            }
        }

        [System.Serializable]
        public class CharacterDataArray
        {
            public CharacterData[] characters;
        }

        public void SetRoulettTableData(Dictionary<string, List<int>> data)
        {
            RouletteTableData.Clear();

            if (data != null)
                foreach (var kvp in data)
                    RouletteTableData[kvp.Key] = kvp.Value;

            //Debug.Log($"RouletTable {RouletteTableData}");
        }
    }
}
