using UnityEngine;
using Framework.GameData.Defense;
using CodeStage.AntiCheat.ObscuredTypes;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Spine;

namespace Framework.Game.Defense
{
    public class ParasiteBossMonster : Monster
    {

        public ObscuredInt targetCount;
        public List<Character> infectedCharacters = new();
        //public List<ObjectParticle> objectParticles = new();
        public override void FieldBossInitialize(BossData bossData)
        {
            monsterType = MonsterType.FIELD_BOSS_MONSTER;
            Debug.Log("Parasite");
            isBoss = true;
            transform.name = "Parasite";
            speed = bossData.monsterSpeed;

            health = bossData.health + GameManager.Instance.tempBossAddHealth;
            SetBossMove();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                StartCoroutine(ParasiteAbillityAction());
            }
        }

        public IEnumerator ParasiteAbillityAction()
        {
            IsBossAttack = true;
            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack", false);
            IsMove = false;
            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");
            InfectCharacter();
            yield return new WaitForSpineAnimationComplete(entry);
            Debug.Log("Parasite Attack End");
            IsMove = true;
            IsBossAttack = false;
            SetWalkSequence();
        }

        public void InfectCharacter()
        {
            int tempTargetCount = targetCount > GameManager.Instance.characterSpawner.summonedCharacters.Count
               ? GameManager.Instance.characterSpawner.summonedCharacters.Count : targetCount;

            int[] summonCharacterIdxs = new int[GameManager.Instance.characterSpawner.summonedCharacters.Count];

            for (int i = 0; i < summonCharacterIdxs.Length; i++)
            {
                summonCharacterIdxs[i] = i;
            }

            for (int i = 0; i < summonCharacterIdxs.Length; ++i)
            {
                int random1 = Random.Range(0, summonCharacterIdxs.Length);
                int random2 = Random.Range(0, summonCharacterIdxs.Length);

                (summonCharacterIdxs[random1], summonCharacterIdxs[random2]) = (summonCharacterIdxs[random2], summonCharacterIdxs[random1]);
            }

            int selectedCount = 0;

            for (int i = 0; i < summonCharacterIdxs.Length; i++)
            {
                Character character = GameManager.Instance.characterSpawner.summonedCharacters[summonCharacterIdxs[i]];

                if (character.IsInfected)
                {
                    continue;
                }
                else
                {
                    infectedCharacters.Add(character);
                    character.IsInfected = true;
                    selectedCount++;
                    
                    if (selectedCount >= tempTargetCount)
                    {
                        break;
                    }
                }
            }
        }

        public override void Abillity()
        {

        }

        public override void DeathSequence()
        {

        }

        public override void EndOfUse()
        {
            if (null != deadTimerCoroutine)
            {
                StopCoroutine(deadTimerCoroutine);
                deadTimerCoroutine = null;
            }

            for (int i = 0; i < infectedCharacters.Count; i++)
            {
                infectedCharacters[i].ReleaseInfect();
            }

            infectedCharacters.Clear();

            monsterInterface.transform.SetParent(transform);
            monsterInterface.ReturnObjectPool();
            IsAlive = false;
            GameManager.Instance.monsterSpawner.RemoveMonster(this);
            GameManager.Instance.objectPoolManager.ReturnObject(this, "Boss_5");
        }
    }
}
