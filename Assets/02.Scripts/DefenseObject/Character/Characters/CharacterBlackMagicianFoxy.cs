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
    public class CharacterBlackMagicianFoxy : Character
    {
        public float stackExplosionDamage;

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType)
                .Subscribe(_ => GetPossibleAttackMonster());
        }

        public override IEnumerator ActionSequence()
        {
            //anim.timeScale = attackSpeed / 1.2f;

            FlipCharacter(targetedMonster[0].transform.localPosition.x);

            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);
            float duration = entry.Animation.Duration;
            float tempSpeed = speedBuffValue == 0 ? attackSpeed : attackSpeed * speedBuffValue;
            entry.TimeScale = Calculator.GetAnimationSpeed(tempSpeed, duration);
            List<Monster> targetCache = targetedMonster;

            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

            bool isCritical;
            if (criticalRange <= 0)
            {
                isCritical = false;
            }
            else
            {
                int tempValue = (int)(criticalRange * 100);
                int criticalValue = Random.Range(0, 100);
                isCritical = criticalValue <= tempValue;
            }

            for (int i = 0; i < targetCache.Count; i++)
            {
                BlackOrbProjectile projectile = GameManager.Instance.objectPoolManager.GetObject<BlackOrbProjectile>(projectileName);
                projectile.transform.localPosition = transform.localPosition;
                projectile.attackValue = totalDamage;
                projectile.criticalDamageRate = criticalDamageRate;
                projectile.isMove = true;
                projectile.provokedIndex = glacierIdx;

                projectile.Initialize(this, targetCache[i], isCritical, projectileSpeed, strikeRange, particleName, stackExplosionDamage);
            }

            yield return new WaitForSpineAnimationComplete(entry);
            entry.TimeScale = 1;
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
                actionCoroutine = null;
            }

            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);
            CharacterAction = null;

            isAttackType = true;
            targetCount = (int)characterData.targetCount;
            stackExplosionDamage = characterData.characterUniqueValue[0] + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1] + (upgradeIndex - 1) * characterData.powerFactor[1] * starGradeIndex + (upgradeIndex - 1);
            basicAttackDamage = characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0];
            attackSpeed = characterData.attackSpeed + (starGradeIndex * characterData.starFactor[0]);
            criticalRange = characterData.criticalRange + GameManager.Instance.buffManager.increaseCriticalRate;
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
            particleName = "BlackHit1";

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

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;
            stackExplosionDamage = characterData.characterUniqueValue[0] + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1] + (upgradeIndex - 1) * characterData.powerFactor[1] * starGradeIndex + (upgradeIndex - 1);
            SetAttackDamage();
        }

        public override void SynthesisCharacter()
        {
            stackExplosionDamage = characterData.characterUniqueValue[0] + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1] + (upgradeIndex - 1) * characterData.powerFactor[1] * starGradeIndex + (upgradeIndex - 1);
            attackSpeed = characterData.attackSpeed + (starGradeIndex * characterData.starFactor[1]);
        }
    }
}