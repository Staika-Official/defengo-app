using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Util;
using Spine;
using Spine.Unity;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Framework.Game.Defense
{
    public class CharacterLemmingToxin : Character
    {
        public float toxicBoardTime;
        public float toxicBoardSize;

        void Start()
        {
            this.UpdateAsObservable()
             .Where(_ => isSummoned && isAttackType)
             .Subscribe(_ => GetPossibleAttackMonster());
        }

        public override IEnumerator ActionSequence()
        {
            FlipCharacter(targetedMonster[0].transform.localPosition.x);

            List<Monster> targetCache = targetedMonster;

            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);
            float duration = entry.Animation.Duration;
            float totalAttacSpeed = speedBuffValue == 0 ? attackSpeed : attackSpeed * speedBuffValue;
            entry.TimeScale = Calculator.GetAnimationSpeed(totalAttacSpeed, duration);

            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

            for (int i = 0; i < targetCache.Count; i++)
            {
                ToxicProjectile projectile = GameManager.Instance.objectPoolManager.GetObject<ToxicProjectile>(projectileName);
                projectile.transform.localPosition = transform.localPosition;
                projectile.attackValue = totalDamage;
                projectile.criticalDamageRate = criticalDamageRate;
                projectile.isMove = true;
                projectile.provokedIndex = glacierIdx;

                projectile.Initialize(this, targetCache[i], criticalRange, projectileSpeed, particleName, toxicBoardTime, toxicBoardSize);

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

            CharacterAction = () =>
            {
                if (isSummoned && targetedMonster.Count > 0)
                {
                    StartCoroutine(ActionSequence());
                }
            };
            particleName = "ToxicHit";
            maxStarGradeValue = characterData.maxStarGradeValue;

            isSummoned = true;
            actionCoroutine = WaitActionSequence();
            StartCoroutine(actionCoroutine);

            bool isAppear = starGradeIndex == 0;

            AppearAction("Idle", isAppear);

            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

            //독 장판 지속시간 셋
            toxicBoardTime = characterData.characterUniqueValue[0] * starGradeIndex + characterData.starFactor[0];
            //독 장판 크기 셋 (이유 : 크기로 몬스터와의 거리를 확인해서 히트를 줘야하기 때문)
            toxicBoardSize = characterData.characterUniqueValue[1];
        }

        public override void SynthesisCharacter()
        {
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;

            SetAttackDamage();

            //아직 아무것도 공식값을 적용할게 없기에 디폴트로 둠
        }

      

    
    }


}
