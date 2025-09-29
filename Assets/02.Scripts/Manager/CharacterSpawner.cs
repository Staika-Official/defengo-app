using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Framework.GameData.Defense;
using Framework.Util;
using Framework.Network;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Detectors;

namespace Framework.Game.Defense
{
    public class DecData
    {
        public CharacterIndex characterIndex;
        public ObscuredInt characterLevel;
    }

    public class CharacterInfo
    {
        public string characterName;
        public ObscuredInt upgrade;
        public ObscuredInt starGrade;
        public ObscuredInt classUp;
        public ObscuredFloat totalDamage;
    }

    public class SpawnData
    {
        public void Initialize()
        {
            this.characterUpgrade = 0;
            this.characterMerge = 0;
            this.summonCharacter = 0;
            this.fourStarGrade = 0;
        }
        
        //현재 웨이브 때 진행됐던 정보를 담아두는 클래스
        public ObscuredInt characterUpgrade;
        public ObscuredInt characterMerge;
        public ObscuredInt summonCharacter;
        public ObscuredInt fourStarGrade;
    }

    public class CharacterSpawner : MonoBehaviour
    {
        public List<Character> summonedCharacters = new();
        public IntReactiveProperty characterCount;
        public IntReactiveProperty repositionCount;
        public Dictionary<string, CharacterData> characterDataDic = new();
        public readonly float[] upgradeValue = { 0.135f, 0.137f, 0.145f, 0.155f, 0.17f, 0.17f };
        public List<DecData> currentDecData = new();
        public CharacterData subCharacter;
        public int summonCount;
        public Slot slot;
        public List<Character> attackRandomCharacters = new();

        public SpawnData currentSpawnData = new();
        public SpawnData totalSpawnData = new();
        
        private void Start()
        {
            if (!UserInfoManager.Instance.userState.finishedTutorial)
            {
                TutorialInitialize();
            }
            else
            {
                Initialize();
            }

            TestInitialize();

            ObscuredCheatingDetector.StartDetection(OnCheterDetected);
        }

        public Character GetAttackRandomCharacter()
        {
            //어택 캐릭터 중 랜덤으로 캐릭터를 뽑아온다.
            //카일도 AttackType.ATTACK이라 attackRandomCharacters이 Count가 0이 나올경우는 없다  
            for (int i = 0; i < summonedCharacters.Count; i++)
            {
                if (summonedCharacters[i].characterData.attackType == AttackType.ATTACK)
                {
                    attackRandomCharacters.Add(summonedCharacters[i]);
                }
            }
            
            int radnomIndex = Random.Range(0, attackRandomCharacters.Count);

            Character obj = attackRandomCharacters[radnomIndex];
            attackRandomCharacters.Clear();

            return obj;
        }

        private void OnCheterDetected()
        {
            _ = NetworkManager.Instance.AbusingRecord("Cheat Character Spawner");
        }


        public Character GetNonShieldRandomCharacter()
        {
            Character character = null;
            List<Character> data = new();
            
            for (int i = 0; i < summonedCharacters.Count; ++i)
            {
                Character obj = summonedCharacters[i];
                if (!obj.isShield)
                {
                    data.Add(obj);
                }
            }

            if (data.Count > 0)
            {
                int randomIndex = Random.Range(0, data.Count);
                Character obj = data[randomIndex];
                character = obj;
            }

            return character;
        }
        
        
        public Character GetNonShieldHighestStarGradeRandomCharacter()
        {
            int curIdx = 0;
            
            List<int> starGradeRandomCharacters = new();
            
            float highStarGrade = 0;

            for (int i = 0; i < summonedCharacters.Count; i++)
            {
                if (!summonedCharacters[i].isShield)
                {
                    if (summonedCharacters[i].starGradeIndex >= highStarGrade)
                    {
                        highStarGrade = summonedCharacters[i].starGradeIndex;
                        curIdx = i;
                        starGradeRandomCharacters.Clear();
                        starGradeRandomCharacters.Add(curIdx);
                    }
                    else if (summonedCharacters[i].starGradeIndex == highStarGrade)
                    {
                        curIdx = i;
                        starGradeRandomCharacters.Add(curIdx);
                    }
                }
            }

            if (starGradeRandomCharacters.Count > 0)
            {
                int radnomIndex = Random.Range(0, starGradeRandomCharacters.Count);
                Character character = summonedCharacters[starGradeRandomCharacters[radnomIndex]];
                starGradeRandomCharacters.Clear();

                return character;
            }

            return null;
        }
        
        public float GetBestAttackSpeed()
        {
            float bestSpeed = 0;

            for (int i = 0; i < summonedCharacters.Count; i++)
            {
                if (summonedCharacters[i].attackSpeed >= bestSpeed)
                {
                    bestSpeed = summonedCharacters[i].attackSpeed;
                }
            }

            return bestSpeed;
        }
        
        public float GetBestAttackDamage()
        {
            float bestDamage = 0;

            for (int i = 0; i < summonedCharacters.Count; i++)
            {
                if (summonedCharacters[i].totalDamage >= bestDamage)
                {
                    bestDamage = summonedCharacters[i].totalDamage;
                }
            }

            return bestDamage;
        }
        
        public float GetBestUpgradeCharacter()
        {
            float bestUpgrade = 0;

            for (int i = 0; i < summonedCharacters.Count; i++)
            {
                if (summonedCharacters[i].upgradeIndex >= bestUpgrade)
                {
                    bestUpgrade = summonedCharacters[i].upgradeIndex;
                }
            }

            return bestUpgrade;
        }
        

