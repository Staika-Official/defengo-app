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
using System.Collections;

namespace Framework.Game.Defense
{
    public class CharacterPatrol : Character
    {
        public bool isChange = false;
        public float totalTransformCycle;
        public float totalTransformSeconds;
        public float totalTransformSpeed;

        public bool isAttacking = false;

        public UnityAction characterActionCallBack;

        public readonly string changeAttack = "Attack_FrontSide_Change";
        public readonly string changeIdle = "Idle_Change";

        //public string attack;

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType)
                .Subscribe(_ => GetPossibleAttackMonster());
        }

        public override IEnumerator ActionSequence()
        {
            //anim.timeScale = attackSpeed / 1.2f;
            characterState = CharacterState.ATTACK;
            FlipCharacter(targetedMonster[0].transform.localPosition.x);

            string attackKey = isChange ? "Attack_FrontSide_Change" : "Attack_FrontSide";

            TrackEntry entry = anim.AnimationState.SetAnimation(0, attackKey, false);
            float duration = entry.Animation.Duration;
            float tempSpeed = speedBuffValue == 0 ? attackSpeed : attackSpeed * speedBuffValue;
            entry.TimeScale = Calculator.GetAnimationSpeed(attackSpeed + speedBuffValue, duration);
            SoundManager.Instance.PlaySound(SoundKey.SF_SWISH);
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
                TornadoProjectile projectile = GameManager.Instance.objectPoolManager.GetObject<TornadoProjectile>(projectileName);
                projectile.transform.localPosition = transform.localPosition;

                float damage = totalDamage;
                projectile.attackValue = damage;
                projectile.criticalDamageRate = criticalDamageRate;
                projectile.isMove = true;
                projectile.provokedIndex = glacierIdx;

                projectile.Initialize(this, targetCache[i], isCritical, projectileSpeed, strikeRange, particleName);
            }

            if (!entry.IsComplete)
            {
                yield return new WaitForSpineAnimationComplete(entry);
            }
            
            entry.TimeScale = 1;
            string idleKey = isChange ? "Idle_Change" : "Idle";
            anim.AnimationState.SetAnimation(0, idleKey, true);
            characterState = CharacterState.DETECT;
            characterActionCallBack?.Invoke();
            characterActionCallBack = null;
        }



        public override void SetCharacterInfo()
        {
            characterState = CharacterState.DETECT;
            //attack = "Attack_FrontSide";
            anim.AnimationState.SetAnimation(0, "Idle", true);

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
            totalTransformCycle = characterData.characterUniqueValue[0];
            totalTransformSeconds = characterData.characterUniqueValue[1] + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1];
            totalTransformSpeed = characterData.characterUniqueValue[2] + (upgradeIndex - 1) * characterData.powerFactor[1];
            targetCount = (int)characterData.targetCount;

            float attackDamage = characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0];

            basicAttackDamage = attackDamage;
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
                    if (characterState == CharacterState.DETECT)
                    {
                        StartCoroutine(ActionSequence());
                    }
                }
            };
            particleName = "TornadoHit";
            maxStarGradeValue = characterData.maxStarGradeValue;
            isSummoned = true;
            actionCoroutine = WaitActionSequence();
            StartCoroutine(actionCoroutine);
            StartCoroutine(ChangeActionSequence());
            isChange = false;
            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

            bool isAppear = starGradeIndex == 0;

            AppearAction("Idle", isAppear);
        }

        public IEnumerator ChangeActionSequence()
        {
            float duration = isChange ? totalTransformSeconds : (1 / totalTransformCycle);
            yield return new WaitForSeconds(duration);
            if (GameManager.Instance.gameState == GameState.PLAY)
            {
                switch (characterState)
                {
                    case CharacterState.DETECT:
                        StartCoroutine(ChangeSeqeunce());
                        break;
                    case CharacterState.ATTACK:
                        characterActionCallBack = null;
                        characterActionCallBack = () =>
                        {
                            StartCoroutine(ChangeSeqeunce());
                        };
                        break;
                    default:
                        break;
                }
            }
            else
            {
                StartCoroutine(ChangeActionSequence());
            }
        }

        public float tempAdditiveSpeedValue;

        public void SetAttackSpeed()
        {
            if (isChange)
            {
                tempAdditiveSpeedValue = totalTransformSpeed;
                attackSpeed += tempAdditiveSpeedValue;
            }
            else
            {
                attackSpeed -= tempAdditiveSpeedValue;
            }
        }

        public IEnumerator ChangeSeqeunce()
        {
            characterState = CharacterState.CHANGE;
            string changeAnimKey = isChange ? "ChangeBack" : "Change";

            TrackEntry entry = anim.AnimationState.SetAnimation(0, changeAnimKey, false);
            SoundManager.Instance.PlaySound(SoundKey.SF_PATROL_CHANGE);
            yield return new WaitForSpineAnimationComplete(entry);
            isChange = !isChange;
            SetAttackSpeed();
            string changeIdleKey = isChange ? "Idle_Change" : "Idle";

            anim.AnimationState.SetAnimation(0, changeIdleKey, true);
            characterState = CharacterState.DETECT;
            StartCoroutine(ChangeActionSequence());
        }

        public override void SynthesisCharacter()
        {

        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;
            SetAttackDamage();
            totalTransformSpeed = characterData.characterUniqueValue[2] + (upgradeIndex - 1) * characterData.powerFactor[1];
        }
    }
}
