using UnityEngine;
using Spine;
using Spine.Unity;
using Framework.GameData.Defense;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(menuName = "CharacterData")]
public class CharacterData : ScriptableObject
{
    public bool isNew;
    public bool isNewInventory;
    public bool isUsed;
    public bool isPossibleUpgrade;
    public bool isUniqueChange;
    public bool isUniqueUpgrade;
    public string characterName;


    public CharacterType characterType;
    public AttackType attackType;
    public CharacterIndex characterIndex;
    public CharacterGrade characterGrade;
    public ObscuredInt characterPowerLevel;
    public ObscuredInt characterClassLevel;
    public ObscuredInt characterQuantity;

    public ObscuredInt attackId;
    public ObscuredFloat targetCount;
    public ObscuredFloat attackDamage;
    public ObscuredFloat attackSpeed;
    public ObscuredFloat detectRange;
    public ObscuredFloat strikeRange;
    public ObscuredFloat criticalRange;
    public ObscuredFloat criticalDamageRate;
    public ObscuredFloat projectileSpeed;
    public bool isThrow;

    public ObscuredFloat buffValue;
    public ObscuredFloat buffRange;
    public ObscuredFloat buffCycle;
    public ObscuredFloat buffDuration;
    public ObscuredInt maxStarGradeValue;

    public string characterNameTextKey;
    public string characterDescTextKey;
    public string characterAttackDescTextKey;
    public bool isNFT;

    public StatusType[] statusTypes;
    public FieldBuffType fieldBuffType;

    public ObscuredFloat[] classUpFactor;
    public ObscuredFloat[] starFactor;
    public ObscuredFloat[] powerFactor;
    public ObscuredFloat[] characterUniqueValue;

    public bool isHiddenMission;

    public CharacterIndex[] relationChracterIndexes;

    public bool isOwned { get { return characterClassLevel > 0; } }

    public int selectSkinID { private set; get; }
    public SkinGradeType selectSkinType { private set; get; }
    public SkeletonDataAsset anim { private set; get; }
    public Sprite sprite_ChracterPortrait { private set; get; }
    public string projectileName { private set; get; }


    public void SetChangeSkin(int skin_id)
    {
        if (selectSkinID == skin_id)
            return;

        selectSkinID = skin_id;
        CharacterResource resource = DataManager.Instance.CharacterResourceData.dic_Character[selectSkinID];
        SkinTableData data = DataManager.Instance.SkinTableDataList.Find(a => a.id == skin_id);

        anim = resource.skeletonDataAsset;
        if (data.skin_grade_type == SkinGradeType.NONE && resource.ChracterPortrait != null)
            sprite_ChracterPortrait = resource.ChracterPortrait;


        if (data != null && data.character_id == (int)characterIndex)
        {
            selectSkinType = data.skin_grade_type;
            projectileName = data.projectile;
        }

    }