        public string GetCharacterInfo()
        {
            if (summonedCharacters.Count > 0)
            {
                int idx = 0;
                float temp = 0;

                for (int i = 0; i < summonedCharacters.Count; i++)
                {
                    if (summonedCharacters[i].totalDamage >= temp)
                    {
                        temp = summonedCharacters[i].totalDamage;
                        idx = i;
                    }
                    else
                    {
                        continue;
                    }
                }

                Character character = summonedCharacters[idx];

                CharacterInfo info = new()
                {
                    characterName = character.characterName,
                    upgrade = character.upgradeIndex,
                    classUp = character.characterData.characterClassLevel,
                    starGrade = character.starGradeIndex,
                    totalDamage = character.totalDamage
                };

                string data = JsonUtility.ToJson(info);
                return data;
            }
            else
            {
                string data = "null";
                return data;
            }
        }

        public void SetQuestCharacterStarCount(int starGrade)
        {
            //이후에 3성 5성 등 다른 성급 보유 카운트를 가지려면 변수 추가후 스위치문에서 작성해주면 됌
            switch (starGrade)
            {
                case 4:
                    ++currentSpawnData.fourStarGrade;
                    ++totalSpawnData.fourStarGrade;
                    break;
                default:
                    break;
            }
        }

        public void SetTotalSpawnData()
        {
            totalSpawnData.characterMerge = Mathf.Max(0,totalSpawnData.characterMerge - currentSpawnData.characterMerge);
            totalSpawnData.characterUpgrade = Mathf.Max(0,totalSpawnData.characterUpgrade - currentSpawnData.characterUpgrade);
            totalSpawnData.fourStarGrade = Mathf.Max(0,totalSpawnData.fourStarGrade - currentSpawnData.fourStarGrade);
            totalSpawnData.summonCharacter = Mathf.Max(0,totalSpawnData.summonCharacter - currentSpawnData.summonCharacter);
        }
        
        public void SpawnDataClear()
        {
            currentSpawnData.Initialize();
        }
        
        
        
        public void TestInitialize()
        {
#if UNITY_EDITOR
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Q))
                .Subscribe(_ => TestSummonCharacter(KeyCode.Q));
            this.UpdateAsObservable()
               .Where(_ => Input.GetKeyDown(KeyCode.W))
               .Subscribe(_ => TestSummonCharacter(KeyCode.W));
            this.UpdateAsObservable()
               .Where(_ => Input.GetKeyDown(KeyCode.E))
               .Subscribe(_ => TestSummonCharacter(KeyCode.E));
            this.UpdateAsObservable()
               .Where(_ => Input.GetKeyDown(KeyCode.R))
               .Subscribe(_ => TestSummonCharacter(KeyCode.R));
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.T))
                .Subscribe(_ => TestSummonCharacter(KeyCode.T));

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Alpha1))
                .Subscribe(_ => TestSummonCharacterFour(KeyCode.Alpha1));
            this.UpdateAsObservable()
               .Where(_ => Input.GetKeyDown(KeyCode.Alpha2))
               .Subscribe(_ => TestSummonCharacterFour(KeyCode.Alpha2));
            this.UpdateAsObservable()
               .Where(_ => Input.GetKeyDown(KeyCode.Alpha3))
               .Subscribe(_ => TestSummonCharacterFour(KeyCode.Alpha3));
            this.UpdateAsObservable()
               .Where(_ => Input.GetKeyDown(KeyCode.Alpha4))
               .Subscribe(_ => TestSummonCharacterFour(KeyCode.Alpha4));
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Alpha5))
                .Subscribe(_ => TestSummonCharacterFour(KeyCode.Alpha5));
