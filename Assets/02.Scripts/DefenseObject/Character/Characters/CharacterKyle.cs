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
    public class CharacterKyle : Character
    {
        public bool isBuffSequence;

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType && isBuffSequence)
                .Subscribe(_ => GetPossibleAttackMonster());
        }

        public override IEnumerator ActionSequence()
        {
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

            for (int i = 0; i < targetCache.Count; i++)
            {
                DragonProjectile projectile = GameManager.Instance.objectPoolManager.GetObject<DragonProjectile>(projectileName);
                projectile.transform.localPosition = transform.localPosition;
                projectile.attackValue = totalDamage;
                projectile.criticalDamageRate = criticalDamageRate;
                projectile.isMove = true;
                projectile.provokedIndex = glacierIdx;
                projectile.Initialize(this, targetCache[i], isCritical, projectileSpeed, strikeRange, particleName);
            }

            yield return new WaitForSpineAnimationComplete(entry);
            anim.AnimationState.SetAnimation(0, "Idle", true);
        }

        public IEnumerator BuffSequence()
        {
            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Buff", false);

            ObjectParticle dragonBuffStart = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("DragonBuff_Start");
            dragonBuffStart.PlayParticle(transform.position, 0);
            float duration = entry.Animation.Duration;
            entry.TimeScale = Calculator.GetAnimationSpeed(attackSpeed + speedBuffValue, duration);
            yield return new WaitForSpineEvent(anim, "Buff");

            //Character character = GameManager.Instance.characterSpawner.GetHighestDamageCharacter();
            Character character = GameManager.Instance.characterSpawner.GetAttackRandomCharacter();


            SoundManager.Instance.PlaySound(SoundKey.SF_CHARACTER_FOXY);
            ObjectParticle getDragonBuff = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("DragonBuff_Get");
            getDragonBuff.PlayParticle(character.transform.position, 0);

            if (character.buffParticle.Count > 0)
            {
                bool isDragonBuff = false;

                for (int i = 0; i < character.buffParticle.Count; i++)
                {
                    if(character.buffParticle[i].particleName == "DragonBuff")
                    {
                        isDragonBuff = true;
                        break;
                    }
                }

                if (!isDragonBuff)
                {
                    //ObjectParticle dragonBuff = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("DragonBuff");
                    //character.buffParticle.Add(dragonBuff);
                    //dragonBuff.transform.SetParent(character.transform);
                    //dragonBuff.transform.localPosition = Vector2.zero;
                    //dragonBuff.SimplePlay();
                    SetDragonBuffParticle(character);
                }
            }
            else
            {
                //ObjectParticle dragonBuff = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("DragonBuff");
                //character.buffParticle.Add(dragonBuff);
                //dragonBuff.transform.SetParent(character.transform);
                //dragonBuff.transform.localPosition = Vector2.zero;
                //dragonBuff.SimplePlay();

                SetDragonBuffParticle(character);
            }

            character.AttackValueBuff(true, buffValue);
            yield return new WaitForSpineAnimationComplete(entry);
            isBuffSequence = true;
            anim.AnimationState.SetAnimation(0, "Idle", true);
        }

        public void SetDragonBuffParticle(Character character)
        {
            ObjectParticle dragonBuff = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("DragonBuff");
            character.buffParticle.Add(dragonBuff);
            dragonBuff.transform.SetParent(character.transform);
            dragonBuff.transform.localPosition = Vector2.zero;
            dragonBuff.SimplePlay();
        }

        public override void SetCharacterInfo()
        {
            isBuffSequence = false;


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

            float temp = (characterData.characterUniqueValue[0] + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1])
                * (Mathf.Pow(ConfigData.STARGRADE_FACTOR + characterData.characterUniqueValue[1], starGradeIndex)) +
                (upgradeIndex - 1) * characterData.powerFactor[1] * (starGradeIndex + characterData.characterUniqueValue[1])
                + upgradeIndex - 1;
            buffValue = Mathf.FloorToInt(temp);

            SetAttackDamage();
            projectileName = characterData.projectileName;

            CharacterAction = () =>
            {
                if (isSummoned && targetedMonster.Count > 0)
                {
                    StartCoroutine(ActionSequence());
                }
            };
            particleName = "DragonHit";

            maxStarGradeValue = characterData.maxStarGradeValue;

            isSummoned = true;
            actionCoroutine = WaitActionSequence();
            StartCoroutine(actionCoroutine);

            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

            bool isAppear = starGradeIndex == 0;

            AppearAction(SetBuff, isAppear);
        }

        public override void SynthesisCharacter()
        {

        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;
            SetAttackDamage();

            float temp = (characterData.characterUniqueValue[0] + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1])
             * (Mathf.Pow(ConfigData.STARGRADE_FACTOR + characterData.characterUniqueValue[1], starGradeIndex)) +
             (upgradeIndex - 1) * characterData.powerFactor[1] * (starGradeIndex + characterData.characterUniqueValue[1])
             + upgradeIndex - 1;
            buffValue = Mathf.FloorToInt(temp);
        }

        public void SetBuff()
        {
            StartCoroutine(BuffSequence());
        }
    }
}