    /// <summary>
    /// JSON 데이터를 가져와서 기존 값들만 덮어쓰는 함수
    /// </summary>
    /// <param name="jsonData">JSON 문자열 데이터</param>
    public void OverrideFromJson(string jsonData)
    {
        try
        {
            // JSON을 Dictionary로 파싱
            var jsonDict = JsonUtility.FromJson<Dictionary<string, object>>(jsonData);
            if (jsonDict == null)
            {
                Debug.LogWarning("JSON 데이터 파싱 실패");
                return;
            }

            // 각 필드를 확인하고 값이 있으면 덮어쓰기
            if (jsonDict.ContainsKey("isNew") && jsonDict["isNew"] != null)
                isNew = Convert.ToBoolean(jsonDict["isNew"]);

            if (jsonDict.ContainsKey("isNewInventory") && jsonDict["isNewInventory"] != null)
                isNewInventory = Convert.ToBoolean(jsonDict["isNewInventory"]);

            if (jsonDict.ContainsKey("isUsed") && jsonDict["isUsed"] != null)
                isUsed = Convert.ToBoolean(jsonDict["isUsed"]);

            if (jsonDict.ContainsKey("isPossibleUpgrade") && jsonDict["isPossibleUpgrade"] != null)
                isPossibleUpgrade = Convert.ToBoolean(jsonDict["isPossibleUpgrade"]);

            if (jsonDict.ContainsKey("isUniqueChange") && jsonDict["isUniqueChange"] != null)
                isUniqueChange = Convert.ToBoolean(jsonDict["isUniqueChange"]);

            if (jsonDict.ContainsKey("isUniqueUpgrade") && jsonDict["isUniqueUpgrade"] != null)
                isUniqueUpgrade = Convert.ToBoolean(jsonDict["isUniqueUpgrade"]);

            if (jsonDict.ContainsKey("characterName") && jsonDict["characterName"] != null)
                characterName = jsonDict["characterName"].ToString();

            if (jsonDict.ContainsKey("characterType") && jsonDict["characterType"] != null)
                characterType = (CharacterType)Convert.ToInt32(jsonDict["characterType"]);

            if (jsonDict.ContainsKey("attackType") && jsonDict["attackType"] != null)
                attackType = (AttackType)Convert.ToInt32(jsonDict["attackType"]);

            if (jsonDict.ContainsKey("characterIndex") && jsonDict["characterIndex"] != null)
                characterIndex = (CharacterIndex)Convert.ToInt32(jsonDict["characterIndex"]);

            if (jsonDict.ContainsKey("characterGrade") && jsonDict["characterGrade"] != null)
                characterGrade = (CharacterGrade)Convert.ToInt32(jsonDict["characterGrade"]);

            if (jsonDict.ContainsKey("characterPowerLevel") && jsonDict["characterPowerLevel"] != null)
                characterPowerLevel = Convert.ToInt32(jsonDict["characterPowerLevel"]);

            if (jsonDict.ContainsKey("characterClassLevel") && jsonDict["characterClassLevel"] != null)
                characterClassLevel = Convert.ToInt32(jsonDict["characterClassLevel"]);

            if (jsonDict.ContainsKey("characterQuantity") && jsonDict["characterQuantity"] != null)
                characterQuantity = Convert.ToInt32(jsonDict["characterQuantity"]);

            if (jsonDict.ContainsKey("attackId") && jsonDict["attackId"] != null)
                attackId = Convert.ToInt32(jsonDict["attackId"]);

            if (jsonDict.ContainsKey("targetCount") && jsonDict["targetCount"] != null)
                targetCount = Convert.ToSingle(jsonDict["targetCount"]);

            if (jsonDict.ContainsKey("attackDamage") && jsonDict["attackDamage"] != null)
                attackDamage = Convert.ToSingle(jsonDict["attackDamage"]);

            if (jsonDict.ContainsKey("attackSpeed") && jsonDict["attackSpeed"] != null)
                attackSpeed = Convert.ToSingle(jsonDict["attackSpeed"]);

            if (jsonDict.ContainsKey("detectRange") && jsonDict["detectRange"] != null)
                detectRange = Convert.ToSingle(jsonDict["detectRange"]);

            if (jsonDict.ContainsKey("strikeRange") && jsonDict["strikeRange"] != null)
                strikeRange = Convert.ToSingle(jsonDict["strikeRange"]);

            if (jsonDict.ContainsKey("criticalRange") && jsonDict["criticalRange"] != null)
                criticalRange = Convert.ToSingle(jsonDict["criticalRange"]);

            if (jsonDict.ContainsKey("criticalDamageRate") && jsonDict["criticalDamageRate"] != null)
                criticalDamageRate = Convert.ToSingle(jsonDict["criticalDamageRate"]);

            if (jsonDict.ContainsKey("projectileSpeed") && jsonDict["projectileSpeed"] != null)
                projectileSpeed = Convert.ToSingle(jsonDict["projectileSpeed"]);

            if (jsonDict.ContainsKey("isThrow") && jsonDict["isThrow"] != null)
                isThrow = Convert.ToBoolean(jsonDict["isThrow"]);

            if (jsonDict.ContainsKey("buffValue") && jsonDict["buffValue"] != null)
                buffValue = Convert.ToSingle(jsonDict["buffValue"]);

            if (jsonDict.ContainsKey("buffRange") && jsonDict["buffRange"] != null)
                buffRange = Convert.ToSingle(jsonDict["buffRange"]);

            if (jsonDict.ContainsKey("buffCycle") && jsonDict["buffCycle"] != null)
                buffCycle = Convert.ToSingle(jsonDict["buffCycle"]);

            if (jsonDict.ContainsKey("buffDuration") && jsonDict["buffDuration"] != null)
                buffDuration = Convert.ToSingle(jsonDict["buffDuration"]);

            if (jsonDict.ContainsKey("maxStarGradeValue") && jsonDict["maxStarGradeValue"] != null)
                maxStarGradeValue = Convert.ToInt32(jsonDict["maxStarGradeValue"]);

            if (jsonDict.ContainsKey("characterNameTextKey") && jsonDict["characterNameTextKey"] != null)
                characterNameTextKey = jsonDict["characterNameTextKey"].ToString();

            if (jsonDict.ContainsKey("characterDescTextKey") && jsonDict["characterDescTextKey"] != null)
                characterDescTextKey = jsonDict["characterDescTextKey"].ToString();

            if (jsonDict.ContainsKey("characterAttackDescTextKey") && jsonDict["characterAttackDescTextKey"] != null)
                characterAttackDescTextKey = jsonDict["characterAttackDescTextKey"].ToString();

            if (jsonDict.ContainsKey("isNFT") && jsonDict["isNFT"] != null)
                isNFT = Convert.ToBoolean(jsonDict["isNFT"]);

            if (jsonDict.ContainsKey("isHiddenMission") && jsonDict["isHiddenMission"] != null)
                isHiddenMission = Convert.ToBoolean(jsonDict["isHiddenMission"]);

            Debug.Log($"CharacterData {characterName} JSON 데이터로 덮어쓰기 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON 데이터 덮어쓰기 중 오류 발생: {e.Message}");
        }
    }

