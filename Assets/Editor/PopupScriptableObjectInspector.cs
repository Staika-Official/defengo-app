using Framework.UI;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[UnityEditor.CustomEditor(typeof(PopupScriptableObject))]
public class PopupScriptableObjectableInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("All Replace"))
        {
            PopupScriptableObject popup = Selection.activeObject as PopupScriptableObject;
            if (popup == null)
                return;

            string folder_path = GetFolderPath();

            popup.dic_Popup.Clear();

            string[] file_array = System.IO.Directory.GetFiles(folder_path);
            foreach (string name in file_array)
            {
                PopupTemplate obj = AssetDatabase.LoadAssetAtPath<PopupTemplate>(name);
                if (obj != null)
                    popup.dic_Popup.Add(new SerializableDictionary<string, PopupTemplate>.Pair(GetKeyString(obj.name), obj));
            }
            EditorUtility.SetDirty(popup);
        }

        if (GUILayout.Button("Add Popup"))
        {
            PopupScriptableObject popup = Selection.activeObject as PopupScriptableObject;
            if (popup == null)
                return;

            string folder_path = GetFolderPath();

            popup.dic_Popup.Clear();

            List<string> file_list = System.IO.Directory.GetFiles(folder_path).ToList();
            foreach (string name in file_list)
            {
                PopupTemplate obj = AssetDatabase.LoadAssetAtPath<PopupTemplate>(name);
                if (obj == null)
                    continue;

                string key = GetKeyString(obj.name);
                if (popup.dic_Popup.ContainsKey(key))
                    continue;

                popup.dic_Popup.Add(new SerializableDictionary<string, PopupTemplate>.Pair(key, obj));
            }

            EditorUtility.SetDirty(popup);
        }
        base.OnInspectorGUI();

    }

    private string GetFolderPath()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        string[] path_array = path.Split('/');
        string folder_path = "";
        for (int i = 0; i < path_array.Length - 1; i++)
        {
            folder_path += path_array[i];
            if (i < path_array.Length - 2)
                folder_path += "/";
        }

        return folder_path;
    }

    private string GetKeyString(string file_name)
    {
        string key = file_name.Replace("Popup - ", "");

        return key[0].ToString().ToLower() + key.Substring(1);
    }
}
