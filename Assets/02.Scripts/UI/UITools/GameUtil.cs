using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework.GameData.Defense;
using Spine.Unity;
using UnityEngine;

public static class GameUtil
{
    public static T ToEnum<T>(this string s)
    {
        if (string.IsNullOrEmpty(s))
            return default(T);
        return (T)System.Enum.Parse(typeof(T), s);
    }

    public static RectTransform RectTransform(this GameObject obj)
    {
        return obj.transform as RectTransform;
    }

    public static void Init(this SkeletonGraphic skeleton, SkeletonDataAsset asset, CharacterType type)
    {
        skeleton.skeletonDataAsset = asset;
        skeleton.gameObject.SetActive(true);
        skeleton.SkeletonDataAsset.Clear();
        skeleton.AnimationState.ClearTracks();

        List<Spine.Skin> skin_list = skeleton.skeletonDataAsset.GetAnimationStateData().SkeletonData.Skins.ToList();
        skeleton.initialSkinName = skin_list.Count > 1 ? skin_list[1].Name : skin_list[0].Name;

        skeleton.Initialize(true);

        bool isOcean = type == CharacterType.SUB;
        string animKey = isOcean ? "Idle_Inventory" : "Idle";
        skeleton.AnimationState.SetAnimation(0, animKey, true);
    }

    public static System.TimeSpan RemainTime(string enddate)
    {
        //현재 기간
        //DateTime fromDate = DateTime.Parse(specialStoreDTO.specialStore[i].fromDate);
        System.DateTime fromDate = System.DateTime.UtcNow;
        //마감 기간
        System.DateTime toDate = System.DateTime.Parse(enddate);
        //남은 기간
        return toDate - fromDate;
    }

    public static T Find<T>(this T[] array, System.Predicate<T> match)
    {
        if (array == null)
            return default;

        return System.Array.Find(array, match);
    }

    public static T[] FindAll<T>(this T[] array, System.Predicate<T> match)
    {
        if (array == null)
            return null;

        return System.Array.FindAll(array, match);
    }
}
