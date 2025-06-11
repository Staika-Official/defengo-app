using Framework.UI;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using Framework.Util;
using Gpm.Common.ThirdParty.LitJson;
using Framework.GameData.Defense;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UniRx;

[UnityEditor.CustomEditor(typeof(CharacterScriptableObject))]
public class CharacterScriptableObjectInspector : Editor
{

    private class AssetData
    {
        public string name;
        public int id;
        public Object asset;

        public AssetData(string name, int id, Object asset)
        {
            this.name = name;
            this.id = id;
            this.asset = asset;
        }
    }


    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("All Replace"))
        {
            CharacterScriptableObject character = Selection.activeObject as CharacterScriptableObject;
            if (character == null)
                return;

            List<SkinTableData> table_list = new List<SkinTableData>();
            List<AssetData> asset_list = new List<AssetData>();
            List<AssetData> profile_list = new List<AssetData>();
            List<AssetData> skin_list = new List<AssetData>();
            character.dic_Character.Clear();


            //TableData Add
            TextAsset asset = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/07.TextAsset/01.Table/SkinData.json", typeof(TextAsset));
            if (asset != null)
                table_list = JsonMapper.ToObject<List<SkinTableData>>(asset.text);


            //resource Add
            string[] file_array = System.IO.Directory.GetFiles("Assets/05.Animations/Character/");
            foreach (string name in file_array)
            {
                SkeletonDataAsset obj = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(name);
                if (obj != null)
                {
                    string[] name_array = obj.name.Split('_');
                    int id = System.Convert.ToInt32(name_array[1]);
                    asset_list.Add(new AssetData(name_array[1], id, obj));
                }
            }

            file_array = System.IO.Directory.GetFiles("Assets/14.Img/UIProfile/");
            foreach (string name in file_array)
            {
                Sprite obj = AssetDatabase.LoadAssetAtPath<Sprite>(name);
                if (obj != null)
                {
                    string[] name_array = obj.name.Split('_');
                    int id = name_array.Length > 2 ? System.Convert.ToInt32(name_array[2]) : 0;
                    string skin_name = name_array.Length > 2 ? $"{name_array[1]}_{name_array[2]}" : $"{name_array[1]}";
                    profile_list.Add(new AssetData(skin_name, id, obj));
                }
            }

            file_array = System.IO.Directory.GetFiles("Assets/14.Img/UISkin/");
            foreach (string name in file_array)
            {
                Sprite obj = AssetDatabase.LoadAssetAtPath<Sprite>(name);
                if (obj != null)
                {
                    string[] name_array = obj.name.Split('_');
                    int id = name_array.Length > 2 ? System.Convert.ToInt32(name_array[2]) : 0;
                    skin_list.Add(new AssetData(name_array[1], id, obj));
                }
            }

            //create list
            foreach (SkinTableData data in table_list)
            {
                CharacterResource resource_data = new CharacterResource();

                resource_data.character_id = data.character_id;

                AssetData asset_data = asset_list.Find(a => a.id == data.id);
                if (asset_data != null)
                    resource_data.skeletonDataAsset = (SkeletonDataAsset)asset_data.asset;

                int id = data.skin_grade_type == SkinGradeType.NONE ? 0 : data.id;
                asset_data = profile_list.Find(a => a.name == data.profile);
                if (asset_data != null)
                    resource_data.ChracterPortrait = (Sprite)asset_data.asset;

                asset_data = skin_list.Find(a => a.id == id);
                if (asset_data != null)
                    resource_data.SkinPortrait = (Sprite)asset_data.asset;

                character.dic_Character.Add(new SerializableDictionary<int, CharacterResource>.Pair(data.id, resource_data));
            }

            EditorUtility.SetDirty(character);
        }

        base.OnInspectorGUI();
    }

}
