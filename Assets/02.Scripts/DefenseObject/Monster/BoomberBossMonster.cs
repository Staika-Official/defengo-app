using UnityEngine;
using Framework.GameData.Defense;
using CodeStage.AntiCheat.ObscuredTypes;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using Framework.Util;

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
        public ObscuredInt targetCount;
        public ObscuredFloat skillInterval;
        public ObscuredFloat skillActiveDelay;
        // public ObscuredFloat reinforceAttackChance;
        // public ObscuredInt coefficient;
        public IEnumerator abilitySequence;
        public List<ObjectParticle> objectParticles = new();

        public override void FieldBossInitialize(BossData bossData)
        {
            monsterType = MonsterType.FIELD_BOSS_MONSTER;
            Debug.Log("Boomber");
            isBoss = true;
            transform.name = "Boomber";
            skillInterval = bossData.uniqueValue[0];
            targetCount = (int)bossData.uniqueValue[1] + (int)(waveIndex / bossData.uniqueValue[2]);
            speed = bossData.monsterSpeed;
            health = bossData.health + GameManager.Instance.tempBossAddHealth;
            skillActiveDelay = bossData.uniqueValue[5];
            // reinforceAttackChance = (bossData.uniqueValue[3] + waveIndex / 5 * bossData.uniqueValue[4]) * 100;
            objectParticles = new();
            SetBossMove();
        }

        public IEnumerator BoomberAbillityAction()
        {
            while (IsAlive)
            {
                yield return new WaitForSeconds(skillInterval);

                IsBossAttack = true;
                TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_Ready", false);
                IsMove = false;
                Debug.Log("Boomber Attack Ready");
                yield return new WaitForSeconds(anim.Skeleton.Data.FindAnimation("Attack_Ready").Duration);

                entry = anim.AnimationState.SetAnimation(0, "Attack", false);

                yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

                SetBomb();

                yield return new WaitForSeconds(anim.Skeleton.Data.FindAnimation("Attack").Duration);
                Debug.Log("Boomber Attack End");
                IsMove = true;
                IsBossAttack = false;
                SetWalkSequence();
            }
        }

        public void SetBomb()
        {
            int tempTargetCount = targetCount > GameManager.Instance.characterSpawner.summonedCharacters.Count
           ? GameManager.Instance.characterSpawner.summonedCharacters.Count : targetCount;

            int[] summonCharacterIdxs = Calculator.GetMultiIndex(GameManager.Instance.characterSpawner.summonedCharacters.Count, tempTargetCount);

            int selectedCount = 0;

            for (int i = 0; i < summonCharacterIdxs.Length; i++)
            {
                Character character = GameManager.Instance.characterSpawner.summonedCharacters[summonCharacterIdxs[i]];
                SetBombSequence(character);
                selectedCount++;
                if (selectedCount >= tempTargetCount)
                {
                    break;
                }
            }
        }

        public void SetBombSequence(Character character)
        {
            ObjectParticle bomb = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("BossEf_Bomb");
            bomb.transform.SetParent(character.transform);
            bomb.transform.localPosition = Vector2.zero;
            objectParticles.Add(bomb);
            bomb.PlayParticle(Vector2.zero, skillActiveDelay, () =>
            {
                ObjectParticle explosion = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("BossEf_BombExplosion");
                explosion.SimplePlay(character.transform);
                objectParticles.Add(explosion);

                character.DestroyedTile();
                Glacier glacier = GridManager.Instance.glaciersTiles[character.glacierIdx];
                if (character.starGradeIndex > 0)
                {
                    GameManager.Instance.characterSpawner
                        .SummonFixedCharacter(character.characterIndex, glacier, character.starGradeIndex - 1);
                }
            });
        }

        public override void Abillity()
        {
            abilitySequence = BoomberAbillityAction();
            StartCoroutine(abilitySequence);
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

            for (int i = 0; i < objectParticles.Count; i++)
            {
                GameManager.Instance.objectPoolManager.ReturnObject(objectParticles[i], objectParticles[i].particleName);
            }

            objectParticles.Clear();

            monsterInterface.transform.SetParent(transform);
            monsterInterface.ReturnObjectPool();
            IsAlive = false;
            GameManager.Instance.monsterSpawner.RemoveMonster(this);
            GameManager.Instance.objectPoolManager.ReturnObject(this, "Boss_6");
        }
    }
}
