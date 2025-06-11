using UnityEngine;
using Spine;
using Spine.Unity;
using Framework.GameData.Defense;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;
using System.Linq;

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
}