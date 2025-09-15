using System;
using UnityEngine;

namespace Framework.GameData.Defense
{
    public enum AnimalType
    {
        BASIC = 1,
        TUTORIAL_REWARD = 2
    }

    public enum AttackType
    {
        ATTACK,
        BUFF,
        CROWDCONTROL
    }

    public enum MenuType
    {
        HOME,
        SHOP,
        INVENTORY,
        RANKING,
        WALLET
    }

    public enum MonsterType
    {
        WAVE_MONSTER,
        CLONE_MONSTER,
        BOSS_MONSTER,
        FIELD_BOSS_MONSTER
    }

    public enum FieldBuffType
    {
        NONE,
        INCREASE_CRITICAL_RATE,
        DECREASE_DEFENSE_VALUE,
        INCREASE_MISSION_REWARD,
        INCREASE_BONUS_RATE,
        INCREASE_BONUS_SPEED,
    }

    public enum CharacterType
    {
        NORMAL,
        SUB,
        TUTORIAL
    }

    public enum DamageType
    {
        NORMAL,
        CRITICAL,
        BLACKORB,
        INSTANT_KILL,
        GAMBLING_KILL,
    }

    public enum UpgradeType
    {
        SUCCESS,
        FAILED,
        RESET
    }

    public enum CharacterIndex
    {
        PENGGO = 1,
        FORTIS = 2,
        MEOSK = 3,
        CUB_FOXY = 4,
        RAZY = 5,
        CARONA = 101,
        KIRING = 102,
        DARKRAZY = 103,
        MARRIER = 105,
        LEMMING_TOXIN = 106,
        LEMMING_RICH = 201,
        BASIC_WOLF = 202,
        WILLOW = 203,
        DAVI = 204,
        PATROL = 301,
        BEOSK = 302,
        KYLE = 303,
        PUDDLE = 304,
        BARD = 305,
        TRICKSTER = 306,
        HIFIVE = 307,
        FLORA = 308,
        RIO = 309,
        EMPEROR_PENGGO = 401,
        WHITE_MAGICIAN_FOXY = 402,
        BLACK_MAGICIAN_FOXY = 403,
        POLARIS = 404,
        DARKWOLF = 405,
        GUARDIAN = 406,
        YURI = 407,
        OWLRUS = 408,
        HAMMERING = 409,
        VEGAS = 410,
        CHAMY = 411,
        POLAFIZZ = 1001,
        SHAPID = 1101,
        TURTLIA = 1201,
        KANADE = 1301,
        HIGHNICKEL = 1401,
        KORRA = 1402,
        LULU = 1403
    }

    public enum CharacterGrade
    {
        COMMON = 0,
        UNCOMMON = 2,
        RARE = 4,
        EPIC = 6,
        LEGENDARY = 9
    }

    public enum HatchingGrade
    {
        COMMON = 0,
        UNCOMMON = 2,
        RARE = 4,
        EPIC = 6,
        LEGENDARY = 9
    }

    public enum Monster
    {
        CAN = 0,
        PETROLEUM = 1,
        PLASTIC_BOTTLE = 2,
        EXHAUST = 3,
        CAR = 4,
        MONSTER = 5
    }

    public enum FieldBossRewardType
    {
        CHARACTER = 1,
        BUFF = 2
    }

    public enum FieldBossBuffType
    {
        GET_RANDOM_CHARACTER = 0,
        GET_GEM = 1,
        DECREASE_RELOCATION_COST = 2,
        INCREASE_ATTACK_DAMAGE_ALL = 3,
        INCREASE_ATTACK_DAMAGE_LESS_RANGE = 4,
        INCREASE_ATTACK_DAMAGE_MORE_RANGE = 5,
        INCREASE_MISSION_REWARD = 6,
        DECREASE_BOSS_COOLTIME = 7,
        INCREASE_BOSS_REWARD  = 8,
        DECREASE_UPGRADE_COST = 11,
        INCREASE_MONSTER_REWARD = 12
    }

