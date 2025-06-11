using System.Collections.Generic;
using Framework.UI;
using JetBrains.Annotations;
using Spine.Unity;
using UnityEngine;

[System.Serializable]
public class CharacterResource
{
    public int character_id;
    public SkeletonDataAsset skeletonDataAsset;
    public Sprite ChracterPortrait;
    public Sprite SkinPortrait;
}

[CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "ScriptableObject/Character")]
public class CharacterScriptableObject : ScriptableObject
{
    public SerializableDictionary<int, CharacterResource> dic_Character;
}
