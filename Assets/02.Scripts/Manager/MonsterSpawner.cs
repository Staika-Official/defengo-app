using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.ObscuredTypes;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Util;
using Spine.Unity;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using DG.Tweening;
using Cysharp.Threading.Tasks.Triggers;

namespace Framework.Game.Defense
{
    public class MonsterInfo
    {
        public int waveIdx;
        public int monsterHealth;

        public MonsterInfo(int waveIdx, int monsterHealth)
        {
            this.waveIdx = waveIdx;
            this.monsterHealth = monsterHealth;
        }
    }

    public class MonsterSpawner : MonoBehaviour
    {
        public int[] monsterPath;
        public int pathLength;

        public bool isApocalypse = false;

        public bool IsGameOver { get; set; }
        public List<Monster> monsters;
        public InfiniteData infiniteData;
        public int currentWaveIdx = 0;
        public bool isWaveStart = false;
        public int waveIdx;
        public int bossIdx;
        public BossMonsterData bossData;
        public int monsterCount;
        public ObscuredInt monsterHealth;
        public ObscuredInt tempWaveIdx;

        public readonly int[] useMonsterIdList = { 1, 2, 3, 4, 51 };

        public Dictionary<int, SkeletonDataAsset> dic_monsterAnimAsset = new();

        public ObscuredInt killedMonsterCount;
        public ObscuredInt killedBossLevel;
        public ObscuredInt killedBossMonsterCount;

        public ObscuredInt totalKilledMonsterCount;
        public ObscuredInt totalKilledBossMonsterCount;

        private void Awake()
        {
            ObscuredCheatingDetector.StartDetection(OnCheterDetected);

            GridManager.onComplete_GenerateGrid = (value) =>
            {
                InitMonsterSpawner(value);
                pathLength = value.Length;
            };
        }

        private void OnCheterDetected()
        {
            _ = NetworkManager.Instance.AbusingRecord("Cheat MonsterSpawner");
        }

        public void InitializeStart()
        {
            bossIdx = 0;
            waveIdx = 0;

            //todo dongmin
            MonsterDataClear();

            GetWaveData();
            GetUserMonsterData();
            isApocalypse = false;
        }

        public string GetWaveMonsterInfo()
        {
            MonsterInfo monsterInfo = new(tempWaveIdx + 1, monsterHealth);
            string data = JsonUtility.ToJson(monsterInfo);
            return data;
        }

        public async void GetWaveData()
        {
            currentWaveIdx = 0;
            //string waveData = "WaveData";
            string waveData = GameManager.Instance.gameMode switch
            {
                GameMode.SINGLE => "WaveData",
                GameMode.BATTLE => "WaveData",
                GameMode.TUTORIAL => "WaveData1",
                _ => "WaveData",
            };
            infiniteData = await DataLoadManager.Instance.GetDataAsyncBinary<InfiniteData>(waveData);

            bossData = await DataLoadManager.Instance.GetDataAsyncBinary<BossMonsterData>("BossMonsterData");
        }

        public async void GetUserMonsterData()
        {
            for (int i = 0; i < useMonsterIdList.Length; i++)
            {
                int idx = useMonsterIdList[i];
                SkeletonDataAsset anim = await DataLoadManager.Instance.GetDataAsync<SkeletonDataAsset>($"Monster_{idx}");
                dic_monsterAnimAsset.Add(idx, anim);
            }
        }

        public int GetNextCellIdx(int value)
        {
            int nextIdx = monsterPath[value];

            return nextIdx;
        }

        public void InitMonsterSpawner(int[] monsterPath)
        {
            this.monsterPath = monsterPath;
        }

        public IEnumerator spawnMonster;

        public void ExecuteNextWave()
        {
            spawnMonster = SetMonsterStart();
            StartCoroutine(spawnMonster);
        }

        public void TutorialExcuteNextWave()
        {
            StartCoroutine(TutorialSetMonsterStart());
        }

        public void StartBossWave()
        {
            switch (GameManager.Instance.gameState)
            {
                case GameState.WAIT:
                    if (spawnMonster != null)
                    {
                        StopCoroutine(spawnMonster);
                    }
                    break;
                case GameState.PLAY:
                    for (int i = 0; i < monsters.Count; i++)
                    {
                    }
                    break;
                case GameState.END:
                    break;
                case GameState.WAIT_BOSS_WAVE:
                    break;
                case GameState.GAME_OVER:
                    break;
                default:
                    break;
            }
        }

