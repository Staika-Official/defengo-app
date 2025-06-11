using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Framework.GameData.Defense;
using Framework.UI;
using Framework.Util;
using Framework.Sound;

namespace Framework.Game.Defense
{
    public class CharacterBard : Character
    {
        public float attackSpeedBuffValue;
        public List<Character> buffCharacters = new();

        private void Start()
        {
            GameManager.Instance.characterSpawner.characterCount
                .Subscribe(_ =>
                {
                    if (gameObject.activeSelf)
                    {
                        StartCoroutine(RepositionBuff());
                    }
                });
        }

        public override IEnumerator ActionSequence()
        {
            yield return null;
        }

        public override void SetCharacterInfo()
        {
            isAttackType = false;

            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);

            attackSpeedBuffValue = 1 + (characterData.characterUniqueValue[0]
                + (characterData.characterClassLevel - 1)
                * characterData.classUpFactor[0])
                * Mathf.Pow(ConfigData.STARGRADE_FACTOR - characterData.starFactor[0], starGradeIndex)
                + (upgradeIndex - 1) * characterData.powerFactor[0];

            endOfUse = () =>
            {
                for (int i = 0; i < buffCharacters.Count; i++)
                {
                    buffCharacters[i].EndSpeedBuff(this);
                }

                buffCharacters.Clear();
            };


            //swapAction
            //스왑 시 바드가 버프주고있던 캐릭터를 순회하면서 해당 캐릭터에 버프주고있던 본인(바드)를
            //지워준이후 다시 셋팅 하는 부분
            //콜백 함수 의도 : 스왑 시 버프받는캐릭터와 안받는캐릭터가 자리교체가 되면
            //버프량과 파티클을 스왑해줘야하기때문
            swapAction = () =>
            {
                for (int i = 0; i < buffCharacters.Count; i++)
                {
                    buffCharacters[i].EndSpeedBuff(this);
                }

                buffCharacters.Clear();

                StartCoroutine(RepositionBuff());
            };

            maxStarGradeValue = characterData.maxStarGradeValue;

            isSummoned = true;

            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

            bool isAppear = starGradeIndex == 0;

            AppearAction("Idle", isAppear);
            SoundManager.Instance.PlaySound(SoundKey.SF_BARD_SOFT);
        }

        public void SetSpeedBuffCharacter()
        {
            if (!isSummoned) return;
            Vector2 pos = Calculator.TransformationVector(constructibleIdx);

            //현재 소환된 캐릭터 순회
            for (int i = 0; i < GameManager.Instance.characterSpawner.summonedCharacters.Count; i++)
            {
                Character character = GameManager.Instance.characterSpawner.summonedCharacters[i];
                float distance = Calculator.DistanceCheck(pos - character.currentGridPos);

                if (distance <= 16 && character.isAttackType && character.attackSpeed != 0)
                {
                    //버프주고있는 캐릭터가 있다면! 조건 성립해서 continue!
                    if (character.dic_speedBuffValue.ContainsKey(this)) continue;

                    character.SetSpeedBuff(this, attackSpeedBuffValue, skinID);
                    buffCharacters.Add(character);
                }
            }
        }

        public override void SynthesisCharacter()
        {

        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;
            attackSpeedBuffValue = 1 + (characterData.characterUniqueValue[0]
                + (characterData.characterClassLevel - 1)
                * characterData.classUpFactor[0])
                * Mathf.Pow(ConfigData.STARGRADE_FACTOR - characterData.starFactor[0], starGradeIndex)
                + (upgradeIndex - 1) * characterData.powerFactor[0];

            for (int i = 0; i < buffCharacters.Count; i++)
            {
                buffCharacters[i].dic_speedBuffValue[this] = attackSpeedBuffValue;
                buffCharacters[i].speedBuffValue = attackSpeedBuffValue;
            }
        }

        public override void RemoveBuffCharacter(Character character)
        {
            //Debug.Log("Remove Speed Buff Character");
            buffCharacters.Remove(character);
        }

        public override void Reposition(Glacier glacier)
        {
            base.Reposition(glacier);

            for (int i = 0; i < buffCharacters.Count; i++)
            {
                buffCharacters[i].EndSpeedBuff(this);
            }

            buffCharacters.Clear();

            StartCoroutine(RepositionBuff());
        }

        public IEnumerator RepositionBuff()
        {
            yield return new WaitForEndOfFrame();
            SetSpeedBuffCharacter();
        }
    }
}
