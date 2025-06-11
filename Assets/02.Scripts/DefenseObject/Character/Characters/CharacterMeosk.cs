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
    public class CharacterMeosk : Character
    {
        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType)
                .Subscribe(_ => GetPossibleAttackMonster());
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

            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);
            CharacterAction = null;

            isAttackType = true;
            targetCount = (int)characterData.targetCount;
            basicAttackDamage = characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0];
            //attackSpeed = Mathf.Pow(characterData.attackSpeed + characterData.starFactor[0] * starGradeIndex, ConfigData.STARGRADE_FACTOR);
            attackSpeed = characterData.attackSpeed;
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

        public override IEnumerator ActionSequence() //Deprecated
        {
            FlipCharacter(targetedMonster[0].transform.localPosition.x);

            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);

            List<Monster> targetCache = targetedMonster;
            float duration = entry.Animation.Duration;
            float tempSpeed = speedBuffValue == 0 ? attackSpeed : attackSpeed * speedBuffValue;
            entry.TimeScale = Calculator.GetAnimationSpeed(tempSpeed, duration);
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

            DamageType damageType = isCritical ? DamageType.CRITICAL : DamageType.NORMAL;

            for (int i = 0; i < targetCache.Count; i++)
            {
                if (!targetCache[i].IsAlive) continue;
                SoundManager.Instance.PlaySound(SoundKey.SF_CHARACTER_MEOSK);
                targetCache[i].HitDamage(totalDamage, damageType, criticalDamageRate);
            }

            yield return new WaitForSpineAnimationComplete(entry);
            anim.AnimationState.SetAnimation(0, "Idle", true);
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;
            SetAttackDamage();
        }

        public override void SynthesisCharacter()
        {
            
        }
    }
}
