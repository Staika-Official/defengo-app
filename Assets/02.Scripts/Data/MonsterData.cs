using UnityEngine;
using Spine;
using Spine.Unity;

[CreateAssetMenu(fileName = "MonsterData", menuName = "MonsterData")]
public class MonsterData : ScriptableObject
{
    public int index;
    public string monsterName;
    public SkeletonDataAsset anim;
}
