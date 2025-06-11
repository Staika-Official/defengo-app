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
    public class CharacterDarkWolf : Character
    {
        public float instantDeathRate;
        public float getGemRate;
        
        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType)
                .Subscribe(_ => GetPossibleAttackMonster());
        }

        public override IEnumerator ActionSequence()
        {
            FlipCharacter(targetedMonster[0].transform.localPosition.x);

            int temp = Random.Range(0, 1000);

            bool isInstantKill = (int)(instantDeathRate * 1000) >= temp;
            string animKey = isInstantKill ? "Attack_FrontSide_Kill" : "Attack_FrontSide";

            TrackEntry entry = anim.AnimationState.SetAnimation(0, animKey, false);

            List<Monster> targetCache = targetedMonster;
            float duration = entry.Animation.Duration;
            float tempSpeed = speedBuffValue == 0 ? attackSpeed : attackSpeed * speedBuffValue;

            Debug.Log($"다크울프 스피드 버프값 : {speedBuffValue}");
            
            Debug.Log($"다크울프 캐릭터 이름 : {gameObject.name}");
            
            foreach (var keyValuePair in dic_speedBuffValue)
            {
                Debug.Log($"버프주는 캐릭터 이름 {keyValuePair.Key.gameObject.name}");                
            }
            
            entry.TimeScale = Calculator.GetAnimationSpeed(tempSpeed, duration);
            if (isInstantKill)
            {
                yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

                for (int i = 0; i < targetCache.Count; i++)
                {
                    if (!targetCache[i].IsAlive) continue;
                    
                    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("DarkWolfKill");
                    particle.PlayParticle(targetCache[0].transform.position, 0);

                    targetCache[i].HitDamage(99000000, DamageType.INSTANT_KILL, 1);
                    int getGem = Random.Range(0, 100);
                    bool isGetGem = (int)(getGemRate * 100) >= getGem;

                    if(isGetGem)
                    {
                        GemAnimation gemAnimation = GameManager.Instance.objectPoolManager.GetObject<GemAnimation>("PlusLifeSton");
                        gemAnimation.transform.SetParent(UIManager.GetDynamicCanvasTransform());
                        gemAnimation.transform.localScale = Vector3.one;
                        gemAnimation.transform.position = targetCache[0].transform.localPosition;
                        gemAnimation.SetGemAnimation(1);
                        GameManager.Instance.ChangeGem(1);
                    }

                    SoundKey soundKey = isGetGem ? SoundKey.SF_ASSASIN_SUCCESS_DOUBLE : SoundKey.SF_ASSASIN_SUCCESS;
                    SoundManager.Instance.PlaySound(soundKey);
                }
            }
            else
            {
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
                    SoundManager.Instance.PlaySound(SoundKey.SF_SWORD_SHORT);
                    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("DarkWolfHit");
                    particle.PlayParticle(targetCache[0].transform.position, 0);
                    particle.transform.localScale = isLeft ? Vector3.one : new Vector3(-1, 1, 1);
                    targetCache[i].HitDamage(totalDamage, damageType, criticalDamageRate);
                }
            }

            yield return new WaitForSpineAnimationComplete(entry);
            entry.TimeScale = 1;
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

            //criticalRange = characterData.criticalRange + GameManager.Instance.buffManager.increaseCriticalRate;

            criticalRange = (characterData.criticalRange + (upgradeIndex - 1) * characterData.powerFactor[1])
                + GameManager.Instance.buffManager.increaseCriticalRate;

            criticalDamageRate = characterData.criticalDamageRate + (characterData.characterClassLevel - 1)
                * characterData.classUpFactor[1];

            instantDeathRate = characterData.characterUniqueValue[0]
                + characterData.starFactor[0]
                * starGradeIndex;

            getGemRate = characterData.characterUniqueValue[1]
                + characterData.classUpFactor[2]
                * starGradeIndex;

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
            criticalRange = (characterData.criticalRange + (upgradeIndex - 1) * characterData.powerFactor[1])
               + GameManager.Instance.buffManager.increaseCriticalRate;
            SetAttackDamage();
        }

    }
}
