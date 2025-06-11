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
    public class CharacterShapid : Character
    {
        public int additiveDamage;

        void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType)
                .Subscribe(_ => GetPossibleAttackMonster());
        }

        public override IEnumerator ActionSequence()
        {
            FlipCharacter(targetedMonster[0].transform.localPosition.x);

            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);

            List<Monster> targetCache = targetedMonster;

            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

            bool isCritical;
            if ((int)criticalRange == 0)
            {
                isCritical = false;
            }
            else
            {
                criticalRange += GameManager.Instance.buffManager.increaseCriticalRate;
                int tempValue = (int)(criticalRange * 100);
                int criticalValue = Random.Range(0, 100);
                isCritical = criticalValue <= tempValue;
            }

            for (int i = 0; i < targetCache.Count; i++)
            {
                if (isThrow)
                {
                    WaterProjectile projectile = GameManager.Instance.objectPoolManager.GetObject<WaterProjectile>(projectileName);
                    projectile.transform.localPosition = transform.localPosition;
                    projectile.attackValue = totalDamage + (GameManager.Instance.monsterSpawner.waveIdx * additiveDamage);
                    projectile.criticalDamageRate = criticalDamageRate;
                    projectile.isMove = true;
                    projectile.provokedIndex = glacierIdx;

                    projectile.Initialize(this, targetCache[i], isCritical, projectileSpeed, strikeRange, particleName, SoundKey.SF_CHARACTER_SHELLFISH);
                }
            }

            yield return new WaitForSpineAnimationComplete(entry);
            anim.AnimationState.SetAnimation(0, "Idle", true);
        }

        public override void SetCharacterInfo()
        {
            if (targetedMonster.Count != 0)
            {
                targetedMonster.Clear();
            }

            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
            }

            upgradeIndex = 1;
            CharacterAction = null;

            isAttackType = true;
            targetCount = (int)characterData.targetCount;
            additiveDamage = (int)characterData.characterUniqueValue[0];
            basicAttackDamage = characterData.attackDamage;
            attackSpeed = characterData.attackSpeed;
            criticalRange = 1 / characterData.criticalRange;
            criticalDamageRate = characterData.criticalDamageRate;
            upgradeFactor = characterData.powerFactor[0];
            projectileSpeed = characterData.projectileSpeed;
            attackRange = Mathf.Pow(characterData.detectRange, 2);
            strikeRange = Mathf.Pow(characterData.strikeRange, 2);
            isThrow = characterData.isThrow;
            buffDuration = characterData.buffDuration;
            buffValue = characterData.buffValue;
            SetAttackDamage();
            projectileName = characterData.projectileName;
            CharacterAction = () =>
            {
                if (isSummoned && targetedMonster.Count > 0)
                {
                    StartCoroutine(ActionSequence());
                }
            };
            particleName = "WaterHit";

            maxStarGradeValue = characterData.maxStarGradeValue;

            isSummoned = true;
            actionCoroutine = WaitActionSequence();
            StartCoroutine(actionCoroutine);

            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

            bool isAppear = starGradeIndex == 0;

            AppearAction("Idle", isAppear);
        }

        public override void SynthesisCharacter()
        {
            
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            
        }
    }
}