using Framework.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using Framework.Util;
using SimpleJSON;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using Newtonsoft.Json;

namespace Framework.Game.Defense
{
    public enum GameState
    {
        WAIT,
        PLAY,
        END,
        WAIT_BOSS_WAVE,
        GAME_OVER
    }

    public enum GameMode
    {
        SINGLE,
        BATTLE,
        TUTORIAL
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public static WaitForSeconds waitForSeconds = new(1);

        public delegate void OnCompleteWave();
        public static OnCompleteWave onCompleteWave;

        public Transform gameWorld;
        public GameState gameState;
        public GameMode gameMode;
        public float timeCount;
        public int waveReadyTimer = 3;
        public string completedMission = "";
        public string totalCompletedMission = "";

        public bool isPaused = false;

        public RewardsTableData rewardData;
        public RewardsTableData bossRewardData;
        public RewardRuleList rewardRuleList;
        public Animator anim_Transition;
        public Animation anim_CloudSequence;
        public Animation anim_GameWorldSequence;
        public Dictionary<string, RewardRule> dic_RewardRules = new();
        public BuffManager buffManager;
        public MissionManager missionManager;
        public CharacterSpawner characterSpawner;
        public ObjectPoolManager objectPoolManager;
        public MonsterSpawner monsterSpawner;
        public RenderSortManager renderSortManager;
        public IntReactiveProperty gemValue;
        public ObscuredInt Gem { get; set; }
        public ObscuredInt Go { get; set; }
        public ObscuredInt currentRewardGoValue;
        public ObscuredInt playId;
        public ObscuredInt waveIdx;
        public ObscuredInt interestGem;
        public ObscuredInt textGem;
        public ObscuredInt summonCost;
        public ObscuredInt summonCount;
        public ObscuredInt bossSummonCost;
        public ObscuredInt relocationCost;
        public ObscuredFloat relocationCostDiscount = 0;
        public ObscuredInt relocationCount;
        public ObscuredFloat missionRewardBuffValue = 0;
        public ObscuredFloat bossRewardBuffValue = 0;
        public ObscuredFloat monsterRewardBuffValue = 0;
        public ObscuredFloat upgradeCostBuffValue = 0;
        public bool isTestMode = false;
        public bool isGameOver = false;
        public int TotalGo = 0;
        public ObscuredInt bossKillCount;

        private Coroutine openPausePopupCorou = null;

        private bool isPlayingIntro = false;

        private void Awake()
        {
            Instance = this;

            ObscuredCheatingDetector.StartDetection(OnCheterDetected);
        }

        private void OnCheterDetected()
        {
            _ = NetworkManager.Instance.AbusingRecord("Cheat GameManager");
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            anim_Transition.gameObject.SetActive(true);
            anim_CloudSequence.gameObject.SetActive(true);
            isPlayingIntro = true;

            onCompleteWave = null;

            ObjectPoolManager.OnCompleteAssetLoad = () =>
            {
                TutorialManager.Instance.Initiailize(!UserInfoManager.Instance.userState.finishedTutorial);
                StartCoroutine(GameStartSequence());
            };

#if UNITY_EDITOR
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.C))
                .Subscribe(_ => ChangeGem(1000));
            this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.P))
            .Subscribe(_ => monsterSpawner.ClearMonster());
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.B))
                .Subscribe(_ => BossWaveSeqeunce(5, UserInfoManager.Instance.nickname));