    public enum FieldBossMonster
    {
        TRUSH = 1,
        SMOKER = 2,
        SOTTY = 3,
        LOCKY = 4,
        PARASITE = 5,
        BOOMBER = 6,
        EMBEREON = 7
    }

    public enum RankingType
    {
        GOLD = 1,
        SILVER = 2,
        BRONZE = 3,
        COMMON = 4,
        MYRANK = 5
    }

    public enum MoneyType
    {
        TAIKA,
        STIK,
        ADMOB,
        GEM,
        INAPP
    }

    public enum StatusType
    {
        ATTACK_DAMAGE = 0,
        ATTACK_SPEED = 1,
        INCREASE_ATTACK_SPEED = 2,
        DETECT_RANGE = 3,
        INCREASE_DETECT_RANGE = 4,
        TARGET_COUNT = 5,
        STRIKE_RANGE = 6,
        BUFF_VALUE = 7,
        BUFF_CYCLE = 8,
        BUFF_DURATION = 9,
        BUFF_RANGE = 10,
        WAVE_DAMAGE = 11,
        SLOW_RATE = 12,
        SLOW_TIME = 13,
        ADD_CRITICAL_RATE = 14,
        GET_GEM_VALUE = 15,
        BOMB_DAMAGE = 16,
        STUN_TIME = 17,
        EXCUTION_HEALTH_RATE = 18,
        BERSERKER_ATTACK_DAMAGE = 19,
        BERSERKER_ATTACK_SPEED = 20,
        BLACKMAGIC_DAMAGE = 21,
        TRANSFER_DAMAGE_RATE = 22,
        TRANSFER_COUNT = 23,
        SLOW_BOARD_RATE = 24,
        SLOW_BOARD_TIME = 25,
        TRANSFORM_TIME = 26,
        TRANSFORM_CYCLE = 27,
        TRANSFORM_ADDITIVE_ATTACK_SPEED = 28,
        GAIN_GEM_RATE = 29,
        KORRA_WAVE = 30,
        POLARIS_STRIKE_RANGE = 31,
        POLARIS_SEC = 32,
        DAVI_ATK = 33,
        DAVI_ATS = 34,
        DAVI_SEC = 35,
        KYLE_ATK = 36,
        BARD_SPEED_BUFF = 37,
        DARKWOLF_INSTANTKILL_RATE = 38,
        DARKWOLF_GET_GEM_RATE = 39,
        MARRIER_ADDITIVE_DAMAGE = 40,
        MARRIER_LIMIT_GEM = 41,
        PUDDLE_GET_GEM_TIMECOUNT = 42,
        PUDDLE_GET_GEM_RATE = 43,
        PUDDLE_GET_GEM_AMOUNT = 44,
        CRITICAL_DAMAGE = 45,
        CARONAPENGO_ADDTIVE_DAMAGE = 46,
        WHITEMAGIC_FOXY_DAMAGE = 47,
        GUARDIAN_BARRIER_CYCLE = 48,
        GUARDIAN_BARRIER_DURATION = 49,
        TRICKSTER_GLACIER_CHANGE = 50,
        LEMMINGTOXIN_TOXIC_RATETIME = 51,
        LULU_BUBBLE = 52,
        YURI_PER_DAMAGE = 53,
        YURI_DURATION = 54,
        YURI_COOLDOWN = 55,
        HIFIVE_EXTRA_DAMAGE = 56,
        HIFIVE_RANGE_UP = 57,
        OWLRUS_DAMAGE = 58,
        FLORA_DAMAGE = 59,
        GUARDIAN_BARRIER_CYCLE_MAX = 60,
        VEGAS_MIN_DAMAGE = 61,
        VEGAS_MAX_DAMAGE = 62,
        VEGAS_SPECIAL_EFFECT = 63,
        HAMMERING_ATTACK_RATE = 64,
        HAMMERING_ATTACK_DAMAGE = 65,
        HAMMERING_UPGRADE_DISCOUNT = 66,
        HAMMERING_UPGRADE_RATE = 67,
        HAMMERING_PER_DAMAGE = 68,
        TRICKSTER_GLACIER_CHANGE_RATE = 69,
        RIO_ATTACK_READY_COOLTIME = 70,
        CHAMY_MERGE_SUCCESS_RATE = 71,
    }

