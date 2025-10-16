using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using Framework.Game.Defense;
using TMPro;

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
            /* "Boomber",  */"Trush", "Smoker"/*"Locky, "Sotty", "Locky", "Parasite", "Boomber", "Emberon"*/
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
                Debug.Log($"Center Boss: {centerBossData.bossNameKey}, Progress: {progress:F2}");
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
                    if(itemCount >= 5)
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

        public void SetSummonBossInfo()
        {
            BossData bossData = dic_bossData[targetBossName];
            text_bossDescription.text = LanguageManager.Instance.GetStringData(bossData.bossDescKey);
        }
    }
}