        public void SummonBoss()
        {
            SetBossMonsterStart();
        }

        public void TutorialSummonBoss()
        {
            isWaveStart = true;

            BossMonster data = bossData.bossMonsters[bossIdx];

            NormalBossMonster normalBossMonster = GameManager.Instance.objectPoolManager.GetObject<NormalBossMonster>($"Boss_105");
            normalBossMonster.monsterType = MonsterType.BOSS_MONSTER;
            normalBossMonster.isBoss = true;
            normalBossMonster.waveIndex = bossIdx + 1;
            normalBossMonster.transform.name = "NormalBoss";
            normalBossMonster.speed = data.bossSpeed;
            normalBossMonster.health = 500;
            normalBossMonster.SetBossMove();
            monsters.Add(normalBossMonster);
        }

        public void SetFieldBossWaveStart(BossData bossData)
        {
            switch (bossData.bossIndex)
            {
                case FieldBossMonster.TRUSH:
                    NormalBossMonster normalBossMonster = GameManager.Instance.objectPoolManager.GetObject<NormalBossMonster>($"Boss_105");
                    normalBossMonster.monsterType = MonsterType.BOSS_MONSTER;
                    normalBossMonster.isBoss = true;
                    normalBossMonster.waveIndex = bossIdx + 1;
                    normalBossMonster.FieldBossInitialize(bossData);
                    monsters.Add(normalBossMonster);
                    break;
                case FieldBossMonster.SMOKER:
                    CarBossMonster carBossMonster = GameManager.Instance.objectPoolManager.GetObject<CarBossMonster>($"Boss_101");
                    carBossMonster.monsterType = MonsterType.BOSS_MONSTER;
                    carBossMonster.isBoss = true;
                    carBossMonster.waveIndex = bossIdx + 1;
                    carBossMonster.monCloneHealth = bossData.uniqueValue[0];
                    carBossMonster.monCloneId = 51;
                    carBossMonster.FieldBossInitialize(bossData);
                    monsters.Add(carBossMonster);
                    break;
            }
            // Monster monster = bossData.bossIndex switch
            // {
            //     FieldBossMonster.SMOKER => GameManager.Instance.objectPoolManager.GetObject<CarBossMonster>($"Boss_101"),
            //     // FieldBossMonster.SOTTY => GameManager.Instance.objectPoolManager.GetObject<SottyBossMonster>($"Boss_3"),
            //     // FieldBossMonster.LOCKY => GameManager.Instance.objectPoolManager.GetObject<LockyBossMonster>($"Boss_4"),
            //     // FieldBossMonster.PARASITE => GameManager.Instance.objectPoolManager.GetObject<ParasiteBossMonster>($"Boss_5"),
            //     FieldBossMonster.BOOMBER => GameManager.Instance.objectPoolManager.GetObject<BoomberBossMonster>($"Boss_6"),
            //     // FieldBossMonster.EMBEREON => throw new System.NotImplementedException(),
            //     _ => throw new System.NotImplementedException(),
            // };
            // monsters.Add(monster);
        }

        public void SetBossMonsterStart()
        {
            IsGameOver = true;
            isWaveStart = true;

            if (GameManager.Instance.isGameOver)
                return;

            if (bossIdx >= bossData.bossMonsters.Length - 1)
                {
                    bossIdx = bossData.bossMonsters.Length - 1;
                }

            BossMonster data = bossData.bossMonsters[bossIdx];
            switch (data.monsterId)
            {
                case 101:
                    CarBossMonster carBossMonster = GameManager.Instance.objectPoolManager.GetObject<CarBossMonster>($"Boss_101");
                    carBossMonster.monsterType = MonsterType.BOSS_MONSTER;
                    carBossMonster.isBoss = true;
                    carBossMonster.waveIndex = bossIdx + 1;
                    carBossMonster.transform.name = "CarBoss";
                    carBossMonster.speed = data.bossSpeed;
                    carBossMonster.health = data.bossHealth;
                    carBossMonster.SetBossMove();
                    carBossMonster.monCloneHealth = data.cloneMonsterHealth;
                    carBossMonster.monCloneId = data.cloneMonsterId;
                    monsters.Add(carBossMonster);
                    break;
                case 105:
                    NormalBossMonster normalBossMonster = GameManager.Instance.objectPoolManager.GetObject<NormalBossMonster>($"Boss_105");
                    normalBossMonster.monsterType = MonsterType.BOSS_MONSTER;
                    normalBossMonster.isBoss = true;
                    normalBossMonster.waveIndex = bossIdx + 1;
                    normalBossMonster.transform.name = "NormalBoss";
                    normalBossMonster.speed = data.bossSpeed;
                    normalBossMonster.health = data.bossHealth;
                    normalBossMonster.SetBossMove();
                    monsters.Add(normalBossMonster);
                    break;
            }
            bossIdx++;

            if (GameManager.Instance.gameMode == GameMode.BATTLE)
            {
                NetworkConnect.Instance.networkGameManager.Rpc_RequestSpawnBoss(UserInfoManager.Instance.nickname, data.bossHealth);
            }
        }