    /// <summary>
    /// 현재 CharacterData의 모든 값을 JSON 형태로 변환하는 함수
    /// </summary>
    /// <returns>JSON 문자열</returns>
    public string ToJsonData()
    {
        try
        {
            var jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("{");

            // 기본 필드들
            jsonBuilder.Append($"\"isNew\":{isNew.ToString().ToLower()},");
            jsonBuilder.Append($"\"isNewInventory\":{isNewInventory.ToString().ToLower()},");
            jsonBuilder.Append($"\"isUsed\":{isUsed.ToString().ToLower()},");
            jsonBuilder.Append($"\"isPossibleUpgrade\":{isPossibleUpgrade.ToString().ToLower()},");
            jsonBuilder.Append($"\"isUniqueChange\":{isUniqueChange.ToString().ToLower()},");
            jsonBuilder.Append($"\"isUniqueUpgrade\":{isUniqueUpgrade.ToString().ToLower()},");
            jsonBuilder.Append($"\"characterName\":\"{characterName}\",");
            jsonBuilder.Append($"\"characterType\":{(int)characterType},");
            jsonBuilder.Append($"\"attackType\":{(int)attackType},");
            jsonBuilder.Append($"\"characterIndex\":{(int)characterIndex},");
            jsonBuilder.Append($"\"characterGrade\":{(int)characterGrade},");
            jsonBuilder.Append($"\"characterPowerLevel\":{characterPowerLevel},");
            jsonBuilder.Append($"\"characterClassLevel\":{characterClassLevel},");
            jsonBuilder.Append($"\"characterQuantity\":{characterQuantity},");
            jsonBuilder.Append($"\"attackId\":{attackId},");
            jsonBuilder.Append($"\"targetCount\":{targetCount},");
            jsonBuilder.Append($"\"attackDamage\":{attackDamage},");
            jsonBuilder.Append($"\"attackSpeed\":{attackSpeed},");
            jsonBuilder.Append($"\"detectRange\":{detectRange},");
            jsonBuilder.Append($"\"strikeRange\":{strikeRange},");
            jsonBuilder.Append($"\"criticalRange\":{criticalRange},");
            jsonBuilder.Append($"\"criticalDamageRate\":{criticalDamageRate},");
            jsonBuilder.Append($"\"projectileSpeed\":{projectileSpeed},");
            jsonBuilder.Append($"\"isThrow\":{isThrow.ToString().ToLower()},");
            jsonBuilder.Append($"\"buffValue\":{buffValue},");
            jsonBuilder.Append($"\"buffRange\":{buffRange},");
            jsonBuilder.Append($"\"buffCycle\":{buffCycle},");
            jsonBuilder.Append($"\"buffDuration\":{buffDuration},");
            jsonBuilder.Append($"\"maxStarGradeValue\":{maxStarGradeValue},");
            jsonBuilder.Append($"\"characterNameTextKey\":\"{characterNameTextKey}\",");
            jsonBuilder.Append($"\"characterDescTextKey\":\"{characterDescTextKey}\",");
            jsonBuilder.Append($"\"characterAttackDescTextKey\":\"{characterAttackDescTextKey}\",");
            jsonBuilder.Append($"\"isNFT\":{isNFT.ToString().ToLower()},");
            jsonBuilder.Append($"\"isHiddenMission\":{isHiddenMission.ToString().ToLower()},");
            jsonBuilder.Append($"\"fieldBuffType\":{(int)fieldBuffType},");
            jsonBuilder.Append($"\"selectSkinID\":{selectSkinID},");
            jsonBuilder.Append($"\"selectSkinType\":{(int)selectSkinType}");

            // 배열 데이터들 추가
            if (statusTypes != null && statusTypes.Length > 0)
            {
                jsonBuilder.Append(",\"statusTypes\":[");
                for (int i = 0; i < statusTypes.Length; i++)
                {
                    jsonBuilder.Append((int)statusTypes[i]);
                    if (i < statusTypes.Length - 1) jsonBuilder.Append(",");
                }
                jsonBuilder.Append("]");
            }

            if (classUpFactor != null && classUpFactor.Length > 0)
            {
                jsonBuilder.Append(",\"classUpFactor\":[");
                for (int i = 0; i < classUpFactor.Length; i++)
                {
                    jsonBuilder.Append((float)classUpFactor[i]);
                    if (i < classUpFactor.Length - 1) jsonBuilder.Append(",");
                }
                jsonBuilder.Append("]");
            }

            if (starFactor != null && starFactor.Length > 0)
            {
                jsonBuilder.Append(",\"starFactor\":[");
                for (int i = 0; i < starFactor.Length; i++)
                {
                    jsonBuilder.Append((float)starFactor[i]);
                    if (i < starFactor.Length - 1) jsonBuilder.Append(",");
                }
                jsonBuilder.Append("]");
            }

            if (powerFactor != null && powerFactor.Length > 0)
            {
                jsonBuilder.Append(",\"powerFactor\":[");
                for (int i = 0; i < powerFactor.Length; i++)
                {
                    jsonBuilder.Append((float)powerFactor[i]);
                    if (i < powerFactor.Length - 1) jsonBuilder.Append(",");
                }
                jsonBuilder.Append("]");
            }

            if (characterUniqueValue != null && characterUniqueValue.Length > 0)
            {
                jsonBuilder.Append(",\"characterUniqueValue\":[");
                for (int i = 0; i < characterUniqueValue.Length; i++)
                {
                    jsonBuilder.Append((float)characterUniqueValue[i]);
                    if (i < characterUniqueValue.Length - 1) jsonBuilder.Append(",");
                }
                jsonBuilder.Append("]");
            }

            if (relationChracterIndexes != null && relationChracterIndexes.Length > 0)
            {
                jsonBuilder.Append(",\"relationChracterIndexes\":[");
                for (int i = 0; i < relationChracterIndexes.Length; i++)
                {
                    jsonBuilder.Append((int)relationChracterIndexes[i]);
                    if (i < relationChracterIndexes.Length - 1) jsonBuilder.Append(",");
                }
                jsonBuilder.Append("]");
            }

            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON 데이터 변환 중 오류 발생: {e.Message}");
            return "{}";
        }
    }

 
   
}