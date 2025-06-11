using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.GameData.Defense;
using DG.Tweening;
using System;
using UnityEngine.UI.Extensions;

namespace Framework.UI
{
    [Serializable]
    public class RankingBoxProperty
    {
        public Sprite sprite_Icon;
        public Color[] color_Layer;
        public Color[] gradientColorKeys;
    }


    [Serializable]
    public class StatusSymbol
    {
        public Sprite sprite_Icon;
    }

    [Serializable]
    public class RandomBoxSprite
    {
        public Sprite sprite_Icon;
        public Sprite sprite_Bottom;
        public Sprite sprite_Top_Open;
        public Sprite sprite_Top;
    }

    [Serializable]
    public class HatchingStoneSprite
    {
        public Sprite sprite_Icon;
        public Sprite sprite_Icon_Light;
        public Sprite sprite_Pieces3;
        public Sprite sprite_Pieces2;
        public Sprite sprite_Pieces1;
        public Texture texture_Attactor;
    }

    [Serializable]
    public class ShopItemInnerFrameColor
    {
        public Color color_InnerFrame;
        public Color color_HighLight;
        public Color color_Base;
        public Color color_Quantity;
        public UnityEngine.Gradient color_Grdient;
        public float grdientOffset;
        public float grdientZoom;
    }

    [Serializable]
    public class RandomBoxTxtColor
    {
        public Color[] colorName;
    }


    [Serializable]
    public class QuestItemColor
    {
        public Color color_BackGround;
        public Color color_MainFrame;
        public Color color_Line;
        public Color color_Reward;
    }

    [Serializable]
    public class SkinColor
    {
        public Color color_Background;
        public Color color_Frame;
        public Color color_Inner;
        public bool is_Active_Grdient;
        public UnityEngine.Gradient color_Inner_Grdient;
        public Color color_Profile;
    }

    [CreateAssetMenu(menuName = "CharacterCardInfo")]
    public class UIPropertyData : ScriptableObject
    {
        public SerializableDictionary<RankingType, RankingBoxProperty> dic_RankingBoxProperties;
        public SerializableDictionary<CharacterGrade, CharacterCardInfo> dic_CharacterCardInfo;
        public SerializableDictionary<string, HatchingStoneCardInfo> dic_HatchingCardInfo;
        public SerializableDictionary<String, Color> TxtColor;
        public SerializableDictionary<MoneyType, ShopPackgeInfo> dic_ShopPackageInfo;
        public SerializableDictionary<string, ShopItemInnerFrameColor> dic_ShopInnerFrameColor;
        public SerializableDictionary<string, Color> dic_WalletTextColor;
        public SerializableDictionary<StatusType, StatusSymbol> dic_StatusInfo;
        public SerializableDictionary<string, RandomBoxSprite> dic_RandomBoxSprite;
        public SerializableDictionary<string, HatchingStoneSprite> dic_HatchingStoneSprite;
        public Sprite sprite_Energy;
        public SerializableDictionary<string, Sprite> dic_EnergySprite;
        public SerializableDictionary<string, Sprite> dic_WalletHistoryIcons;
        public SerializableDictionary<string, Sprite> dic_InboxItemIcon;
        public SerializableDictionary<HistoryType, Sprite> dic_CoinSymbol;
        public SerializableDictionary<string, RandomBoxTxtColor> dic_RandomBoxItemTextColor;
        public SerializableDictionary<CharacterGrade, RandomBoxTxtColor> dic_RandomBoxEffetColor;
        public SerializableDictionary<string, Sprite> dic_gemIcon;
        public SerializableDictionary<string, Sprite> dic_weeklyRewardSymbol;
        public SerializableDictionary<string, Color> dic_walletPropertyColor;
        public SerializableDictionary<string, Sprite> dic_BattlePassItemIcon;
        public SerializableDictionary<BattlePassLevelType, PassLevelColorInfo> dic_BattlePassLevelColor;
        public SerializableDictionary<BattlePassCrystalType, Sprite> dic_BattlePassCrystal;
        public SerializableDictionary<MonthlyPackageType, Sprite> dic_MonthlyPackageSprite;
        public SerializableDictionary<QuestRewardStateType, QuestItemColor> dic_QuestItemColor;
        public SerializableDictionary<string, Sprite> dic_RewardItemIcon;
        public SerializableDictionary<string, Sprite> dic_RandomRewardIcon;
        public SerializableDictionary<SkinGradeType, SkinColor> dic_CharacterSkinColor;
        public SerializableDictionary<string, Sprite> dic_userItemIcon;
    }
}
