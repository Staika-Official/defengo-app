using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using Framework.Game.Defense;
using TMPro;
using Framework.Network;

namespace Framework.UI
{
    public class BossSelectPopup : PopupTemplate
    {
        public BossSelectItem bossSelectItem;
        public List<BossSelectItem> bossSelectItems;
        public InfiniteHorizontalScroll infiniteHorizontalScroll;
        public RectTransform scrollRect;
        public TextMeshProUGUI text_bossDescription;
        public Dictionary<string, BossData> dic_bossData = new();
        public Animation openSequence;
        public int targetIdx;
        public ObscuredString targetBossName;
        public readonly string[] TempbossList = {
            "Trush", "Smoker", "Sotty",  "Locky", "Parasite", "Boomber", "Emberon"
        };

        // Define ELO-based boss spawn configuration
        // Each tier maps to field_boss_id (1-based) with their emergence rates
        Dictionary<int, Dictionary<int, float>> eloTiers = new Dictionary<int, Dictionary<int, float>>()
        {
            // ELO 800 tier
            { 800, new Dictionary<int, float> { {1, 0.5f}, {2, 0.5f} } },

            // ELO 900 tier
            { 900, new Dictionary<int, float> { {1, 0.33f}, {2, 0.33f}, {3, 0.34f} } },

            // ELO 1000 tier
            { 1000, new Dictionary<int, float> { {1, 0.25f}, {2, 0.25f}, {3, 0.25f}, {4, 0.25f} } },

            // ELO 1300 tier
            { 1300, new Dictionary<int, float> { {1, 0.15f}, {2, 0.15f}, {3, 0.23f}, {4, 0.23f}, {5, 0.24f} } },

            // ELO 1500 tier
            { 1500, new Dictionary<int, float> { {1, 0.1f}, {2, 0.1f}, {3, 0.2f}, {4, 0.2f}, {5, 0.2f}, {6, 0.2f} } },

            // ELO 1800 tier
            { 1800, new Dictionary<int, float> { {1, 0.01f}, {2, 0.01f}, {3, 0.22f}, {4, 0.22f}, {5, 0.22f}, {6, 0.22f}, {7, 0.1f} } }
        };

        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public void Show(int randomIdx)
        {
            StartCoroutine(SetOpenAnimation(randomIdx));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public IEnumerator SetOpenAnimation(int randomIdx)
        {
            infiniteHorizontalScroll.SetStart();
            openSequence.Play();

            yield return new WaitForSeconds(openSequence.clip.length);

            int idx = (int)dic_bossData[TempbossList[randomIdx]].bossIndex;

            BossData bossData = dic_bossData[TempbossList[randomIdx]];

            GameManager.Instance.BossDataAction = () =>
            {
                GameManager.Instance.monsterSpawner.SetFieldBossWaveStart(bossData);
            };

            infiniteHorizontalScroll.OnCompleteSeqeunce = () =>
            {
                PopUpSequence(false);
                GameManager.Instance.BossWaveCountStart();
                // 카운트 다운 시작 시퀀스
            };

            for (int i = bossSelectItems.Count - 1; i >= 0; i--)
            {
                if (bossSelectItems[i].bossIdx == idx)
                {
                    int temp = i;
                    while (temp < 10)
                    {
                        temp += bossSelectItems.Count;
                    }
                    infiniteHorizontalScroll.SetSeqeunce(temp);
                    break;
                }
            }
        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            GetBossData();

            // 스크롤 중앙 인덱스 변경 이벤트 연결
            infiniteHorizontalScroll.OnCenterIndexChanged.AddListener(OnScrollCenterChange);
        }

        /// <summary>
        /// 스크롤 중앙에 표시되는 보스가 변경될 때 호출
        /// </summary>
        /// <param name="centerIndex">중앙에 있는 아이템의 인덱스</param>
        /// <param name="progress">전환 진행도 (0~1)</param>
        public void OnScrollCenterChange(int centerIndex, float progress)
        {
            // 중앙 인덱스에 해당하는 BossSelectItem 가져오기
            if (centerIndex < 0 || centerIndex >= bossSelectItems.Count)
                return;

            BossSelectItem centerItem = bossSelectItems[centerIndex];

            // BossData 가져오기
            BossData centerBossData = null;
            foreach (var kvp in dic_bossData)
            {
                if ((int)kvp.Value.bossIndex == centerItem.bossIdx)
                {
                    centerBossData = kvp.Value;
                    targetBossName = kvp.Key;
                    break;
                }
            }

            // 보스 정보 업데이트
            if (centerBossData != null)
            {
                text_bossDescription.text = LanguageManager.Instance.GetStringData(centerBossData.bossDescKey);
                // Debug.Log($"Center Boss: {centerBossData.bossNameKey}, Progress: {progress:F2}");
            }
        }

        public async void GetBossData()
        {
            int itemCount = 0;
            if (TempbossList.Length < 5)
            {
                while (true)
                {
                    itemCount += TempbossList.Length;
                    if (itemCount >= 5)
                    {
                        break;
                    }
                }
            }
            else
            {
                itemCount = TempbossList.Length;
            }

            for (int i = 0; i < itemCount; i++)
            {
                BossSelectItem item = Instantiate(bossSelectItem);
                item.transform.SetParent(scrollRect);
                item.transform.localPosition = new(0, -65f);
                item.transform.localScale = Vector3.one;
                bossSelectItems.Add(item);
            }

            for (int i = 0; i < TempbossList.Length; i++)
            {
                BossData data = await DataLoadManager.Instance.GetDataAsync<BossData>(TempbossList[i]);
                dic_bossData.Add(TempbossList[i], data);
            }

            List<BossData> tempList = dic_bossData.Values.ToList();

            for (int i = 0; i < bossSelectItems.Count; i++)
            {
                BossData bossData = tempList[i % tempList.Count];

                bossSelectItems[i].Initialize(bossData);
            }

            infiniteHorizontalScroll.Initiailize();
        }

        /// <summary>
        /// Selects a random boss index based on average ELO rating from NetworkConnect.
        /// Uses probability-based selection according to ELO tier configuration.
        /// </summary>
        /// <returns>Index of the selected boss in TempbossList (0-6)</returns>
        public int RandomBoss()
        {
            // Get average ELO from NetworkConnect
            float avgElo = NetworkConnect.Instance.avgElo;

            Debug.Log($"[BossSelectPopup] RandomBoss - avgElo: {avgElo}");

            // Determine which ELO tier to use based on avgElo
            Dictionary<int, float> selectedTier;
            if (avgElo < 800)
                selectedTier = eloTiers[800];
            else if (avgElo < 900)
                selectedTier = eloTiers[900];
            else if (avgElo < 1000)
                selectedTier = eloTiers[1000];
            else if (avgElo < 1300)
                selectedTier = eloTiers[1300];
            else if (avgElo < 1500)
                selectedTier = eloTiers[1500];
            else
                selectedTier = eloTiers[1800];

            // Generate random value between 0 and 1
            float randomValue = Random.Range(0f, 1f);
            float cumulativeProbability = 0f;

            // Select boss based on probability
            int selectedBossId = 1; // Default to first boss
            foreach (var kvp in selectedTier)
            {
                cumulativeProbability += kvp.Value;
                if (randomValue <= cumulativeProbability)
                {
                    selectedBossId = kvp.Key;
                    break;
                }
            }

            // Convert field_boss_id (1-based) to TempbossList index (0-based)
            int bossIndex = selectedBossId - 1;

            // Ensure index is within valid range
            bossIndex = Mathf.Clamp(bossIndex, 0, TempbossList.Length - 1);

            Debug.Log($"[BossSelectPopup] Selected boss - avgElo: {avgElo}, bossId: {selectedBossId}, bossIndex: {bossIndex}, bossName: {TempbossList[bossIndex]}");

            return bossIndex;
        }
    }
}