#endif
        }

        public float GetGradeScale(int idx)
        {
            float value = upgradeValue[idx];
            return value;
        }

        public void AllClear()
        {
            summonedCharacters.Clear();
            characterDataDic.Clear();
        }

        public void TutorialInitialize()
        {
            Slot slot = new()
            {
                slotNumber = 1,
                slotCharacterIds = new int[] { 1, 2, 3, 4, 5, 1001 }
            };

            this.slot = slot;

            summonCount = 0;
            for (int i = 0; i < slot.slotCharacterIds.Length - 1; i++)
            {
                CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)slot.slotCharacterIds[i]];
                characterDataDic.Add($"Character_{slot.slotCharacterIds[i]}", data);
                DecData dec = new()
                {
                    characterIndex = data.characterIndex,
                    characterLevel = 1
                };

                currentDecData.Add(dec);

                UIManager.Instance.buttons_Upgrade[i].Initialize(data);
            }

            subCharacter = DataManager.Instance.dic_CharacterData[(CharacterIndex)slot.slotCharacterIds[5]];
            GameManager.Instance.buffManager.SetFieldBuff(subCharacter);
        }

        public void Initialize()
        {
            slot = UserSlotManager.Instance.GetSlotFocusIndexData();
            
            //todo dongmin
            SpawnDataClear();
            
            summonCount = 0;
  
            UIManager.Instance.missionPopup.isLemmingRichHiddenMission = false;
            for (int i = 0; i < slot.slotCharacterIds.Length - 1; i++)
            {
                CharacterData data = DataManager.Instance.dic_CharacterData[(CharacterIndex)slot.slotCharacterIds[i]];
                characterDataDic.Add($"Character_{slot.slotCharacterIds[i]}", data);

                DecData dec = new()
                {
                    characterIndex = data.characterIndex,
                    characterLevel = 1
                };

                currentDecData.Add(dec);

                UIManager.Instance.buttons_Upgrade[i].Initialize(data);
                if (data.isHiddenMission)
                {
                    GameManager.Instance.missionManager.SetHiddenMission(data.characterIndex, data.attackType);
                }
            }

            if (IsCharacterInDec(CharacterIndex.KIRING))
            {
                GridManager.Instance.SetDamageAction();
            }

            subCharacter = DataManager.Instance.dic_CharacterData[(CharacterIndex)slot.slotCharacterIds[5]];
            GameManager.Instance.buffManager.SetFieldBuff(subCharacter);
        }

        public bool IsCharacterInDec(CharacterIndex characterIndex)
        {
            bool isThing = false;

            //함수 의미 : 내가 설정해둔 덱을 살펴보면서 외부에서 인덱스를 받아 찾고싶은 캐릭터가 있는지 찾는 함수
            //예 : 키링을 찾고싶다 characterIndex를 키링으로 매개변수를 넣어주고 있는지없는지 확인하는 작업

            //설정해둔 덱 데이터를 순회한다
            for (int i = 0; i < currentDecData.Count; i++)
            {
                //내가 지정한 덱 안에 찾고싶은 캐릭터 인덱스가 있다면 true로 반환해준다
                if (currentDecData[i].characterIndex == characterIndex)
                {
                    isThing = true;
                    return isThing;
                }
            }

            return isThing;
        }

        public List<int> GetGlacierCharacterSet(CharacterIndex characterIndex)
        {
            //함수 의도 : 소환된 캐릭터들중에 찾고싶은 캐릭터의 빙하 인덱스를 얻기위한 작업
            //예 : 키링이 소환되면 제일먼저 빙하가 깨져야하기때문에 키링이 소환된 빙하를 찾는 작업을 해야한다

            List<int> data = new();

            //덱안에 내가 찾고싶은 캐릭터가 있는지 확인하는 조건
            if (IsCharacterInDec(characterIndex))
            {
                //있다면 소환된 캐릭터를 순회
                for (int i = 0; i < summonedCharacters.Count; i++)
                {
                    //찾고싶은 캐릭터를 찾았다면
                    if (summonedCharacters[i].characterIndex == characterIndex)
                    {
                        //캐릭터의 빙하 인덱스를 넘겨줄 데이터에 추가해준다.
                        data.Add(summonedCharacters[i].glacierIdx);
                    }
                }
            }
            return data;
        }

        public void UpgradeLevel(CharacterIndex characterIndex)
        {
            int level = 0;

            for (int i = 0; i < currentDecData.Count; i++)
            {
                if (characterIndex == currentDecData[i].characterIndex)
                {
                    currentDecData[i].characterLevel++;

                    ++currentSpawnData.characterUpgrade;
                    ++totalSpawnData.characterUpgrade;
                    
                    level = currentDecData[i].characterLevel;
                    break;
                }
            }

            for (int i = 0; i < summonedCharacters.Count; i++)
            {
                if (summonedCharacters[i].characterIndex == characterIndex)
                {
                    summonedCharacters[i].UpgradeLevel(level);
                    summonedCharacters[i].CharacterParticle(true);
                }
            }
        }

        public void FailedUpgrade(CharacterIndex characterIndex, bool isReset)
        {
            int level = 0;

            ++currentSpawnData.characterUpgrade;
            ++totalSpawnData.characterUpgrade;


            for (int i = 0; i < currentDecData.Count; i++)
            {
                if (characterIndex == currentDecData[i].characterIndex)
                {
                    if (isReset)
                    {
                        currentDecData[i].characterLevel = 1;
                        level = currentDecData[i].characterLevel;
                    }
                    
                    UIManager.Instance.buttons_Upgrade[i].UpgradeFailed(isReset);
                    break;
                }
            }


            for (int i = 0; i < summonedCharacters.Count; i++)
            {
                if (summonedCharacters[i].characterIndex == characterIndex)
                {
                    if (isReset)
                    {
                        summonedCharacters[i].UpgradeLevel(level);
                    }
                    summonedCharacters[i].FailedCharacterParticle();
                }
            }
        }

        public void FocusCharacter(Character character, bool isFocus)
        {
            //현재 소환된 캐릭터를 순회하면서 같은 캐릭터인덱스 같은 성급을 가진 캐릭터를
            //포커스 시켜주는 함수.
            for (int i = 0; i < summonedCharacters.Count; i++)
            {
                Character targetCharacter = summonedCharacters[i];

                //new dongmin 트릭스터를 위한 조건
                if (character.characterData.isUniqueChange)
                {
                    if (targetCharacter.starGradeIndex == character.starGradeIndex)
                    {
                        targetCharacter.FocusCharacter(isFocus);
                    }
                }
                else
                {
                    if(character.isNonFocus) continue;
                    
                    if (targetCharacter.characterIndex == character.characterIndex && targetCharacter.starGradeIndex == character.starGradeIndex)
                    {
                        targetCharacter.FocusCharacter(isFocus);
                    }
                }

                //if (targetCharacter.characterIndex == character.characterIndex && targetCharacter.starGradeIndex == character.starGradeIndex)
                //{
                //targetCharacter.FocusCharacter(isFocus);
                //}
            }
        }

        public CharacterData GetRandomCharacterData()
        {
            int idx = Random.Range(0, slot.slotCharacterIds.Length - 1);

            CharacterData data = characterDataDic[$"Character_{slot.slotCharacterIds[idx]}"];
            return data;
        }

        public Character TutorialCharacter(CharacterIndex characterIndex)
        {
            ObjectPoolManager objectPoolManager = GameManager.Instance.objectPoolManager;

            Character character = characterIndex switch
            {
                CharacterIndex.PENGGO => objectPoolManager.GetObject<CharacterPenggo>("PengGo"),
                CharacterIndex.FORTIS => objectPoolManager.GetObject<CharacterFortis>("Fortis"),
                CharacterIndex.MEOSK => objectPoolManager.GetObject<CharacterMeosk>("Meosk"),
                CharacterIndex.CUB_FOXY => objectPoolManager.GetObject<CharacterCubFoxy>("CubFoxy"),
                CharacterIndex.RAZY => objectPoolManager.GetObject<CharacterRazy>("Razy"),
                CharacterIndex.CARONA => objectPoolManager.GetObject<CharacterCaronaPenggo>("CaronaPengGo"),
                CharacterIndex.LEMMING_RICH => objectPoolManager.GetObject<CharacterLemmingRich>("LemmingRich"),
                CharacterIndex.DARKRAZY => objectPoolManager.GetObject<CharacterDarkRazy>("DarkRazy"),
                CharacterIndex.KIRING => objectPoolManager.GetObject<CharacterKiring>("Kiring"),
                CharacterIndex.PATROL => objectPoolManager.GetObject<CharacterPatrol>("Patrol"),
                CharacterIndex.EMPEROR_PENGGO => objectPoolManager.GetObject<CharacterEmperorPenggo>("EmperorPengGo"),
                CharacterIndex.WHITE_MAGICIAN_FOXY => objectPoolManager.GetObject<CharacterWhiteMagicianFoxy>("WhiteMagicianFoxy"),
                CharacterIndex.BLACK_MAGICIAN_FOXY => objectPoolManager.GetObject<CharacterBlackMagicianFoxy>("BlackMagicianFoxy"),
                CharacterIndex.BASIC_WOLF => objectPoolManager.GetObject<CharacterBasicWolf>("BasicWolf"),
                CharacterIndex.BEOSK => objectPoolManager.GetObject<CharacterBeosk>("Beosk"),
                CharacterIndex.WILLOW => objectPoolManager.GetObject<CharacterWillow>("Willow"),
                CharacterIndex.DAVI => objectPoolManager.GetObject<CharacterDavi>("Davi"),
                CharacterIndex.POLARIS => objectPoolManager.GetObject<CharacterPolaris>("Polaris"),
                CharacterIndex.KYLE => objectPoolManager.GetObject<CharacterKyle>("Kyle"),
                CharacterIndex.MARRIER => objectPoolManager.GetObject<CharacterMarrier>("Marrier"),
                CharacterIndex.DARKWOLF => objectPoolManager.GetObject<CharacterDarkWolf>("DarkWolf"),
                CharacterIndex.PUDDLE => objectPoolManager.GetObject<CharacterPuddle>("Puddle"),
                CharacterIndex.BARD => objectPoolManager.GetObject<CharacterBard>("Bard"),
                CharacterIndex.GUARDIAN => objectPoolManager.GetObject<CharacterGuardian>("Guardian"),
                CharacterIndex.TRICKSTER => objectPoolManager.GetObject<CharacterTrickster>("Trickster"),
                CharacterIndex.LEMMING_TOXIN => objectPoolManager.GetObject<CharacterLemmingToxin>("LemmingToxin"),
                CharacterIndex.HIFIVE => objectPoolManager.GetObject<CharacterHiFive>("HiFive"),
                CharacterIndex.YURI => objectPoolManager.GetObject<CharacterYuri>("Yuri"),
                CharacterIndex.OWLRUS => objectPoolManager.GetObject<CharacterOwlrus>("Owlrus"),
                CharacterIndex.FLORA => objectPoolManager.GetObject<CharacterFlora>("Flora"),
                CharacterIndex.VEGAS => objectPoolManager.GetObject<CharacterVegas>("Vegas"),
                CharacterIndex.HAMMERING => objectPoolManager.GetObject<CharacterHammering>("Hammering"),
                CharacterIndex.RIO => objectPoolManager.GetObject<CharacterRio>("Rio"),
                CharacterIndex.CHAMY => objectPoolManager.GetObject<CharacterChamy>("Chamy"),
                _ => null,
            };

            return character;
        }

        public Character GetRandomCharacter()
        {
            //함수의도 : 랜덤으로 캐릭터를 가져오기위한 함수
            //장착되어있는 슬롯에 캐릭터의 인덱스를 사용해서 캐릭터를 랜덤으로 가져옴
            //오브젝트 풀 매니저에 애들을 담아서 관리함

            int idx = Random.Range(0, 5);
            CharacterIndex characterIndex = (CharacterIndex)slot.slotCharacterIds[idx];

            ObjectPoolManager objectPoolManager = GameManager.Instance.objectPoolManager;

            Character character = characterIndex switch
            {
                CharacterIndex.PENGGO => objectPoolManager.GetObject<CharacterPenggo>("PengGo"),
                CharacterIndex.FORTIS => objectPoolManager.GetObject<CharacterFortis>("Fortis"),
                CharacterIndex.MEOSK => objectPoolManager.GetObject<CharacterMeosk>("Meosk"),
                CharacterIndex.CUB_FOXY => objectPoolManager.GetObject<CharacterCubFoxy>("CubFoxy"),
                CharacterIndex.RAZY => objectPoolManager.GetObject<CharacterRazy>("Razy"),
                CharacterIndex.CARONA => objectPoolManager.GetObject<CharacterCaronaPenggo>("CaronaPengGo"),
                CharacterIndex.LEMMING_RICH => objectPoolManager.GetObject<CharacterLemmingRich>("LemmingRich"),
                CharacterIndex.DARKRAZY => objectPoolManager.GetObject<CharacterDarkRazy>("DarkRazy"),
                CharacterIndex.KIRING => objectPoolManager.GetObject<CharacterKiring>("Kiring"),
                CharacterIndex.PATROL => objectPoolManager.GetObject<CharacterPatrol>("Patrol"),
                CharacterIndex.EMPEROR_PENGGO => objectPoolManager.GetObject<CharacterEmperorPenggo>("EmperorPengGo"),
                CharacterIndex.WHITE_MAGICIAN_FOXY => objectPoolManager.GetObject<CharacterWhiteMagicianFoxy>("WhiteMagicianFoxy"),
                CharacterIndex.BLACK_MAGICIAN_FOXY => objectPoolManager.GetObject<CharacterBlackMagicianFoxy>("BlackMagicianFoxy"),
                CharacterIndex.BASIC_WOLF => objectPoolManager.GetObject<CharacterBasicWolf>("BasicWolf"),
                CharacterIndex.BEOSK => objectPoolManager.GetObject<CharacterBeosk>("Beosk"),
                CharacterIndex.WILLOW => objectPoolManager.GetObject<CharacterWillow>("Willow"),
                CharacterIndex.DAVI => objectPoolManager.GetObject<CharacterDavi>("Davi"),
                CharacterIndex.POLARIS => objectPoolManager.GetObject<CharacterPolaris>("Polaris"),
                CharacterIndex.KYLE => objectPoolManager.GetObject<CharacterKyle>("Kyle"),
                CharacterIndex.MARRIER => objectPoolManager.GetObject<CharacterMarrier>("Marrier"),
                CharacterIndex.DARKWOLF => objectPoolManager.GetObject<CharacterDarkWolf>("DarkWolf"),
                CharacterIndex.PUDDLE => objectPoolManager.GetObject<CharacterPuddle>("Puddle"),
                CharacterIndex.BARD => objectPoolManager.GetObject<CharacterBard>("Bard"),
                CharacterIndex.GUARDIAN => objectPoolManager.GetObject<CharacterGuardian>("Guardian"),
                CharacterIndex.TRICKSTER => objectPoolManager.GetObject<CharacterTrickster>("Trickster"),
                CharacterIndex.LEMMING_TOXIN => objectPoolManager.GetObject<CharacterLemmingToxin>("LemmingToxin"),
                CharacterIndex.HIFIVE => objectPoolManager.GetObject<CharacterHiFive>("HiFive"),
                CharacterIndex.YURI => objectPoolManager.GetObject<CharacterYuri>("Yuri"),
                CharacterIndex.OWLRUS => objectPoolManager.GetObject<CharacterOwlrus>("Owlrus"),
                CharacterIndex.FLORA => objectPoolManager.GetObject<CharacterFlora>("Flora"),
                CharacterIndex.VEGAS => objectPoolManager.GetObject<CharacterVegas>("Vegas"),
                CharacterIndex.HAMMERING => objectPoolManager.GetObject<CharacterHammering>("Hammering"),
                CharacterIndex.RIO => objectPoolManager.GetObject<CharacterRio>("Rio"),
                CharacterIndex.CHAMY => objectPoolManager.GetObject<CharacterChamy>("Chamy"),
                _ => null,
            };

            return character;
        }

        public void AddOrRemoveSummonedCharacterList(Character character, bool isAdd)
        {
            if (isAdd)
            {
                summonedCharacters.Add(character);
            }
            else
            {
                summonedCharacters.Remove(character);
            }

            characterCount.Value = summonedCharacters.Count;
        }

        public void BroadcastCharacterList()
        {

        }

        public Character TestGetRandomCharacter(KeyCode keyCode)
        {
            int idx = keyCode switch
            {
                KeyCode.Q => 0,
                KeyCode.W => 1,
                KeyCode.E => 2,
                KeyCode.R => 3,
                KeyCode.T => 4,
                KeyCode.Alpha1 => 0,
                KeyCode.Alpha2 => 1,
                KeyCode.Alpha3 => 2,
                KeyCode.Alpha4 => 3,
                KeyCode.Alpha5 => 4,
                _ => throw new System.NotImplementedException(),
            };

            CharacterIndex characterIndex = (CharacterIndex)slot.slotCharacterIds[idx];

            ObjectPoolManager objectPoolManager = GameManager.Instance.objectPoolManager;

            Character character = characterIndex switch
            {
                CharacterIndex.PENGGO => objectPoolManager.GetObject<CharacterPenggo>("PengGo"),
                CharacterIndex.FORTIS => objectPoolManager.GetObject<CharacterFortis>("Fortis"),
                CharacterIndex.MEOSK => objectPoolManager.GetObject<CharacterMeosk>("Meosk"),
                CharacterIndex.CUB_FOXY => objectPoolManager.GetObject<CharacterCubFoxy>("CubFoxy"),
                CharacterIndex.RAZY => objectPoolManager.GetObject<CharacterRazy>("Razy"),
                CharacterIndex.CARONA => objectPoolManager.GetObject<CharacterCaronaPenggo>("CaronaPengGo"),
                CharacterIndex.LEMMING_RICH => objectPoolManager.GetObject<CharacterLemmingRich>("LemmingRich"),
                CharacterIndex.DARKRAZY => objectPoolManager.GetObject<CharacterDarkRazy>("DarkRazy"),
                CharacterIndex.KIRING => objectPoolManager.GetObject<CharacterKiring>("Kiring"),
                CharacterIndex.PATROL => objectPoolManager.GetObject<CharacterPatrol>("Patrol"),
                CharacterIndex.EMPEROR_PENGGO => objectPoolManager.GetObject<CharacterEmperorPenggo>("EmperorPengGo"),
                CharacterIndex.WHITE_MAGICIAN_FOXY => objectPoolManager.GetObject<CharacterWhiteMagicianFoxy>("WhiteMagicianFoxy"),
                CharacterIndex.BLACK_MAGICIAN_FOXY => objectPoolManager.GetObject<CharacterBlackMagicianFoxy>("BlackMagicianFoxy"),
                CharacterIndex.BASIC_WOLF => objectPoolManager.GetObject<CharacterBasicWolf>("BasicWolf"),
                CharacterIndex.BEOSK => objectPoolManager.GetObject<CharacterBeosk>("Beosk"),
                CharacterIndex.WILLOW => objectPoolManager.GetObject<CharacterWillow>("Willow"),
                CharacterIndex.DAVI => objectPoolManager.GetObject<CharacterDavi>("Davi"),
                CharacterIndex.POLARIS => objectPoolManager.GetObject<CharacterPolaris>("Polaris"),
                CharacterIndex.KYLE => objectPoolManager.GetObject<CharacterKyle>("Kyle"),
                CharacterIndex.MARRIER => objectPoolManager.GetObject<CharacterMarrier>("Marrier"),
                CharacterIndex.DARKWOLF => objectPoolManager.GetObject<CharacterDarkWolf>("DarkWolf"),
                CharacterIndex.PUDDLE => objectPoolManager.GetObject<CharacterPuddle>("Puddle"),
                CharacterIndex.BARD => objectPoolManager.GetObject<CharacterBard>("Bard"),
                CharacterIndex.GUARDIAN => objectPoolManager.GetObject<CharacterGuardian>("Guardian"),
                CharacterIndex.TRICKSTER => objectPoolManager.GetObject<CharacterTrickster>("Trickster"),
                CharacterIndex.LEMMING_TOXIN => objectPoolManager.GetObject<CharacterLemmingToxin>("LemmingToxin"),
                CharacterIndex.HIFIVE => objectPoolManager.GetObject<CharacterHiFive>("HiFive"),
                CharacterIndex.YURI => objectPoolManager.GetObject<CharacterYuri>("Yuri"),
                CharacterIndex.OWLRUS => objectPoolManager.GetObject<CharacterOwlrus>("Owlrus"),
                CharacterIndex.FLORA => objectPoolManager.GetObject<CharacterFlora>("Flora"),
                CharacterIndex.VEGAS => objectPoolManager.GetObject<CharacterVegas>("Vegas"),
                CharacterIndex.HAMMERING => objectPoolManager.GetObject<CharacterHammering>("Hammering"),
                CharacterIndex.RIO => objectPoolManager.GetObject<CharacterRio>("Rio"),
                CharacterIndex.CHAMY => objectPoolManager.GetObject<CharacterChamy>("Chamy"),
                _ => null,
            };

            return character;
        }
        
        public Character GetFixedCharacter(CharacterIndex characterIndex)
        {
            ObjectPoolManager objectPoolManager = GameManager.Instance.objectPoolManager;

            Character character = characterIndex switch
            {
                CharacterIndex.PENGGO => objectPoolManager.GetObject<CharacterPenggo>("PengGo"),
                CharacterIndex.FORTIS => objectPoolManager.GetObject<CharacterFortis>("Fortis"),
                CharacterIndex.MEOSK => objectPoolManager.GetObject<CharacterMeosk>("Meosk"),
                CharacterIndex.CUB_FOXY => objectPoolManager.GetObject<CharacterCubFoxy>("CubFoxy"),
                CharacterIndex.RAZY => objectPoolManager.GetObject<CharacterRazy>("Razy"),
                CharacterIndex.CARONA => objectPoolManager.GetObject<CharacterCaronaPenggo>("CaronaPengGo"),
                CharacterIndex.LEMMING_RICH => objectPoolManager.GetObject<CharacterLemmingRich>("LemmingRich"),
                CharacterIndex.DARKRAZY => objectPoolManager.GetObject<CharacterDarkRazy>("DarkRazy"),
                CharacterIndex.KIRING => objectPoolManager.GetObject<CharacterKiring>("Kiring"),
                CharacterIndex.PATROL => objectPoolManager.GetObject<CharacterPatrol>("Patrol"),
                CharacterIndex.EMPEROR_PENGGO => objectPoolManager.GetObject<CharacterEmperorPenggo>("EmperorPengGo"),
                CharacterIndex.WHITE_MAGICIAN_FOXY => objectPoolManager.GetObject<CharacterWhiteMagicianFoxy>("WhiteMagicianFoxy"),
                CharacterIndex.BLACK_MAGICIAN_FOXY => objectPoolManager.GetObject<CharacterBlackMagicianFoxy>("BlackMagicianFoxy"),
                CharacterIndex.BASIC_WOLF => objectPoolManager.GetObject<CharacterBasicWolf>("BasicWolf"),
                CharacterIndex.BEOSK => objectPoolManager.GetObject<CharacterBeosk>("Beosk"),
                CharacterIndex.WILLOW => objectPoolManager.GetObject<CharacterWillow>("Willow"),
                CharacterIndex.DAVI => objectPoolManager.GetObject<CharacterDavi>("Davi"),
                CharacterIndex.POLARIS => objectPoolManager.GetObject<CharacterPolaris>("Polaris"),
                CharacterIndex.KYLE => objectPoolManager.GetObject<CharacterKyle>("Kyle"),
                CharacterIndex.MARRIER => objectPoolManager.GetObject<CharacterMarrier>("Marrier"),
                CharacterIndex.DARKWOLF => objectPoolManager.GetObject<CharacterDarkWolf>("DarkWolf"),
                CharacterIndex.PUDDLE => objectPoolManager.GetObject<CharacterPuddle>("Puddle"),
                CharacterIndex.BARD => objectPoolManager.GetObject<CharacterBard>("Bard"),
                CharacterIndex.GUARDIAN => objectPoolManager.GetObject<CharacterGuardian>("Guardian"),
                CharacterIndex.TRICKSTER => objectPoolManager.GetObject<CharacterTrickster>("Trickster"),
                CharacterIndex.LEMMING_TOXIN => objectPoolManager.GetObject<CharacterLemmingToxin>("LemmingToxin"),
                CharacterIndex.HIFIVE => objectPoolManager.GetObject<CharacterHiFive>("HiFive"),
                CharacterIndex.YURI => objectPoolManager.GetObject<CharacterYuri>("Yuri"),
                CharacterIndex.OWLRUS => objectPoolManager.GetObject<CharacterOwlrus>("Owlrus"),
                CharacterIndex.FLORA => objectPoolManager.GetObject<CharacterFlora>("Flora"),
                CharacterIndex.VEGAS => objectPoolManager.GetObject<CharacterVegas>("Vegas"),
                CharacterIndex.HAMMERING => objectPoolManager.GetObject<CharacterHammering>("Hammering"),
                CharacterIndex.RIO => objectPoolManager.GetObject<CharacterRio>("Rio"),
                CharacterIndex.CHAMY => objectPoolManager.GetObject<CharacterChamy>("Chamy"),
                _ => null,
            };

            return character;
        }

        public void TestSummonCharacter(KeyCode keyCode)
        {
            if (!GridManager.Instance.IsPossibleSummon()) return;
            summonCount++;

            Character character = TestGetRandomCharacter(keyCode);
            Glacier glacier = GridManager.Instance.PossibleSummonIdx();
            glacier.isImpossibleSummon = true;
            character.Initialize(glacier, 0);
        }

        public void TestSummonCharacterFour(KeyCode keyCode)
        {
            if (!GridManager.Instance.IsPossibleSummon()) return;
            summonCount++;

            Character character = TestGetRandomCharacter(keyCode);
            Glacier glacier = GridManager.Instance.PossibleSummonIdx();
            glacier.isImpossibleSummon = true;
            character.Initialize(glacier, 4);
            //character.Initialize(glacier, 3);
            
            GameManager.Instance.characterSpawner.SetQuestCharacterStarCount(character.starGradeIndex);
        }
        
        public void SummonFixedCharacter(CharacterIndex characterIndex, int starGrade)
        {
            if (!GridManager.Instance.IsPossibleSummon()) return;
            summonCount++;

            Character character = GetFixedCharacter(characterIndex);
            Glacier glacier = GridManager.Instance.PossibleSummonIdx();
            glacier.isImpossibleSummon = true;
            character.Initialize(glacier, starGrade);
        }

        public void SummonFixedCharacter(CharacterIndex characterIndex, Glacier glacier, int starGrade)
        {
            if (!GridManager.Instance.IsPossibleSummon()) return;
            summonCount++;
            Debug.Log("Summon Fixed Monster Idx : " + characterIndex);
            Character character = GetFixedCharacter(characterIndex);
            // Glacier glacier = GridManager.Instance.PossibleSummonIdx();
            glacier.isImpossibleSummon = true;
            character.Initialize(glacier, starGrade);
        }

        //UI Controller
        public void SummonCharacter()
        {
            if (!GridManager.Instance.IsPossibleSummon()) return;
            summonCount++;
            Character character = GetRandomCharacter();
            Glacier glacier = GridManager.Instance.PossibleSummonIdx();
            glacier.isImpossibleSummon = true;
            character.Initialize(glacier, 0);
        }

        public void TutorialSummonCharacter(CharacterIndex characterIndex, int glacierIdx)
        {
            if (!GridManager.Instance.IsPossibleSummon()) return;
            summonCount++;

            Character character = TutorialCharacter(characterIndex);

            Glacier glacier = GridManager.Instance.glaciersTiles[glacierIdx];
            glacier.isImpossibleSummon = true;
            character.Initialize(glacier, 0, true);
        }

        public void SummonSynthesisCharacter(int glacierIdx, int starGradeIndex)
        {
            Character character = GetRandomCharacter();
            
            Glacier glacier = GridManager.Instance.glaciersTiles[glacierIdx];

            ++currentSpawnData.characterMerge;
            ++totalSpawnData.characterMerge;
            //GameManager.Instance.QuestTypeUpdateCount(QuestMissionType.MERGE);
            
            //todo dongmin 이후 대전모드에서 추가될 소환중에 바로4성이 나오는로직 추가되면 한번더 추가해줘야함
            GameManager.Instance.characterSpawner.SetQuestCharacterStarCount(starGradeIndex);

            character.Initialize(glacier, starGradeIndex);
        }

        //Test용 합성 시 원하는 캐릭터 생성하기위함
        public void SummonSynthesisWantCharacter<T>(int glacierIdx, int starGradeIndex, string characterName) where T : Character
        {
            Character character = GameManager.Instance.objectPoolManager.GetObject<T>(characterName);

            Glacier glacier = GridManager.Instance.glaciersTiles[glacierIdx];
            character.Initialize(glacier, starGradeIndex);
        }

        public void TutorialSummonSynthesisCharacter(int glacierIdx, int starGradeIndex)
        {
            CharacterIndex characterIndex = TutorialManager.Instance.GetSummonData().characterIndex;

            Character character = TutorialCharacter(characterIndex);

            Glacier glacier = GridManager.Instance.glaciersTiles[glacierIdx];
            character.Initialize(glacier, starGradeIndex);
        }

        
        public int GetCharacterLevel(CharacterIndex characterIndex)
        {
            int level = 0;
            for (int i = 0; i < currentDecData.Count; i++)
            {
                if (currentDecData[i].characterIndex == characterIndex)
                {
                    level = currentDecData[i].characterLevel;
                    break;
                }
            }
            return level;
        }

        public void SummonSubCharacter(Glacier glacier)
        {
            CharacterIndex characterIndex = (CharacterIndex)slot.slotCharacterIds[5];

            Character character = characterIndex switch
            {
                CharacterIndex.POLAFIZZ => GameManager.Instance.objectPoolManager.GetObject<CharacterPolaFizz>("PolaFizz"),
                CharacterIndex.SHAPID => GameManager.Instance.objectPoolManager.GetObject<CharacterShapid>("Shapid"),
                CharacterIndex.KANADE => GameManager.Instance.objectPoolManager.GetObject<CharacterKanade>("Kanade"),
                CharacterIndex.HIGHNICKEL => GameManager.Instance.objectPoolManager.GetObject<CharacterHighnickel>("Highnickel"),
                CharacterIndex.TURTLIA => GameManager.Instance.objectPoolManager.GetObject<CharacterTurtlia>("Turtlia"),
                CharacterIndex.KORRA => GameManager.Instance.objectPoolManager.GetObject<CharacterKorra>("Korra"),
                CharacterIndex.LULU => GameManager.Instance.objectPoolManager.GetObject<CharacterLulu>("Lulu"),
                _ => null
            };
            character.Initialize(glacier, 0);
        }

        public void RelocationCharacter()
        {
            //빙하 갯수만큼 순회를 한다   
            for (int i = 0; i < GridManager.Instance.glaciersTiles.Count; i++)
            {
                //초기화 작업으로 전부 소환가능으로 일단 바꿔줌
                GridManager.Instance.glaciersTiles[i].isImpossibleSummon = false;
            }

            //안깨진 빙하인덱스를 얻기위한 작업
            List<int> data = GridManager.Instance.ConstructibleTiles();

            int random1, random2;
            int temp;

            //받아온 안깨진 빙하 인덱스를 순회하며 랜덤으로 인덱스를 뒤섞어주기 위함
            for (int i = 0; i < data.Count; ++i)
            {
                random1 = Random.Range(0, data.Count);
                random2 = Random.Range(0, data.Count);

                temp = data[random1];
                data[random1] = data[random2];
                data[random2] = temp;
            }

            //소환된 캐릭터를 얻어와서 순회
            for (int i = 0; i < summonedCharacters.Count; i++)
            {
                //안 깨진 빙하를 순회
                for (int j = 0; j < data.Count; j++)
                {
                    Glacier glacier = GridManager.Instance.glaciersTiles[data[j]];

                    //안깨진 빙하 인덱스와 소환된 캐릭터의 빙하 인덱스가 다르다면
                    if (data[j] != summonedCharacters[i].glacierIdx)
                    {
                        //소환된 캐릭터의 빙하 인덱스를 뒤섞어준 안깨진 빙하 인덱스로 바꿔줌
                        summonedCharacters[i].glacierIdx = data[j];

                        //뒤섞여진 빙하 인덱스를 이용해서 현재 빙하를 얻어옴

                        //바꿔준 빙하를 소환 불가능으로 해줌
                        //이유 : 현재 빙하위에 캐릭터가 있으니 소환하지말라는 의미로 지정
                        glacier.isImpossibleSummon = true;

                        //캐릭터의 위치셋팅과 빙하 인덱스 셋팅, 캐릭터 이름 설정을 해주는 함수
                        summonedCharacters[i].Reposition(glacier);

                        //레벨업 파티클 소환
                        summonedCharacters[i].CharacterParticle(true);

                        if (summonedCharacters[i].characterInterface != null)
                        {
                            summonedCharacters[i].characterInterface.transform.position = summonedCharacters[i].transform.localPosition;
                        }

                        //사용된 빙하 인덱스를 제거를해서 동일한 인덱스를 적용 못하게하기위해서 삭제
                        data.Remove(data[j]);
                        break;
                    }
                }
            }

            //위에 로직을 한번읽고 와야 이해 가능한 주석
            //캐릭터 생성 직후 리로케이션을 돌리면 위 조건에 안들어가는 data는 그대로 isImpossibleSummon = false가 되서 캐릭터가 생성됬는데 설치가 한번더 가능하게 된다
            //이러한 조건을 막아주기 위함이다
            //근대 왜 저기 조건 else문에 작성을 안하느냐
            //sell Character를하면 isImpossibleSummon 를 False로 바꿔줘야하는데 else문에 작성하면 모든 glacier.isImpossibleSummon를 True로 해주기 때문에
            //설치가 불가능한 지역이 된다 -> 궁금하면 위에 else조건에 glacier.isImpossibleSummon = true를 넣어보자
            for (int i = 0; i < summonedCharacters.Count; i++)
            {
                for (int j = 0; j < data.Count; ++j)
                {
                    if (summonedCharacters[i].glacierIdx == data[j])
                    {
                        Glacier glacier = GridManager.Instance.glaciersTiles[data[j]];
                        glacier.isImpossibleSummon = true;
                    }
                }
            }

            //소환 불가능 빙하를 다시 셋팅해주기위한 함수
            GridManager.Instance.PostRelocationProcess();

            //리포지션이 되면 콜백함수를 호출시키기 위한 작업
            repositionCount.Value += 1;

            //캐릭터 개수가 변경되면 콜백함수를 호출시키기 위한 작업
            characterCount.Value = summonedCharacters.Count;
        }

        public void DestroyedTile(int idx)
        {
            //함수 용도 : 현재 랜덤값의 인덱스가 들어왔는데 그 랜덤 빙하위에
            //소환된 캐릭터가 있는지 검사하는것 있다면 타일제거와 캐릭터를 지워줘야하기때문

            //현재 소환된 캐릭터를 순회한다
            for (int i = 0; i < summonedCharacters.Count; i++)
            {
                //소환된 캐릭터의 빙하 인덱스와 매개변수로 받아온 인덱스가 동일하다면
                //소환된 캐릭터에 디스트로이 타일을 해준다
                if (summonedCharacters[i].glacierIdx == idx)
                {
                    summonedCharacters[i].DestroyedTile();
                }
            }
        }

        public bool ShieldTile(int idx)
        {
            bool isShield = false;

            for (int i = 0; i < summonedCharacters.Count; i++)
            {
                //소환된 캐릭터의 빙하 인덱스와 매개변수로 받아온 인덱스가 동일하다면
                //소환된 캐릭터에 디스트로이 타일을 해준다
                if (summonedCharacters[i].glacierIdx == idx)
                {
                    isShield = summonedCharacters[i].ShieldTile();
                }
            }

            return isShield;
        }

    }
}
