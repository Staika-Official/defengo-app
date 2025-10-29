using UnityEngine;
using Framework.GameData.Defense;
using CodeStage.AntiCheat.ObscuredTypes;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Newtonsoft.Json;
using Framework.Util;

namespace Framework.Game.Defense
{
    public class ParasiteBossMonster : Monster
    {

        public ObscuredInt targetCount;
        public ObscuredFloat skillInterval;

        public IEnumerator abilitySequence;

        public override void FieldBossInitialize(BossData bossData)
        {
            monsterType = MonsterType.FIELD_BOSS_MONSTER;
            Debug.Log("Parasite");
            isBoss = true;
            transform.name = "Parasite";
            speed = bossData.monsterSpeed;
            skillInterval = bossData.uniqueValue[0];
            targetCount = (int)bossData.uniqueValue[1] + (int)(waveIndex / bossData.uniqueValue[2]);
            health = bossData.health + GameManager.Instance.tempBossAddHealth + (waveIndex - 1) / 5 * bossData.healthFactor;

            SetBossMove();
        }

        public IEnumerator ParasiteAbillityAction()
        {
            while (IsAlive)
            {
                yield return new WaitForSeconds(skillInterval);
                IsBossAttack = true;
                TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack", false);
                IsMove = false;
                yield return new WaitForSpineEvent(anim.AnimationState, "Attack");
                InfectCharacter();
                yield return new WaitForSeconds(0.4f);
                Debug.Log("Parasite Attack End");
                IsMove = true;
                IsBossAttack = false;
                SetWalkSequence();
            }
        }

        public void InfectCharacter()
        {
            Debug.Log($"{gameObject.name} infect character start");
            int tempTargetCount = targetCount > GameManager.Instance.characterSpawner.summonedCharacters.Count
               ? GameManager.Instance.characterSpawner.summonedCharacters.Count : targetCount;
            Debug.Log($"{gameObject.name} infect character target count {tempTargetCount}");

            int listCount = GameManager.Instance.characterSpawner.summonedCharacters.Count;
            int[] targets = Calculator.GetMultiIndex(listCount, listCount);

            int selectedCount = 0;

            for (int i = 0; i < targets.Length; i++)
            {
                Character character = GameManager.Instance.characterSpawner.summonedCharacters[targets[i]];

                if (character.IsInfected || !character.isAttackType)
                {
                    continue;
                }
                else
                {
                    InfectSequence(character);
                    selectedCount++;

                    if (selectedCount >= tempTargetCount)
                    {
                        break;
                    }
                }
            }
        }

        public void InfectSequence(Character character)
        {
            Debug.Log($"{gameObject.name} infect character {character.name}");
            character.Infect();
        }

        public override void Abillity()
        {
            abilitySequence = ParasiteAbillityAction();
            StartCoroutine(abilitySequence);
        }

        public override void DeathSequence()
        {
            StopCoroutine(abilitySequence);
        }

        public override void EndOfUse()
        {
            if (null != deadTimerCoroutine)
            {
                StopCoroutine(deadTimerCoroutine);
                deadTimerCoroutine = null;
            }

            for (int i = 0; i < GameManager.Instance.characterSpawner.summonedCharacters.Count; i++)
            {
                GameManager.Instance.characterSpawner.summonedCharacters[i].ReleaseInfect();
            }

            monsterInterface.transform.SetParent(transform);
            monsterInterface.ReturnObjectPool();
            IsAlive = false;
            GameManager.Instance.monsterSpawner.RemoveMonster(this);
            GameManager.Instance.objectPoolManager.ReturnObject(this, "Boss_5");
        }
    }
}
