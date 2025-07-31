using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(menuName = "LimitedShopUIItem")]
public class LimitedShopUIItem : ScriptableObject
{
    public Sprite backgroundImage;
    public SkeletonDataAsset characterSkeletonData;
    public string descLocalizeKey;
}