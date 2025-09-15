using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using DG.Tweening;


namespace Framework.UI
{
    public class BossSelectItem : MonoBehaviour
    {
        public SkeletonGraphic anim_boss;
        public TextMeshProUGUI text_bossName;
        public int bossIdx;

        public void Initialize(BossData bossData)
        {
            bossIdx = (int)bossData.bossIndex;
            text_bossName.text = bossData.bossNameKey;
            anim_boss.SkeletonDataAsset.Clear();
            anim_boss.AnimationState.ClearTracks();
            anim_boss.skeletonDataAsset = bossData.anim;
            anim_boss.Initialize(true);
        }

        public void SetMovement()
        {

        }
    }
}

