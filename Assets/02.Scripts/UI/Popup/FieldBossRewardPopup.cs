using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Framework.GameData.Defense;
using Framework.Util;
using System.Linq;
using Framework.Network;
using Newtonsoft.Json;
using TMPro;


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
        public ButtonComponent button_refresh;
        public TextMeshProUGUI text_time;
        public TextMeshProUGUI text_Refresh;
        public Slider slider_time;
        public Slider slider_time2;
        public Button button_Down;
        public Button button_Up;
        public GameObject go_Open;
        public GameObject go_Close;

        public IEnumerator SetCountDown()
        {
            float timeCount = ConfigData.BATTLE_REWARD_TIME_OUT;

            while (true)
            {
                timeCount -= 1;
                slider_time.value = timeCount / ConfigData.BATTLE_REWARD_TIME_OUT;
                slider_time2.value = timeCount / ConfigData.BATTLE_REWARD_TIME_OUT;
                text_time.text = $"{timeCount}";
                yield return new WaitForSeconds(1);

                if (timeCount <= 0)
                    break;
            }

            fieldBossRewardItems[0].OnClick_SelectReward(fieldBossRewardItems[0].fieldBossBuffType);

            Debug.Log("Time Over");
        }

        public override void ActivePopup()
        {
            SetRewardItem();

            PopUpSequence(true);

            openAnimation.Play();

            StartCoroutine(SetCountDown());
        }

        public async void SetRewardItem()
        {
            if (refreshCount <= 0)
                return;
            refreshCount--;
            text_Refresh.text = $"{refreshCount}";
            button_refresh.SetInterectible(refreshCount > 0);
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
                Debug.Log($"Selected rewards: {JsonConvert.SerializeObject(fieldBossRewardDatas[selected[i]])}");
                fieldBossRewardItems[i].Initialize(fieldBossRewardDatas[selected[i]]);
            }
        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            refreshCount = ConfigData.BATTLE_REWARD_REFRESH_COUNT;
            text_Refresh.text = $"{refreshCount}";
            button_refresh.SetInterectible(refreshCount > 0);

            button_refresh.onPointerUp = Onclick_Refresh;
            button_Down.onClick.AddListener(() =>
            {
                go_Close.gameObject.SetActive(true);
                go_Open.gameObject.SetActive(false);
            });
            button_Up.onClick.AddListener(() =>
            {
                go_Close.gameObject.SetActive(false);
                go_Open.gameObject.SetActive(true);
            });
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