#endif
        }

        public IEnumerator GameStartSequence()
        {
            GetRewardData();
            Debug.Log(gameMode);

            if (!UserInfoManager.Instance.userState.finishedTutorial && gameMode == GameMode.SINGLE)
            {
                gameMode = GameMode.TUTORIAL;
            }

            anim_Transition.SetTrigger("TransitionOut");

            yield return new WaitForSeconds(2);

            anim_CloudSequence.Rewind();
            anim_CloudSequence.Play();

            float temp = Camera.main.orthographicSize;
            Camera.main.orthographicSize = temp * 1.3f;
            Camera.main.DOOrthoSize(temp, 1.3f);

            yield return new WaitForSeconds(1f);

            isPlayingIntro = false;

            switch (gameMode)
            {
                case GameMode.SINGLE:
                    Initialize();
                    yield return new WaitForSeconds(1f);
                    CountStart();
                    break;
                case GameMode.BATTLE:
                    BattleInitialize();
                    break;
                case GameMode.TUTORIAL:
                    TutorialManager.Instance.Initialize();
                    TutorialInitialize();
                    break;
                default:

                    break;
            }

            // yield return new WaitForSeconds(1f);

            // if (UserInfoManager.Instance.userState.finishedTutorial)
            // {
            //     CountStart();
            // }
        }

        public static bool GetGameState()
        {
            bool isGaming = Instance.gameState == GameState.PLAY;
            return isGaming;
        }

        public void TutorialInitialize()
        {
            relocationCount = 0;
            textGem = 0;
            summonCount = 0;
            Go = 0;
            IncreaseRelocationCost();
            ChangeGem(ConfigData.START_GEM);
            UIManager.Instance.TutorialInitialize();
            relocationCost = 0;
            summonCost = ConfigData.GEM_SUMMON_FIRST;
            waveReadyTimer = ConfigData.WAVE_READY_TIMER;
            UIManager.Instance.ChangeValueSequnce(summonCost, 0, UIManager.Instance.text_SummonCharacterCost);
        }

        public void BattleInitialize()
        {
            relocationCount = 0;
            textGem = 0;
            summonCount = 0;
            Go = 0;
            IncreaseRelocationCost();
            ChangeGem(ConfigData.START_GEM);
            UIManager.Instance.Initialize(true);
            relocationCost = 0;
            summonCost = ConfigData.GEM_SUMMON_FIRST;
            waveReadyTimer = ConfigData.WAVE_READY_TIMER;
            UIManager.Instance.ChangeValueSequnce(summonCost, 0, UIManager.Instance.text_SummonCharacterCost);
            buffManager.Initialize();
            renderSortManager.Initialized();
            NetworkConnect.Instance.networkGameManager.RpcInitializeComplete(NetworkConnect.Instance.playerIdx);
        }

        public void Initialize()
        {
            relocationCount = 0;
            textGem = 0;
            summonCount = 0;
            Go = 0;

            bossKillCount = 0;
            completedMission = "";
            totalCompletedMission = "";

            IncreaseRelocationCost();
            ChangeGem(ConfigData.START_GEM);
            UIManager.Instance.Initialize(false);
            relocationCost = 0;
            summonCost = ConfigData.GEM_SUMMON_FIRST;
            waveReadyTimer = ConfigData.WAVE_READY_TIMER;
            UIManager.Instance.ChangeValueSequnce(summonCost, 0, UIManager.Instance.text_SummonCharacterCost);
            buffManager.Initialize();
            renderSortManager.Initialized();
        }

        public async void GetRewardData()
        {
            rewardData = await DataLoadManager.Instance.GetDataAsyncBinary<RewardsTableData>("gemRewardData");
            bossRewardData = await DataLoadManager.Instance.GetDataAsyncBinary<RewardsTableData>("BossRewardData");

            Debug.Log($"Get reward data: {JsonConvert.SerializeObject(rewardData)}");
            Debug.Log($"Get boss reward data: {JsonConvert.SerializeObject(bossRewardData)}");

            switch (gameMode)
            {
                case GameMode.SINGLE:
                    for (int i = 0; i < UserInfoManager.Instance.rewardRuleList.rewardRules.Length; i++)
                    {
                        RewardRule rewardRule = UserInfoManager.Instance.rewardRuleList.rewardRules[i];
                        dic_RewardRules.Add(rewardRule.code, rewardRule);
                    }
                    playId = UserInfoManager.Instance.playId;
                    break;
                case GameMode.BATTLE:
                    RewardRuleList rewardRuleList = await DataLoadManager.Instance.GetDataAsyncBinary<RewardRuleList>("testWaveData");

                    Debug.Log($"Get battle reward rule list: {JsonConvert.SerializeObject(rewardRuleList)}");
                    for (int i = 0; i < rewardRuleList.rewardRules.Length; i++)
                    {
                        RewardRule rewardRule = rewardRuleList.rewardRules[i];
                        dic_RewardRules.Add(rewardRule.code, rewardRule);
                    }
                    break;
                default:

                    break;
            }
        }

        public void MonsterGemReward(int idx, bool isLoss, bool isBoss)
        {
            Debug.Log($"Monster Gem Reward: IDX: {idx} - IsLoss: {isLoss} - isBoss: {isBoss}");
            RewardsTableData data = isBoss ? bossRewardData : rewardData;

            if (idx >= data.rewards.Length)
            {
                idx = isBoss ? data.rewards.Length - 1 : data.rewards.Length - 1;
            }
            int value = 0;
            if (!isBoss)
                value = (int)(data.rewards[idx].rewardValue + data.rewards[idx].rewardValue * monsterRewardBuffValue);
            else
                value = (int)(data.rewards[idx].rewardValue + data.rewards[idx].rewardValue * bossRewardBuffValue);

            if (isLoss)
            {
                float temp = isBoss ? ConfigData.WAVE_BOSS_LOSS : ConfigData.WAVE_MONSTER_LOSS;
                float lossValue = value * temp;

                ChangeGem((int)lossValue);
            }
            else
            {
                ChangeGem(value);
            }
        }

        public void MissionGemReward()
        {

        }

        public void TutorialSummon(CharacterIndex characterIndex, int glacierIdx)
        {
            if (Gem >= 10)
            {
                characterSpawner.TutorialSummonCharacter(characterIndex, glacierIdx);
                ChangeGem(-10);
                summonCount++;
                bool isPossible = GridManager.Instance.IsPossibleSummon();
                UIManager.Instance.SetPossibleSummon(isPossible);
            }
            else
            {
                //Debug.Log("Not Enough Gem");
            }
        }

        public void SummonFixedCharacter(CharacterIndex characterIndex, int starGrade)
        {
            characterSpawner.SummonFixedCharacter(characterIndex, starGrade);
            summonCount++;
            bool isPossible = GridManager.Instance.IsPossibleSummon();
            UIManager.Instance.SetPossibleSummon(isPossible);
        }

        public void SummonCharacter()
        {
            //젬이 10개 이상이면 조건성립
            if (Gem >= 10)
            {
                characterSpawner.SummonCharacter();
                //BroadCast 
                ChangeGem(-10);
                summonCount++;

                //todo dongmin
                ++characterSpawner.currentSpawnData.summonCharacter;
                ++characterSpawner.totalSpawnData.summonCharacter;

                bool isPossible = GridManager.Instance.IsPossibleSummon();
                //캐릭터 생성 가능 상테인지 UI버튼 클릭과 아이콘 변경
                UIManager.Instance.SetPossibleSummon(isPossible);
            }
            else
            {
                //Debug.Log("Not Enough Gem");
            }
        }

        public void SummonBoss()
        {
            monsterSpawner.SummonBoss();
        }

        public void TutorialSummonBoss()
        {
            monsterSpawner.TutorialSummonBoss();
        }

        public void DecreaseRelocationCost(float value)
        {
            if (relocationCostDiscount == 0)
            {
                relocationCostDiscount = value;
            }
            else
            {
                relocationCostDiscount = relocationCostDiscount + relocationCostDiscount * value;
            }
        }

        public void IncreaseRelocationCost()
        {
            int temp = relocationCost;
            int cost = ConfigData.GEM_RELOCATION * relocationCount;
            relocationCost = (int)(cost - cost * relocationCostDiscount);
            UIManager.Instance.ChangeValueSequnce(relocationCost, temp, UIManager.Instance.text_RelocationCost);
            CalcPossibleCostAction();
        }

        public void Relocation()
        {
            //소환된 캐릭터가 0이라면 리턴
            if (characterSpawner.summonedCharacters.Count == 0) return;

            //젬 갯수가 현재 리로케이션 코스트보다 크거나 같다면 진행
            if (Gem >= relocationCost)
            {
                //현재 가지고있는 젬 갯수를 코스트 만큼 떨어뜨림
                ChangeGem(-1 * relocationCost);

                //캐릭터를 위치를 안깨진 빙하위에 재셋팅해주기위한 함수
                characterSpawner.RelocationCharacter();

                //재사용 한만큼 코스트를 증가시켜주기위한 작업
                //ConfigData.GEM_RELOCATION * relocationCount 공식값을 적용해야하기에
                //relocationCount 를 증가시켜줌
                relocationCount++;

                //코스트 값을 재셋팅해주고 UI에 다음에 사용될 리로케이션 젬을 표시해주기위한 함수
                IncreaseRelocationCost();
            }
            else
            {
                Debug.Log("Not Enough Gem");
            }
        }

        public void UpgradeLevel(CharacterIndex characterIndex, int cost, UnityAction action)
        {
            int upgradeCost = (int)(cost - cost * upgradeCostBuffValue);
            if (Gem >= upgradeCost)
            {
                characterSpawner.UpgradeLevel(characterIndex);
                //UIManager.Instance.SetUpgradeAnim(characterIndex);
                SoundManager.Instance.PlaySound(SoundKey.SF_UPGRADE);
                ChangeGem(-1 * upgradeCost);

                action();
            }
            else
            {
                Debug.Log("Not Enough Gem");
            }
            CalcPossibleCostAction();
        }

        public void CheckUpgradeLevel(CharacterIndex characterIndex, int cost, int levelValue, UnityAction action)
        {
            if (Gem >= cost)
            {
                CharacterData characterData = DataManager.Instance.dic_CharacterData[characterIndex];
                UpgradeType type = GetCharacterUpgradeStatus(characterData, levelValue);

                switch (type)
                {
                    case UpgradeType.SUCCESS:
                        characterSpawner.UpgradeLevel(characterIndex);
                        SoundManager.Instance.PlaySound(SoundKey.SF_UPGRADE);
                        action?.Invoke();
                        break;
                    case UpgradeType.FAILED:
                        characterSpawner.FailedUpgrade(characterIndex, false);
                        SoundManager.Instance.PlaySound(SoundKey.SF_HAMMERING_UPGRADE_FAILED);
                        break;
                    case UpgradeType.RESET:
                        characterSpawner.FailedUpgrade(characterIndex, true);
                        SoundManager.Instance.PlaySound(SoundKey.SF_HAMMERING_UPGRADE_BROKEN);
                        break;
                }
                ChangeGem(-1 * cost);
            }
            else
            {
                Debug.Log("Not Enough Gem");
            }

            CalcPossibleCostAction();
        }

        public static UpgradeType GetCharacterUpgradeStatus(CharacterData data, int levelValue)
        {
            UpgradeType type = UpgradeType.FAILED;
            bool isSuccess;

            switch (data.characterIndex)
            {
                case CharacterIndex.HAMMERING:
                    float successValue = data.characterUniqueValue[4] * Mathf.Pow(data.powerFactor[1], levelValue - 1) * 100;
                    float randomValue = Random.Range(0f, 100f);
                    isSuccess = randomValue <= successValue;
                    if (isSuccess)
                    {
                        type = UpgradeType.SUCCESS;
                    }
                    else
                    {
                        float resetValue = (data.characterUniqueValue[5] + (levelValue - 1) * data.powerFactor[2]) * 100;
                        float resetRandomValue = Random.Range(0f, 100f);
                        type = resetRandomValue <= resetValue ? UpgradeType.RESET : UpgradeType.FAILED;
                    }
                    break;
            }

            //Debug.Log($"레벨 {levelValue}");
            //float temp1 = data.characterUniqueValue[4] * Mathf.Pow(data.powerFactor[1], levelValue - 1) * 100;
            //Debug.Log($"업그레이드 성공 확률 {temp1}");
            //float temp2 = (data.characterUniqueValue[5] + (levelValue - 1) * data.powerFactor[2]) * 100;
            //Debug.Log($"업그레이드 초기화 확률 {temp2}");

            return type;
        }



        public void IncreaseSummonCost()
        {
            int temp = summonCost;
            summonCost = summonCost + summonCount + ConfigData.GEM_SUMMON_FACTOR;
            UIManager.Instance.ChangeValueSequnce(summonCost, temp, UIManager.Instance.text_SummonCharacterCost);
            CalcPossibleCostAction();
        }

        public void ChangeGem(int changeValue)
        {
            int temp = Gem;
            Gem += changeValue;
            gemValue.Value = Gem;
            CalcPossibleCostAction();
            CalcInterestValue();
            UIManager.Instance.ChangeValueSequnce(Gem, temp, UIManager.Instance.text_Gem);
        }

        public void ChangeGo(int changeValue)
        {
            int temp = Go;
            Go += changeValue;

            UIManager.Instance.ChangeValueSequnce(Go, temp, UIManager.Instance.text_Go);
        }

        public static Transform GetGameWorldTransform()
        {
            return Instance.gameWorld;
        }

        public void CalcPossibleCostAction()
        {
            bool isPossibleSummon = summonCost <= Gem && GridManager.Instance.IsPossibleSummon();
            UIManager.Instance.SetPossibleSummon(isPossibleSummon);

            bool isPossibleRelocation = relocationCost <= Gem;
            UIManager.Instance.SetPossibleRelocation(isPossibleRelocation);
            UIManager.Instance.SetPossibleUpgrade(Gem);
        }

        public void CalcInterestValue()
        {
            int value = Gem;

            if (value > ConfigData.GEM_INTEREST_LIMIT)
            {
                value = ConfigData.GEM_INTEREST_LIMIT;
            }

            float temp = value * ConfigData.GEM_INTEREST_RATE;
            interestGem = (int)temp;

            UIManager.Instance.SetInterestValue(interestGem);
        }

        public void InterestPayment()
        {
            if (interestGem <= 0) return;
            SoundManager.Instance.PlaySound(SoundKey.SF_INTEREST);
            UIManager.Instance.SetInterestParticle(interestGem);
            ChangeGem(interestGem);
        }

        public void CompleteMission(int missionIndex)
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_MISSION_COMPLETE);

            int go = dic_RewardRules[$"MISSION_{missionIndex}"].rewardGo;
            int goValue = (int)(go + go * missionRewardBuffValue);

            ++missionManager.currentMissionClear;
            ++missionManager.totalMissionClear;

            ChangeGo(goValue);

            if (string.IsNullOrEmpty(completedMission))
            {
                completedMission = missionIndex.ToString();
            }
            else
            {
                completedMission += $",{missionIndex}";
            }

            if (gameMode == GameMode.TUTORIAL)
            {
                TutorialManager.Instance.nextSequence?.Invoke();
            }
        }

        public void GameOverHome()
        {
            if (isGameOver) return;
            isGameOver = true;
            gameState = GameState.GAME_OVER;
            monsterSpawner.GameOverHome();

            GameFinishedSend();
        }

        public void GameOver()
        {
            switch (gameMode)
            {
                case GameMode.SINGLE:
                    SingleGameOver();
                    break;
                case GameMode.BATTLE:
                    BattleGameOver();
                    break;
                case GameMode.TUTORIAL:
                    break;
            }
        }

        public void SingleGameOver()
        {
            if (isGameOver) return;
            isGameOver = true;

            //수정사항
            //게임 홈화면에서 라운드 수 표기 오류
            monsterSpawner.isWaveStart = false;

            gameState = GameState.GAME_OVER;
            SoundManager.Instance.PlaySound(SoundKey.SF_GAMEOVER);

            // 룰렛 호출
            if (DataManager.Instance.RouletteTableData.Count > 0)
            {
                _ = NetworkManager.Instance.GetRouletteGroup(playId, (ReqRouletteGroupData data) =>
                {
                    UIManager.Instance.RoulettePopup(data);
                }, () =>
                {
                    GameFinishedSend();
                });
            }
            else
                GameFinishedSend();
        }

        public void BattleGameOver()
        {
            if (isGameOver) return;
            isGameOver = true;
            gameState = GameState.GAME_OVER;
            SoundManager.Instance.PlaySound(SoundKey.SF_GAMEOVER);
            NetworkConnect.Instance.networkGameManager.Rpc_GameOver(NetworkConnect.Instance.playerIdx, waveIdx);

            UIManager.Instance.battleResultPopup.SetResultInfo(0, waveIdx);
            Debug.Log("Battle Game Over");
        }

        public void GameFinishedSend(int bounsWave = 0)
        {
            Dictionary<string, string> gameEndData = new()
            {
                { "id", playId.ToString() },
                { "userId" , UserInfoManager.Instance.userId },
                { "requestGo", Go.ToString() },
                { "lastWave", waveIdx.ToString() },
                { "slotNumber", UserSlotManager.Instance.GetFocusIdx().ToString() },
                { "appVersion", Application.version}
            };

            #region 퀘스트 결과 총횟수 (어뷰징이 극심하면 주석 해제 후 다시 로직구성)
            // -- 추가 --
            // //퀘스트 정보
            // //몬스터 처치 총 횟수
            // //보스 몬스터 처치 총 횟수
            // //캐릭터 소환 총 횟수
            // //캐릭터 합성 총 횟수
            // //미션 클리어 총 횟수
            // //4성 보유 총 횟수
            // //캐릭터 업그레이드 총 횟수
            // ---------
            // Dictionary<string, string> gameEndDataV2 = new()
            // {
            //     { "id", playId.ToString() },
            //     { "userId", UserInfoManager.Instance.userId },
            //     { "requestGo", Go.ToString() },
            //     { "lastWave", waveIdx.ToString() },
            //     { "slotNumber", UserSlotManager.Instance.GetFocusIdx().ToString() },
            //     { "appVersion", Application.version },
            //     { "killedMonster", monsterSpawner.totalKilledMonsterCount.ToString() },
            //     { "killedBossMonster", monsterSpawner.totalKilledBossMonsterCount.ToString() },
            //     { "characterMerge", characterSpawner.totalSpawnData.characterMerge.ToString() },
            //     { "characterSummon", characterSpawner.totalSpawnData.summonCharacter.ToString() },
            //     { "characterFourStar", characterSpawner.totalSpawnData.fourStarGrade.ToString() },
            //     { "characterUpgrade", characterSpawner.totalSpawnData.characterUpgrade.ToString() },
            //     { "missionClear", missionManager.totalMissionClear.ToString() }
            // };
            //
            // Debug.Log($"totalMonsterKill : {gameEndDataV2["killedMonster"]}");
            // Debug.Log($"totalBossMonsterKill : {gameEndDataV2["killedBossMonster"]}");
            // Debug.Log($"totalSpawnDataMerge : {gameEndDataV2["characterMerge"]}");
            // Debug.Log($"totalSpawnDataSummon : {gameEndDataV2["characterSummon"]}");
            // Debug.Log($"totalSpawnDataFourStar : {gameEndDataV2["characterFourStar"]}");
            // Debug.Log($"totalSpawnDataUpgrade : {gameEndDataV2["characterUpgrade"]}");
            // Debug.Log($"totalMissionClear : {gameEndDataV2["missionClear"]}");
            #endregion

            _ = NetworkManager.Instance.GameFinished(gameEndData);
            UIManager.Instance.GameOver(bounsWave);
        }

        public void SetCurrentRewardValue(int idx)
        {
            currentRewardGoValue = dic_RewardRules[$"WAVE_{idx}"].rewardGo;
        }

        public void WaveStart()
        {
            if (gameState == GameState.GAME_OVER) return;
            gameState = GameState.PLAY;
            monsterSpawner.ExecuteNextWave();
            //waveIdx = monsterSpawner.currentWaveIdx;
        }

        public UnityAction BossDataAction;
        public void FieldBossWaveStart()
        {
            if (gameState == GameState.GAME_OVER) return;
            gameState = GameState.PLAY;
            //monsterSpawner.SetFieldBossWaveStart(bossData);
            BossDataAction?.Invoke();
        }

        public void TutorialWaveStart()
        {
            if (gameState == GameState.GAME_OVER) return;
            gameState = GameState.PLAY;
            monsterSpawner.TutorialExcuteNextWave();
            //waveIdx = monsterSpawner.currentWaveIdx;
        }

        public int testTemp = 0;

        public void ChangeBGM(bool isAppocalypseMode)
        {
            if (isAppocalypseMode)
            {
                SoundManager.Instance.PlaySound(SoundKey.BGM_APOCALYPSE);
            }
        }

        public void SetGameOverRecordDataClear()
        {
            missionManager.SetTotalMissionData();
            monsterSpawner.SetTotalMonsterData();
            characterSpawner.SetTotalSpawnData();
        }

        public void SetRecordClear()
        {
            missionManager.MissionDataClear();
            monsterSpawner.MonsterDataClear();
            characterSpawner.SpawnDataClear();
        }

        public void SetPlayRecordData(int killedBossLevel, int waveIdx)
        {
            if (gameState == GameState.GAME_OVER) return;
            int waveValue = waveIdx > 71 ? 71 : waveIdx;
            RewardRule rewardRule = dic_RewardRules[$"WAVE_{waveValue}"];

            //Debug.Log("SetPlayRecord Data Reward Value : " + rewardRule.rewardGo);

            int reqGo = rewardRule.rewardGo * monsterSpawner.killedMonsterCount;
            int bossGo = killedBossLevel != 0 ? dic_RewardRules[$"BOSS_{killedBossLevel}"].rewardGo : 0;
            int missionGo = 0;


            if (!string.IsNullOrEmpty(completedMission))
            {
                string[] temp = completedMission.Split(',');

                for (int i = 0; i < temp.Length; i++)
                {
                    int reward = dic_RewardRules[$"MISSION_{temp[i]}"].rewardGo;
                    missionGo += reward;
                }
            }

            #region 2.9.0 이전의 플레이 레코드
            // PlayRecord data = new()
            // {
            //     waveNumber = waveIdx,
            //     requestGo = reqGo + bossGo + missionGo,
            //     killedMonster = monsterSpawner.killedMonsterCount,
            //     completedMissions = this.completedMission,
            //     bossLevel = killedBossLevel,
            //     playId = this.playId,
            //     characterInfo = characterSpawner.GetCharacterInfo(),
            //     monsterInfo = monsterSpawner.GetWaveMonsterInfo(),
            // };
            #endregion

            float bestAttackSpeed = characterSpawner.GetBestAttackSpeed();
            int bestAttackDamage = (int)characterSpawner.GetBestAttackDamage();
            int bestUpgrade = (int)characterSpawner.GetBestUpgradeCharacter();

            SpawnData spawnData = characterSpawner.currentSpawnData;
            bestAttackSpeed = (float)Math.Round(bestAttackSpeed, 2);

            PlayRecord data = new()
            {
                waveNumber = waveIdx,
                requestGo = reqGo + bossGo + missionGo,
                killedMonster = monsterSpawner.killedMonsterCount,
                completedMissions = this.completedMission,
                bossLevel = killedBossLevel,
                playId = this.playId,
                attackSpeed = bestAttackSpeed,
                attackDamage = bestAttackDamage,
                upgrade = bestUpgrade,
                monsterHealth = monsterSpawner.monsterHealth,
                killedBossMonster = monsterSpawner.killedBossMonsterCount,
                missionClear = missionManager.currentMissionClear,
                characterSummon = spawnData.summonCharacter,
                characterMerge = spawnData.characterMerge,
                characterUpgrade = spawnData.characterUpgrade,
                fourStarGrade = spawnData.fourStarGrade
            };

            NetworkManager.Instance.SendPlayRecord(data);

            SetRecordClear();
            completedMission = "";
        }

        public void WaveEnd()
        {
            if (gameState == GameState.GAME_OVER) return;
            gameState = GameState.WAIT;
            onCompleteWave?.Invoke();
            UIManager.Instance.WaveEndSequence();
            InterestPayment();
            if (waveIdx != 0 && waveIdx % 5 == 0 && gameMode == GameMode.BATTLE)
            {
                ReachBossWave();
            }
            else
            {
                countNextWave = WaitForNextWave(WaveStart);
                StartCoroutine(countNextWave);
            }
        }

        /// <summary>
        /// 보스 웨이브 도달한 유저가 호출
        /// </summary>
        public void ReachBossWave()
        {
            float[] rewardGroup = new float[]
            {
                0.35f,
                0.43f,
                0.22f
            };
            int rewardGroupIndex = Calculator.GetIndependentTrial(rewardGroup);
            Debug.Log("rewardGroup Index : " + rewardGroupIndex);
            NetworkConnect.Instance.networkGameManager.Rpc_ReachBossWave(waveIdx, UserInfoManager.Instance.nickname, rewardGroupIndex);
        }
        public float tempBossAddHealth;
        public float SetAddBossHealth(int roundId)
        {
            tempBossAddHealth = 0;
            float addHealth = 0;
            int addRound = roundId - 1 - waveIdx;
            int tempRoundIdx = waveIdx;
            if (addRound == 0)
            {
                WaveData tempData = monsterSpawner.GetCurrentWaveData();
                int monsterCount = tempData.appearMonCount - monsterSpawner.killedMonsterCount;
                float health = tempData.monHealth * monsterCount;
                addHealth += health;
            }
            else
            {
                for (int i = 0; i < addRound; i++)
                {
                    if (i == 0)
                    {
                        WaveData tempData = monsterSpawner.GetCurrentWaveData();
                        int monsterCount = tempData.appearMonCount - monsterSpawner.killedMonsterCount;
                        float health = tempData.monHealth * monsterCount;
                        addHealth += health;
                    }
                    else
                    {
                        WaveData tempData = monsterSpawner.GetCurrentWaveData(tempRoundIdx);
                        float health = tempData.appearMonCount * tempData.monHealth;
                        addHealth += health;
                    }
                    tempRoundIdx++;
                }
            }
            tempBossAddHealth = addHealth;
            return addHealth;
        }
        /// <summary>
        /// 보스 시퀀스 시작
        /// </summary>
        public void BossWaveSeqeunce(int roundId, string nickname)
        {
            UIManager.Instance.BossWaveItem.gameObject.SetActive(true);
            Debug.Log("BossWave Sequence wave Idx : " + roundId);
            float addhealth = SetAddBossHealth(roundId);
            waveIdx = roundId;
            monsterSpawner.currentWaveIdx = roundId - 1;
            monsterSpawner.waveIdx = roundId;

            Debug.Log("AddHealth : " + addhealth);
            UIManager.Instance.BossWaveItem.Initialize(addhealth);
            UIManager.Instance.ChangeWaveValue(roundId);
            waveIdx = roundId;
            switch (gameState)
            {
                case GameState.WAIT:
                    if (countNextWave != null)
                    {
                        StopCoroutine(countNextWave);
                    }
                    UIManager.Instance.ActiveTimeCount(false);
                    monsterSpawner.StartBossWave();
                    break;
                case GameState.PLAY:
                    monsterSpawner.ClearMonster();
                    UIManager.Instance.WaveEndSequence();
                    break;
                case GameState.END:
                    break;
                case GameState.WAIT_BOSS_WAVE:
                    break;
                case GameState.GAME_OVER:
                    break;
            }
        }

        public void Damaged()
        {
            if (!UserInfoManager.Instance.userState.finishedTutorial)
            {
                GridManager.Instance.TutorialDamage();
            }
            else
            {
                GridManager.Instance.Damage();
            }
        }

        public void FieldBossDamage()
        {
            GridManager.Instance.FieldBossDamage();
        }

        public void TileCheck()
        {
            // Todo : Execute Tile Check
            // Game Over or Next Wave
        }
        public IEnumerator countNextWave;

        public void CountStart()
        {
            countNextWave = WaitForNextWave(WaveStart);
            StartCoroutine(countNextWave);
        }

        public void BossWaveCountStart()
        {
            countNextWave = WaitForNextWave(FieldBossWaveStart);
            StartCoroutine(countNextWave);
        }

        public void TutorialCountStart()
        {
            StartCoroutine(TutorialWaitForNextWave());
        }

        public IEnumerator TutorialWaitForNextWave()
        {
            UIManager.Instance.ActiveTimeCount(true);

            float count = waveReadyTimer;
            int temp = (int)count;
            UIManager.Instance.SetTimeCount(temp);
            while (true)
            {
                count -= Time.deltaTime;

                float a = (waveReadyTimer - count) / waveReadyTimer;
                UIManager.Instance.FillWaveCount(a);
                if ((int)count != temp)
                {
                    temp = (int)count;
                    UIManager.Instance.SetTimeCount(temp);
                    SoundManager.Instance.PlaySound(SoundKey.SF_COUNTDOWN);
                }
                yield return null;

                if (count <= 0)
                {
                    break;
                }
            }
            UIManager.Instance.ActiveTimeCount(false);

            TutorialWaveStart();
        }

        public IEnumerator WaitForNextWave(UnityAction action)
        {
            UIManager.Instance.ActiveTimeCount(true);

            float count = waveReadyTimer;
            int temp = (int)count;
            UIManager.Instance.SetTimeCount(temp);
            while (true)
            {
                count -= Time.deltaTime;

                float a = (waveReadyTimer - count) / waveReadyTimer;
                UIManager.Instance.FillWaveCount(a);
                if ((int)count != temp)
                {
                    temp = (int)count;
                    UIManager.Instance.SetTimeCount(temp);
                    SoundManager.Instance.PlaySound(SoundKey.SF_COUNTDOWN);
                }
                yield return null;

                if (count <= 0)
                {
                    break;
                }
            }
            UIManager.Instance.ActiveTimeCount(false);

            action?.Invoke();
        }

        private IEnumerator WaitOpenPausePopup()
        {
            yield return new WaitUntil(() => !isPlayingIntro);

            openPausePopupCorou = null;

            if (TutorialManager.Instance.isTutorial)
            {
                yield break;
            }

            if (!UIManager.Instance.pausePopup.isPaused)
            {
                UIManager.Instance.pausePopup.ActivePopup();
            }
        }

        public void OnDestroy()
        {
            Instance = null;
        }

        public void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                if (UserInfoManager.Instance.userState.finishedTutorial) return;

                //if (!UIManager.Instance.pausePopup.isPaused)
                //{
                //    UIManager.Instance.pausePopup.ActivePopup();
                //}
            }
            else
            {
                if (gameState == GameState.GAME_OVER) return;

                //if (!UIManager.Instance.pausePopup.isPaused)
                //{
                //    UIManager.Instance.pausePopup.ActivePopup();
                //}
            }

            // 튜토리얼에서는 팝업 미출력
            if (TutorialManager.Instance.isTutorial) return;
            // 씬 전환 연출 중이면 끝나고 팝업 출력
            if (isPlayingIntro)
            {
                openPausePopupCorou ??= StartCoroutine(WaitOpenPausePopup());
                return;
            }

            if (!UIManager.Instance.pausePopup.isPaused)
            {
                UIManager.Instance.pausePopup.ActivePopup();
            }
        }
    }
}
