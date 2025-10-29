using UnityEngine;
using Framework.GameData.Defense;
using CodeStage.AntiCheat.ObscuredTypes;
using Spine.Unity;
using System.Collections;
using Spine;
using Framework.Util;
using System.Linq;

namespace Framework.Game.Defense
{
    public class SottyBossMonster : Monster
    {
        public ObscuredFloat abillityTimeCount;
        public ObscuredInt targetCount;
        public ObscuredInt coefficient;
        public ObscuredFloat skillInterval;

        public IEnumerator abilitySequence;

        public override void FieldBossInitialize(BossData bossData)
        {
            monsterType = MonsterType.FIELD_BOSS_MONSTER;
            Debug.Log("Sotty");
            isBoss = true;
            transform.name = "Sotty";
            speed = bossData.monsterSpeed;
            skillInterval = bossData.uniqueValue[0];
            coefficient = (int)bossData.uniqueValue[2];
            health = bossData.health + GameManager.Instance.tempBossAddHealth + (waveIndex - 1) / 5 * bossData.healthFactor;
            targetCount = (int)bossData.uniqueValue[1] + (GameManager.Instance.waveIdx / coefficient);
            SetBossMove();
        }

        public IEnumerator SottyAbillityAction()
        {
            while (IsAlive)
            {
                yield return new WaitForSeconds(skillInterval);
                IsBossAttack = true;
                TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack", false);
                IsMove = false;
                yield return new WaitForSpineEvent(anim.AnimationState, "Attack");
                ConversionCharacter();
                yield return new WaitForSeconds(0.45f);
                Debug.Log("Sotty Attack End");
                IsMove = true;
                IsBossAttack = false;
                SetWalkSequence();
            }
        }

        public override void Abillity()
        {
            abilitySequence = SottyAbillityAction();
            StartCoroutine(abilitySequence);
        }

        public void ConversionCharacter()
        {
            int tempTargetCount = targetCount > GameManager.Instance.characterSpawner.summonedCharacters.Count
            ? GameManager.Instance.characterSpawner.summonedCharacters.Count : targetCount;

            int listCount = GameManager.Instance.characterSpawner.summonedCharacters.Count;
            int[] targets = Calculator.GetMultiIndex(listCount, listCount);

            int selectedCount = 0;

            int[] tempSlot = new int[5];
            for (int j = 0; j < tempSlot.Length; j++)
            {
                tempSlot[j] = UserSlotManager.Instance.GetSlotFocusIndexData().slotCharacterIds[j];
            }

            for (int i = 0; i < targets.Length; i++)
            {
                Debug.Log($"Sotty Target : {GameManager.Instance.characterSpawner.summonedCharacters[targets[i]].transform.name}");
                Character character = GameManager.Instance.characterSpawner.summonedCharacters[targets[i]];

                int characterIdx = (int)character.characterIndex;

                while (true)
                {
                    int temp = Random.Range(0, tempSlot.Length);
                    if (tempSlot[temp] != characterIdx)
                    {
                        ObjectParticle objectParticle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("BossEf_Soti");
                        // objectParticle.transform.position = character.transform.localPosition;
                        objectParticle.PlayParticle(character.transform.localPosition, 0, () =>
                        {
                            GameManager.Instance.objectPoolManager.ReturnObject(objectParticle, objectParticle.particleName);
                        });
                        character.DestroyedTile();
                        Glacier glacier = GridManager.Instance.glaciersTiles[character.glacierIdx];
                        GameManager.Instance.characterSpawner
                            .SummonFixedCharacter((CharacterIndex)tempSlot[temp], glacier, character.starGradeIndex);
                        selectedCount++;
                        break;
                    }
                }

                if (selectedCount >= tempTargetCount)
                {
                    break;
                }
            }
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

            monsterInterface.transform.SetParent(transform);
            monsterInterface.ReturnObjectPool();
            IsAlive = false;
            GameManager.Instance.monsterSpawner.RemoveMonster(this);
            GameManager.Instance.objectPoolManager.ReturnObject(this, "Boss_3");
        }
    }
}