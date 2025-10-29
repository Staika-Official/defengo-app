using UnityEngine;
using Framework.GameData.Defense;
using CodeStage.AntiCheat.ObscuredTypes;
using Spine.Unity;
using System.Collections;
using Framework.Util;
using Spine;
using System.Collections.Generic;

namespace Framework.Game.Defense
{
    public class LockyBossMonster : Monster
    {
        public ObscuredInt targetCount;
        public List<Character> lockdownCharacters = new();
        public List<ObjectParticle> objectParticles = new();
        public ObscuredFloat skillInterval;

        public IEnumerator abilitySequence;

        public override void FieldBossInitialize(BossData bossData)
        {
            monsterType = MonsterType.FIELD_BOSS_MONSTER;
            Debug.Log("Locky");
            isBoss = true;
            transform.name = "Locky";
            skillInterval = bossData.uniqueValue[0];
            speed = bossData.monsterSpeed;
            targetCount = (int)bossData.uniqueValue[1] + (int)(GameManager.Instance.waveIdx / bossData.uniqueValue[2]);
            health = bossData.health + GameManager.Instance.tempBossAddHealth + (waveIndex - 1) / 5 * bossData.healthFactor;
            objectParticles = new();
            SetBossMove();
        }


        public override void Abillity()
        {
            abilitySequence = LockyAbillityAction();
            StartCoroutine(abilitySequence);
        }

        public override void DeathSequence()
        {
            StopCoroutine(abilitySequence);
        }

        public IEnumerator LockyAbillityAction()
        {
            while (IsAlive)
            {
                yield return new WaitForSeconds(skillInterval);
                IsBossAttack = true;
                TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack", false);
                IsMove = false;
                yield return new WaitForSpineEvent(anim.AnimationState, "Attack");
                LockCharacter();
                yield return new WaitForSeconds(0.3f);
                Debug.Log("Locky Attack End");
                IsMove = true;
                IsBossAttack = false;
                SetWalkSequence();
            }
        }

        public void LockCharacter()
        {
            int tempTargetCount = targetCount > GameManager.Instance.characterSpawner.summonedCharacters.Count
                ? GameManager.Instance.characterSpawner.summonedCharacters.Count : targetCount;

            int listCount = GameManager.Instance.characterSpawner.summonedCharacters.Count;
            int[] summonCharacterIdxs = Calculator.GetMultiIndex(listCount, listCount);

            int selectedCount = 0;

            for (int i = 0; i < summonCharacterIdxs.Length; i++)
            {
                Character character = GameManager.Instance.characterSpawner.summonedCharacters[summonCharacterIdxs[i]];

                if (character.IsLockdown)
                {
                    continue;
                }
                else
                {
                    lockdownCharacters.Add(character);
                    LockdownSequence(character);
                    selectedCount++;

                    if (selectedCount >= tempTargetCount)
                    {
                        break;
                    }
                }
            }
        }

        public void LockdownSequence(Character character)
        {
            character.Lockdown();
            ObjectParticle lockStart = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("BossEf_LockStart");
            lockStart.transform.SetParent(character.transform);
            lockStart.transform.localPosition = Vector2.zero;
            lockStart.PlayParticle(Vector2.zero, 0.5f, () =>
            {
                if (!IsAlive)
                    return;
                Debug.Log("after particle");
                ObjectParticle lockIdle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("BossEf_LockIdle");
                objectParticles.Add(lockIdle);
                lockIdle.SimplePlay();
                lockIdle.transform.SetParent(character.transform);
                lockIdle.transform.localPosition = Vector2.zero;
            });
        }

        public override void EndOfUse()
        {
            if (null != deadTimerCoroutine)
            {
                StopCoroutine(deadTimerCoroutine);
                deadTimerCoroutine = null;
            }

            for (int i = 0; i < objectParticles.Count; i++)
            {
                GameManager.Instance.objectPoolManager.ReturnObject(objectParticles[i], objectParticles[i].particleName);
            }

            objectParticles.Clear();

            for (int i = 0; i < lockdownCharacters.Count; i++)
            {
                lockdownCharacters[i].ReleaseLockdown();
            }

            lockdownCharacters.Clear();

            monsterInterface.transform.SetParent(transform);
            monsterInterface.ReturnObjectPool();
            IsAlive = false;
            GameManager.Instance.monsterSpawner.RemoveMonster(this);
            GameManager.Instance.objectPoolManager.ReturnObject(this, "Boss_4");
        }
    }
}
