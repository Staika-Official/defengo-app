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
    public class CharacterDavi : Character
    {
        public ObscuredInt revolutionCount;
        public ObscuredFloat additiveDamage;
        public ObscuredFloat revolutionTime;
        public bool isRevolution;

        public TimeIndicator timeIndicator;
        public IEnumerator timerCoroutine;
        public Coroutine attack;
        public UnityAction revolution;

        void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType && !isRevolution)
                .Subscribe(_ => GetPossibleAttackMonster());
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
                    if(characterState == CharacterState.ATTACK)
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
                    
                    break;
                }
            }
        }

        public IEnumerator SetMaxGradeCharacter()
        {
            if (characterState == CharacterState.ATTACK)
            {
                StopCoroutine(attack);
                characterState = CharacterState.DETECT;
            }

            isRevolution = true;
            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Change", false);
            yield return new WaitForSpineEvent(anim, "Change");
            revolutionCount++;
            SoundManager.Instance.PlaySound(SoundKey.SF_BOXOPEN_EPIC);
            anim.skeleton.SetSkin($"Lv_{revolutionCount}");
            SetRevolutionDamage();

            yield return new WaitForSpineAnimationComplete(entry);

            if (revolutionCount < 3)
            {
                timerCoroutine = RevolutionTimer();
                StartCoroutine(timerCoroutine);
            }
            else
            {
                GameManager.Instance.objectPoolManager.ReturnObject(timeIndicator, "PolarisIndicator");
                timeIndicator = null;
            }

            revolution = null;

            anim.AnimationState.SetAnimation(0, "Idle", true);
            isRevolution = false;
        }

        public override IEnumerator ActionSequence() //Deprecated
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

                SoundKey soundKey = revolutionCount < 3 ? SoundKey.SF_WOODEN_HAMMER : SoundKey.SF_STEEL_HAMMER;

                SoundManager.Instance.PlaySound(soundKey);
                targetCache[i].HitDamage(totalDamage + additiveDamage, damageType, criticalDamageRate);
            }

            yield return new WaitForSpineAnimationComplete(entry);
            anim.AnimationState.SetAnimation(0, "Idle", true);

            revolution?.Invoke();
            characterState = CharacterState.DETECT;
        }

        public override void SetCharacterInfo()
        {
            revolutionCount = 0;

            if (targetedMonster.Count != 0)
            {
                targetedMonster.Clear();
            }

            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
            }

            TimeIndicator indicator = GameManager.Instance.objectPoolManager.GetObject<TimeIndicator>("PolarisIndicator");
            this.timeIndicator = indicator;

            indicator.Initialize(this);

            endOfUse = () =>
            {
                if (revolutionCount != 3)
                {
                    GameManager.Instance.objectPoolManager.ReturnObject(timeIndicator, "PolarisIndicator");
                    timeIndicator = null;
                    StopCoroutine(timerCoroutine);
                }
            };

            revolutionTime = characterData.characterUniqueValue[3] - (characterData.characterClassLevel - 1) * characterData.classUpFactor[2];

            //revolutionTime = 10.0f;

            timerCoroutine = RevolutionTimer();
            StartCoroutine(timerCoroutine);

            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);
            CharacterAction = null;

            isAttackType = true;
            targetCount = (int)characterData.targetCount;
            basicAttackDamage = characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0];
            revolutionCount = 0;
            attackSpeed = characterData.attackSpeed + (revolutionCount * characterData.characterUniqueValue[1]);
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
                if (isSummoned && targetedMonster.Count > 0 && !isRevolution)
                {
                    attack = StartCoroutine(ActionSequence());
                }
            };


            anim.skeleton.SetSkin($"Lv_{revolutionCount}");
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

        public void SetRevolutionDamage()
        {
            additiveDamage = revolutionCount * (characterData.characterUniqueValue[0] + (characterData.characterClassLevel - 1)
                * characterData.classUpFactor[1]) + (upgradeIndex - 1)
                * characterData.powerFactor[1] * starGradeIndex;

            attackSpeed = characterData.attackSpeed + (revolutionCount * characterData.characterUniqueValue[1]);
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex; 
            SetAttackDamage();

            additiveDamage = revolutionCount * (characterData.characterUniqueValue[0] + (characterData.characterClassLevel - 1)
               * characterData.classUpFactor[1]) + (upgradeIndex - 1)
               * characterData.powerFactor[1] * starGradeIndex;

            attackSpeed = characterData.attackSpeed + (revolutionCount * characterData.characterUniqueValue[1]);
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
