using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

namespace Framework.Game.Defense
{
    public class CharacterLulu : Character
    {
        public float bubbleShieldTime;

        public Dictionary<Character, ShieldBubble> dic_CharacterShield = new();

        public IEnumerator FirstActionSequence()
        {
            //처음 생성됐을 때 바로 방어막을 쏴주기 위한 코루틴 함수
            anim.AnimationState.SetAnimation(0, "Idle", true);
            //첫 등장했을 때 비눗방울 생성해주기위함
            CharacterSpawner spawner = GameManager.Instance.characterSpawner;
            Character obj = spawner.GetNonShieldHighestStarGradeRandomCharacter();

            if(obj == null)
            {
                StartCoroutine(ActionSequence());
                yield break;
            }

            FlipCharacter(obj.transform.localPosition.x);

            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);

            //애니메이션 이벤트 Attack이 발생되면 넘어감
            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

            //비눗방울 이펙트를 생성
            ShieldAction(obj);

            //애니메이션이 끝날때까지 대기
            yield return new WaitForSpineAnimationComplete(entry);

            //애니메이션이 끝난다면 아이들 상태
            anim.AnimationState.SetAnimation(0, "Idle", true);

            StartCoroutine(ActionSequence());
        }

        public override IEnumerator ActionSequence()
        {
            while (isSummoned)
            {
                //첫 소환부터 캐릭터가 스킬을 쓰면 안되기에 빙판세우는 주기만큼 대기
                yield return new WaitForSeconds(bubbleShieldTime);

                CharacterSpawner spawner = GameManager.Instance.characterSpawner;
                Character obj = spawner.GetNonShieldHighestStarGradeRandomCharacter();

                if (obj == null)
                {
                    anim.AnimationState.SetAnimation(0, "Idle", true);
                    continue;
                }

                FlipCharacter(obj.transform.localPosition.x);

                TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);

                //애니메이션 이벤트 Attack이 발생되면 넘어감
                yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

                //비눗방울 이펙트를 생성
                ShieldAction(obj);

                //애니메이션이 끝날때까지 대기
                yield return new WaitForSpineAnimationComplete(entry);

                //애니메이션이 끝난다면 아이들 상태
                anim.AnimationState.SetAnimation(0, "Idle", true);
            }
        }

        public void ShieldAction(Character character)
        {
            //등장하거나 일정 시간마다
            //오브젝트 풀에서 비눗발울 생성 및 초기화 해주기
            if (character != null && !character.isShield)
            {
                character.ShieldSequence();
            }
        }

        public override void SetCharacterInfo()
        {
            bubbleShieldTime = characterData.characterUniqueValue[0];
            //bubbleShieldTime = 20;

            projectileName = characterData.projectileName;

            particleName = "ShieldBubbleBurst";

            isSummoned = true;

            StartCoroutine(FirstActionSequence());
        }


        public override void SynthesisCharacter()
        {
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
        }

  

    }
}
