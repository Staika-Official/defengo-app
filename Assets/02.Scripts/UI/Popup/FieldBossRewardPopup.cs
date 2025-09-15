using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Framework.GameData.Defense;
using Framework.Util;
using System.Linq;
using Framework.Network;


namespace Framework.UI
{
    public class FieldBossRewardPopup : PopupTemplate
    {
        public Image image_progress;
        public float timeCount;
        public int refreshCount;
        public Animation openAnimation;
        public FieldBossRewardItem[] fieldBossRewardItems;
        public FieldBossRewardGroupData fieldBossRewardGroupData;
        public IEnumerator countDown;
        public Button button_refresh;

        public IEnumerator SetCountDown()
        {
            float timeCount = 6f;

            while (true)
            {
                timeCount -= Time.deltaTime;
                yield return null;

                if (timeCount <= 0)
                    break;
            }


            Debug.Log("Time Over");
        }

        public override void ActivePopup()
        {
            SetRewardItem();

            PopUpSequence(true);

            openAnimation.Play();
        }

        public async void SetRewardItem()
        {
            fieldBossRewardGroupData = await DataLoadManager.Instance.GetDataAsyncBinary<FieldBossRewardGroupData>("FieldBossRewardGroupData");
            int rewardGroupIndex = NetworkConnect.Instance.networkGameManager.rewardGroupIndex;

            var linqResult = from data in fieldBossRewardGroupData.data
                             where data.reward_grade_group == rewardGroupIndex + 1
                             select data;

            float temp = 0;
            var fieldBossRewardDatas = linqResult as FieldBossRewardData[] ?? linqResult.ToArray();
            float[] tempArr = new float[fieldBossRewardDatas.Count()];

            Debug.Log($"Group : {rewardGroupIndex + 1} & Group Length : {fieldBossRewardDatas.Count()} ");

            for (int i = 0; i < fieldBossRewardDatas.Length; i++)
            {
                tempArr[i] = fieldBossRewardDatas[i].emerge_rate;
                temp += fieldBossRewardDatas[i].emerge_rate;
            }

            Debug.Log("reward emerge : " + temp);
            int[] selected = GetFieldBossReward(tempArr);

            for (int i = 0; i < selected.Length; i++)
            {
                fieldBossRewardItems[i].Initialize(fieldBossRewardDatas[selected[i]]);
            }
        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            refreshCount = 2;

            button_refresh.onClick.AddListener(() => Onclick_Refresh());
        }

        public void Onclick_Refresh()
        {
            SetRewardItem();
        }

        public int[] GetFieldBossReward(float[] array)
        {
            int[] tempArr = new int[3];

            for (int i = 0; i < tempArr.Length; i++)
            {
                tempArr[i] = Calculator.GetIndependentTrial(array);

                int diffCount = i;
                while (diffCount > 0)
                {
                    for (int j = 0; j < tempArr.Length; j++)
                    {
                        if (i == j) continue;
                        if (tempArr[i] == tempArr[j])
                        {
                            tempArr[i] = Calculator.GetIndependentTrial(array);
                            diffCount++;
                        }
                        else
                        {
                            diffCount--;
                        }
                    }
                }
            }

            return tempArr;
        }

        public IEnumerator SetTimeCount()
        {
            while (true)
            {
                yield return null;
            }
        }
    }
}
