using UnityEngine;
using Spine;
using Spine.Unity;
using Framework.GameData.Defense;
using CodeStage.AntiCheat.ObscuredTypes;

[CreateAssetMenu(menuName = "BossData")]
public class BossData : ScriptableObject
{
    public FieldBossMonster bossIndex;
    public string bossNameKey;
    public SkeletonDataAsset anim;
    public ObscuredFloat[] uniqueValue;
    public ObscuredFloat health;
    public ObscuredFloat healthFactor;
    public ObscuredFloat monsterSpeed;
}
