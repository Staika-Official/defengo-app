using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Sound;
using Framework.Util;
using Framework.GameData.Defense;

namespace Framework.Game.Defense
{
    public class MissionPopup : MonoBehaviour
    {
        public Button button_Panel;

        public bool isLemmingRichHiddenMission = false;
        public TextMeshProUGUI text_MissionMessage;
        public TextMeshProUGUI text_MissionReward;
        public GameObject missionMessage;
        public GameObject hiddenMissionPanel;
        public CanvasGroup canvasGroup;
        public Animation anim;
        public List<Animation> checkAnimList;
        public MissionCheckBox[] missionCheckBoxes;

        public GameObject eventGroup;
        public GameObject eventMissionCompleteObejct;
        public TextMeshProUGUI text_EventDesc;

        public bool isEvent;

        private void Awake()
        {
            OnClick_MissionButton(false);

            isEvent = true;
        }

        public void Initialize(MissionCheck[] missionChecks)
        {
            isEvent = UserInfoManager.Instance.eventInfo.eventBoss != 0;

            if (isEvent)
            {
                //Debug.Log("isEvent = " + isEvent);
                eventGroup.SetActive(true);
                SetEventMissionComplete(false);
                text_EventDesc.text = string.Format(LanguageManager.Instance.GetStringData("UI_Beta_Mission_Title"), UserInfoManager.Instance.eventInfo.eventBoss);
            }
            else
            {
                //Debug.Log("isEvent = " + isEvent);
                eventGroup.SetActive(false);
            }

            for (int i = 0; i < missionChecks.Length; i++)
            {
                if (missionChecks[i].isNonUi) continue;
                missionCheckBoxes[i].Initialize(missionChecks[i]);
            }

            button_Panel.onClick.AddListener(() =>
            {
                OnClick_MissionButton(false);
            });
        }

        public void SetEventMissionComplete(bool isComplete)
        {
            eventMissionCompleteObejct.SetActive(isComplete);
        }
        

        public void OnClick_MissionButton(bool isActive)
        {
            hiddenMissionPanel.SetActive(false);
            if(isLemmingRichHiddenMission) hiddenMissionPanel.SetActive(true);

            SoundManager.Instance.PlaySound(SoundKey.SF_MISSION_CHECK);
            canvasGroup.blocksRaycasts = isActive;
            canvasGroup.interactable = isActive;
            canvasGroup.alpha = isActive ? 1 : 0;
            anim.Rewind();
            anim.Play();
            CheckAnimPlay();
        }

        public void CheckAnimPlay()
        {
            foreach (Animation item in checkAnimList)
            {
                if (checkAnimList.Count < 0 || checkAnimList == null) break;
                item.Rewind();
                item.Play();
            }
        }

        public void EventMissionComplete()
        {
            //eventMissionCheck.MissionComplete();
            string complete = LanguageManager.Instance.GetStringData("UI_Complete");

            int level = UserInfoManager.Instance.eventInfo.eventBoss;

            string missionMessage = string.Format(LanguageManager.Instance.GetStringData("UI_Beta_Mission_Title"), level);

            text_MissionMessage.text = $"{missionMessage} {complete}";
            text_MissionReward.text = "x1";
            StartCoroutine(MissionMessage());
        }

        public void MissionComplete(int index)
        {
            missionCheckBoxes[index].MissionComplete();
            string complete = LanguageManager.Instance.GetStringData("UI_Complete");
            text_MissionMessage.text = $"{missionCheckBoxes[index].text_Mission.text} {complete}";
            text_MissionReward.text = $"x{missionCheckBoxes[index].reward}";
            StartCoroutine(MissionMessage());
        }

        public void NonMinssionBoxComplete(MissionCheck missionData)
        {
            GameManager.Instance.ChangeGem(missionData.rewardValue);
            string complete = LanguageManager.Instance.GetStringData("UI_Complete");
            text_MissionMessage.text = $"{LanguageManager.Instance.GetStringData(missionData.descriptionKey)} {complete}";
            text_MissionReward.text = $"x{missionData.rewardValue}";
            StartCoroutine(MissionMessage());
        }


        public IEnumerator MissionMessage()
        {
            missionMessage.SetActive(true);
            if(UIManager.Instance.anim_Interest.gameObject.activeSelf)
            {
                UIManager.Instance.anim_Interest.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(2.05f);
            if(TutorialManager.Instance.isTutorial)
            {
                TutorialManager.Instance.MissionDetail();
            }
            missionMessage.SetActive(false);   
        }
    }
}
