using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.GameData.Defense;
using Framework.Sound;
using Framework.Util;
using Spine;
using Spine.Unity;
using UniRx;
using UniRx.Triggers;

namespace Framework.Game.Defense
{
    public class CharacterYuri : Character
    {
        public float addtiveDamage;
        public float arrowRainDuration;
        public float arrowRainAttackCoolDown;
        public float arrowRainScale;

        void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType)
                .Subscribe(_ => GetPossibleAttackMonster());
        }

        public override IEnumerator ActionSequence()
        {
            while (isSummoned)
            {
                if (targetedMonster.Count > 0 && characterState == CharacterState.DETECT)
                {
                    characterState = CharacterState.ATTACK;
                    List<Monster> targetCache = targetedMonster;

                    FlipCharacter(targetedMonster[0].transform.localPosition.x);

                    TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);
                    float duration = entry.Animation.Duration;
                    float totalAttacSpeed = attackSpeed;
                    entry.TimeScale = Calculator.GetAnimationSpeed(totalAttacSpeed, duration);

                    yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

                    SoundManager.Instance.PlaySound(SoundKey.SF_YURI_ATTACK);

                    for (int i = 0; i < targetCache.Count; i++)
                    {
                        ArrowProjectile arrowRain = GameManager.Instance.objectPoolManager.GetObject<ArrowProjectile>(projectileName);
                        arrowRain.transform.localPosition = transform.localPosition;
                        arrowRain.isMove = true;
                        arrowRain.arrowRaineDamage = totalDamage;
                        arrowRain.arrowRainAddtiveDamage = addtiveDamage;
                        arrowRain.arrowCriticalRange = criticalRange;
                        arrowRain.arrowCriticalDamageRate = criticalDamageRate;
                        arrowRain.arrowRainRange = arrowRainScale;
                        arrowRain.arrowRainDuration = arrowRainDuration;

                        arrowRain.Initialize(this, targetCache[i], projectileSpeed);
                    }

                    yield return new WaitForSpineAnimationComplete(entry);
                    entry.TimeScale = 1;
                    anim.AnimationState.SetAnimation(0, "Idle", true);
                    characterState = CharacterState.DETECT;

                    yield return new WaitForSeconds(arrowRainAttackCoolDown);
                }

                yield return null;
            }
        }

        public override void SetCharacterInfo()
        {
            characterState = CharacterState.APPEAR;
            
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

            basicAttackDamage = characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0];
            targetCount = (int)characterData.targetCount;
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
            maxStarGradeValue = characterData.maxStarGradeValue;

            //유리 전용 스탯
            addtiveDamage = ((characterData.characterUniqueValue[0] + (characterData.characterClassLevel - 1)
                * characterData.classUpFactor[1])
                + (upgradeIndex - 1) * characterData.powerFactor[1]) * starGradeIndex;

            arrowRainDuration = characterData.characterUniqueValue[2] + (characterData.characterClassLevel - 1) * characterData.classUpFactor[2];
            arrowRainAttackCoolDown = characterData.characterUniqueValue[3];
            arrowRainScale = characterData.characterUniqueValue[1];

            isSummoned = true;
            
            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

            bool isAppear = starGradeIndex == 0;

            AppearAction(ArrowAction, isAppear);
        }

        public void ArrowAction()
        {
            anim.AnimationState.SetAnimation(0, "Idle", true);
            
            actionCoroutine = ActionSequence(); 
            StartCoroutine(actionCoroutine);
        }
        
        public override void SynthesisCharacter()
        {

        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;

            addtiveDamage = ((characterData.characterUniqueValue[0] + (characterData.characterClassLevel - 1)
                * characterData.classUpFactor[1])
                + (upgradeIndex - 1) * characterData.powerFactor[1]) * starGradeIndex;

            SetAttackDamage();
        }



    }
}