        public void SetCloneMonster(float health, float speed, int cloneId, int arrayIdx)
        {
            Monster monster = GameManager.Instance.objectPoolManager.GetObject<Monster>("Monster");
            SkeletonDataAsset asset = dic_monsterAnimAsset[cloneId];

            monsters.Add(monster);
            monster.speed = speed;
            monster.health = health;
            monster.SetMonsterMove(asset, arrayIdx, MonsterType.CLONE_MONSTER);
        }

        public void SpawnPlasticMonster(int remainingPlayer = 1, float casterBossHealth = 1000)
        {
            StartCoroutine(SpawnPlasticMonsterSequence(remainingPlayer, casterBossHealth));
        }
        IEnumerator SpawnPlasticMonsterSequence(int remainingPlayer, float casterBossHealth)
        {
            var interval = ConfigData.CREATE_MONSTER_TIME;
            SkeletonDataAsset asset = dic_monsterAnimAsset[2];

            int amount = (int)(ConfigData.BATTLE_BOSS_MONSTER_APPEAR / ConfigData.BATTLE_BOSS_MONSTER_DEVICE / remainingPlayer);

            for (int i = 0; i < amount; i++)
            {
                Monster monster = GameManager.Instance.objectPoolManager.GetObject<Monster>("Monster");

                monsters.Add(monster);
                monster.speed = ConfigData.BATTLE_BOSS_MONSTER_SPEED;
                monster.health = casterBossHealth / ConfigData.BATTLE_BOSS_MONSTER_HEALTH_DEVICE;
                monster.SetMonsterMove(asset, MonsterType.CLONE_MONSTER);
                yield return new WaitForSeconds(interval);
            }
        }

        public IEnumerator TutorialMonsterSpawn()
        {
            IsGameOver = true;
            isWaveStart = true;
            WaveData data;
            int tempMonsterhealth;
            if (currentWaveIdx >= infiniteData.waveDatas.Length)
            {
                if (!isApocalypse)
                {
                    isApocalypse = true;
                    GameManager.Instance.ChangeBGM(isApocalypse);
                }

                data = infiniteData.waveDatas[^1];

                Debug.Log(data.wave);
                float temp = data.monHealth;

                tempMonsterhealth = (int)(Mathf.Pow(ConfigData.INFINITE_HEALTH_FACTOR_DIFFERENTIAL, currentWaveIdx - infiniteData.waveDatas.Length + 1) * ConfigData.INFINITE_HEALTH_FACTOR - ConfigData.INFINITE_HEALTH_FACTOR + temp);

                waveIdx = data.wave;
                GameManager.Instance.SetCurrentRewardValue(data.wave);
            }
            else
            {
                data = infiniteData.waveDatas[currentWaveIdx];
                tempMonsterhealth = data.monHealth;
                waveIdx = data.wave;
                GameManager.Instance.SetCurrentRewardValue(data.wave);
            }

            float interval = ConfigData.CREATE_MONSTER_TIME;

            monsterCount = data.appearMonCount;

            int monsterIdx = data.monsterId;

            SkeletonDataAsset animation = dic_monsterAnimAsset[monsterIdx];

            for (int i = 0; i < data.appearMonCount; i++)
            {
                Monster monsterComponent = GameManager.Instance.objectPoolManager.GetObject<Monster>("Monster");
                monsterComponent.isBoss = false;
                monsters.Add(monsterComponent);
                monsterComponent.name = $"Wave_{currentWaveIdx} monster_{i}";
                monsterComponent.indexInWave = i;
                monsterComponent.speed = data.monSpeed;
                monsterComponent.health = tempMonsterhealth;
                monsterComponent.defense = data.monDepense;
                monsterComponent.waveIndex = data.wave;
                monsterComponent.SetMonsterMove(animation);

                yield return new WaitForSeconds(interval);
            }
        }