    public enum CharacterState
    {
        APPEAR,
        DETECT,
        ATTACK,
        DURING_ATTACK,
        CHANGE
    }

    public enum WorldType
    {
        GLACIER = 0
    }

    public enum DisplayTokenType
    {
        TAIKA = 0,
        STAIKA = 1
    }

    public enum GlacierState
    {
        NORMAL = 0,
        BROKEN = 1,
        DESTROYED = 2
    }

    public enum HorizontalInfo
    {
        LEFT,
        MIDDLE,
        RIGHT
    }

    public enum VerticalInfo
    {
        UPPER,
        MIDDLE,
        LOWER
    }

    public enum CellType
    {
        NONE,
        CONSTRUCTIBLE,
        MOVABLE,
        BROKEN
    }

    public enum NoticeType
    {
        GENERAL,
        SALE,
        NEW
    }

    public enum StoreType
    {
        NORMAL,
        SPECIAL,
    }

    public enum StorePackageType
    {
        ENERGY,
        RANDOM_BOX,
        GEM,
        ITEM,
    }

    public enum SpecialStorePackageType
    {
        ITEM = 0,
        SKIN = 1,
    }


    public enum ReinforcementType
    {
        STAR = 0,
        UPGRADE = 1,
        LEVELUP = 2
    }

    public enum GlacierSummonDirectionType
    {
        NONE = 0,
        NEAR_LEFT = 1,
        NEAR_RIGHT = 2,
        NEAR_UP = 3,
        NEAR_DOWN = 4,
        FAR_LEFT = 5,
        FAR_RIGHT = 6,
        FAR_UP = 7,
        FAR_DOWN = 8,
    }

    public enum ItemType
    {
        CHARACTER = 0,
        ITEM = 1,
    }

    public enum BattlePassDisplayType
    {
        GENERAL = 0,
        HIGHLIGHT = 1
    }
    public enum BattlePassRewardType
    {
        RANDOM_BOX = 0,
        HATCHING_ORB = 1,
        GEM = 2,
        ENERGY = 3,
        TIK = 4,
        STIK = 5,
        GO = 6,
        STIK_RANDOMBOX = 7,
        PTIK_RANDOMBOX = 8,
        TIK_RANDOMBOX = 9,
        PTIK = 10,
    }

    public enum BattlePassLevelType
    {
        CLEAR_LEVEL = 0,
        NEXT_LEVEL = 1,
        OTHER_LEVEL = 2,
    }

    public enum MonthlyPackageType
    {
        GEM_PACKAGE = 0,
    }


    public enum BattlePassCrystalType
    {
        LOW_CRYSTAL = 5,
        MIDDLE_CRYSTAL = 30,
        HIGH_CRYSTAL = 70,
    }

    public enum QuestType
    {
        DAILY = 0,
        ACHIEVEMENT = 1,
        NONE = 2
    }

    public enum QuestRewardStateType
    {
        NONE = 0,
        AVAILABLE = 1,
        REWARDED = 2
    }

    public enum IapBuyType
    {
        NONE = 0,
        GEM = 1,
        BATTLEPASS_PREMIUM = 2,
        BATTLEPASS_EXP = 3,
        MONTHLY_GEM = 4,
        SKIN = 5,
    }

    public enum RandomRewardType
    {
        ENERGY = 0,
        GEM = 1,
        HATCHING_ORB = 2,
        DAILYEXP = 4,
    }

    public enum TutorialProgressType
    {
        GAME_START = 0,
        GAME_PROGRESS = 1,
        GAME_END = 2,
    }

