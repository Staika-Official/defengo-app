using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Sound;
using UnityEngine;
using Spine;
using Spine.Unity;
using UniRx;
using UniRx.Triggers;
using Framework.Util;

namespace Framework.Game.Defense
{
    public class CharacterRio : Character
    {
        private float attackCoolTime;
        
        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType)
                .Subscribe(_ => GetPossibleAttackNearOrFarMonster(true));
        }

        public override IEnumerator ActionSequence()
        {
            while (isSummoned && !IsLockdown)
            {
                if (targetedMonster.Count > 0 && characterState == CharacterState.DETECT)
                {
                    characterState = CharacterState.ATTACK;
                    FlipCharacter(targetedMonster[0].transform.localPosition.x);

                    TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);

                    SoundManager.Instance.PlaySound(SoundKey.SF_RIO_READY);

                    List<Monster> targetCache = targetedMonster;
                    float duration = entry.Animation.Duration;
                    float characterSpeed = speedBuffValue == 0 ? attackSpeed : attackSpeed * speedBuffValue;
                    float projectileSpeed = characterSpeed * characterData.characterUniqueValue[0];
                    SetAttackCoolTime(characterSpeed);
                    
                    Debug.Log($"Rio Attack Speed : {characterSpeed}");
                    Debug.Log($"Rio Attack CoolTime : {attackCoolTime}");
                    Debug.Log($"Rio projectileSpeed : {projectileSpeed}");
                    
                    entry.TimeScale = Calculator.GetAnimationSpeed(characterSpeed, duration);
                    yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

                    BoomerangProjectile projectile = null;

                    for (int i = 0; i < targetCache.Count; i++)
                    {
                        projectile = GameManager.Instance.objectPoolManager.GetObject<BoomerangProjectile>(projectileName);
                        projectile.objectName = projectileName;
                        projectile.transform.localPosition = pivotPosition;
                        projectile.attackValue = totalDamage;
                        projectile.criticalDamageRate = criticalDamageRate;
                        projectile.isMove = true;
                        projectile.provokedIndex = glacierIdx;
                        projectile.criticalRange = criticalRange;
                        projectile.Initialize(this, targetCache[i], projectileSpeed, particleName);
                    }

                    if (!entry.IsComplete)
                    {
                        yield return new WaitForSpineAnimationComplete(entry);
                    }
                    
                    entry.TimeScale = 1;
                    entry = anim.AnimationState.SetAnimation(0, "Attack_Ing", true);
                    entry.TimeScale = Calculator.GetAnimationSpeed(characterSpeed, duration);

                    while (true)
                    {
                        if (!(projectile is null))
                        {
                            if (projectile.isTurnEnd) break;
                        }
                        else
                        {
                            break;
                        }

                        yield return null;
                    }

                    entry.TimeScale = 1;
                    entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide_Back", false);
                    entry.TimeScale = Calculator.GetAnimationSpeed(characterSpeed, duration);
                    SoundManager.Instance.PlaySound(SoundKey.SF_RIO_SUCCESS);

                    if (!entry.IsComplete)
                    {
                        yield return new WaitForSpineAnimationComplete(entry);
                    }

                    entry.TimeScale = 1;
                    anim.AnimationState.SetAnimation(0, "Idle", true);
                    characterState = CharacterState.DETECT;

                    yield return new WaitForSeconds(attackCoolTime);
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
            }

            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);
            CharacterAction = null;

            isAttackType = true;
            targetCount = (int)characterData.targetCount;
            basicAttackDamage = characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0];
            attackSpeed = (characterData.attackSpeed + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1]) + (upgradeIndex - 1) * characterData.powerFactor[1];
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
            SetAttackCoolTime(attackSpeed);
            projectileName = characterData.projectileName;


            particleName = characterData.selectSkinType == SkinGradeType.NONE ? "BoomerangHit" : $"BoomerangHit_{characterData.selectSkinID}";

            maxStarGradeValue = characterData.maxStarGradeValue;

            isSummoned = true;

            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

            bool isAppear = starGradeIndex == 0;

            AppearAction(RioAppearAction, isAppear);
        }

        
        public void SetAttackCoolTime(float attackSpeed)
        {
            attackCoolTime = characterData.characterUniqueValue[1] 
                             - (attackSpeed * (characterData.characterUniqueValue[3]
                                               + (characterData.characterClassLevel - 1) 
                                               * characterData.classUpFactor[2]));
        }

        public void RioAppearAction()
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
            basicAttackDamage = characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0];
            attackSpeed = (characterData.attackSpeed + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1]) + (this.upgradeIndex - 1) * characterData.powerFactor[1];
            SetAttackCoolTime(attackSpeed);
            
            SetAttackDamage();
        }
    }
}