        public IEnumerator TutorialSetMonsterStart()
        {
            IsGameOver = true;
            isWaveStart = true;
            WaveData data = infiniteData.waveDatas[currentWaveIdx];
            int testNum;

            testNum = data.monHealth;
            waveIdx = data.wave;
            GameManager.Instance.SetCurrentRewardValue(data.wave);

            float interval = ConfigData.CREATE_MONSTER_TIME;

            monsterCount = data.appearMonCount;

            int monsterIdx = data.monsterId;

            SkeletonDataAsset animation = dic_monsterAnimAsset[monsterIdx];

            for (int i = 0; i < data.appearMonCount; i++)
            {
                Monster monsterComponent = GameManager.Instance.objectPoolManager.GetObject<Monster>("Monster");
                monsterComponent.isBoss = false;
                monsters.Add(monsterComponent);
                monsterComponent.name = $"Wave_{currentWaveIdx} monster_{i}";
                monsterComponent.indexInWave = i;
                monsterComponent.speed = data.monSpeed;
                monsterComponent.health = testNum;
                monsterComponent.defense = data.monDepense;
                monsterComponent.waveIndex = data.wave;
                monsterComponent.SetMonsterMove(animation);

                yield return new WaitForSeconds(interval);
            }
        }

        public WaveData GetCurrentWaveData()
        {
            WaveData waveData = currentWaveIdx >= infiniteData.waveDatas.Length ? infiniteData.waveDatas[^1] : infiniteData.waveDatas[currentWaveIdx];

            return waveData;
        }

        public WaveData GetCurrentWaveData(int waveIdx)
        {
            WaveData waveData = waveIdx >= infiniteData.waveDatas.Length ? infiniteData.waveDatas[^1] : infiniteData.waveDatas[waveIdx];
            //Debug.Log("current Wave idx : " + waveData.index);
            return waveData;
        }

        public IEnumerator SetMonsterStart()
        {
            Debug.Log($"[{gameObject.name}] Start wave {currentWaveIdx}");
            IsGameOver = true;
            isWaveStart = true;
            WaveData data;
            ObscuredInt tempMonsterHealth;
            ObscuredInt monsterIdx;

            int waveSize = infiniteData.waveDatas.Length;
            if (currentWaveIdx >= waveSize)
            {
                data = infiniteData.waveDatas[^1];

                if (!isApocalypse)
                {
                    isApocalypse = true;
                    GameManager.Instance.ChangeBGM(isApocalypse);
                }

                float temp = data.monHealth;
                tempMonsterHealth = (int)(Mathf.Pow(ConfigData.INFINITE_HEALTH_FACTOR_DIFFERENTIAL, currentWaveIdx - waveSize + 1)
                    * ConfigData.INFINITE_HEALTH_FACTOR - ConfigData.INFINITE_HEALTH_FACTOR + temp);
                waveIdx = data.wave;
                int currentWaveIndex = currentWaveIdx > waveSize - 1 ? waveSize - 1 : currentWaveIdx;
                monsterIdx = 4;
                GameManager.Instance.SetCurrentRewardValue(currentWaveIndex);
                //GameManager.Instance.SetCurrentRewardValue(data.wave);
            }
            else
            {
                data = infiniteData.waveDatas[currentWaveIdx];
                monsterIdx = data.monsterId;
                tempMonsterHealth = data.monHealth;
                waveIdx = data.wave;
                GameManager.Instance.SetCurrentRewardValue(data.wave);
            }

            tempWaveIdx = currentWaveIdx;
            monsterHealth = tempMonsterHealth;

            float interval = ConfigData.CREATE_MONSTER_TIME;

            monsterCount = data.appearMonCount;

            SkeletonDataAsset animation = dic_monsterAnimAsset[monsterIdx];

            for (int i = 0; i < data.appearMonCount; i++)
            {
                Monster monsterComponent = GameManager.Instance.objectPoolManager.GetObject<Monster>("Monster");
                monsterComponent.isBoss = false;
                monsters.Add(monsterComponent);
                monsterComponent.name = $"Wave_{currentWaveIdx} monster_{i}";
                monsterComponent.indexInWave = i;
                monsterComponent.speed = data.monSpeed;
                monsterComponent.health = tempMonsterHealth;
                monsterComponent.defense = data.monDepense;
                monsterComponent.waveIndex = data.wave;
                monsterComponent.SetMonsterMove(animation);

                yield return new WaitForSeconds(interval);
            }
        }

