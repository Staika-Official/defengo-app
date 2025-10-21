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
    public class CharacterWillow : Character
    {
        public float berserkerDamage;
        public float berserkerAttackSpeed;
        public float additiveBerserkerDamage;
        public bool isBerserker = false;

        void Start()
        {
            this.UpdateAsObservable()
               .Where(_ => isSummoned && isAttackType)
               .Subscribe(_ => GetPossibleAttackMonster());

            GameManager.Instance.characterSpawner.characterCount.Subscribe(_ => CalcBerserker());
            GameManager.Instance.characterSpawner.repositionCount.Subscribe(_ => CalcBerserker());
        }

        public void CalcBerserker()
        {
            float currentRange = 18;
            List<Character> characters = GameManager.Instance.characterSpawner.summonedCharacters;

            isBerserker = true;
            SetAttackDamage();
            if(characters.Count == 1)
            {
                isBerserker = true;
                attackSpeed = berserkerAttackSpeed;
                berserkerDamage = totalDamage + additiveBerserkerDamage;
            }
            else
            {
                for (int i = 0; i < GameManager.Instance.characterSpawner.summonedCharacters.Count; i++)
                {
                    Character character = GameManager.Instance.characterSpawner.summonedCharacters[i];
                    if (character == this) continue;
                    float distance = Calculator.DistanceCheck(currentGridPos - character.currentGridPos);

                    if (distance <= currentRange)
                    {
                        isBerserker = false;
                        attackSpeed = characterData.attackSpeed;
                        //totalDamage = basicAttackDamage;
                        switch (characterState)
                        {
                            case CharacterState.DETECT:
                                anim.AnimationState.SetAnimation(0, "Idle", true);
                                break;
                            case CharacterState.ATTACK:
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                    else
                    {
                        isBerserker = true;
                        attackSpeed = berserkerAttackSpeed;
                        berserkerDamage = totalDamage + additiveBerserkerDamage;

                        switch (characterState)
                        {
                            case CharacterState.DETECT:
                                anim.AnimationState.SetAnimation(0, "Idle_Change", true);
                                break;
                            case CharacterState.ATTACK:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public override IEnumerator ActionSequence()
        {
            FlipCharacter(targetedMonster[0].transform.localPosition.x);

            string animKey = isBerserker ? "Attack_FrontSide_Change" : "Attack_FrontSide";

            TrackEntry entry = anim.AnimationState.SetAnimation(0, animKey, false);
            float duration = entry.Animation.Duration;
            float tempSpeed = speedBuffValue == 0 ? attackSpeed : attackSpeed * speedBuffValue;
            entry.TimeScale = Calculator.GetAnimationSpeed(tempSpeed, duration);
            characterState = CharacterState.ATTACK;
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

            DamageType damageType = isCritical ? DamageType.CRITICAL : DamageType.NORMAL;

            for (int i = 0; i < targetCache.Count; i++)
            {
                if (!targetCache[i].IsAlive) continue;
                
                particleName = isBerserker ? "GroundHitBerserker" : "GroundHitNormal";

                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(particleName);

                float damage = isBerserker ? berserkerDamage : totalDamage;

                particle.PlayParticle(targetCache[0].transform.position, 0);
                SoundKey soundKey = isBerserker ? SoundKey.SF_SWORD_BERSERKER : SoundKey.SF_SWORD_NORMAL;
                SoundManager.Instance.PlaySound(soundKey);
                targetCache[i].HitDamage(damage, damageType, criticalDamageRate, this);
            }

            yield return new WaitForSpineAnimationComplete(entry);
            entry.TimeScale = 1;
            characterState = CharacterState.DETECT;
            animKey = isBerserker ? "Idle_Change" : "Idle"; 
            anim.AnimationState.SetAnimation(0, animKey, true);
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
            additiveBerserkerDamage = characterData.characterUniqueValue[0] + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1]
                + ((upgradeIndex - 1) * characterData.powerFactor[1] * starGradeIndex + (upgradeIndex - 1));

            basicAttackDamage = (characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0]);
            
            attackSpeed = characterData.attackSpeed;
            berserkerAttackSpeed = attackSpeed + (characterData.characterUniqueValue[1] + characterData.starFactor[0] * starGradeIndex);

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

            CalcBerserker();

            string animKey = isBerserker ? "Idle_Change" : "Idle";

            bool isAppear = starGradeIndex == 0;

            AppearAction(animKey, isAppear);
        }

        public override void SynthesisCharacter()
        {
            //berserkerAttackSpeed = attackSpeed
            berserkerAttackSpeed = attackSpeed + (characterData.characterUniqueValue[1] + characterData.starFactor[0] * starGradeIndex);
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;
            additiveBerserkerDamage = characterData.characterUniqueValue[0] + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1]
                + ((upgradeIndex - 1) * characterData.powerFactor[1] * starGradeIndex + (upgradeIndex - 1));

            basicAttackDamage = (characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0]);

            SetAttackDamage();
            berserkerDamage = totalDamage + additiveBerserkerDamage;
            //totalDamage = isBerserker ? totalDamage += additiveBerserkerDamage : totalDamage;777
        }
    }
}
