using UnityEngine;
using Framework.GameData.Defense;
using CodeStage.AntiCheat.ObscuredTypes;
using Spine.Unity;
using System.Collections;
using Spine;
using Framework.Util;

namespace Framework.Game.Defense
{
    public class SottyBossMonster : Monster
    {
        public ObscuredFloat abillityTimeCount;
        public ObscuredInt targetCount;
        public ObscuredInt coefficient;

        public override void FieldBossInitialize(BossData bossData)
        {
            monsterType = MonsterType.FIELD_BOSS_MONSTER;
            Debug.Log("Sotty");
            isBoss = true;
            transform.name = "Sotty";
            speed = bossData.monsterSpeed;
            coefficient = 30;

            health = bossData.health + GameManager.Instance.tempBossAddHealth;
            targetCount = 1 + (5 / coefficient);
            SetBossMove();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                StartCoroutine(SottyAbillityAction());
            }
        }

        public IEnumerator SottyAbillityAction()
        {
            IsBossAttack = true;
            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack", false);
            IsMove = false;
            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");
            ConversionCharacter();
            yield return new WaitForSpineAnimationComplete(entry);
            Debug.Log("Sotty Attack End");
            IsMove = true;
            IsBossAttack = false;
            SetWalkSequence();
        }

        public override void Abillity()
        {

        }

        public void ConversionCharacter()
        {
            int tempTargetCount = targetCount > GameManager.Instance.characterSpawner.summonedCharacters.Count
            ? GameManager.Instance.characterSpawner.summonedCharacters.Count : targetCount;

            int[] tempCharacterIdxs = new int[GameManager.Instance.characterSpawner.summonedCharacters.Count];

            int[] targets = Calculator.GetMultiIndex(GameManager.Instance.characterSpawner.summonedCharacters.Count, tempTargetCount);

            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = tempCharacterIdxs[i];
                Debug.Log($"Sotty Target : {GameManager.Instance.characterSpawner.summonedCharacters[targets[i]].transform.name}");
                Character character = GameManager.Instance.characterSpawner.summonedCharacters[targets[i]];
                int[] tempSlot = new int[5];

                for (int j = 0; j < tempSlot.Length; j++)
                {
                    tempSlot[j] = UserSlotManager.Instance.GetSlotFocusIndexData().slotCharacterIds[j];
                }

                int characterIdx = (int)character.characterIndex;

                while (true)
                {
                    int temp = Random.Range(0, tempSlot.Length);
                    if (tempSlot[temp] != characterIdx)
                    {
                        ObjectParticle objectParticle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("BossEf_Soti");
                        // objectParticle.transform.position = character.transform.localPosition;
                        objectParticle.PlayParticle(character.transform.localPosition, 0);
                        character.DestroyedTile();
                        Glacier glacier = GridManager.Instance.glaciersTiles[character.glacierIdx];
                        GameManager.Instance.characterSpawner
                            .SummonFixedCharacter((CharacterIndex)tempSlot[temp], glacier, character.starGradeIndex);
                        break;
                    }
                }
            }
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
            GameManager.Instance.objectPoolManager.ReturnObject(this, "Boss_3");
        }
    }
}