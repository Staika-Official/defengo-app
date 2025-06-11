using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using Framework.GameData.Defense;
using Framework.Sound;
using Spine;
using Spine.Unity;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Framework.Game.Defense
{
    public class CharacterGuardian : Character
    {
        //유니크 벨류 1,4 : 빙판 세우는 주기 (최소, 최대)
        public ObscuredFloat iceBarrierMinCycle;
        public ObscuredFloat iceBarrierMaxCycle;
        
        //유니크 벨류 2 : 빙판 지속시간
        public ObscuredFloat iceBarrierDuration;
        //유니크 벨류 3 : 빙판 판정 (거리로 판정내림)
        public ObscuredFloat iceBarrierBlockDistance;

        public void BarrierAction()
        {
            //오브젝트 풀에서 베리어 이펙트를 활성화 및 초기화를 위해 컴포넌트 들고옴
            GuardianBarrier barrier = GameManager.Instance.objectPoolManager.GetObject<GuardianBarrier>("EffectBarrier");

            GameManager.Instance.renderSortManager.AddDebuffSortLayer(barrier);

            //베리어 스크립트 에서 포지션설정, 지속시간, 판정이 필요함으로 매개변수로 넣어줌
            barrier.Initialize(CalcBarrierPosition(), iceBarrierDuration, iceBarrierBlockDistance);

        }

        public int CalcBarrierPosition()
        {
            //현재 캐릭터가 생성된 위치의 셀 인덱스를 들고옴
            int pivot = constructibleIdx;

            //글레시아타일에 현재 설정된 방향으로 인덱스를 들고오게 처리
            //stageData에 방향 설정
            int directionIndex = GridManager.Instance.glaciersTiles[glacierIdx].GlacierDirectionTypeToIndex();

            //현재 위치에 기획서에서 지정해준 방향의 셀 인덱스 값을 더해줌으로 해당 셀 위치를 가져올 수 있음
            pivot += directionIndex;

            return pivot;
        }

        public TrackEntry GuardianLookAnimation()
        {
            //함수 의도 : 캐릭터가 쳐다보는 방향과, 애니메이션 진행 시 어떤 방향으로 처리를 해줄지에 대한 함수

            //빙하 타일의 방향값을 가져와서 애니메이션 처리
            GlacierSummonDirectionType glacierDirType = GridManager.Instance.glaciersTiles[glacierIdx].glacierDirType;

            int pivot = constructibleIdx;
            int dirIndex = GridManager.Instance.glaciersTiles[glacierIdx].GlacierDirectionTypeToIndex();

            //셀 데이터를 얻어와서 포지션을 얻기위함
            CellData cell = GridManager.Instance.cellDatas[pivot + dirIndex];

            //이렇게 넣어준 이유 몬스터 기준이아닌 이펙트 생성부분 셀 방향을 바라보게끔 처리
            FlipCharacter(cell.cellPosition.x);

            // = new TrackEntry();

            string animKey = glacierDirType switch
            {
                GlacierSummonDirectionType.NEAR_LEFT => "Attack_FrontSide",
                GlacierSummonDirectionType.NEAR_RIGHT => "Attack_FrontSide",
                GlacierSummonDirectionType.NEAR_UP => "Attack_Back",
                GlacierSummonDirectionType.NEAR_DOWN => "Attack_Front",
                GlacierSummonDirectionType.FAR_LEFT => "Attack_FrontSide",
                GlacierSummonDirectionType.FAR_RIGHT => "Attack_FrontSide",
                _ => "Attack_FrontSide"
            };

            TrackEntry entry = anim.AnimationState.SetAnimation(0, $"{animKey}", false);
            return entry;
        }


        public override void SetCharacterInfo()
        {
            //캐릭터 정보 초기화
            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);

            //공격하는 타입인지 아닌지로 버프를 넣어주는지 안넣어주는지 결정하기에 공격하는 타입이아니라고 체크
            isAttackType = false;

            //공식값 대입 빙판세우는 주기, 빙판지속시, 빙판의 판정 
            iceBarrierMinCycle = characterData.characterUniqueValue[0]
                - (characterData.characterClassLevel - 1)
                * characterData.classUpFactor[0];
            
            iceBarrierMaxCycle = characterData.characterUniqueValue[3]
                                 - (characterData.characterClassLevel - 1)
                                 * characterData.classUpFactor[1];
            
            iceBarrierDuration = characterData.characterUniqueValue[1] 
                + starGradeIndex * characterData.starFactor[0];

            iceBarrierBlockDistance = characterData.characterUniqueValue[2];

            //최대 별 갯수를 지정해서 최대 별 갯수를 넘어서면 사운드를 바꿔줌 (폴라리스와 연관)
            maxStarGradeValue = characterData.maxStarGradeValue;

            //현재 소환됨을 결정해줌
            isSummoned = true;

            //캐릭터의 액션을 진행해주기위한 코루틴 함수
            StartCoroutine(ActionSequence());

            //미션에 넣어줌으로 같은 성급 등 다양한 미션에 참여시키기위함
            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

            //처음 소환됬는지 합쳐진 캐랙터인지 확인을 위함
            bool isAppear = starGradeIndex == 0;

            //처음소환되면 빙하도같이 애니메이션 아니라면 캐릭터만 애니메이션
            AppearAction("Idle", isAppear);
        }


        public override IEnumerator ActionSequence()
        {
            //캐릭터 공격을 진행해줄 때 처리해주는 함수
            //예 : 애니메이션 몬스터 뽑아와서 데미지 입히는 거 등 다양한 정보 처리

            while (isSummoned)
            {
                //첫 소환부터 캐릭터가 스킬을 쓰면 안되기에 빙판세우는 주기만큼 대기
                float iceBarrierRandomCycle = Random.Range(iceBarrierMinCycle, iceBarrierMaxCycle);
                Debug.Log($"Guardian : {iceBarrierRandomCycle}");
                yield return new WaitForSeconds(iceBarrierRandomCycle);

                //캐릭터의 쳐다보는 위치와 위치마다 애니메이션이 다르기에 값을 설정해주는 함수
                TrackEntry entry = GuardianLookAnimation();

                //애니메이션 이벤트 Attack이 발생되면 넘어감
                yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

                //베리어 이펙트를 생성
                BarrierAction();

                //애니메이션이 끝날때까지 대기
                yield return new WaitForSpineAnimationComplete(entry);

                //애니메이션이 끝난다면 아이들 상태
                anim.AnimationState.SetAnimation(0, "Idle", true);
            }
        }

        public override void SynthesisCharacter()
        {
            //캐릭터 합성 시 스탯이나 발사체가 바꾸려먼 사용되는 함수
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            //업그레이드 시 처리해줄 스탯이나 정보들
            this.upgradeIndex = upgradeIndex;

            //합성이되면 벨류값을 조절해줘야하기에 넣어줌
            //현재로썬 iceBarrierDuration 값만 변경됨
            iceBarrierDuration = characterData.characterUniqueValue[1]
                + starGradeIndex * characterData.starFactor[0];
        }
    }
}
