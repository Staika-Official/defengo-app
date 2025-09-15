using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using UnityEngine;
using Spine;
using Spine.Unity;
using UniRx;
using UniRx.Triggers;
using Framework.Util;
using DG.Tweening;
using UnityEngine.Events;
using Framework.Sound;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Detectors;
using System;
using System.Linq;
using Framework.Network;
using UnityEngine.Purchasing;

namespace Framework.Game.Defense
{
    public abstract class Character : MonoBehaviour
    {
        public UnityAction endOfUse;
        public UnityAction swapAction;
        public SkeletonAnimation anim;
        public string characterName;
        public ObscuredFloat range;
        public bool isTouched = false;
        public ObscuredInt constructibleIdx;
        public ObscuredInt glacierIdx;
        public bool isTutorial;
        public UnityAction CharacterAction;
        public ObscuredFloat totalDamage;
        public CharacterIndex characterIndex;

        [Header("Character Upgrade")]
        public ObscuredInt starGradeIndex = 0;
        public ObscuredInt upgradeIndex = 0;

        [Header("CharacterBase Status")]
        public CharacterData characterData;
        public ObscuredFloat upgradeFactor;
        public ObscuredFloat basicAttackDamage;
        public ObscuredFloat classDamage;
        public ObscuredFloat attackSpeed;
        //public ObscuredFloat buffAttackSpeed;
        public ObscuredFloat criticalRange;
        public ObscuredInt targetCount = 2;
        public ObscuredFloat attackRange = 0;
        public ObscuredFloat projectileSpeed;
        public ObscuredFloat strikeRange;
        public ObscuredFloat buffValue;
        public ObscuredFloat buffRange;
        public ObscuredFloat buffCycle;
        public ObscuredFloat buffDuration;
        public ObscuredFloat totalBuffValue;
        public ObscuredFloat getAttackBuffValue;
        public ObscuredFloat criticalDamageRate;
        public bool isThrow;
        public bool isDrag;
        public bool isAttackType;
        public bool isDraggable;
        public bool isLeft;
        public int maxStarGradeValue;
        public ObscuredFloat sellPriceRatio;
        public bool isShield;
        public UnityAction shieldDestroyAction;
        public bool isNonFocus;

        [Header("ETC")]
        public List<Collider2D> collisions;
        public CharacterState characterState;
        public Vector2 pivotPosition;
        public Vector2 currentGridPos;
        public Transform animTransform;
        public bool isSummoned = false;
        public MeshRenderer meshRenderer;
        public ObscuredFloat speedBuffValue;

        //public SerializableDictionary<Character, float> dic_speedBuffValue = new();
        public Dictionary<Character, float> dic_speedBuffValue = new();

        public string projectileName;
        public string particleName;
        public CharacterInterface characterInterface;
        public ShieldBubble characterShield;

        public List<Monster> targetedMonster = new();

        public List<ObjectParticle> buffParticle = new();

        public IEnumerator actionCoroutine;
        
        public bool IsLockdown { get; set; }
        public bool IsInfected { get; set; }

        public int skinID;

        private readonly string bard_effect_name = "EfMusicBuff";

        public abstract void SetCharacterInfo();
        public abstract IEnumerator ActionSequence();
        public abstract void SynthesisCharacter();

        private void Awake()
        {
            ObscuredCheatingDetector.StartDetection(OnCheterDetected);
        }

        private void OnCheterDetected()
        {
            _ = NetworkManager.Instance.AbusingRecord("Cheat On Character");
        }

