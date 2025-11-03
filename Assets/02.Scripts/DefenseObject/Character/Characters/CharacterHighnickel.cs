using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using UnityEngine;
using Spine;
using Spine.Unity;
using UniRx;
using UniRx.Triggers;
using Framework.Util;
using DG.Tweening;
using UnityEngine.Events;
using Framework.Sound;

namespace Framework.Game.Defense
{
    public class CharacterHighnickel : Character
    {
        public float stunDration;

        public override IEnumerator ActionSequence()
        {
            if (!IsLockdown)
            {
                TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);

                yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

                List<Monster> targetCache = GameManager.Instance.monsterSpawner.monsters;

                for (int i = 0; i < targetCache.Count; i++)
                {
                    if (!targetCache[i].IsAlive) continue;

                    HighnickelProjectile projectile = GameManager.Instance.objectPoolManager.GetObject<HighnickelProjectile>(characterData.projectileName);
                    projectile.transform.localPosition = transform.localPosition;
                    projectile.attackValue = totalDamage;
                    projectile.criticalDamageRate = criticalDamageRate;
                    projectile.isMove = true;
                    projectile.provokedIndex = glacierIdx;

                    projectile.Initialize(this, targetCache[i], false, projectileSpeed, "ElectroHit", stunDration);
                }

                yield return new WaitForSpineAnimationComplete(entry);
            }

            anim.AnimationState.SetAnimation(0, "Idle", true);
        }

        public override void SetCharacterInfo()
        {
            basicAttackDamage = characterData.attackDamage;
            projectileSpeed = 20;
            upgradeIndex = 1;
            SetAttackDamage();
            isSummoned = true;

            stunDration = characterData.characterUniqueValue[0];

            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

            StartCoroutine(ActionSequence());
        }

        public override void SynthesisCharacter()
        {

        }

        public override void UpgradeLevel(int upgradeIndex)
        {

        }
    }
}