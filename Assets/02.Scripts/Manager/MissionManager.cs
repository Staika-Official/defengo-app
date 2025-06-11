using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Framework.GameData.Defense;
using System.Linq;
using Framework.Util;
using UnityEngine.Events;
using Framework.Sound;

namespace Framework.Game.Defense
{
    [System.Serializable]
    public class MissionList
    {
        public MissionCheck[] missions;
    }

    /// <summary>
    /// target Character 0은 모두, 타켓 설정 시 해당 캐릭터 Index
    /// </summary>
    [System.Serializable]
    public class MissionCheck
    {
        public int index;
        public int starGradeIdx;
        public int characterTypeCount;
        public int amount;
        public int minimumSummonCount;
        public bool isDone;
        public bool isNonUi;
        public int rewardValue;
        public int targetCharacter;
        public string descriptionKey;
    }

    //미션 인덱스
    //public int index;
    //미션 합성 캐릭터 (n성 캐릭터 보유) 사용
    //public int starGradeIdx;
    //몇개의 캐릭터가 필요한지 (예 : 서로다른 0성 캐릭터, 최초 3성 캐릭터)
    //public int characterTypeCount;
    //동일한 캐릭터 갯수
    //public int amount;
    //소환된 캐릭터의 제일 적은 갯수 (사용 용도 :소환된 캐릭터가 몇마리 이상일 때 미션 시작일 경우도 있으니)
    //public int minimumSummonCount;
    //지금 미션이 완료가 됐는지
    //public bool isDone;
    //지급될 리워드
    //public int rewardValue;
    //캐릭터 타겟 인덱스
    //public int targetCharacter;
    //UI Key
    //public string descriptionKey;


    [System.Serializable]
    public class SummonedCharacter
    {
        public List<Character> starGradeList = new();
    }

    public class MissionManager : MonoBehaviour
    {
        public MissionList missionList;
        public UnityAction checkHiddenMission;

        public int currentMissionClear;
        public int totalMissionClear;

        public Dictionary<CharacterIndex, SummonedCharacter> dic_CharacterList = new();

        private void Start()
        {
            Initialize();
        }

        public void SetTotalMissionData()
        {
            totalMissionClear = Mathf.Max(0, totalMissionClear - currentMissionClear);
        }

        public void MissionDataClear()
        {
            currentMissionClear = 0;
        }

        public async void Initialize()
        {
            currentMissionClear = 0;
            totalMissionClear = 0;

            missionList = await DataLoadManager.Instance.GetDataAsyncBinary<MissionList>("MissionList");

            // for (int i = 0; i < missionList.missions.Length; i++)
            // {
            //     //MissionCheck data = missionList.missions[i];
            //     //int minCount = ((int)Mathf.Pow(2, data.starGradeIdx)) * data.characterTypeCount * data.amount;
            //     //data.minimumSummonCount = minCount;
            // }

            int[] datas = GameManager.Instance.characterSpawner.slot.slotCharacterIds;
            for (int i = 0; i < GameManager.Instance.characterSpawner.slot.slotCharacterIds.Length - 1; i++)
            {
                CharacterIndex idx = (CharacterIndex)datas[i];
                SummonedCharacter summonedCharacter = new();
                dic_CharacterList.Add(idx, summonedCharacter);
            }

            UIManager.Instance.missionPopup.Initialize(missionList.missions);
        }

        public void AddOrRemoveCharacter(Character data, bool isAdd)
        {
            if (isAdd)
            {
                dic_CharacterList[data.characterIndex].starGradeList.Add(data);
                CheckMission();
            }
            else
            {
                dic_CharacterList[data.characterIndex].starGradeList.Remove(data);
            }
        }

        public void SetHiddenMission(CharacterIndex characterIndex, AttackType attackType)
        {
            switch (characterIndex)
            {
                case CharacterIndex.LEMMING_RICH:
                    checkHiddenMission -= CheckLemmingRichMission;
                    checkHiddenMission += CheckLemmingRichMission;
                    //Debug.Log("Set HiddenMission");
                    UIManager.Instance.missionPopup.isLemmingRichHiddenMission = true;
                    break;
            }

            switch (attackType)
            {
                case AttackType.BUFF:
                case AttackType.CROWDCONTROL:
                    checkHiddenMission -= CheckNonAttackTypeMission;
                    checkHiddenMission += CheckNonAttackTypeMission;
                    break;
            }

        }

