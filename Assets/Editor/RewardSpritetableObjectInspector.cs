using Framework.UI;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Framework.GameData.Defense;
using System.Threading.Tasks;
using Gpm.Common.ThirdParty.LitJson;



[UnityEditor.CustomEditor(typeof(RewardSpritetableObject))]
public class RewardSpritetableObjectInspector : Editor
{ 
    private List<RewardTableData> rewardTableDataList;

  public override void OnInspectorGUI()
    {
        if (GUILayout.Button("All Replace"))
        {
            RewardSpritetableObject reward = Selection.activeObject as RewardSpritetableObject;
            if (reward == null)
                return;

            reward.dic_RewardSprite.Clear();

            LoadSpriteFromFile(reward);
            
            EditorUtility.SetDirty(reward);
        }

        if (GUILayout.Button("Reward Table Update"))
        {
            RewardSpritetableObject reward = Selection.activeObject as RewardSpritetableObject;
            if (reward == null)
                return;

            rewardTableDataList = null;
        
            EditorUtility.SetDirty(reward);
        }
        base.OnInspectorGUI();
    }

  
    private async void LoadSpriteFromFile(RewardSpritetableObject reward)
    {
        string uiFolderPath = "Assets/14.Img/UI";
        string shopPackageFolderPath = "Assets/14.Img/ShopPakages";
        
        string[] uiFiles = System.IO.Directory.GetFiles(uiFolderPath, "*.png");
        string[] shopPackagefiles = System.IO.Directory.GetFiles(shopPackageFolderPath, "*.png");

        if (rewardTableDataList == null)
            rewardTableDataList = await GetDataAsync<List<RewardTableData>>("RewardData");

        foreach (var filePath in uiFiles)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            
            RewardTableData tableData = rewardTableDataList.Find(a => a.asset_thumbnail == fileName);
            
            if (tableData != null && !reward.dic_RewardSprite.ContainsKey(tableData.asset_thumbnail))
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(uiFolderPath + "/" + System.IO.Path.GetFileName(filePath));
                if (null != sprite)
                {
                    string key = tableData.asset_thumbnail;
                    reward.dic_RewardSprite.Add(new SerializableDictionary<string, Sprite>.Pair(key, sprite));
                }
            }
        }
        
        foreach (var filePath in shopPackagefiles)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            
            RewardTableData tableData = rewardTableDataList.Find(a => a.asset_thumbnail == fileName);
            
            if (tableData != null && !reward.dic_RewardSprite.ContainsKey(tableData.asset_thumbnail))
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(shopPackageFolderPath + "/" + System.IO.Path.GetFileName(filePath));
                if (null != sprite)
                {
                    string key = tableData.asset_thumbnail;
                    reward.dic_RewardSprite.Add(new SerializableDictionary<string, Sprite>.Pair(key, sprite));
                }
            }
        }
    }

    private async Task<T> GetDataAsync<T>(string dataKey)
    {   
        T data = default;
        
        TextAsset asset = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/07.TextAsset/01.Table/" + dataKey + ".json", typeof(TextAsset));
        if (asset != null)
            data = JsonMapper.ToObject<T>(asset.text);
        
        return data;
    }
}