    public enum PityRewardType
    {
        RANDOM_BOX = 0,
        HATCHING_ORB = 1,
        GEM = 2,
        ENERGY = 3,
        CHARACTER = 4
    }

    [Serializable]
    public class CellData
    {
        public CellType cellType;
        public int cellIndex;
        public Vector2 cellPosition;
    }

    [Serializable]
    public class DecButtonColorInfo
    {
        public string backGroundColor;
        public string innerDecoColor;
    }

    [Serializable]
    public class TileData
    {
        public int index;
        public int landType;
        public int startingPoint;
        public string assetName;
        public int width;
        public int height;
        public GlacierSummonDirectionType directionType;
    }

    [Serializable]
    public class StageData
    {
        public TileData[] tileDatas;
    }

    [Serializable]
    public class InfiniteData
    {
        public WaveData[] waveDatas;
    }

    [Serializable]
    public class WaveData
    {
        public int index;
        public int worldType;
        public int wave;
        public int monsterId;
        public int monHealth;
        public int monSpeed;
        public int monDepense;
        public int rewardId;
        public int appearMonCount;
    }

    [Serializable]
    public class RewardsTableData
    {
        public Reward[] rewards;
    }

    [Serializable]
    public class BossRewardData
    {
        public Reward[] bossRewards;
    }

    [Serializable]
    public class Reward
    {
        public int index;
        public int rewardValue;
    }

    [Serializable]
    public static class ConfigData
    {
        public static readonly int START_GEM = 40;
        public static readonly float CRITICAL_DAMAGE = 1.5f;
        public static readonly float INFINITE_HEALTH_FACTOR = 60000f;
        public static readonly float INFINITE_HEALTH_FACTOR_DIFFERENTIAL = 1.08f;
        public static readonly int GEM_INTEREST_TIME = 40;
        public static readonly float GEM_INTEREST_RATE = 0.02f;
        public static readonly int GEM_INTEREST_LIMIT = 500;
        public static readonly int WAVE_READY_TIMER = 2;
        public static readonly float WAVE_MONSTER_LOSS = 1f;
        public static readonly float WAVE_BOSS_LOSS = 0.5f;
        public static readonly int GEM_RELOCATION = 5;
        public static readonly int BOSS_COOLTIME = 105;
        public static readonly float CREATE_MONSTER_TIME = 0.4f;
        public static readonly int GEM_SUMMON_FIRST = 10;
        public static readonly int GEM_SUMMON_FACTOR = 0;
        public static readonly int GEM_UPGRADE_FIRST = 10;
        public static readonly int GEM_UPGRADE_FACTOR = 1;
        // ( GEM_UPGRADE_FACTOR / 2 ) * 업그레이드 레벨 ^ 2 - ( GEM_UPGRADE_FACTOR / 2 ) * 업그레이드 레벨 + GEM_SUMMON_FIRST
        public static readonly int GEM_UPGRADE_LIMIT = 50;
        public static readonly float STARGRADE_FACTOR = 1.8f;
        public static readonly int NICKNAME_MAXIMUM_LIMIT = 14;
        public static readonly int NICKNAME_MINIMUM_LIMIT = 3;
        public static readonly int ENERGY_TOTAL = 30;
        public static readonly int ENERGY_COOLTIME = 300;
        public static readonly int ENERGY_ACQUISITION_VALUE = 5;
        public static readonly float START_CHARACTER_SLOT_COUNT = 2;
        public static readonly float ATTACKSPEED_LIMIT = 3.5f;
        public static readonly float MONSTER_SPEED_LIMIT = 1f;
        public static readonly float CREATE_CLONEMONSTER_TIME = 1.3f;
        public static readonly float CHARACTER_SELL_RATE = 0.4f;
        public static readonly float MONTHLY_PACKAGEREWARD_RATE = 20f;
        public static readonly float ATTACK_HIT_TIK_RATE = 0.5f;
    }

    [Serializable]
    public class UserInfo
    {

    }

    [Serializable]
    public class Package
    {
        public int packageIndex;
        public int price;
        public int paymentType;
    }

