using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.GameData.Defense;
using Framework.Util;
using Framework.Sound;
using UniRx.Triggers;
using UniRx;
using Spine.Unity;
using Spine;

namespace Framework.Game.Defense
{
    public class CharacterOwlrus : Character
    {
        public float addtiveDamage;
        void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType)
                .Subscribe(_ => GetPossibleAttackNearOrFarMonster(true));
        }

     
        public override IEnumerator ActionSequence()
        {
            FlipCharacter(targetedMonster[0].transform.localPosition.x);

            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);
            float duration = entry.Animation.Duration;
            float tempSpeed = speedBuffValue == 0 ? attackSpeed : attackSpeed * speedBuffValue;
            entry.TimeScale = Calculator.GetAnimationSpeed(tempSpeed, duration);
            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");
            List<Monster> targetCache = targetedMonster;

            SoundManager.Instance.PlaySound(SoundKey.SF_OWLRUS_ATTACK);
            
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
                OwlProjectile projectile = GameManager.Instance.objectPoolManager.GetObject<OwlProjectile>(projectileName);
                projectile.transform.localPosition = pivotPosition;
                projectile.attackValue = totalDamage;
                projectile.criticalDamageRate = criticalDamageRate;
                projectile.isMove = true;
                projectile.provokedIndex = glacierIdx;
                projectile.addtiveDamage = addtiveDamage;
                projectile.Initialize(this, targetCache[i], isCritical, projectileSpeed, strikeRange);
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
            }

            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);
            CharacterAction = null;

            isAttackType = true;
            targetCount = (int)characterData.targetCount;
            basicAttackDamage = characterData.attackDamage +
                                (characterData.characterClassLevel - 1) * characterData.classUpFactor[0];
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
            
            addtiveDamage = ((characterData.characterUniqueValue[0]
                              +(characterData.characterClassLevel - 1) 
                              * characterData.classUpFactor[1])
                                + (upgradeIndex - 1) * characterData.powerFactor[1]) 
                              * (starGradeIndex + characterData.starFactor[0]);

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
            this.upgradeIndex = upgradeIndex;
            
            SetAttackDamage();
            
            addtiveDamage = ((characterData.characterUniqueValue[0]
                              +(characterData.characterClassLevel - 1) 
                              * characterData.classUpFactor[1])
                             + (upgradeIndex - 1) * characterData.powerFactor[1]) 
                            * (starGradeIndex + characterData.starFactor[0]);
        }
    }
}