        public void SetTotalMonsterData()
        {
            totalKilledMonsterCount = Mathf.Max(0, totalKilledMonsterCount - killedMonsterCount);
            totalKilledBossMonsterCount = Mathf.Max(0, totalKilledBossMonsterCount - killedBossMonsterCount);
        }

        public void MonsterDataClear()
        {
            killedMonsterCount = 0;
            killedBossMonsterCount = 0;
        }

        public void WaveEnd()
        {
            if (TutorialManager.Instance.isTutorial)
            {
                TutorialManager.Instance.nextSequence?.Invoke();
            }
            else
            {
                switch (GameManager.Instance.gameMode)
                {
                    case GameMode.SINGLE:
                        GameManager.Instance.SetPlayRecordData(killedBossLevel, currentWaveIdx);
                        killedMonsterCount = 0;
                        killedBossLevel = 0;
                        GameManager.Instance.WaveEnd();
                        isWaveStart = false;
                        break;
                    case GameMode.BATTLE:
                        GameManager.Instance.SetPlayRecordData(killedBossLevel, currentWaveIdx);
                        killedMonsterCount = 0;
                        killedBossLevel = 0;
                        GameManager.Instance.WaveEnd();
                        isWaveStart = false;
                        break;
                }
            }
        }

        public void ClearMonster()
        {
            if (spawnMonster != null)
            {
                StopCoroutine(spawnMonster);
            }

            for (int i = 0; i < monsters.Count; i++)
            {
                if (monsters[i].monsterType == MonsterType.WAVE_MONSTER)
                {
                    StartCoroutine(monsters[i].EnterBossSequenceAsync());
                }
            }
        }

        public void GameOverHome()
        {
            GameManager.Instance.SetGameOverRecordDataClear();
            GameManager.Instance.SetRecordClear();
            GameManager.Instance.SetPlayRecordData(killedBossLevel, currentWaveIdx + 1);
        }

        public void RemoveMonster(Monster monster)
        {
            switch (monster.monsterType)
            {
                case MonsterType.WAVE_MONSTER:
                    monsterCount -= 1;
                    monsters.Remove(monster);
                    if (monsterCount == 0)
                    {
                        if (isWaveStart)
                        {
                            currentWaveIdx++;
                            if (currentWaveIdx >= infiniteData.waveDatas.Length)
                            {
                                UIManager.Instance.ChangeWaveValue(currentWaveIdx + 1);
                                if (GameManager.Instance.gameMode == GameMode.BATTLE)
                                {
                                    NetworkConnect.Instance.networkGameManager.Rpc_RequestWaveComplete(NetworkConnect.Instance.playerIdx, currentWaveIdx + 1, totalKilledMonsterCount, totalKilledBossMonsterCount);
                                }
                                GameManager.Instance.waveIdx = currentWaveIdx + 1;
                            }
                            else
                            {
                                int idx = infiniteData.waveDatas[currentWaveIdx].wave;
                                UIManager.Instance.ChangeWaveValue(idx);
                                if (GameManager.Instance.gameMode == GameMode.BATTLE)
                                {
                                    NetworkConnect.Instance.networkGameManager.Rpc_RequestWaveComplete(NetworkConnect.Instance.playerIdx, idx, totalKilledMonsterCount, totalKilledBossMonsterCount);
                                }
                                GameManager.Instance.waveIdx = idx;
                            }
                            WaveEnd();
                        }
                    }
                    break;
                case MonsterType.CLONE_MONSTER:
                    monsters.Remove(monster);
                    break;
                case MonsterType.BOSS_MONSTER:
                    monsters.Remove(monster);
                    if (!UserInfoManager.Instance.userState.finishedTutorial)
                    {
                        TutorialManager.Instance.nextSequence?.Invoke();
                    }
                    break;
                case MonsterType.FIELD_BOSS_MONSTER:
                    Debug.Log("at monsterSpawner wave Idx : " + currentWaveIdx);
                    currentWaveIdx++;
                    if (currentWaveIdx >= infiniteData.waveDatas.Length)
                    {
                        //UIManager.Instance.ChangeWaveValue(currentWaveIdx + 1);
                        GameManager.Instance.waveIdx = currentWaveIdx + 1;
                    }
                    else
                    {
                        int idx = infiniteData.waveDatas[currentWaveIdx].wave;
                        //UIManager.Instance.ChangeWaveValue(idx);
                        GameManager.Instance.waveIdx = idx;
                    }
                    monsters.Remove(monster);
                    WaveEnd();
                    //Debug.Log("Field Boss!!!");
                    break;
            }
        }
    }
}