    [Serializable]
    public class ShopPackage
    {
        public Package[] shopPackageList;
    }

    [Serializable]
    public class RankingDataList
    {
        public RankingData[] rankingDataList;
    }

    [Serializable]
    public class DisplayType
    {
        public DisplayInfo[] displayType;
    }

    [Serializable]
    public class DisplayInfo
    {
        public int displayTypeIndex;
        public string[] displayInfo;
    }

    [Serializable]
    public class RankingData
    {
        public string id;
        public int userId;
        public float rewardGo;
        public float rewardTik;
        public string aggregatedDate;
        public string nickname;
        public string profileImageUrl;
    }

    [Serializable]
    public class BossMonsterData
    {
        public BossMonster[] bossMonsters;
    }

    [Serializable]
    public class BossMonster
    {
        public string bossName;
        public int bossId;
        public WorldType worldType;
        public int bossLevel;
        public int monsterId;
        public int cloneMonsterId;
        public int cloneMonsterHealth;
        public int bossHealth;
        public int bossSpeed;
        public int rewardId;
    }

    [Serializable]
    public class CharacterCardInfo
    {
        public Sprite sprite_Frame;
        public Color color_BackGround;
        public Color color_Shadow;
        public Color color_decGradeText;
        public Color disableShadowColor;
        public Gradient particleColorLifeTime_NFT_Gradient_color;
        public Color light_NFT_color;
        public Color color_LevelText;
    }

    [Serializable]
    public class HatchingStoneCardInfo
    {
        //바깥을 둘러싸는 막 이미지
        public Sprite sprite_Frame;
        //뒷 배경 색상 값
        public Color color_BackGround;
        //그림자 색상 값
        public Color color_Shadow;
        //등급별 색상 값
        public Color color_GradeText;
    }

    [Serializable]
    public class ShopPackgeInfo
    {
        public Sprite sprite_Icon;
        public Color color_BackGround;
        public Color color_Button;
        public Color color_Line;
    }

    [Serializable]
    public class FieldBossRewardGroupData
    {
        public FieldBossRewardData[] data;
    }

    [Serializable]
    public class FieldBossRewardData
    {
        public int id;
        public int reward_grade_group;
        public float emerge_rate;
        public int reward_type;
        public int buff_type;
        public float value_1;
        public float value_2;
        public float value_3;
        public string title;
        public string desc;
    }

    [Serializable]
    public class PassLevelColorInfo
    {
        public Color color_Shadow;
        public Color color_EndOutBottomFrame;
        public Color color_EndOutUpFrame;
        public Color color_MiddleOutBottom;
        public Color color_FirstFrame;
        public Color color_MainFrame;
        public Color color_Text;
        public Material mat_Text;
    }

    public class AssetList
    {
        public WorldType worldType;
        public string[] assetName;
    }

    public enum TutorialType
    {
        TEXT,
        TOUCH,
        RANGE
    }

    public enum TutorialBattleType
    {
        SUMMON,
        COMBINE
    }

    public class TutorialData
    {
        public int id;
        public TutorialType tutorialType;
        public bool isTrigger;
        public string descTextKey;
        public bool isClear;
    }

    public class TutorialBattleData
    {
        public int id;
        public int glacierIdx;
        public TutorialBattleType tutorialBattleType;
        public int characterIndex;
    }

    //newDongmin
    public struct IncreaseStruct
    {
        public IncreaseStruct(int star, int upgrade, int classLevel, int increaseClassLevel, bool isStar, bool isUpgrade, bool isLevelUp)
        {
            this.star = star;
            this.upgrade = upgrade;
            this.classLevel = classLevel;
            this.increaseClassLevel = increaseClassLevel;
            this.isStarUsed = isStar;
            this.isUpgradeUsed = isUpgrade;
            this.isLevelupUsed = isLevelUp;
        }

        public int star;
        public int upgrade;
        public int classLevel;
        public int increaseClassLevel;
        public bool isStarUsed;
        public bool isUpgradeUsed;
        public bool isLevelupUsed;
    }

