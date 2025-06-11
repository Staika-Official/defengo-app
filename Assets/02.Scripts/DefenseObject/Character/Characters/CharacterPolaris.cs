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
using CodeStage.AntiCheat.ObscuredTypes;

namespace Framework.Game.Defense
{
    public class CharacterPolaris : Character
    {
        public TimeIndicator timeIndicator;
        public float revolutionTime;
        public bool isRevolution;
        public IEnumerator timerCoroutine;
        public UnityAction revolution;

        public IEnumerator revolutionUpTimerCoroutine;
        public readonly float revolutionCheckTime = 5f;

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType && !isRevolution)
                .Subscribe(_ => GetPossibleAttackMonster());
        }

        public IEnumerator RevolutionActionSequence()
        {
            characterState = CharacterState.ATTACK;
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

            DamageType damageType = isCritical ? DamageType.CRITICAL : DamageType.NORMAL;


            for (int i = 0; i < targetCache.Count; i++)
            {
                if (!targetCache[i].IsAlive) continue;
                SoundManager.Instance.PlaySound(SoundKey.SF_CHARACTER_MEOSK);

                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(particleName);
                particle.PlayParticle(targetCache[0].transform.position, 0);
                targetCache[i].HitDamage(totalDamage, damageType, criticalDamageRate);

                for (int j = 0; j < GameManager.Instance.monsterSpawner.monsters.Count; j++)
                {
                    Monster near = GameManager.Instance.monsterSpawner.monsters[j];

                    if (!near.IsAlive || targetCache[0] == near) continue;

                    Vector2 targetVec = Calculator.TransformationVector(targetCache[0].prevPositionIdx);
                    Vector2 monVec = Calculator.TransformationVector(near.prevPositionIdx);

                    if (Calculator.DistanceCheck(targetVec - monVec) < strikeRange)
                    {
                        if (near.IsAlive)
                        {
                            near.HitDamage(totalDamage, damageType, criticalDamageRate);
                        }
                    }
                }
            }

            yield return new WaitForSpineAnimationComplete(entry);
            anim.AnimationState.SetAnimation(0, "Idle", true);
            revolution?.Invoke();
            characterState = CharacterState.DETECT;
        }


        public override IEnumerator ActionSequence()
        {
            characterState = CharacterState.ATTACK;
            FlipCharacter(targetedMonster[0].transform.localPosition.x);

            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);
            float duration = entry.Animation.Duration;
            entry.TimeScale = Calculator.GetAnimationSpeed(attackSpeed, duration);
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
                SoundManager.Instance.PlaySound(SoundKey.SF_CHARACTER_MEOSK);
                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(particleName);
                particle.PlayParticle(targetCache[0].transform.position, 0);
                targetCache[i].HitDamage(totalDamage, damageType, criticalDamageRate);
            }

            yield return new WaitForSpineAnimationComplete(entry);
            anim.AnimationState.SetAnimation(0, "Idle", true);
            revolution?.Invoke();
            characterState = CharacterState.DETECT;
        }

        public IEnumerator SetMaxGradeCharacter()
        {
            //Debug.Log("Revolution Polaris " + transform.name);
            
            isRevolution = true;
            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Change", false);

            yield return new WaitForSpineAnimationComplete(entry);

            //Test코드
            //GameManager.Instance.characterSpawner.SummonSynthesisWantCharacter<CharacterPolaris>(glacierIdx, starGradeIndex + 1, "Polaris");
            //GameManager.Instance.characterSpawner.SummonSynthesisWantCharacter<CharacterYuri>(glacierIdx, starGradeIndex + 1, "Yuri");
            GameManager.Instance.characterSpawner.SummonSynthesisCharacter(glacierIdx, starGradeIndex + 1);

            endOfUse?.Invoke();
            shieldDestroyAction?.Invoke();
            
            FocusCharacter(false);

            if (characterInterface != null)
            {
                GameManager.Instance.objectPoolManager.ReturnObject(characterInterface, "CharacterInterface");
            }

            characterInterface = null;

            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
            }

            //todo::dongmin
            if (revolutionUpTimerCoroutine != null)
            {
                StopCoroutine(revolutionUpTimerCoroutine);
                revolutionUpTimerCoroutine = null;
            }

            if (isDrag)
            {
                isDrag = false;
                isDestroyed = true;
            }
            ReturnParticle();
            GameManager.Instance.characterSpawner.AddOrRemoveSummonedCharacterList(this, false);
            GameManager.Instance.missionManager.AddOrRemoveCharacter(this, false);
            GameManager.Instance.objectPoolManager.ReturnObject(this, characterName);
            revolution = null;
            isRevolution = false;
        }

        public IEnumerator RevolutionTimer()
        {
            float count = revolutionTime;

            while (true)
            {
                if (!isSummoned) break;
                count -= Time.deltaTime;
                float value = (revolutionTime - count) / revolutionTime;
                timeIndicator.SetIndicator(value);
                yield return null;

                if (count <= 0)
                {
                    if (characterState == CharacterState.ATTACK)
                    {
                        revolution = () =>
                        {
                            StartCoroutine(SetMaxGradeCharacter());
                        };
                    }
                    else
                    {
                        StartCoroutine(SetMaxGradeCharacter());
                    }

                    //todo::dongmin
                    revolutionUpTimerCoroutine = RevolutionTimerCoroutine();
                    StartCoroutine(revolutionUpTimerCoroutine);
                    break;
                }
            }
        }

        public IEnumerator RevolutionTimerCoroutine()
        {
            yield return new WaitForSeconds(revolutionCheckTime);
            StartCoroutine(SetMaxGradeCharacter());
        }

        public override void SetCharacterInfo()
        {
            revolutionTime = characterData.characterUniqueValue[1] - (characterData.characterClassLevel - 1) 
                * characterData.classUpFactor[1]
                + (starGradeIndex * characterData.starFactor[1]);
            
            //revolutionTime = 5.0f;
            if (targetedMonster.Count != 0)
            {
                targetedMonster.Clear();
            }

            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
            }

            if (starGradeIndex < 5)
            {
                TimeIndicator indicator = GameManager.Instance.objectPoolManager.GetObject<TimeIndicator>("PolarisIndicator");
                this.timeIndicator = indicator;
                indicator.Initialize(this);

                endOfUse = () =>
                {
                    //todo::dongmin
                    //폴라리스 성장이 4성에서 멈추며 모든 동작이 중지할 때 QA건으로 인한 방어 코드 
                    if (revolutionUpTimerCoroutine != null)
                    {
                        StopCoroutine(revolutionUpTimerCoroutine);
                    }

                    GameManager.Instance.objectPoolManager.ReturnObject(timeIndicator, "PolarisIndicator");
                    timeIndicator = null;
                    StopCoroutine(timerCoroutine);
                };

                timerCoroutine = RevolutionTimer();
                StartCoroutine(timerCoroutine);
            }
            else
            {
                endOfUse = null;
            }


            particleName = "PolarisHit";
            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);
            CharacterAction = null;

            isAttackType = true;
            targetCount = (int)characterData.targetCount;


            basicAttackDamage = (characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0]);
            attackSpeed = (characterData.attackSpeed + Mathf.Pow((starGradeIndex * characterData.starFactor[0]), ConfigData.STARGRADE_FACTOR));

            criticalRange = characterData.criticalRange + GameManager.Instance.buffManager.increaseCriticalRate;
            criticalDamageRate = characterData.criticalDamageRate;
            upgradeFactor = characterData.powerFactor[0];
            projectileSpeed = characterData.projectileSpeed;
            attackRange = Mathf.Pow(characterData.detectRange, 2);
            strikeRange = characterData.characterUniqueValue[0];
            isThrow = characterData.isThrow;
            buffDuration = characterData.buffDuration;
            buffValue = characterData.buffValue;
            SetAttackDamage();
            projectileName = characterData.projectileName;

            CharacterAction = () =>
            {
                if (isSummoned && targetedMonster.Count > 0)
                {
                    if (isRevolution) return;
                    if (starGradeIndex < 5)
                    {
                        StartCoroutine(ActionSequence());
                    }
                    else
                    {
                        StartCoroutine(RevolutionActionSequence());
                    }
                }
            };

            //todo::dongmin
            revolutionUpTimerCoroutine = null;

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
            basicAttackDamage = (characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0]);
            SetAttackDamage();
        }

        public override void Reposition(Glacier glacier)
        {
            base.Reposition(glacier);

            if(timeIndicator != null)
            {
                timeIndicator.SetPosition();
            }
        }
    }
}