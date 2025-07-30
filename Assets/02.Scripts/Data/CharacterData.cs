using UnityEngine;
using Spine;
using Spine.Unity;
using Framework.GameData.Defense;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json.Linq;

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
    public void OverrideFromJson(JObject jsonData)
    {
        try
        {
            // JSON을 Dictionary로 파싱
            
            if (jsonData == null)
            {
                Debug.LogWarning("JSON 데이터 파싱 실패");
                return;
            }
    
            // 각 필드를 확인하고 값이 있으면 덮어쓰기
            if (jsonData.TryGetValue("isNew", out var isNewToken) && isNewToken != null)
                isNew = isNewToken.Value<bool>();

            if (jsonData.TryGetValue("isNewInventory", out var isNewInventoryToken) && isNewInventoryToken != null)
                isNewInventory = isNewInventoryToken.Value<bool>();

            if (jsonData.TryGetValue("isUsed", out var isUsedToken) && isUsedToken != null)
                isUsed = isUsedToken.Value<bool>();

            if (jsonData.TryGetValue("isPossibleUpgrade", out var isPossibleUpgradeToken) && isPossibleUpgradeToken != null)
                isPossibleUpgrade = isPossibleUpgradeToken.Value<bool>();

            if (jsonData.TryGetValue("isUniqueChange", out var isUniqueChangeToken) && isUniqueChangeToken != null)
                isUniqueChange = isUniqueChangeToken.Value<bool>();

            if (jsonData.TryGetValue("isUniqueUpgrade", out var isUniqueUpgradeToken) && isUniqueUpgradeToken != null)
                isUniqueUpgrade = isUniqueUpgradeToken.Value<bool>();

            if (jsonData.TryGetValue("characterName", out var characterNameToken) && characterNameToken != null)
                characterName = characterNameToken.Value<string>();

            if (jsonData.TryGetValue("characterType", out var characterTypeToken) && characterTypeToken != null)
                characterType = (CharacterType)characterTypeToken.Value<int>();

            if (jsonData.TryGetValue("attackType", out var attackTypeToken) && attackTypeToken != null)
                attackType = (AttackType)attackTypeToken.Value<int>();

            if (jsonData.TryGetValue("characterIndex", out var characterIndexToken) && characterIndexToken != null)
                characterIndex = (CharacterIndex)characterIndexToken.Value<int>();

            if (jsonData.TryGetValue("characterGrade", out var characterGradeToken) && characterGradeToken != null)
                characterGrade = (CharacterGrade)characterGradeToken.Value<int>();

            if (jsonData.TryGetValue("characterPowerLevel", out var characterPowerLevelToken) && characterPowerLevelToken != null)
                characterPowerLevel = characterPowerLevelToken.Value<int>();

            if (jsonData.TryGetValue("characterClassLevel", out var characterClassLevelToken) && characterClassLevelToken != null)
                characterClassLevel = characterClassLevelToken.Value<int>();

            if (jsonData.TryGetValue("characterQuantity", out var characterQuantityToken) && characterQuantityToken != null)
                characterQuantity = characterQuantityToken.Value<int>();

            if (jsonData.TryGetValue("attackId", out var attackIdToken) && attackIdToken != null)
                attackId = attackIdToken.Value<int>();

            if (jsonData.TryGetValue("targetCount", out var targetCountToken) && targetCountToken != null)
                targetCount = targetCountToken.Value<float>();

            if (jsonData.TryGetValue("attackDamage", out var attackDamageToken) && attackDamageToken != null)
                attackDamage = attackDamageToken.Value<float>();

            if (jsonData.TryGetValue("attackSpeed", out var attackSpeedToken) && attackSpeedToken != null)
                attackSpeed = attackSpeedToken.Value<float>();

            if (jsonData.TryGetValue("detectRange", out var detectRangeToken) && detectRangeToken != null)
                detectRange = detectRangeToken.Value<float>();

            if (jsonData.TryGetValue("strikeRange", out var strikeRangeToken) && strikeRangeToken != null)
                strikeRange = strikeRangeToken.Value<float>();

            if (jsonData.TryGetValue("criticalRange", out var criticalRangeToken) && criticalRangeToken != null)
                criticalRange = criticalRangeToken.Value<float>();

            if (jsonData.TryGetValue("criticalDamageRate", out var criticalDamageRateToken) && criticalDamageRateToken != null)
                criticalDamageRate = criticalDamageRateToken.Value<float>();

            if (jsonData.TryGetValue("projectileSpeed", out var projectileSpeedToken) && projectileSpeedToken != null)
                projectileSpeed = projectileSpeedToken.Value<float>();

            if (jsonData.TryGetValue("isThrow", out var isThrowToken) && isThrowToken != null)
                isThrow = isThrowToken.Value<bool>();

            if (jsonData.TryGetValue("buffValue", out var buffValueToken) && buffValueToken != null)
                buffValue = buffValueToken.Value<float>();

            if (jsonData.TryGetValue("buffRange", out var buffRangeToken) && buffRangeToken != null)
                buffRange = buffRangeToken.Value<float>();

            if (jsonData.TryGetValue("buffCycle", out var buffCycleToken) && buffCycleToken != null)
                buffCycle = buffCycleToken.Value<float>();

            if (jsonData.TryGetValue("buffDuration", out var buffDurationToken) && buffDurationToken != null)
                buffDuration = buffDurationToken.Value<float>();

            if (jsonData.TryGetValue("maxStarGradeValue", out var maxStarGradeValueToken) && maxStarGradeValueToken != null)
                maxStarGradeValue = maxStarGradeValueToken.Value<int>();

            if (jsonData.TryGetValue("characterNameTextKey", out var characterNameTextKeyToken) && characterNameTextKeyToken != null)
                characterNameTextKey = characterNameTextKeyToken.Value<string>();

            if (jsonData.TryGetValue("characterDescTextKey", out var characterDescTextKeyToken) && characterDescTextKeyToken != null)
                characterDescTextKey = characterDescTextKeyToken.Value<string>();

            if (jsonData.TryGetValue("characterAttackDescTextKey", out var characterAttackDescTextKeyToken) && characterAttackDescTextKeyToken != null)
                characterAttackDescTextKey = characterAttackDescTextKeyToken.Value<string>();

            if (jsonData.TryGetValue("isNFT", out var isNFTToken) && isNFTToken != null)
                isNFT = isNFTToken.Value<bool>();

            if (jsonData.TryGetValue("isHiddenMission", out var isHiddenMissionToken) && isHiddenMissionToken != null)
                isHiddenMission = isHiddenMissionToken.Value<bool>();

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