        public void Initialize(Glacier glacier, int starGradeValue)
        {
            //cubfoxy를 보면 알 수 있다 요약 : 버프데미지
            getAttackBuffValue = 0;
            //죽었는지 안죽었는지 체크
            isDestroyed = false;
            //튜토리얼 진행중인지 아닌지
            isTutorial = false;
            //소환상태인지 초기화 함수를 호출했다는것은 캐릭터를 소환했다는 얘기다 캐릭터스포너 SummonCharacter를 보면 알 수있캐
            isSummoned = true;
            //SellCharacter 함수를 보면 캐릭터를 팔때 젬가격을 공식값에 대입해주기 위한 변수 
            sellPriceRatio = ConfigData.CHARACTER_SELL_RATE;
            //최대 합성 갯수
            maxStarGradeValue = characterData.maxStarGradeValue;
            //현재 합성 갯수
            starGradeIndex = starGradeValue;
            //빙하의 가운데 그리드 인덱스를 저장 (이유 : 피봇 좌표값도 얻을 수 있고 해당 셀 데이터를 얻을 수 있기 때문)
            constructibleIdx = glacier.boundaryIndexes[4];
            //가운데 그리드를 이용해서 좌표를 뽑아옴
            currentGridPos = Calculator.TransformationVector(constructibleIdx);
            //캐릭터이름 저장
            characterName = characterData.characterName;
            //현재 빙하 가운데 포지션 벡터 만들기
            pivotPosition = new Vector2(glacier.centerPivot.x, glacier.centerPivot.y - 0.4f);
            //캐릭터 포지션 설정
            transform.localPosition = pivotPosition;
            //빙하 인덱스 설정 (본인이 서있는 빙하 인덱스로 그리드 매니저에서 빙하를 얻어올 수 있다)
            glacierIdx = glacier.glacierIndex;
            //게임오브젝트 이름 설정
            gameObject.name = $"{characterData.characterName}_{glacierIdx}";
            //합성 갯수 최대치를 제한 해둔것
            Mathf.Clamp(starGradeIndex, 0, maxStarGradeValue - 1);
            //합성 갯수마다 크기를 설정해주기위한 변수
            float value = GameManager.Instance.characterSpawner.GetGradeScale(starGradeIndex);
            //스킨마다 이펙트를 찾아주기위한 변수
            skinID = characterData.selectSkinType == SkinGradeType.NONE ? 0 : characterData.selectSkinID;
            //캐릭터 클릭시 포커스되면 안되는 캐릭터는 true 예: 챠미 (같은캐릭터는 포커스되지만 다른캐릭터에서 챠미를 포커스 하지못하게 하도록) 
            isNonFocus = false;

            //스파인 애니메이션 데이터
            SkeletonDataAsset data = characterData.anim;
            anim.skeletonDataAsset = data;
            anim.skeletonDataAsset.Clear();
            anim.AnimationState.ClearTracks();
            anim.initialSkinName = "default";
            anim.Initialize(true);
            meshRenderer = anim.GetComponent<MeshRenderer>();
            meshRenderer.sortingOrder = glacier.orderLayer;
            animTransform = anim.transform;
            animTransform.localScale = new Vector3(value, value, 1);

            //캐릭터 합성 계수가 0보다 크다면
            if (starGradeValue > 0)
            {
                //현재 캐릭터 합성 갯수를 표현해주는 UI를 조절해주는 스크립트
                if (characterInterface == null)
                {
                    //오브젝트풀에서 인터페이스를 꺼내온다
                    characterInterface = GameManager.Instance.objectPoolManager.GetObject<CharacterInterface>("CharacterInterface");
                    //타켓 캐릭터 설정
                    characterInterface.targetCharacter = this;
                    //초기 설정
                    characterInterface.Initialize();
                }

                //합성 UI 설정
                characterInterface.UpdateValue(starGradeIndex);
            }
            //캐릭터가 노말 타입을가진 캐릭터라면 true 아니라면 false
            isDraggable = characterData.characterType == CharacterType.NORMAL;

            //함수 오버라이드 받은 각각의 캐릭터 스탯을 설정해주는 함수
            SetCharacterInfo();

            //캐릭터 합성이나 업그레이드 시 파티클을 만들어주는 함수
            CharacterParticle(false);

            //캐릭터가 서브캐릭터가 아니라면 캐릭터 스포너에서 관리하는 소환된 캐릭터 리스트에 추가
            //합쳐지면 소환된캐릭터를 지우는 것도 있다.
            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.characterSpawner.AddOrRemoveSummonedCharacterList(this, true);
            }
        }


        //캐릭터가 나타나면 나오는 애니메이션
        public void AppearAction(string animKey, bool isAppear)
        {
            if (characterData.characterType == CharacterType.SUB)
            {
                anim.AnimationState.SetAnimation(0, animKey, true);
            }
            else
            {
                //캐릭터 소환되는 애니메이션 진행
                StartCoroutine(AppearActionSequence(animKey, isAppear));
                //빙하 타일 소환되면 같이 애니메이션 진행
                StartCoroutine(GridManager.Instance.glaciersTiles[glacierIdx].AppearAction());
            }
        }

        //카일만 쓰고있는 함수 따로 콜백하면서 애니메이션을 진행해야하는경우 호출
        public void AppearAction(UnityAction action, bool isAppear)
        {
            StartCoroutine(AppearActionSequence(action, isAppear));
            StartCoroutine(GridManager.Instance.glaciersTiles[glacierIdx].AppearAction());
        }

        public IEnumerator AppearActionSequence(string animKey, bool isAppear)
        {
            //합성 계수로 합쳐진 캐릭터인지 아닌지 확인해서 해당 애니메이션을 출력해주기 위함
            string key = isAppear ? "Appear" : "Merge";

            TrackEntry entry = anim.AnimationState.SetAnimation(0, key, false);
            characterState = CharacterState.APPEAR;
            yield return new WaitForSpineAnimationComplete(entry);
            characterState = CharacterState.DETECT;
            anim.AnimationState.SetAnimation(0, animKey, true);
        }

        public IEnumerator AppearActionSequence(UnityAction action, bool isAppear)
        {
            string key = isAppear ? "Appear" : "Merge";

            TrackEntry entry = anim.AnimationState.SetAnimation(0, key, false);
            characterState = CharacterState.APPEAR;
            yield return new WaitForSpineAnimationComplete(entry);
            characterState = CharacterState.DETECT;
            action?.Invoke();
        }