        public void CheckNonAttackTypeMission()
        {
            bool isWaveCheck = GameManager.Instance.waveIdx == 0;
            if (isWaveCheck)
            {
                //제이슨 10번째 데이터
                MissionCheck data = missionList.missions[10];
                //현재 소환된 캐릭터가 데이터의 미니멈갯수 보다 작거나 퀘스트가 완료된 상태라면 return
                if (data.minimumSummonCount > GameManager.Instance.characterSpawner.summonCount || data.isDone) return;
                //한캐릭터의 종류당 갯수
                int targetAmount = data.amount;
                //목표치 캐릭터의 갯수 지정 (현재함수로선 비공격 타입의 캐릭터의 갯수를 설정한 것)
                int targetTypeCount = data.characterTypeCount;
                //원하는 조건에 부합하면 개수를 늘려주기 위한 변수
                int currentAmount = 0;

                //장착된 캐릭터 슬롯의 캐릭터가 몇마리 소환됬는지에 대한 딕셔너리 자료구조
                //순회
                foreach (var item in dic_CharacterList)
                {
                    //현재 소환된 캐릭터 리스트를 들고옴
                    List<Character> summonedData = dic_CharacterList[item.Key].starGradeList;

                    //소환된 캐릭터의 갯수가 내가 원하는 갯수보다 적을 경우 continue
                    if (summonedData.Count < targetAmount) continue;

                    //소환된 캐릭터 순회
                    for (int i = 0; i < summonedData.Count; ++i)
                    {
                        //현재 어택타입이 아니라면 조건성립
                        if (summonedData[i].characterData.attackType != AttackType.ATTACK)
                        {
                            //조건에 부합하면 목표치 조건에 성립시킬 변수 값 상승
                            ++currentAmount;
                        }
                    }

                    //비공격 캐릭터가 4마리 이상일 경우 조건 성립
                    if (currentAmount >= targetTypeCount)
                    {
                        //미션 성공
                        UIManager.Instance.missionPopup.NonMinssionBoxComplete(data);

                        //go지급해주는건 현재로썬 제외여서 CompleteMission 주석처리
                        SoundManager.Instance.PlaySound(SoundKey.SF_MISSION_COMPLETE);
                        //GameManager.Instance.CompleteMission(index);
                        data.isDone = true;
                        break;
                    }
                }
            }
            else
            {
                checkHiddenMission -= CheckNonAttackTypeMission;
            }


        }

        public void CheckLemmingRichMission()
        {
            MissionCheck data = missionList.missions[9];
            if (data.minimumSummonCount > GameManager.Instance.characterSpawner.summonCount || data.isDone) return;
            int targetStarGrade = data.starGradeIdx;
            //int targetTypeCount = data.characterTypeCount;
            int targetAmount = data.amount;

            //int currentTypeCount = 0;

            List<Character> summonedData = dic_CharacterList[CharacterIndex.LEMMING_RICH].starGradeList;
            int currentAmount = 0;
            if (summonedData.Count < targetAmount) return;

            for (int i = 0; i < summonedData.Count; i++)
            {
                if (summonedData[i].starGradeIndex == targetStarGrade)
                {
                    currentAmount++;
                }
            }

            if (currentAmount >= targetAmount)
            {
                int index = data.index;
                UIManager.Instance.missionPopup.MissionComplete(9);
                GameManager.Instance.CompleteMission(index);
                data.isDone = true;
            }
        }

        public void EventMission(int bossLevel)
        {
            if (bossLevel == UserInfoManager.Instance.eventInfo.eventBoss)
            {
                UIManager.Instance.missionPopup.SetEventMissionComplete(true);
            }
        }

        public void CheckMission()
        {
            int normalMissionCount = 9;

            for (int i = 0; i < normalMissionCount; i++)
            {
                MissionCheck data = missionList.missions[i];
                if (data.minimumSummonCount > GameManager.Instance.characterSpawner.summonCount || data.isDone) continue;
                int targetStarGrade = data.starGradeIdx;
                int targetTypeCount = data.characterTypeCount;
                int targetAmount = data.amount;

                int currentTypeCount = 0;

                foreach (var item in dic_CharacterList)
                {
                    List<Character> summonedData = dic_CharacterList[item.Key].starGradeList;
                    if (summonedData.Count < targetAmount) continue;
                    int currentAmount = 0;

                    for (int j = 0; j < summonedData.Count; j++)
                    {
                        if (summonedData[j].starGradeIndex == targetStarGrade)
                        {
                            currentAmount++;
                        }
                    }

                    if (currentAmount >= targetAmount)
                    {
                        currentTypeCount++;
                        if (currentTypeCount == targetTypeCount)
                        {
                            UIManager.Instance.missionPopup.MissionComplete(i);
                            int index = data.index;
                            GameManager.Instance.CompleteMission(index);
                            data.isDone = true;
                            break;
                        }
                    }
                }
            }
            checkHiddenMission?.Invoke();
        }
    }
}
