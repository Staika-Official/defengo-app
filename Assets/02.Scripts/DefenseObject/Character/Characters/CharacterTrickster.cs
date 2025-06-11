using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Sound;
using Framework.Util;
using Spine;
using Spine.Unity;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Game.Defense
{
    public class CharacterTrickster : Character
    {
        public bool isDying;
        public bool isStun;
        public float glacierChangeStunTime;
        public float glacierChangeSuccessRate;

        public readonly string Trickster_effet_name = "EfTricksterDie";
        
        public IEnumerator actionSequence;

        void Start()
        {
            this.UpdateAsObservable()
             .Where(_ => isSummoned && isAttackType)
             .Subscribe(_ => GetPossibleAttackMonster());
        }

        public override IEnumerator ActionSequence()
        {
            FlipCharacter(targetedMonster[0].transform.localPosition.x);

            List<Monster> targetCache = targetedMonster;
            
            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);
            float duration = entry.Animation.Duration;
            float totalAttacSpeed = speedBuffValue == 0 ? attackSpeed : attackSpeed * speedBuffValue;
            entry.TimeScale = Calculator.GetAnimationSpeed(totalAttacSpeed, duration);

            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

            bool isCritical = false;
            //크리티컬 확률이 0 초과라면 크리티컬 데미지를 적용할수 있기에 조건넣어줌
            if (criticalRange > 0)
            {
                //크리티컬 확률
                int criticalRangeValue = (int)(criticalRange * 100);

                //크리티컬 확률 랜덤값
                int RandomcriticalValue = Random.Range(0, 100);

                //랜덤값보다 크리티컬확률이 크다면 크리티컬 데미지를 주기로 결정
                isCritical = RandomcriticalValue <= criticalRangeValue;
            }

            //타겟 갯수만큼 투사체 생성
            for (int i = 0; i < targetCache.Count; i++)
            {
                //포지션, 데미지, 크리티컬 데미지, 움직임, 
                CardProjectile projectile = GameManager.Instance.objectPoolManager.GetObject<CardProjectile>(projectileName);
                projectile.transform.localPosition = transform.localPosition;
                projectile.attackValue = totalDamage;
                projectile.criticalDamageRate = criticalDamageRate;
                projectile.isMove = true;

                //TODO :: 이건뭔지 잘모르겠습니다
                projectile.provokedIndex = glacierIdx;

                //투사체 초기화
                projectile.Initialize(this, targetCache[i], isCritical, projectileSpeed, strikeRange, particleName);
            }

            yield return new WaitForSpineAnimationComplete(entry);

            //예외처리 코루틴함수라 스턴이 되고나서 이 로직으로 들어올수있는 로직이라 예외처리해줘야함
            //if(!isStun)
            //{
            anim.AnimationState.SetAnimation(0, "Idle", true);
            //}
        }

        public override void SetCharacterInfo()
        {
            if (targetedMonster.Count != 0)
            {
                targetedMonster.Clear();
            }

            //액션이 진행중이라면 스탑
            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
            }

            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);
            CharacterAction = null;
            actionSequence = null;

            isAttackType = true;

            //각종 스탯 셋팅
            basicAttackDamage = characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0];
            targetCount = (int)characterData.targetCount;
            attackSpeed = characterData.attackSpeed;
            criticalRange = characterData.criticalRange + GameManager.Instance.buffManager.increaseCriticalRate;
            criticalDamageRate = characterData.criticalDamageRate;
            upgradeFactor = characterData.powerFactor[0];
            projectileSpeed = characterData.projectileSpeed;
            attackRange = Mathf.Pow(characterData.detectRange, 2);
            strikeRange = Mathf.Pow(characterData.strikeRange, 2);
            isThrow = characterData.isThrow;
            buffDuration = characterData.buffDuration;
            buffValue = characterData.buffValue;
            SetAttackDamage();
            projectileName = characterData.projectileName;

            CharacterAction = () =>
            {
                if (isSummoned && targetedMonster.Count > 0 && !isStun)
                {
                    actionSequence = ActionSequence();
                    StartCoroutine(actionSequence);

                    //StartCoroutine(ActionSequence());
                }
            };

            particleName = "CardHit";
            maxStarGradeValue = characterData.maxStarGradeValue;

            isSummoned = true;
            isStun = false;
            isDying = false;
            actionCoroutine = WaitActionSequence();


            StartCoroutine(actionCoroutine);

            bool isAppear = starGradeIndex == 0;

            AppearAction("Idle", isAppear);

            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

            //스턴 시간 공식값 처리
            glacierChangeStunTime = characterData.characterUniqueValue[0]
               - (characterData.characterClassLevel - 1)
               * characterData.classUpFactor[1];

            //생존 확률 공식값 처리
            glacierChangeSuccessRate = characterData.characterUniqueValue[1]
               + (characterData.characterClassLevel - 1)
               * characterData.classUpFactor[2];

        }

        public override void SynthesisCharacter()
        {
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;

            SetAttackDamage();

            //합성 시 
            glacierChangeStunTime = characterData.characterUniqueValue[0]
                - (characterData.characterClassLevel - 1)
                * characterData.classUpFactor[1];

            glacierChangeSuccessRate = characterData.characterUniqueValue[1]
                + (characterData.characterClassLevel - 1)
                * characterData.classUpFactor[2];

        }

        public void SwapReposition(Character character)
        {
            //리포지션 함수를 위해 현재 나의 캐릭터와 바꾸려는 캐릭터의 빙하를 들고옴
            Glacier myGlacier = GridManager.Instance.glaciersTiles[glacierIdx];
            Glacier targetGlacier = GridManager.Instance.glaciersTiles[character.glacierIdx];

            //리포지션 함수 -> 내가원하는 빙하를 건네주면 해당 빙하의 값으로 초기화되며
            //리포지션은 가상 함수라서 따로 처리해줄 함수가있어서
            //각캐릭터마다 자리가 변경될 시 해당 캐릭터마다 원하는 값 초기화 진행
            Reposition(targetGlacier);
            character.Reposition(myGlacier);

            //캐릭터 인터페이스 포지션 초기화
            if (null != characterInterface && null != character.characterInterface)
            {
                character.characterInterface.SetTargetCharacterPosition();
                characterInterface.SetTargetCharacterPosition();
            }

            //메쉬 렌더러 순서 변경
            meshRenderer.sortingOrder = targetGlacier.orderLayer;
            character.meshRenderer.sortingOrder = myGlacier.orderLayer;
        }

        public IEnumerator StunCharacter(Character character)
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_TRICKSTER_DIZZY);

            //스턴이면 액션 코루틴함수 중지
            isStun = true;

            //스왑하며 위치 초기화 및 스왑 도중 필요한 셋 해주는 함수
            SwapReposition(character);

            //스왑하면서 버프 초기화
            character.SwapBuffCharacters();
            SwapBuffCharacters();

            //anim.AnimationState.ClearTrack(0);
            //anim.AnimationState.0
            anim.AnimationState.SetAnimation(0, "Stun", true);

            //자리 교체시 받아온 기절 시간동안 코루틴 반환
            yield return new WaitForSeconds(glacierChangeStunTime);

            //스턴이 해제되면 다시 액션 코루틴함수 시작 CharacterAction에 조건있음
            isStun = false;

            anim.AnimationState.SetAnimation(0, "Idle", true);
        }


        public IEnumerator TricksterDieSequence(Character character, UnityAction action)
        {
            if (null != actionCoroutine)
            {
                StopCoroutine(actionCoroutine);
            }
            
            SoundManager.Instance.PlaySound(SoundKey.SF_TRICKSTER_FAIL);

            isDying = true;
            isDraggable = false;
            
            //DieMotion취할때 뒤로 가려지는것 때문
            meshRenderer.sortingOrder = (character.meshRenderer.sortingOrder + 1); 
            
            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Die", false);
            
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(GetSkinParticleName(Trickster_effet_name, skinID));
            particle.PlayParticle(character.transform.localPosition, 0);
            
            if (!entry.IsComplete)
            {
                yield return new WaitForSpineAnimationComplete(entry);
            }
            
            action?.Invoke();
        }
        
        public override void CharacterTypeCheckToMarge(Character character)
        {
            if (isDying) return;
            //해당 함수를 오버라이드 받아서
            //합쳐지거나 자리교체를 하기위한 함수

            //같은 합성인덱스에 스턴이 아니라면 조건성립

            //스턴 시 합성이 도ㅑㅐ는지 
            //if (character.starGradeIndex == this.starGradeIndex && !isStun)
            //character.characterData.characterType != CharacterType.SUB 조건 넣은이유 -> 현재 로직상 바다 친구들도 해당 함수를 타고 들어올 수 있는 부분이라 예외처리해줌
            if (character.characterData.characterType != CharacterType.SUB && character.starGradeIndex == this.starGradeIndex && !isStun)
            {
                //같은 캐릭터 인덱스다 (같은 트릭스터) 그러면 합쳐져야하기에 이러한 조건걸어줌
                if (character.characterIndex == this.characterIndex)
                {
                    if (starGradeIndex < maxStarGradeValue)
                    {
                        MergeCharacter(character);
                        transform.localPosition = character.pivotPosition;

                        //TODO :: 이 로직에는 그냥 단순 클릭하고 떼면 그대로
                        //characterInterface.isDrag = true가 유지
                        isDrag = false;
                        if (characterInterface != null)
                        {
                            characterInterface.isDrag = false;
                        }
                    }
                    else
                    {
                        transform.localPosition = pivotPosition;
                    }
                }
                else
                {
                    //만약 다른 캐릭터 인덱스면 해당 조건으로 들어옴

                    //공격중인걸 중단하기 위함
                    if (null != actionSequence)
                        StopCoroutine(actionSequence);
                    //StopCoroutine(ActionSequence());


                    float randomValue = Random.Range(0f, 100f);
                    bool isSuccess = randomValue <= glacierChangeSuccessRate * 100f;
                    if (isSuccess)
                    {
                        //스턴 표현과 자리교환을 해주는 메인 로직
                        StartCoroutine(StunCharacter(character));
                    }
                    else
                    {
                        
                        //DestroyedCharacter();
                        StartCoroutine(TricksterDieSequence(character, DestroyedCharacter));
                    }
                }
            }
            else
            {
                transform.localPosition = pivotPosition;
            }

        }
    }
}
