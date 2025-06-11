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
    public enum MarrierState
    {
        NONE,
        NORMAL,
        UPGRADE
    }

    public class CharacterMarrier : Character
    {
        public MarrierState marrierState;
        public Vector3 attackPos;
        public float additiveDamage;
        //public float temp;

        void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType)
                .Subscribe(_ => GetPossibleAttackMonster());


            GameManager.Instance.gemValue
                .Subscribe(value => SetSkinState(value));
        }

        public override IEnumerator ActionSequence()
        {
            FlipCharacter(targetedMonster[0].transform.localPosition.x);

            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);

            List<Monster> targetCache = targetedMonster;
            float duration = entry.Animation.Duration;
            entry.TimeScale = Calculator.GetAnimationSpeed(attackSpeed + speedBuffValue, duration);

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

            SoundKey soundKey = marrierState == MarrierState.NORMAL ? SoundKey.SF_NOTIFICATION_1 : SoundKey.SF_CHARACTER_SHELLFISH_2;

            for (int i = 0; i < targetCache.Count; i++)
            {
                WaterProjectile projectile = GameManager.Instance.objectPoolManager.GetObject<WaterProjectile>(projectileName);
                Vector3 attackPosition = isLeft ? attackPos : new Vector3(attackPos.x + 1, attackPos.y, attackPos.z);
                projectile.transform.localPosition = attackPosition;
                projectile.attackValue = totalDamage + additiveDamage;
                projectile.criticalDamageRate = criticalDamageRate;
                projectile.isMove = true;
                projectile.provokedIndex = glacierIdx;
                projectile.Initialize(this, targetCache[i], isCritical, projectileSpeed, strikeRange, particleName, soundKey);
            }

            yield return new WaitForSpineAnimationComplete(entry);
            anim.AnimationState.SetAnimation(0, "Idle", true);
        }

        public override void SetCharacterInfo()
        {
            anim.Skeleton.SetSkin("Lv_0");
            attackPos = anim.Skeleton.FindBone("AttackPosition").GetWorldPosition(anim.transform);

            marrierState = MarrierState.NONE;

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
            particleName = "WaterHit";

            maxStarGradeValue = characterData.maxStarGradeValue;

            isSummoned = true;
            actionCoroutine = WaitActionSequence();
            StartCoroutine(actionCoroutine);
            SetSkinState(GameManager.Instance.Gem);

            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

            bool isAppear = starGradeIndex == 0;

            AppearAction("Idle", isAppear);
        }

        public void SetSkinState(int gemValue)
        {
            int temp = gemValue > 1000 ? 1000 : gemValue;
            bool isUpgrade = temp >= 400;
            float damage = (1 + characterData.characterUniqueValue[0]
                + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1])
                * (temp * 0.1f)
                * characterData.starFactor[0]
                * starGradeIndex;
            additiveDamage = damage;
            switch (marrierState)
            {
                case MarrierState.NORMAL:
                    if (isUpgrade)
                    {
                        anim.Skeleton.SetSkin("Lv_1");
                        marrierState = MarrierState.UPGRADE;
                    }
                    break;
                case MarrierState.UPGRADE:
                    if (!isUpgrade)
                    {
                        anim.Skeleton.SetSkin("Lv_0");
                        marrierState = MarrierState.NORMAL;
                    }
                    break;
                case MarrierState.NONE:
                    string skinName = isUpgrade ? "Lv_1" : "Lv_0";
                    anim.Skeleton.SetSkin(skinName);
                    marrierState = isUpgrade ? MarrierState.UPGRADE : MarrierState.NORMAL;
                    break;
            }
        }

        public override void SynthesisCharacter()
        {
            
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;
            SetAttackDamage();
        }

        public override void Reposition(Glacier glacier)
        {
            base.Reposition(glacier);
            attackPos = anim.Skeleton.FindBone("AttackPosition").GetWorldPosition(anim.transform);
        }
    }
}