        public void Initialize(Glacier glacier, int starGradeValue, bool isTutorial)
        {
            getAttackBuffValue = 0;
            isDestroyed = false;
            this.isTutorial = isTutorial;
            isSummoned = true;
            sellPriceRatio = 0.6f;
            maxStarGradeValue = characterData.maxStarGradeValue;
            starGradeIndex = starGradeValue;
            constructibleIdx = glacier.boundaryIndexes[4];
            currentGridPos = Calculator.TransformationVector(constructibleIdx);
            characterName = characterData.characterName;
            pivotPosition = new Vector2(glacier.centerPivot.x, glacier.centerPivot.y - 0.4f);
            transform.localPosition = pivotPosition;
            glacierIdx = glacier.glacierIndex;
            gameObject.name = $"{characterData.characterName}_{glacierIdx}";
            Mathf.Clamp(starGradeIndex, 0, maxStarGradeValue - 1);
            float value = GameManager.Instance.characterSpawner.GetGradeScale(starGradeIndex);
            skinID = 0;

            SkeletonDataAsset data = characterData.anim;
            anim.skeletonDataAsset = data;
            anim.skeletonDataAsset.Clear();
            anim.AnimationState.ClearTracks();
            anim.initialSkinName = "default";
            anim.Initialize(true);
            //anim.AnimationState.SetAnimation(0, "Idle", true);
            meshRenderer = anim.GetComponent<MeshRenderer>();
            meshRenderer.sortingOrder = glacier.orderLayer;
            animTransform = anim.transform;
            animTransform.localScale = new Vector3(value, value, 1);

            if (starGradeValue > 0)
            {
                if (characterInterface == null)
                {
                    characterInterface = GameManager.Instance.objectPoolManager.GetObject<CharacterInterface>("CharacterInterface");
                    characterInterface.targetCharacter = this;
                    characterInterface.Initialize();
                }

                characterInterface.UpdateValue(starGradeIndex);
            }

            if (isTutorial) isDraggable = false;
            SetCharacterInfo();
            CharacterParticle(false);

            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.characterSpawner.AddOrRemoveSummonedCharacterList(this, true);
            }
        }

        public virtual void CharacterUpgradeSound(Character mergeCharacter)
        {
            //기본 합성 계수가 맥스 합성계수보다 낮으면 조건 성립
            //맥스 합성계수랑 일반 합성계수랑 사운드가 다름
            if (mergeCharacter.starGradeIndex < mergeCharacter.maxStarGradeValue)
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_COMBINE_SMALL);
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_COMBINE_BIG);
            }
        }

        public void UpgradeCharacter()
        {
            //업그레이드 되면 버프나 폴라리스의 타이머나 진행중인 것들을 초기화해주기위한
            endOfUse?.Invoke();

            //캐릭터가 삭제되면 쉴드도 반환해줘야함
            shieldDestroyAction?.Invoke();

            //합싱이되면 캐릭터가 1개가되기때문에 소환되어있는 캐릭터리스트에서 제외해줘야하기에 함수 호출
            GameManager.Instance.missionManager.AddOrRemoveCharacter(this, false);
            //소환 해제 상태 표시
            isSummoned = false;
            //합성 인덱스 증가
            starGradeIndex++;

            //기본 합성 계수가 맥스 합성계수보다 낮으면 조건 성립
            //맥스 합성계수랑 일반 합성계수랑 사운드가 다름
            // if (starGradeIndex < maxStarGradeValue)
            // {
            //     SoundManager.Instance.PlaySound(SoundKey.SF_COMBINE_SMALL);
            // }
            // else
            // {
            //     SoundManager.Instance.PlaySound(SoundKey.SF_COMBINE_BIG);
            // }

            //캐릭터 인터페이스가 그대로 있다면 반환한다
            //합쳐지기 전의 캐릭터 마다 인터페이스가 있다면 안되기 때문
            if (characterInterface != null)
            {
                GameManager.Instance.objectPoolManager.ReturnObject(characterInterface, "CharacterInterface");
            }
            characterInterface = null;

            //지금 공격상태의 함수가 돌아가고있는지아닌지 확인해서 공격하고있는 코루틴함수가 돌아가고있으면 중지해줌
            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
            }

            //소환중인 캐릭터를 제외해줌 이니셜라이즈에서 다시 추가해주기에 문제X
            GameManager.Instance.characterSpawner.AddOrRemoveSummonedCharacterList(this, false);

            //미션중인 것도 중지시킴
            GameManager.Instance.missionManager.AddOrRemoveCharacter(this, false);

            //튜도리얼인지 확인
            if (isTutorial)
            {
                //튜토리얼이라면 캐릭터를 합성 시 지정된 캐릭터를 생성해주기위함
                GameManager.Instance.characterSpawner.TutorialSummonSynthesisCharacter(glacierIdx, starGradeIndex);
            }
            else
            {
                //튜도리얼이아니라면 랜덤캐릭터를 얻어와서 생성시켜줌
                GameManager.Instance.characterSpawner.SummonSynthesisCharacter(glacierIdx, starGradeIndex);
                //GameManager.Instance.characterSpawner.SummonSynthesisWantCharacter<CharacterHiFive>(glacierIdx, starGradeIndex, "HiFive");
            }

            //본인이 버프를 주고있는 캐릭터가 0이상이다 0이하면 버프를 안주고있다는 뜻
            if (dic_speedBuffValue.Count > 0)
            {
                //버프 주고있는 캐릭터를 전부 지워줌
                foreach (var item in dic_speedBuffValue)
                {
                    item.Key.RemoveBuffCharacter(this);
                }

                dic_speedBuffValue.Clear();
                //셋팅된 버프량 0으로 변경
                speedBuffValue = 0;
            }

            //소환된 버프 파티클 제거
            ReturnParticle();

            //오브젝트 풀링다시 풀링시켜놓음
            GameManager.Instance.objectPoolManager.ReturnObject(this, characterName);
        }

        public void MergeCharacter(Character mergeCharacter)
        {
            //합쳐지 되면 버프나 폴라리스의 타이머나 진행중인 것들을 초기화해주기위한 이벤트 함수
            endOfUse?.Invoke();

            //캐릭터가 삭제되면 쉴드도 반환해줘야함
            shieldDestroyAction?.Invoke();

            //소환 해제로 변경해줌 어짜피 새롭게 인잇하면 isSummoned값 true로 변경
            isSummoned = false;

            //합쳐진 캐릭터를 업그레이드 시켜주는 함수 (합쳐지는 캐릭터도 반환하면서 새로운 캐릭터를 생성시키는거와 동일)
            mergeCharacter.UpgradeCharacter();
            CharacterUpgradeSound(mergeCharacter);

            //생성된 캐릭터 리스트에서 제외
            GameManager.Instance.characterSpawner.AddOrRemoveSummonedCharacterList(this, false);
            //현재빙하에캐릭터 생성가능 표시를 해줌
            GridManager.Instance.glaciersTiles[glacierIdx].isImpossibleSummon = false;

            //빙하에 캐릭터생성이 가능한지 확인 후 젬이 10개이상인지 확인해서 아이콘 표시 변경해줌
            //꽉찬상태라면 캐릭터 생성을 막아줘야하는데 캐릭터를합치면 남는 빙하가 있을 수도있으니 다시 재셋팅을 해주는 것
            UIManager.Instance.SetPossibleSummon(GridManager.Instance.IsPossibleSummon() && GameManager.Instance.Gem >= 10);

            //합쳐져서 사라지는 캐릭터 인터페이스에 대한 함수
            if (characterInterface != null)
            {
                //사라져야하니 반환을 해준다
                GameManager.Instance.objectPoolManager.ReturnObject(characterInterface, "CharacterInterface");
            }

            characterInterface = null;

            //공격하는게에 대한 코루틴 함수
            if (actionCoroutine != null)
            {
                //공격하는것을 중단
                StopCoroutine(actionCoroutine);
            }

            //버프 캐릭터가 버프값을 주고있는지에 대한 체크
            if (dic_speedBuffValue.Count > 0)
            {
                //버프를 주고있는 캐릭터를 순회
                foreach (var item in dic_speedBuffValue)
                {
                    //캐릭터에 버프를 주는것을 지워준다
                    item.Key.RemoveBuffCharacter(this);
                }

                dic_speedBuffValue.Clear();
                speedBuffValue = 0;
            }

            //미션 매니저에서 미션을 진행하는 캐릭터를 지워줌
            GameManager.Instance.missionManager.AddOrRemoveCharacter(this, false);

            //파티클을 오브젝트 풀에 반환을 시켜주기 위한 작업
            ReturnParticle();

            //캐릭터 반환을 시켜준다.
            GameManager.Instance.objectPoolManager.ReturnObject(this, characterName);
        }

        public abstract void UpgradeLevel(int upgradeIndex); //Common

        //토탈 데미지를 정해주는 함수 (토탈 데미지 = 공식데미지 + 버프 데미지)
        public void SetAttackDamage() //Common
        {
            float calcStarGradeAttackDamage = Calculator.SynthesisAttackValue(starGradeIndex, basicAttackDamage);
            float calcUpgradeAttackDamage = Calculator.UpgradeAttackValue(upgradeIndex, upgradeFactor, starGradeIndex);
            float damage = calcStarGradeAttackDamage + calcUpgradeAttackDamage;

            totalDamage = damage + getAttackBuffValue;
        }

        //파티클 버프를 추가시켜줄 함수 (함수 의도 : 버프파티클을 소지한 캐릭터를 찾고 반환하기 위한 작업)
        public void SetBuff(ObjectParticle objectParticle)
        {
            buffParticle.Add(objectParticle);
        }

        //업그레이드하면 공격속도 버프를 증가시켜주기 위한 함수
        public void SetUpgradeSpeedBuffValue(float speed)
        {
            speedBuffValue = speed;
        }

        public void SetSpeedBuff<T>(T character, float value, int skin_id) where T : Character
        {
            //함수의도 : 캐릭터 본인의 버프량 증가

            //같은 캐릭터가 있는지 확인
            if (dic_speedBuffValue.ContainsKey(character)) return;

            //dic_speedBuffValue.Set(character, value);
            //dic_speedBuffValue.Add()

            //같은 캐릭터가 아니라면 버프를 주고있는 캐릭터를 관리하기 위한 함수
            dic_speedBuffValue.Add(character, value);

            //버프주는 캐릭터가 2개보다 같거나 크다면
            if (dic_speedBuffValue.Count >= 2) // 2개 이상
            {
                float temp = 0;

                //버프주는 캐릭터 순회
                foreach (var item in dic_speedBuffValue)
                {
                    //버프주는 캐릭터들중 가장 큰 버프량을 가진 벨류값 선정
                    if (dic_speedBuffValue[item.Key] > temp)
                    {
                        temp = dic_speedBuffValue[item.Key];
                    }
                    //가장 큰 벨류값 대입
                    speedBuffValue = temp;
                }
            }
            else // 1개
            {
                //바드 전용이라그런지 뮤직버프 파티클 생성
                ObjectParticle musicBuff = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(GetSkinParticleName(bard_effect_name, skin_id));
                //버프 파티클 추가
                buffParticle.Add(musicBuff);
                //버프 파티클 캐릭터에 고정시키기위해 부모를 캐릭터로 설정
                musicBuff.transform.SetParent(transform);
                musicBuff.transform.localPosition = Vector2.zero;
                //파티클 실행
                musicBuff.SimplePlay();
                //버프값 설정
                speedBuffValue = value;
            }
        }

        public void EndSpeedBuff(CharacterBard character)
        {
            //Debug.Log("End Speed Buff");

            //함수 의도 : 현재 바드가 호출해주고있는 함수인데 주변에 버프받는 캐릭터를 순회하면서
            //매개변수로 (바드)자신을 넣어주면서 자신을 제외시키고 버프량을 조절해주거나 파티클을 제거해주는 역할을 해줍니다.

            //캐릭터가 사라지면 버프를 주고있는 캐릭터를 지워줘야한다
            //예 : 바드(본인)가 사라지면 주변에 캐릭터를 순회하며 바드(본인이) 주고있는 버프량을 지워줘야하기때문
            dic_speedBuffValue.Remove(character);

            //Debug.Log("dic Count : " + dic_speedBuffValue.Count);
            if (dic_speedBuffValue.Count >= 1) // 1개 이상
            {
                float temp = 0;

                //최대 버프량을 결정지어주는 방식
                foreach (var item in dic_speedBuffValue)
                {
                    if (dic_speedBuffValue[item.Key] > temp)
                    {
                        temp = dic_speedBuffValue[item.Key];
                    }
                    speedBuffValue = temp;
                }
            }
            else // 없는 경
            {
                string particle_name = GetSkinParticleName(bard_effect_name, character.skinID);
                //버프주는 캐릭이 1개도 없는경우 파티클을 지워줘야하기때문에 파티클하고 벨류값을 지워준다
                for (int i = 0; i < buffParticle.Count; i++)
                {
                    if (buffParticle[i].particleName == particle_name)
                    {
                        GameManager.Instance.objectPoolManager.ReturnObject(buffParticle[i], particle_name);
                        buffParticle.Remove(buffParticle[i]);
                        break;
                    }
                }
                speedBuffValue = 0;
            }
        }

        public void SwapBuffCharacters()
        {
            //함수 의도 : 버프를 주고있는 캐릭터를 전부 초기화해주기위함

            //버프를 주고있는 캐릭터가 있다면 조건 성립
            if (dic_speedBuffValue.Count > 0)
            {
                //버프를 주고있는 캐릭터를 가져와 내가 원하는 수행을 콜백함수로 만들어서 호출해줌


                //콜백함수에 dic_speedBuffValue의 삭제가 있으므로 키값을 이용해 리스트로 변환 후 순회
                foreach (var item in dic_speedBuffValue.Keys.ToList())
                {
                    //스왑하면서 일어나는 이벤트를 추가해줬습니다. (현재로서는 바드만)
                    //예 : 바드에게 버프받는캐릭터와 버프안받는 캐릭터가 위치가 변경되면
                    //버프도 변경되야하기에 바드에게 콜백함수를 추가해줬습니다.
                    item.swapAction?.Invoke();
                }
            }
        }


        //오브젝트 풀에 파티클을 리턴시켜주는 함수
        public void ReturnParticle()
        {
            if (buffParticle.Count > 0)
            {
                for (int i = 0; i < buffParticle.Count; i++)
                {
                    GameManager.Instance.objectPoolManager.ReturnObject(buffParticle[i], buffParticle[i].particleName);
                }
                buffParticle.Clear();
            }
        }

        //견습생 폭시를 위한 함수같음 공식상 버프량을 결정해줌
        public void SetBuffValue() //Common
        {
            float calcStarGradeBuffValue = Calculator.SynthesisAttackValue(starGradeIndex, buffValue);
            float calcUpgradeBuffValue = Calculator.UpgradeAttackValue(upgradeIndex, upgradeFactor, starGradeIndex);

            totalBuffValue = calcStarGradeBuffValue + calcUpgradeBuffValue;
        }

        //Attackable
        public IEnumerator WaitActionSequence()
        {
            while (isSummoned)
            {
                //CharacterAction();

                //캐릭터가 공격이후 잠깐 대기하는 시간
                yield return new WaitForSeconds(GetDuration());

                //캐릭터 공격 액션 진행
                CharacterAction();
            }
        }

        public void AttackValueBuff(bool isStart, float value)
        {
            //버프벨류를 추가해주거나 제외해주는 함수
            if (isStart)
            {
                getAttackBuffValue += value;
                SetAttackDamage();
            }
            else
            {
                getAttackBuffValue = Mathf.Max(0f, getAttackBuffValue - value);
                //getAttackBuffValue -= value;
                SetAttackDamage();
            }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------
        // Common
        public void GetPossibleAttackMonster()
        {
            //업데이트마다 확인하는 uniRX옵저버 패턴으로 인한 호출

            // 기존 타겟 리스트 확인
            for (int i = 0; i < targetedMonster.Count; i++)
            {
                //기존 타겟 리스트를 확인해서 살아있지 않다면 조건 성립
                if (!targetedMonster[i].IsAlive)
                {
                    targetedMonster.Remove(targetedMonster[i]);
                    continue;
                }

                Vector2 monPos = Calculator.TransformationVector(targetedMonster[i].prevPositionIdx);
                float dis = Calculator.DistanceCheck(currentGridPos - monPos);

                //몬스터와의 거리가 사거리보다 길다면 타겟 몬스터 제외 
                if (dis > attackRange)
                {
                    targetedMonster.Remove(targetedMonster[i]);
                }
            }

            // 이미 타겟이 있다면 넘김
            if (targetedMonster.Count >= targetCount) return;

            // 타겟팅
            for (int j = 0; j < GameManager.Instance.monsterSpawner.monsters.Count; j++)
            {
                Monster monster = GameManager.Instance.monsterSpawner.monsters[j];

                if (!monster.IsAlive) continue;

                Vector2 monPos = Calculator.TransformationVector(monster.prevPositionIdx);

                float dis = Calculator.DistanceCheck(currentGridPos - monPos);
                if (dis <= attackRange)
                {
                    bool isThing = true;

                    //이러한 조건을 걸어서 하는 이유
                    //-> 타켓 몬스터를 찾았는데 내가 이미 타겟팅한 몬스터랑 동일하다면 더이상 찾지 않게해주기위한 작업
                    for (int i = 0; i < targetedMonster.Count; i++)
                    {
                        if (monster.indexInWave == targetedMonster[i].indexInWave)
                        {
                            isThing = false;
                            break;
                        }
                    }
                    if (isThing) targetedMonster.Add(monster);
                    if (targetCount == targetedMonster.Count) break;
                }
            }
        }

        public void GetPossibleAttackNearOrFarMonster(bool isFar)
        {
            targetedMonster.Clear();

            bool isFindMonster = false;
            float CheckDistance = 0f;
            int findIndex = 0;

            if (GameManager.Instance.monsterSpawner.monsters.Count != 0)
            {
                Monster monster = GameManager.Instance.monsterSpawner.monsters[0];
                Vector2 monPos = Calculator.TransformationVector(monster.prevPositionIdx);
                CheckDistance = Calculator.DistanceCheck(currentGridPos - monPos);
            }

            for (int j = 0; j < GameManager.Instance.monsterSpawner.monsters.Count; j++)
            {
                Monster monster = GameManager.Instance.monsterSpawner.monsters[j];

                if (!monster.IsAlive) continue;

                Vector2 monPos = Calculator.TransformationVector(monster.prevPositionIdx);

                float dis = Calculator.DistanceCheck(currentGridPos - monPos);

                //사거리 범위에있는 몬스터 추출
                if (dis <= attackRange && isFar ? dis >= CheckDistance : dis <= CheckDistance)
                {
                    findIndex = j;
                    CheckDistance = dis;
                    isFindMonster = true;
                }


            }

            if (isFindMonster)
            {
                targetedMonster.Add(GameManager.Instance.monsterSpawner.monsters[findIndex]);
            }
        }

        public float GetDuration()
        {
            float temp;

            //본인이 공격하는 타입인지 버프주는 타입인지 체크
            if (isAttackType)
            {
                //캐릭터 스피드 버프 벨류가 0보다 크고 1보다 작으면 1로 고정
                if (speedBuffValue > 0 && speedBuffValue < 1)
                {
                    speedBuffValue = 1;
                }
                //스피드버프 벨류가 0이아니라면 어택스피드 * 스피드버프 벨류 곱하기 하고 아니라면 그냥 어택스피드
                float duration = speedBuffValue != 0 ? attackSpeed * speedBuffValue : attackSpeed;

                //1초 시간의 0.초 단위로 구분해줌
                temp = 1 / duration;
                //(duration + (duration * speedBuffValue));

                //듀레이션 값에 최대 스피드 리미트를 걸어준다
                temp = temp > ConfigData.ATTACKSPEED_LIMIT ? ConfigData.ATTACKSPEED_LIMIT : temp;
            }
            else
            {
                temp = 1 / buffCycle;
            }

            return temp;
        }

        public virtual void Reposition(Glacier glacier)
        {
            //빙하의 가운데 그리드 인덱스 셋팅

            //빙하 가운데 그리드 인덱스 설정
            constructibleIdx = glacier.boundaryIndexes[4];

            //빙하 가운데 그리드 포지션 위치를 얻어오는 작업
            currentGridPos = Calculator.TransformationVector(constructibleIdx);

            //피봇 위치 셋팅
            pivotPosition = new Vector2(glacier.centerPivot.x, glacier.centerPivot.y - 0.4f);

            //현재 캐릭터의 로컬 포지션을 셋팅
            transform.localPosition = pivotPosition;

            //캐릭터의 빙하 인덱스를 받아온 빙하 인덱스로 셋팅
            glacierIdx = glacier.glacierIndex;

            //캐릭터 이름을 재셋팅
            gameObject.name = $"{characterName}_{glacierIdx}";
        }

        public bool isDestroyed;

        public bool ShieldTile()
        {
            if (isShield == true)
            {
                shieldDestroyAction?.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ShieldSequence()
        {
            ShieldBubble shield = GameManager.Instance.objectPoolManager.GetObject<ShieldBubble>("ShieldBubble");
            shield.Initialize(this, "ShieldBubbleBurst");

            isShield = true;
            characterShield = shield;

            shieldDestroyAction = ShieldDestroyedSequence;
        }

        public void ShieldDestroyedSequence()
        {
            if (characterShield != null)
            {
                characterShield.DestroyShield();
                isShield = false;
                characterShield = null;
            }

            shieldDestroyAction = null;
        }

        public virtual void DestroyedTile()
        {
            isDestroyed = true;

            //캐릭터가 삭제되면 호출해줄 이벤트 함수
            endOfUse?.Invoke();

            //캐릭터가 삭제되면 쉴드도 반환해줘야함
            shieldDestroyAction?.Invoke();

            //newDongmin
            FocusCharacter(false);

            //버프 주고있는 캐릭터가 있다면 조건 성립
            if (dic_speedBuffValue.Count > 0)
            {
                //버프주고있는 캐릭터를 순회
                foreach (var item in dic_speedBuffValue)
                {
                    //각 버프주고있는 캐릭터에게 나를 제외시켜달라고 하는것
                    item.Key.RemoveBuffCharacter(this);
                }

                dic_speedBuffValue.Clear();
            }
            //소환된 캐릭터 리스트에서 제외 
            GameManager.Instance.characterSpawner.AddOrRemoveSummonedCharacterList(this, false);

            //합성된 캐릭터인지 아닌지 체크
            if (characterInterface != null)
            {
                GameManager.Instance.objectPoolManager.ReturnObject(characterInterface, "CharacterInterface");
                characterInterface = null;
            }

            //공격하는 코루틴이 진행중인지 체크
            if (actionCoroutine != null)
            {
                //공격하고있는 코루틴함수가 있다면 중지
                StopCoroutine(actionCoroutine);
            }

            //소환된 캐릭터가 미션중인 리스트에서 제외
            GameManager.Instance.missionManager.AddOrRemoveCharacter(this, false);

            //소환 해제를 선언
            isSummoned = false;

            //파티클 반환
            ReturnParticle();

            //캐릭터 오브젝트 반환
            GameManager.Instance.objectPoolManager.ReturnObject(this, characterName);
        }

        public void SellCharacter()
        {
            //isSummoned = false;

            //캐릭터가 삭제되면 호출해줄 이벤트 함수
            endOfUse?.Invoke();

            //캐릭터가 삭제되면 쉴드도 반환해줘야함
            shieldDestroyAction?.Invoke();

            //버프주고있는 캐릭터 찾기
            if (dic_speedBuffValue.Count > 0)
            {
                foreach (var item in dic_speedBuffValue)
                {
                    item.Key.RemoveBuffCharacter(this);
                }

                dic_speedBuffValue.Clear();
                speedBuffValue = 0;
            }

            //판매사운드
            SoundManager.Instance.PlaySound(SoundKey.SF_SELL_CHARACTER);

            //판매 가격 설정 후 판매진행
            int price = (int)(Mathf.Pow(2, starGradeIndex) * sellPriceRatio * ConfigData.GEM_SUMMON_FIRST);
            GameManager.Instance.characterSpawner.FocusCharacter(this, false);
            GameManager.Instance.ChangeGem(price);

            //소환 해제 선언
            isSummoned = false;

            //소환된 캐릭터 밑에 빙하에게 이제 여기 소환가능하다고 알려주는것
            GridManager.Instance.glaciersTiles[glacierIdx].isImpossibleSummon = false;

            //만약 캐릭터가 만땅이였다가 판매가되면 소환을 할 수 있게되느 ui를 활성화 시켜주는 함수
            UIManager.Instance.SetPossibleSummon(GridManager.Instance.IsPossibleSummon());

            GameManager.Instance.characterSpawner.AddOrRemoveSummonedCharacterList(this, false);
            if (characterInterface != null)
            {
                GameManager.Instance.objectPoolManager.ReturnObject(characterInterface, "CharacterInterface");
                characterInterface = null;
            }

            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
            }

            GameManager.Instance.missionManager.AddOrRemoveCharacter(this, false);

            ReturnParticle();
            GameManager.Instance.objectPoolManager.ReturnObject(this, characterName);
        }

        public void DestroyedCharacter()
        {
            //isSummoned = false;

            //캐릭터가 삭제되면 호출해줄 이벤트 함수
            endOfUse?.Invoke();

            //캐릭터가 삭제되면 쉴드도 반환해줘야함
            shieldDestroyAction?.Invoke();

            //버프주고있는 캐릭터 찾기
            if (dic_speedBuffValue.Count > 0)
            {
                foreach (var item in dic_speedBuffValue)
                {
                    item.Key.RemoveBuffCharacter(this);
                }

                dic_speedBuffValue.Clear();
                speedBuffValue = 0;
            }

            //소환 해제 선언
            isSummoned = false;

            //소환된 캐릭터 밑에 빙하에게 이제 여기 소환가능하다고 알려주는것
            GridManager.Instance.glaciersTiles[glacierIdx].isImpossibleSummon = false;

            //만약 캐릭터가 만땅이였다가 제거가되면 소환을 할 수 있게되느 ui를 활성화 시켜주는 함수
            UIManager.Instance.SetPossibleSummon(GridManager.Instance.IsPossibleSummon());

            GameManager.Instance.characterSpawner.AddOrRemoveSummonedCharacterList(this, false);
            if (characterInterface != null)
            {
                GameManager.Instance.objectPoolManager.ReturnObject(characterInterface, "CharacterInterface");
                characterInterface = null;
            }

            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
            }

            GameManager.Instance.missionManager.AddOrRemoveCharacter(this, false);

            ReturnParticle();
            GameManager.Instance.objectPoolManager.ReturnObject(this, characterName);
        }

        public void CharacterParticle(bool isLevelUp)
        {
            //함수 의도 : 캐릭터가 레벨업이나 합성을 하면 파티클을 생성시켜주기 위한 함수

            if (isLevelUp)
            {
                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("LvUp");
                particle.PlayParticle(transform.localPosition, 0);
            }
            else
            {
                if (starGradeIndex >= 3)
                {
                    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("HighRankAppear");
                    particle.PlayParticle(transform.localPosition, 0);
                }
                else
                {
                    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("Appear");
                    particle.PlayParticle(transform.localPosition, 0);
                }
            }
        }

        public void FailedCharacterParticle()
        {
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("LvUpFail");
            particle.PlayParticle(transform.localPosition, 0);
        }

        public string GetSkinParticleName(string effect_name, int skin_id)
        {
            string particle_name = effect_name;
            if (skin_id != 0)
                particle_name = string.Format("{0}_{1}", particle_name, skin_id);

            return particle_name;
        }


        #region Control

        public void FlipCharacter(float targetXValue) //Common
        {
            //함수 의도 : 현재 스프라이트 캐릭터의 왼쪽 오른쪽으로 값을 변경시켜주기 위한 작업
            if (transform.localPosition.x >= targetXValue)
            {
                transform.DOScaleX(Mathf.Abs(transform.localScale.x), 0f);
                isLeft = true;
            }
            else
            {
                transform.DOScaleX(-1 * Mathf.Abs(transform.localScale.x), 0f);
                isLeft = false;
            }
        }

        public void SetTouch()
        {
            //터치가되면 드래드 매니저에서 RaycastHit2D 가지고있는 타겟을 찾으면 호출되는 함수

            //지금 드래그 중이 아니라면 리턴
            if (!isDraggable) return;

            //현재 터치중인지 
            isTouched = true;

            //드래그중인지 체크
            isDrag = true;
            //meshRenderer.sortingOrder = 32;

            //합성된 캐릭터라면
            if (characterInterface != null)
            {
                //합성 ui도 같이 움직여야하기에 드래그 트루를 해준다
                //그러면 업데이트문에서 같이 따라오게끔 처리되어있음
                characterInterface.isDrag = true;
            }

            //같은 동일한 캐릭터와 같은 성급을 가진 캐릭터를 포커스해주기위한 함수
            GameManager.Instance.characterSpawner.FocusCharacter(this, true);
        }

        public void FocusCharacter(bool isFocus)
        {
            if (isFocus)
            {
                meshRenderer.sortingOrder = 33;
                GridManager.Instance.glaciersTiles[glacierIdx].meshRenderer.sortingOrder = 32;
                if (characterInterface != null)
                {
                    characterInterface.FocusCharacter(isFocus);
                }
            }
            else
            {
                meshRenderer.sortingOrder = GridManager.Instance.glaciersTiles[glacierIdx].orderLayer;
                GridManager.Instance.glaciersTiles[glacierIdx].meshRenderer.sortingOrder = 2;
                if (characterInterface != null)
                {
                    characterInterface.FocusCharacter(isFocus);
                }
            }
        }

        public void SetRelease()
        {
            //함수 의도 : 포커스된 캐릭터를 합치거나 원상 복귀 시켜주는 함수

            if (!isDraggable) return;
            GameManager.Instance.characterSpawner.FocusCharacter(this, false);

            //곂쳐있는 캐릭터가 있다면 조건 성립
            if (collisions.Count > 0)
            {
                float temp = 10.0f;
                int idx = 0;

                //곂쳐있는 캐릭터 중 제일 가까운 캐릭터의 인덱스를 찾기위한 함수
                for (int i = 0; i < collisions.Count; i++)
                {
                    Vector2 myPos = transform.position;
                    Vector2 objPos = collisions[i].transform.localPosition;

                    Vector2 vec = myPos - objPos;

                    float dis = Vector2.SqrMagnitude(vec);

                    if (temp >= dis)
                    {
                        temp = dis;
                        idx = i;
                    }
                }
                //Todo : Same type Check, Anim 


                //현재 합쳐져있는 캐릭터의 인덱스를 뽑아와서 캐릭터 컴포넌트를 뽑아온다
                Character character = collisions[idx].GetComponent<Character>();


                //같은 성급 같은 캐릭터 인지 확인 후 합치게 해주려고하는 로직
                //if (character.starGradeIndex == this.starGradeIndex && character.characterIndex == this.characterIndex && starGradeIndex < 4)
                //{
                //    MergeCharacter(character);
                //    transform.localPosition = character.pivotPosition;
                //    isDrag = false;
                //    if (characterInterface != null)
                //    {
                //        characterInterface.isDrag = false;
                //    }
                //}
                //else
                //{
                //    transform.localPosition = pivotPosition;
                //}

                //virual change dongmin

                CharacterTypeCheckToMarge(character);
            }
            else
            {
                transform.localPosition = pivotPosition;
            }
        }

        public virtual void CharacterTypeCheckToMarge(Character character)
        {
            if (character.starGradeIndex == this.starGradeIndex && character.characterIndex == this.characterIndex && starGradeIndex < 4)
            {
                MergeCharacter(character);
                transform.localPosition = character.pivotPosition;
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isTouched) return;

            if (collision.CompareTag("Player"))
            {
                collisions.Add(collision);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!isTouched) return;

            if (collision.CompareTag("Player"))
            {
                collisions.Remove(collision);
            }
        }

        public virtual void RemoveBuffCharacter(Character character) { }

        public virtual void Lockdown()
        {
            Debug.Log($"{transform.name} is LockDown");
            IsLockdown = true;

        }

        public virtual void ReleaseLockdown()
        {
            IsLockdown = false;
            Debug.Log($"{transform.name} Release Lockdown");
        }

        public void Infect()
        {

        }

        public void ReleaseInfect()
        {

        }

        public void SetBomb()
        {
            Debug.Log($"{transform.name} : Set Bomb");

            string particleName = starGradeIndex == 0 ? "BossEf_Bomb1" : "BossEf_Bomb2";

            ObjectParticle objectParticle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(particleName);
            objectParticle.transform.position = transform.position;
            objectParticle.SimplePlay();

            if (starGradeIndex == 0)
            {

            }
            else
            {

            }

        }

        public void BombExplosion()
        {
            string particleName = starGradeIndex == 0 ? "BossEf_BombExplosion1" : "BossEf_BombExplosion1";

            ObjectParticle objectParticle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(particleName);
            objectParticle.transform.position = transform.position;
            objectParticle.SimplePlay();
        }
        #endregion
    }
}
