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
    public class CharacterWhiteMagicianFoxy : Character
    {
        public Transform electric;
        public int totalTransferCount;
        public List<Monster> electricMonsters = new();
        public float totalTransferDamage;

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => isSummoned && isAttackType)
                .Subscribe(_ => GetPossibleAttackMonster());

            electric.gameObject.SetActive(false);
        }

        public Monster GetHighestHealthMonster(List<Monster> targetCache)
        {
            int temp = 0;
            int idx = 0;

            for (int i = 0; i < targetCache.Count; i++)
            {
                if(targetCache[i].indexInWave > temp)
                {
                    temp = targetCache[i].indexInWave;
                    idx = i;
                }
            }
            return targetCache[idx];
        }

        public void GetNearMonsters(Monster monster, List<Monster> targetCache)
        {
            electricMonsters.Add(monster);
            monster.ElectricSequence(true);
            for (int i = 0; i < targetCache.Count; i++)
            {
                Monster tempMonster = targetCache[i];
                if (tempMonster == monster) continue;
                Vector2 tarMonsterVector = Calculator.TransformationVector(monster.prevPositionIdx);
                Vector2 monsterVector = Calculator.TransformationVector(tempMonster.prevPositionIdx);

                float dis = Calculator.DistanceCheck(tarMonsterVector - monsterVector);
                if (dis <= strikeRange)
                {
                    if (electricMonsters.Count > totalTransferCount)
                    {
                        break;
                    }
                    tempMonster.ElectricSequence(true);
                    electricMonsters.Add(tempMonster);
                }
            }
            StartCoroutine(HitDamage(monster));
        }

        public IEnumerator HitDamage(Monster monster)
        {
            while (true)
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_ELECTRONIC_BASIC);
                yield return new WaitForSeconds(0.5f);

                for (int i = 0; i < electricMonsters.Count; i++)
                {
                    if (electricMonsters[i] == monster)
                    {
                        electricMonsters[i].ElectricDamage(totalDamage, this);
                    }
                    else
                    {
                        electricMonsters[i].ElectricDamage(totalDamage * totalTransferDamage, this);
                    }

                }

                if (characterState != CharacterState.DURING_ATTACK) break;
            }
        }

        public override IEnumerator ActionSequence()
        {
            characterState = CharacterState.DURING_ATTACK;

            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);

            List<Monster> targetCache = targetedMonster;

            Monster targetMonster = GetHighestHealthMonster(targetCache);

            yield return new WaitForSpineAnimationComplete(entry);
            FlipCharacter(targetMonster.transform.localPosition.x);
            _ = anim.AnimationState.SetAnimation(0, "Attack_Ing", true);

            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");
           
            electric.gameObject.SetActive(true);

            GetNearMonsters(targetMonster, targetCache);

            while (true)
            {
                if (targetCache.Count == 0 || !targetMonster.IsAlive)
                {
                    for (int i = 0; i < electricMonsters.Count; i++)
                    {
                        electricMonsters[i].ElectricSequence(false);
                    }

                    electricMonsters.Clear();

                    electric.gameObject.SetActive(false);
                    break;
                }

                FlipCharacter(targetMonster.transform.localPosition.x);

                Vector3 targetPos = targetMonster.transform.position;
                Vector3 moveDir = targetPos - electric.position;

                float rotationZ = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                electric.rotation = Quaternion.AngleAxis(rotationZ + 90.0f, Vector3.forward);
                float distance = Vector2.Distance(targetPos, electric.position);
                electric.localScale = new Vector2(1, distance);

                yield return null;
            }

            entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide_Back", false);
            yield return new WaitForSpineAnimationComplete(entry);
            _ = anim.AnimationState.SetAnimation(0, "Idle", true);
            //Debug.Log("End White Attack Seq");
            characterState = CharacterState.DETECT;
        }

        public override void SetCharacterInfo()
        {
            characterState = CharacterState.DETECT;
            electric.gameObject.SetActive(false);
            endOfUse = () =>
            {
                for (int i = 0; i < electricMonsters.Count; i++)
                {
                    electricMonsters[i].ElectricSequence(false);
                }

                electricMonsters.Clear();
            };

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

            float tempSpreadCount = characterData.characterUniqueValue[1] * Mathf.Pow(characterData.starFactor[0], starGradeIndex);

            totalTransferCount = (int)tempSpreadCount;
            totalTransferDamage = characterData.characterUniqueValue[0] + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1]
                + (upgradeIndex - 1) * characterData.powerFactor[1];
            targetCount = (int)tempSpreadCount;
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
            
            CharacterAction = () =>
            {
                if (isSummoned && targetedMonster.Count > 0 && characterState != CharacterState.DURING_ATTACK)
                {
                    //Debug.Log("White Foxy Character Action");
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

            bool isAppear = starGradeIndex == 0;

            AppearAction("Idle", isAppear);
        }

        public override void SynthesisCharacter()
        {

        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;
            SetAttackDamage();
            totalTransferDamage = characterData.characterUniqueValue[0] + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1]
                + (upgradeIndex - 1) * characterData.powerFactor[1];
        }
    }
}