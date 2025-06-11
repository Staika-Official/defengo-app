using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Localization;
using System.IO;
using UnityEditor.Localization.Plugins.CSV;

public class JsonToBinarayEditor : EditorWindow
{
    [MenuItem("Window/Table/Convert")]
    public static void JsonToBinaray()
    {
        EditorWindow.GetWindow(typeof(JsonToBinarayEditor));
    }

    void OnGUI()
    {
        if (GUILayout.Button("ALL File"))
        {
            EditorUtility.DisplayProgressBar("Searching Prefabs", "", 0.0f);

            string[] files = System.IO.Directory.GetFiles("Assets/07.TextAsset/01.Table", "*.json");
            EditorUtility.DisplayCancelableProgressBar("Searching Prefabs", "Found " + files.Length + " prefabs", 0.0f);

            for (int i = 0; i < files.Length; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Processing Prefabs " + i + "/" + files.Length, files[i], (float)i / (float)files.Length))
                    break;

                TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(files[i]);

                if (asset != null)
                {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(asset.text);
                    string save_string = System.Convert.ToBase64String(bytes);

                    string byte_file_name = "Assets/07.TextAsset/01.Table/01.ExPort/" + asset.name + ".bytes";
                    System.IO.File.WriteAllText(byte_file_name, save_string);
                    AddAdressablesGroup(byte_file_name, asset.name);
                }
            }

            System.GC.Collect();
            EditorUtility.ClearProgressBar();
        }
        if (GUILayout.Button("Localize"))
        {
            var collection = LocalizationEditorSettings.GetStringTableCollection("UI_Text");
            using (var stream = new StreamReader("Assets/UI_Text.csv"))
            {
                Csv.ImportInto(stream, collection, false, null, true);
            }

        }
    }

    private void AddAdressablesGroup(string file_path, string name)
    {
        string guid = AssetDatabase.AssetPathToGUID(file_path);
        AddressableAssetSettings setting = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
        AddressableAssetGroup groups = setting.FindGroup("GameData");
        setting.CreateOrMoveEntry(guid, groups);
        AddressableAssetEntry entry = setting.FindAssetEntry(guid);
        if (entry != null)
        {
            entry.SetLabel("TextData", true);
            entry.SetAddress(name);
        }
    }
}