    public enum SkinGradeType
    {
        NONE,
        COMMON,
        LEGENDARY,
        MYTHIC
    }

    public class SkinTableData
    {
        public int id;
        public int character_id;
        public SkinGradeType skin_grade_type;
        public string projectile;
        public string profile;
        public string sound;
        public MoneyType price_type;
        public int price;
    }

    [Serializable]
    public enum ProfileType
    {
        Character,
        Skin,
        ETC,
    }

    public class StoreTableData
    {
        public int id;
        public string code;
        public string inapp_tier;
        public IapBuyType inapp_type;
        public UnityEngine.Purchasing.ProductType product_type;
        public bool is_activated;
        public int resion_type;
        public int reward_group_id;
        public int store_group_id;
        public int adjust_price;
    }

    public class RewardTemplate
    {
        public string reward_type;
        public string reward_type_refer;
        public int reward_amount;

        private RewardData reward_data = null;

        public void CreateReward(RewardType type, string refer, int amount)
        {
            reward_type = type.ToString();
            reward_type_refer = refer;
            reward_amount = amount;
        }

        public RewardData RewardData
        {
            get
            {
                if (reward_data == null)
                    reward_data = new RewardData(reward_type, reward_type_refer, reward_amount);

                return reward_data;
            }
        }
    }

    public class RewardTableData : RewardTemplate
    {
        public int id;
        public int reward_group;
        public string asset_thumbnail;
        public bool is_display_quantity;
    }

    public class RewardData
    {
        private string reward_type;
        private string reward_type_refer;
        private int reward_amount;

        public RewardData(string type, string refer, int amount)
        {
            reward_type = type;
            reward_type_refer = refer;
            reward_amount = amount;
        }

        public RewardType Type
        {
            get
            {
                switch (reward_type)
                {
                    case "CURRENCY":
                        {
                            switch (reward_type_refer)
                            {
                                case "GEM":
                                    return RewardType.GEM;

                                case "SKIK":
                                    return RewardType.SKIK;

                                case "TIK":
                                    return RewardType.TIK;

                                default:
                                    return RewardType.NONE;
                            }
                        }

                    case "BOX":
                        return RewardType.BOX;

                    case "ITEM":
                        {
                            switch (reward_type_refer)
                            {
                                case "ENERGY":
                                    return RewardType.ENERGY;

                                default:
                                    return RewardType.ITEM;
                            }
                        }

                    case "CHARACTER":
                        return RewardType.CHARACTER;

                    case "CHARACTER_SKIN":
                        return RewardType.CHARACTER_SKIN;

                    case "PROFILE":
                        return RewardType.PROFILE;

                    default:
                        return RewardType.NONE;

                }
            }
        }

        public int IntValue
        {
            get
            {
                switch (Type)
                {
                    case RewardType.CHARACTER:
                    case RewardType.CHARACTER_SKIN:
                    case RewardType.PROFILE:
                        return Convert.ToInt32(reward_type_refer);

                    default:
                        return 0;
                }
            }
        }

        public string StringValue { get { return reward_type_refer; } }
        public int Amount { get { return reward_amount; } }
    }

    public enum RewardType
    {
        NONE,
        GEM,
        TIK,
        SKIK,
        BOX,
        ITEM,
        ENERGY,
        CHARACTER,
        CHARACTER_SKIN,
        PROFILE,
    }

    public enum PityRewardStateType
    {
        NONE = 0,
        AVAILABLE = 1,
        REWARDED = 2
    }

    public class LimitedShopCeilingTableData
    {
        public int id;
        public int request_step;
        public int request_value;
        public int reward_group_id;
        public int store_group_id;
    }

    public class RewardNetData
    {
        public RewardTemplate[] reward;
    }

    public enum RoulettePaidType
    {
        FREE,
        TICKET,
        AD,
        STIK
    }

    public enum UserItemType
    {
        ROULETTE_TICKET
    }
}
