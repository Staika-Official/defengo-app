using UnityEngine;
using Framework.GameData.Defense;
using CodeStage.AntiCheat.ObscuredTypes;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;

/*
BossEf_Bomb1
BossEf_Bomb2
BossEf_BombExplosion
BossEf_BombExplosion1
BossEf_BombExplosion2
*/

namespace Framework.Game.Defense
{
    public class BoomberBossMonster : Monster
    {
        public ObscuredFloat abillityTimeCount;
        public ObscuredInt targetCount;
        public ObscuredInt coefficient;
        public List<Character> bombCharacterList = new();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                StartCoroutine(BoomberAbillityAction());
            }
        }
        public override void FieldBossInitialize(BossData bossData)
        {
            targetCount = 2;
            monsterType = MonsterType.FIELD_BOSS_MONSTER;
            Debug.Log("Boomber");
            isBoss = true;
            transform.name = "Boomber";
            speed = bossData.monsterSpeed;
            health = bossData.health + GameManager.Instance.tempBossAddHealth;
            SetBossMove();
        }

        public IEnumerator BoomberAbillityAction()
        {
            bool isReinforceAttack = Random.Range(0, 2) == 0;

            string animKey = isReinforceAttack ? "Attack2" : "Attack";

            IsBossAttack = true;
            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_Ready", false);
            IsMove = false;
            Debug.Log("AttackReady");
            yield return new WaitForSpineAnimationComplete(entry);

            entry = anim.AnimationState.SetAnimation(0, "Attack", false);
            
            SetBomb();
            Debug.Log(animKey);
            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");
            ObjectParticle objectParticle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("BossEf_BombExplosion");
            objectParticle.transform.position = transform.position;
            objectParticle.SimplePlay();
            Debug.Log("Explosion");
            yield return new WaitForSpineAnimationComplete(entry);
            Debug.Log("Boomber Attack End");
            IsMove = true;
            IsBossAttack = false;
            bombCharacterList.Clear();
            SetWalkSequence();
        }

        public void SetBomb()
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
                bombCharacterList.Add(character);
                selectedCount++;
                character.SetBomb();
                if (selectedCount >= tempTargetCount)
                {
                    break;
                }
            }
        }

        public void SetBombExplosion()
        {
            foreach (var item in bombCharacterList)
            {
                item.BombExplosion();
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

            monsterInterface.transform.SetParent(transform);
            monsterInterface.ReturnObjectPool();
            IsAlive = false;
            GameManager.Instance.monsterSpawner.RemoveMonster(this);
            GameManager.Instance.objectPoolManager.ReturnObject(this, "Boss_6");
        }
    }
}
