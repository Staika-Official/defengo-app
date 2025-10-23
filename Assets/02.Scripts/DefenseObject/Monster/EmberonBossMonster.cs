using UnityEngine;
using Framework.GameData.Defense;
using CodeStage.AntiCheat.ObscuredTypes;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using Framework.Util;

namespace Framework.Game.Defense
{
    public class EmberonBossMonster : Monster
    {
        public ObscuredInt targetCount;
        public ObscuredFloat skillInterval;
        public ObscuredFloat skillActiveDelay;
        public IEnumerator abilitySequence;
        public List<ObjectParticle> objectParticles = new();

        public override void FieldBossInitialize(BossData bossData)
        {
            monsterType = MonsterType.FIELD_BOSS_MONSTER;
            Debug.Log("Emberon");
            isBoss = true;
            transform.name = "Emberon";
            skillInterval = bossData.uniqueValue[0];
            targetCount = (int)bossData.uniqueValue[1] + (int)(waveIndex / bossData.uniqueValue[2]);
            speed = bossData.monsterSpeed;
            health = bossData.health + GameManager.Instance.tempBossAddHealth;
            skillActiveDelay = bossData.uniqueValue[3];
            // reinforceAttackChance = (bossData.uniqueValue[3] + waveIndex / 5 * bossData.uniqueValue[4]) * 100;
            objectParticles = new();
            SetBossMove();
        }
        public IEnumerator EmberonAbillityAction()
        {
            while (IsAlive)
            {
                yield return new WaitForSeconds(skillInterval);

                Debug.Log("Emberon Attack Ready");

                IsBossAttack = true;
                IsMove = false;
                SetFireball();
                TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_Start", false);
                entry.Loop = false;

                yield return new WaitForSeconds(anim.Skeleton.Data.FindAnimation("Attack_Start").Duration);

                entry = anim.AnimationState.SetAnimation(0, "Attack_Idle", false);
                entry.Loop = false;

                // yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

                yield return new WaitForSeconds(anim.Skeleton.Data.FindAnimation("Attack_Start").Duration);

                Debug.Log("Emberon Attack End");

                IsMove = true;
                IsBossAttack = false;
                SetWalkSequence();
            }
        }

        public void SetFireball()
        {
            int tempTargetCount = targetCount > GameManager.Instance.characterSpawner.summonedCharacters.Count
           ? GameManager.Instance.characterSpawner.summonedCharacters.Count : targetCount;

            int[] summonCharacterIdxs = Calculator.GetMultiIndex(GameManager.Instance.characterSpawner.summonedCharacters.Count, tempTargetCount);

            int selectedCount = 0;

            for (int i = 0; i < summonCharacterIdxs.Length; i++)
            {
                Character character = GameManager.Instance.characterSpawner.summonedCharacters[summonCharacterIdxs[i]];
                SetFireballSequence(character);
                selectedCount++;
                if (selectedCount >= tempTargetCount)
                {
                    break;
                }
            }
        }

        public void SetFireballSequence(Character character)
        {
            ObjectParticle bomb = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("BossEf_FireBall");
            bomb.transform.SetParent(character.transform);
            bomb.transform.localPosition = Vector2.zero;
            objectParticles.Add(bomb);

            var bomAnim = bomb.GetComponent<Animator>();
            AnimatorClipInfo[] clipInfo = bomAnim.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                AnimationClip clip = clipInfo[0].clip;
                float originalDuration = clip.length;

                // Calculate speed to make it exactly 2 seconds
                float targetDuration = skillActiveDelay;
                float newSpeed = originalDuration / targetDuration;

                bomAnim.speed = newSpeed;
            }

            bomb.PlayParticle(Vector2.zero, skillActiveDelay, () =>
            {
                ObjectParticle explosion = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("BossEf_FireExplosion");
                explosion.SimplePlay(character.transform);
                objectParticles.Add(explosion);

                character.DestroyedTile();
                Glacier glacier = GridManager.Instance.glaciersTiles[character.glacierIdx];
                glacier.Damage();
            });
        }

        public override void Abillity()
        {
            abilitySequence = EmberonAbillityAction();
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
            GameManager.Instance.objectPoolManager.ReturnObject(this, "Boss_7");
        }
    }
}