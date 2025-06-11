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
    public class CharacterVegas : Character
    {

        public float additiveMinDamage;
        public float additiveMaxDamage;
        public float additiveRandomDamage;
        public float specialEffectValue;
        
        public Vector3 attackPosition;

        private void Start()
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
            float duration = entry.Animation.Duration;
            float tempSpeed = speedBuffValue == 0 ? attackSpeed : attackSpeed * speedBuffValue;
            entry.TimeScale = Calculator.GetAnimationSpeed(tempSpeed, duration);
            
            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

            SetAttackValue();
            
            bool isGambling = false;
            bool isCritical;

            float gamblingDamage;
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

            additiveRandomDamage = Random.Range(additiveMinDamage, additiveMaxDamage);
            
            if (isCritical)
            {
                gamblingDamage = additiveRandomDamage * criticalDamageRate;
                //Debug.Log($"IsCritical {gamblingDamage}");
            }
            else
            {
                gamblingDamage = additiveRandomDamage;
            }
            
            if (gamblingDamage >= specialEffectValue)
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_VEGAS_ATTACK_BEST);
                isGambling = true;
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_VEGAS_ATTACK_BASE);
            }
            
            //Debug.Log($"Vegas RandomDamage : {gamblingDamage}");

            for (int i = 0; i < targetCache.Count; i++)
            {
                GunProjectile projectile = GameManager.Instance.objectPoolManager.GetObject<GunProjectile>(projectileName);
                Vector3 tempPosition = isLeft ? attackPosition : new Vector3(attackPosition.x + 2, attackPosition.y, attackPosition.z);
                projectile.transform.localPosition = tempPosition;
                projectile.attackValue = gamblingDamage;
                projectile.criticalDamageRate = criticalDamageRate;
                projectile.isMove = true;
                projectile.provokedIndex = glacierIdx;
                projectile.Initialize(this, targetCache[i], isGambling, projectileSpeed, strikeRange, particleName);
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
                    StartCoroutine(ActionSequence());
                }
            };
            particleName = "YellowHit";
            
            maxStarGradeValue = characterData.maxStarGradeValue;

            isSummoned = true;

            attackPosition = anim.Skeleton.FindBone("AttackPosition").GetWorldPosition(anim.transform);
            SetAttackValue();
            
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
            
            SetAttackValue();
        }

        public override void SynthesisCharacter()
        {
            
        }

        public override void Reposition(Glacier glacier)
        {
            base.Reposition(glacier);
            attackPosition = anim.Skeleton.FindBone("AttackPosition").GetWorldPosition(anim.transform);
        }

        public void SetAttackValue()
        {
            float vegasTotalDamage = totalDamage;
            
            float maxDamage = characterData.characterUniqueValue[1] 
                              + ((characterData.characterClassLevel - 1) 
                                 * characterData.classUpFactor[1]) + 
                              (starGradeIndex * characterData.starFactor[0])
                              + ((upgradeIndex - 1) * characterData.powerFactor[1]);
            
            additiveMinDamage = Mathf.Floor(vegasTotalDamage * characterData.characterUniqueValue[0]);
            additiveMaxDamage = Mathf.Floor(vegasTotalDamage * maxDamage);
            specialEffectValue = additiveMaxDamage * characterData.characterUniqueValue[2];
            
            Debug.Log($"additiveMin {additiveMinDamage}");
            Debug.Log($"additiveMax {additiveMaxDamage}");
            Debug.Log($"special Effect {specialEffectValue}");
        }
        
    }

}
