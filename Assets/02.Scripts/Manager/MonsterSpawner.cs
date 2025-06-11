using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.ObscuredTypes;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Util;
using Spine.Unity;
using UnityEngine;

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

        private void Start()
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
            string waveData = UserInfoManager.Instance.userState.finishedTutorial ? "WaveData" : "WaveData1";
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

        public void ExecuteNextWave()
        {
            StartCoroutine(SetMonsterStart());
        }

        public void TutorialExcuteNextWave()
        {
            StartCoroutine(TutorialSetMonsterStart());
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
            normalBossMonster.isBoss = true;
            normalBossMonster.waveIndex = bossIdx + 1;
            normalBossMonster.transform.name = "NormalBoss";
            normalBossMonster.speed = data.bossSpeed;
            normalBossMonster.health = 500;
            normalBossMonster.SetBossMove();
            monsters.Add(normalBossMonster);
        }

        public void SetBossMonsterStart()
        {
            IsGameOver = true;
            isWaveStart = true;

            if (bossIdx >= bossData.bossMonsters.Length - 1)
            {
                bossIdx = bossData.bossMonsters.Length - 1;
            }

            BossMonster data = bossData.bossMonsters[bossIdx];
            switch (data.monsterId)
            {
                case 101:
                    CarBossMonster carBossMonster = GameManager.Instance.objectPoolManager.GetObject<CarBossMonster>($"Boss_101");
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
        }

        public void SetCloneMonster(float health, float speed, int cloneId, int arrayIdx)
        {
            Monster monster = GameManager.Instance.objectPoolManager.GetObject<Monster>("Monster");
            SkeletonDataAsset asset = dic_monsterAnimAsset[cloneId];

            monsters.Add(monster);
            monster.speed = speed;
            monster.health = health;
            monster.maxHealth = health;
            monster.SetMonsterMove(asset, arrayIdx);
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

        public IEnumerator SetMonsterStart()
        {
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
                GameManager.Instance.SetPlayRecordData(killedBossLevel, currentWaveIdx);
                killedBossLevel = 0;

                GameManager.Instance.WaveEnd();
                isWaveStart = false;
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
                                GameManager.Instance.waveIdx = currentWaveIdx + 1;
                            }
                            else
                            {
                                int idx = infiniteData.waveDatas[currentWaveIdx].wave;
                                UIManager.Instance.ChangeWaveValue(idx);
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
            }
        }
    }
}
