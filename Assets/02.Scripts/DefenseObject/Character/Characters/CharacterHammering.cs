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
    public class CharacterHammering : Character
    {
        public float hammerAttackRate;
        public float hammerAttackDamage;
        public float hammerAttackRange;

        public float hammerAttackAdditiveDamage;

        public int maxUpgradeValue;
        public bool isUpgradePerDamageActive;
        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType)
                .Subscribe(_ => GetPossibleAttackMonster());
        }

        public void AttackAction()
        {
            bool isHammer = false;
            float rate = hammerAttackRate * 100;
            float randomValue = Random.Range(0f, 100f);
            isHammer = randomValue <= rate;

            if (isHammer)
            {
                StartCoroutine(HammerActionSequence());
            }
            else
            {
                StartCoroutine(ActionSequence());
            }
        }
        
        public IEnumerator HammerActionSequence()
        {
            FlipCharacter(targetedMonster[0].transform.localPosition.x);
            
            string animKey ="Attack_FrontSide2";
            
            anim.skeleton.SetSkin($"Lv_{1}");

            TrackEntry entry = anim.AnimationState.SetAnimation(0, animKey, false);

            List<Monster> targetCache = targetedMonster;
            float duration = entry.Animation.Duration;
            float tempSpeed = speedBuffValue == 0 ? attackSpeed : attackSpeed * speedBuffValue;
            entry.TimeScale = Calculator.GetAnimationSpeed(tempSpeed, duration);
            
            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");
            
            SoundManager.Instance.PlaySound(SoundKey.SF_HAMMERING_BEST);
            
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
            
            DamageType damageType = !isCritical ? DamageType.NORMAL : DamageType.CRITICAL;

            for (int i = 0; i < targetCache.Count; i++)
            {
                Vector2 targetPosition = Calculator.TransformationVector(targetCache[i].prevPositionIdx);

                for (int j = 0; j < GameManager.Instance.monsterSpawner.monsters.Count; ++j)
                {
                    Monster monster = GameManager.Instance.monsterSpawner.monsters[j];

                    Vector2 monsterPosition = Calculator.TransformationVector(monster.prevPositionIdx);

                    float distance = Calculator.DistanceCheck(targetPosition - monsterPosition);

                    if (distance <= hammerAttackRange)
                    {
                        if (isUpgradePerDamageActive)
                        {
                            monster.HammerSequence(hammerAttackDamage, hammerAttackAdditiveDamage, damageType, criticalDamageRate, this);
                        }
                        else
                        {
                            float attackDamage = Mathf.Floor(hammerAttackDamage);
                            //Debug.Log($"Hammering 해머 데미지 : {attackDamage}");
                            monster.HitDamage(Mathf.Floor(attackDamage), damageType, criticalDamageRate, this);
                        }
                    }
                }
            }

            yield return new WaitForSpineAnimationComplete(entry);
            
            anim.skeleton.SetSkin($"Lv_{0}");
            anim.AnimationState.SetAnimation(0, "Idle", true);
        }
        

        public override IEnumerator ActionSequence()
        {
            FlipCharacter(targetedMonster[0].transform.localPosition.x);
            
            string animKey = "Attack_FrontSide";

            anim.skeleton.SetSkin($"Lv_{0}");
            
            TrackEntry entry = anim.AnimationState.SetAnimation(0, animKey, false);

            List<Monster> targetCache = targetedMonster;
            float duration = entry.Animation.Duration;
            float tempSpeed = speedBuffValue == 0 ? attackSpeed : attackSpeed * speedBuffValue;
            entry.TimeScale = Calculator.GetAnimationSpeed(tempSpeed, duration);
            
            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");
            
            SoundManager.Instance.PlaySound(SoundKey.SF_HAMMERING_BASE);
            
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

            DamageType damageType = !isCritical ? DamageType.NORMAL : DamageType.CRITICAL;

            for (int i = 0; i < targetCache.Count; i++)
            {
                targetCache[i].HitDamage(totalDamage, damageType, criticalDamageRate, this);
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

            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);
            CharacterAction = null;

            isAttackType = true;
            targetCount = (int)characterData.targetCount;
            basicAttackDamage = characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0];
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
                    AttackAction();
                }
            };
            
            //particleName = "YellowHit";
            
            anim.skeleton.SetSkin($"Lv_{0}");
            
            maxStarGradeValue = characterData.maxStarGradeValue;
            
            //체력 비례뎀 효과 활성화 기준의 업그레이드 수치
            maxUpgradeValue = (int)characterData.characterUniqueValue[7];
            
            //해머 공격 범위
            hammerAttackRange = characterData.characterUniqueValue[2];
            
            //해머 공격 데미지
            hammerAttackDamage = characterData.characterUniqueValue[1] +
                                 (characterData.characterClassLevel - 1) * characterData.classUpFactor[2];
            
            //해머 공격 확률
            hammerAttackRate = characterData.characterUniqueValue[0] + characterData.characterUniqueValue[0] *
                Mathf.Pow(starGradeIndex, characterData.starFactor[0]);

            //체력 비례댐 효과 데미지
            hammerAttackAdditiveDamage = ((characterData.characterUniqueValue[6] +
                                           (characterData.characterClassLevel - 1)
                                           * characterData.classUpFactor[3]) +
                                          (upgradeIndex - 1) * characterData.powerFactor[3]) * starGradeIndex;
            
            //체력 비례댐 수치 체크 후 활성화 및 비활성화
            UpgradeEffectActiveCheck(upgradeIndex);
            
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
            SetAttackDamage();
            
            hammerAttackAdditiveDamage = ((characterData.characterUniqueValue[6] +
                                           (characterData.characterClassLevel - 1)
                                           * characterData.classUpFactor[3]) +
                                          (upgradeIndex - 1) * characterData.powerFactor[3]) * starGradeIndex;
            
            UpgradeEffectActiveCheck(upgradeIndex);
        }

        public override void SynthesisCharacter()
        {
            
        }

        public void UpgradeEffectActiveCheck(int upgradeIndex)
        {
            isUpgradePerDamageActive = upgradeIndex >= maxUpgradeValue;
        }
        
    }

}
