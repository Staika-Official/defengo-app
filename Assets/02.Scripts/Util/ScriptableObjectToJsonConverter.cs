#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class ScriptableObjectToJsonConverter
{
    [MenuItem("Tools/Convert All ScriptableObjects to JSON")]
    static void ConvertAll()
    {
        string inputPath = "Assets/03.Prefabs/DataPrefabs/Character"; // .asset 파일들이 있는 경로
        string outputPath = Application.dataPath + "/ExportedJson";

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        var guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { inputPath });
        var characterDataList = new System.Collections.Generic.List<CharacterData>();

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (obj != null && obj is CharacterData)
            {
                characterDataList.Add(obj as CharacterData);
            }
        }

        // 모든 CharacterData를 JSON 배열로 변환
        string jsonArray = ConvertCharacterDataListToJsonArray(characterDataList);
        File.WriteAllText(Path.Combine(outputPath, "CharacterDataArray.json"), jsonArray);

        Debug.Log($"변환 완료! {characterDataList.Count}개의 CharacterData가 배열로 저장되었습니다. 경로: {outputPath}/CharacterDataArray.json");
    }

    /// <summary>
    /// CharacterData 리스트를 JSON 배열로 변환
    /// </summary>
    /// <param name="characterDataList">CharacterData 리스트</param>
    /// <returns>JSON 배열 문자열</returns>
    static string ConvertCharacterDataListToJsonArray(System.Collections.Generic.List<CharacterData> characterDataList)
    {
        var jsonBuilder = new System.Text.StringBuilder();
        jsonBuilder.Append("[");

        for (int i = 0; i < characterDataList.Count; i++)
        {
            var characterData = characterDataList[i];
            jsonBuilder.Append(characterData.ToJsonData());
            
            if (i < characterDataList.Count - 1)
                jsonBuilder.Append(",");
        }

        jsonBuilder.Append("]");
        return jsonBuilder.ToString();
    }

    /// <summary>
    /// JSON 배열에서 CharacterData 리스트로 변환하는 함수 (역변환용)
    /// </summary>
    /// <param name="jsonArray">JSON 배열 문자열</param>
    /// <returns>CharacterData 리스트</returns>
    public static System.Collections.Generic.List<CharacterData> ConvertJsonArrayToCharacterDataList(string jsonArray)
    {
        var characterDataList = new System.Collections.Generic.List<CharacterData>();
        
        try
        {
            // JSON 배열을 파싱하여 각 CharacterData를 생성
            // 이 부분은 JSON 파싱 라이브러리나 Unity의 JsonUtility를 사용하여 구현할 수 있습니다.
            // 현재는 기본 구조만 제공합니다.
            
            Debug.Log("JSON 배열에서 CharacterData 리스트로 변환하는 기능은 별도 구현이 필요합니다.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON 배열 파싱 중 오류 발생: {e.Message}");
        }
        
        return characterDataList;
    }
}


#endif