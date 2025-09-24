using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Framework.Game.Defense;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Framework.UI
{
    public class LobbyTutorialManager : MonoBehaviour
    {
        public enum TutorialMessageType
        {
            NORMAL_MESSAGE,
            FOCUS_MESSAGE,
            NONE_MESSAGE
        }

        public enum TutorialArrowType
        {
            UP_ARROW,
            DOWN_ARROW,
            NONE_ARROW
        }

        public enum UnMaskActiveType
        {
            UNMASK_ON,
            UNMASK_OFF,
        }

        public enum ButtonListenerType
        {
            ADDLISTENER,
            REMOVELISTENER
        }

        public static LobbyTutorialManager Instance;

        [HideInInspector]
        public GameObject object_Null;

        public GameObject object_Tutorial;
        public GameObject object_FocusMessage;
        public GameObject object_Message;
        public GameObject object_NullPrefab;
        public GameObject invisibleDim;

        public TextMeshProUGUI text_FocusTutorialMessage;
        public TextMeshProUGUI text_TutorialMessage;

        public Image image_Focus;
        public Image image_unmask;

        public Transform tutorialPivot;

        public RectTransform rect_UnMask;
        public RectTransform rect_DownArrow;
        public RectTransform rect_UpArrow;

        public Dictionary<string, Transform> dic_RectParent = new();
        public Dictionary<string, int> dic_Slibling = new();

        public Button button_NextSequence;
        public UnityAction nextSequence;
        public IEnumerator TypingSequence;

        public readonly float defalutRectSize = 5000f;
        public readonly float defalutDuration = 0.45f;

        public readonly float topAnchor = 0.6f;
        public readonly float bottomAnchor = -1f;

        public readonly float fadeInOutDuration = 0.5f;
        public readonly float fadeInOutAlpha = 0.2f;
        public Sequence sequenceImageFadeInOut;
        public Sequence sequenceTextFadeInOut;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void Initialize()
        {
            object_Tutorial.SetActive(true);

            invisibleDim.gameObject.SetActive(true);

            button_NextSequence.onClick.AddListener(() => { nextSequence?.Invoke(); });

            SetToastMessageSequence();

            switch (UserInfoManager.Instance.tutorial_State)
            {
                case TutorialProgressType.GAME_START:
                    //StartCoroutine( LeaderboardSequence());
                    StartCoroutine(GameStartSequence());
                    break;
                case TutorialProgressType.GAME_END:
                    StartCoroutine(LeaderboardSequence());
                    break;
            }
        }

        public IEnumerator GameStartSequence()
        {
            yield return null;

            //화살표 셋
            SetArrow(TutorialArrowType.NONE_ARROW, Vector2.zero, Vector2.zero);

            //알파값이없는 오브젝트
            SetUnMask(UnMaskActiveType.UNMASK_OFF, Vector2.zero, Vector2.one, defalutDuration);

            //튜토리얼 메세지 셋
            string text = LanguageManager.Instance.GetStringData("UI_Tutorial_1");
            SetTutorialMessage(TutorialMessageType.NORMAL_MESSAGE, text, GameStartTutorial);
        }

        public void GameStartTutorial()
        {
            nextSequence = () =>
            {
                GamePlayTutorial();
            };
        }

        public void GamePlayTutorial()
        {
            HomeScreen.Instance.button_NetworkPlay.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
            HomeScreen.Instance.button_Play.transform.SetParent(tutorialPivot);
            //nextSequence = null;

            SetTutorialMessage(TutorialMessageType.FOCUS_MESSAGE, null);

            Vector2 unmaskPosition = HomeScreen.Instance.button_Play.transform.position;
            Vector2 sizeDelta = new Vector2(530f, 245f);

            UnityAction action = () =>
            {
                Vector2 anchor = new Vector2(0f, topAnchor * rect_UnMask.rect.height);
                Vector2 worldVector = rect_UnMask.TransformPoint(anchor);
                Vector2 arrowVector = new Vector2(unmaskPosition.x, worldVector.y);
                SetArrow(TutorialArrowType.DOWN_ARROW, arrowVector, sizeDelta);
            };

            SetUnMask(UnMaskActiveType.UNMASK_ON, unmaskPosition, sizeDelta, defalutDuration, action);
        }

        public IEnumerator LeaderboardSequence()
        {
            yield return null;

            Button rankingButton = LobbyNavigator.Instance.dic_bottomProperty[MenuType.RANKING].button;

            Vector2 unmaskPosition = rankingButton.transform.position;

            unmaskPosition.y += 10f;
            Vector2 sizeDelta = new Vector2(180f, 190f);

            UnityAction action = () =>
            {
                if (object_Null == null)
                {
                    object_Null = Instantiate(object_NullPrefab);
                }

                SetButtonListener(ButtonListenerType.ADDLISTENER, rankingButton, "rankingButton", WeeklyLeaderBoardTutorial);

                object_Null.transform.SetParent(dic_RectParent["rankingButton"]);
                dic_Slibling.Add("rankingButton", (int)MenuType.RANKING);
                object_Null.transform.SetSiblingIndex((int)MenuType.RANKING);

                Vector2 anchor = new Vector2(0f, topAnchor * rect_UnMask.rect.height);
                Vector2 worldVector = rect_UnMask.TransformPoint(anchor);
                Vector2 arrowVector = new Vector2(unmaskPosition.x, worldVector.y);
                SetArrow(TutorialArrowType.DOWN_ARROW, arrowVector, sizeDelta);
            };

            //nextSequence = null;

            SetTutorialMessage(TutorialMessageType.FOCUS_MESSAGE, null);

            SetUnMask(UnMaskActiveType.UNMASK_ON, unmaskPosition, sizeDelta, defalutDuration, action);
        }

        public void WeeklyLeaderBoardTutorial()
        {
            object_Null.SetActive(false);

            Button rankingButton = LobbyNavigator.Instance.dic_bottomProperty[MenuType.RANKING].button;

            SetButtonListener(ButtonListenerType.REMOVELISTENER, rankingButton, "rankingButton", WeeklyLeaderBoardTutorial);
            rankingButton.transform.SetSiblingIndex(dic_Slibling["rankingButton"]);

            SetButtonListener(ButtonListenerType.ADDLISTENER, RankingScreen.Instance.button_weeklyTip, "weeklyTip", WeeklyLeaderBoardRewardTutorial);

            SetTutorialMessage(TutorialMessageType.FOCUS_MESSAGE, null);

            Vector2 unmaskPosition = RankingScreen.Instance.button_weeklyTip.transform.position;
            Vector2 sizeDelta = new Vector2(158f, 158f);
            SetUnMask(UnMaskActiveType.UNMASK_ON, unmaskPosition, sizeDelta, defalutDuration);


            Vector2 anchor = new Vector2(0f, bottomAnchor * rect_UnMask.rect.height);
            Vector2 worldVector = rect_UnMask.TransformPoint(anchor);
            Vector2 arrowVector = new Vector2(unmaskPosition.x, worldVector.y);
            SetArrow(TutorialArrowType.UP_ARROW, arrowVector, sizeDelta);
        }


        public void WeeklyLeaderBoardRewardTutorial()
        {
            //위클리 보상 설명
            SetButtonListener(ButtonListenerType.REMOVELISTENER, RankingScreen.Instance.button_weeklyTip, "weeklyTip", WeeklyLeaderBoardRewardTutorial);

            nextSequence = () =>
            {
                WeeklyLeaderBoardEndTutorial();
            };

            string text = LanguageManager.Instance.GetStringData("UI_Tutorial_14");
            SetTutorialMessage(TutorialMessageType.NORMAL_MESSAGE, text);

            SetArrow(TutorialArrowType.NONE_ARROW, Vector2.zero, Vector2.zero);

            SetUnMask(UnMaskActiveType.UNMASK_OFF, Vector2.zero, Vector2.one, defalutDuration);
        }

        public void WeeklyLeaderBoardEndTutorial()
        {
            //위클릭 보상 닫기

            RewardGuidePopup popup = PopupManager.Instance.GetPopUp<RewardGuidePopup>("rewardGuide");

            SetButtonListener(ButtonListenerType.ADDLISTENER, popup.button_Close, "weeklyClose", HomeButtonTutorial);

            //nextSequence = null;

            SetTutorialMessage(TutorialMessageType.FOCUS_MESSAGE, null);

            Vector2 unmaskPosition = popup.button_Close.transform.position;
            Vector2 sizeDelta = new Vector2(100f, 100f);

            SetUnMask(UnMaskActiveType.UNMASK_ON, unmaskPosition, sizeDelta, defalutDuration);

            Vector2 anchor = new Vector2(0f, bottomAnchor * rect_UnMask.rect.height);
            Vector2 worldVector = rect_UnMask.TransformPoint(anchor);
            Vector2 arrowVector = new Vector2(unmaskPosition.x, worldVector.y);
            SetArrow(TutorialArrowType.UP_ARROW, arrowVector, sizeDelta);
        }

        public void HomeButtonTutorial()
        {
            //위클리 보상리스트 닫기

            RewardGuidePopup popup = PopupManager.Instance.GetPopUp<RewardGuidePopup>("rewardGuide");
            Button button = popup.button_Close;

            SetButtonListener(ButtonListenerType.REMOVELISTENER, button, "weeklyClose", HomeButtonTutorial);

            //빈 오브젝트 추가 및 삭제 -> 부모, 이동 인덱스, 널 오브젝트(1개로 왔다가 갔다가 하는 형식으로)
            //빈오브젝트 추가이유 리스트 추가할때 위치가 이동되서 사용되기에 이동인덱스 필요
            if (object_Null == null)
            {
                object_Null = Instantiate(object_NullPrefab);
            }

            Button homeButton = LobbyNavigator.Instance.dic_bottomProperty[MenuType.HOME].button;

            object_Null.SetActive(true);
            object_Null.transform.SetParent(homeButton.transform.parent);
            dic_Slibling.Add("HomeButton", 2);
            object_Null.transform.SetSiblingIndex(2);

            SetButtonListener(ButtonListenerType.ADDLISTENER, homeButton, "HomeButton", InboxTutorial);

            Vector2 unmaskPosition = homeButton.transform.position;
            unmaskPosition.y += 10f;
            Vector2 sizeDelta = new Vector2(180f, 190f);

            //홈버튼 클릭
            SetTutorialMessage(TutorialMessageType.FOCUS_MESSAGE, null);

            SetUnMask(UnMaskActiveType.UNMASK_ON, unmaskPosition, sizeDelta, defalutDuration);

            Vector2 anchor = new Vector2(0f, 1f * rect_UnMask.rect.height);
            Vector2 worldVector = rect_UnMask.TransformPoint(anchor);
            Vector2 arrowVector = new Vector2(unmaskPosition.x, worldVector.y);
            SetArrow(TutorialArrowType.DOWN_ARROW, arrowVector, sizeDelta);

            //todo finished 튜토
            FinishedTutorial();
        }

        public void InboxTutorial()
        {
            //우편함 클릭

            ButtonComponent inboxButton = HomeScreen.Instance.button_Inbox;

            //활성화된 친구를 알아야함 고로 부모를 구해와서 활성화된 친구 찾기
            int count = -1;
            for (int i = 0; i < inboxButton.transform.parent.childCount; ++i)
            {
                var temp = inboxButton.transform.parent.GetChild(i);
                if (temp.gameObject.activeSelf)
                {
                    count++;
                    if (temp == inboxButton.transform)
                    {
                        break;
                    }
                }
            }

            SetButtonListener(ButtonListenerType.ADDLISTENER, inboxButton, "Inbox", InboxTutorialEnd);

            if (object_Null == null)
            {
                object_Null = Instantiate(object_NullPrefab);
            }

            if (count > -1)
            {
                Debug.Log(count);
                object_Null.GetComponent<RectTransform>().sizeDelta = inboxButton.GetComponent<RectTransform>().sizeDelta;
                object_Null.transform.SetParent(dic_RectParent["Inbox"]);
                object_Null.SetActive(true);
                object_Null.transform.SetSiblingIndex(count);
                dic_Slibling.Add("Inbox", count);
            }

            SetButtonListener(ButtonListenerType.REMOVELISTENER, LobbyNavigator.Instance.dic_bottomProperty[MenuType.HOME].button, "HomeButton", InboxTutorial);
            LobbyNavigator.Instance.dic_bottomProperty[MenuType.HOME].button.transform.SetSiblingIndex(dic_Slibling["HomeButton"]);

            SetTutorialMessage(TutorialMessageType.FOCUS_MESSAGE, null);

            Vector2 unmaskPosition = inboxButton.transform.position;
            Vector2 sizeDelta = new Vector2(110f, 110f);
            SetUnMask(UnMaskActiveType.UNMASK_ON, unmaskPosition, sizeDelta, defalutDuration);


            Vector2 anchor = new Vector2(0f, bottomAnchor * rect_UnMask.rect.height);
            Vector2 worldVector = rect_UnMask.TransformPoint(anchor);
            Vector2 arrowVector = new Vector2(unmaskPosition.x, worldVector.y);
            SetArrow(TutorialArrowType.UP_ARROW, arrowVector, sizeDelta);
        }


        public void InboxTutorialEnd()
        {
            //우편함 클릭 후 메세지
            object_Null.SetActive(false);

            ButtonComponent button = HomeScreen.Instance.button_Inbox;
            SetButtonListener(ButtonListenerType.REMOVELISTENER, button, "Inbox", InboxTutorialEnd);
            HomeScreen.Instance.button_Inbox.button.transform.SetSiblingIndex(dic_Slibling["Inbox"]);

            nextSequence = () =>
            {
                TutorialEnd();
            };

            SetArrow(TutorialArrowType.NONE_ARROW, Vector2.zero, Vector2.zero);

            SetUnMask(UnMaskActiveType.UNMASK_OFF, Vector2.zero, Vector2.zero, defalutDuration);

            string text = LanguageManager.Instance.GetStringData("UI_Tutorial_15");
            SetTutorialMessage(TutorialMessageType.NORMAL_MESSAGE, text);

            //우편함 닫고 기존에 로비씬들어갈때 날짜 체크하던거 해줘야함
            //추가로 우편함닫고 공지나 스킨판매가 나오게 처리해줘야함
            //만약 튜토 마지막이 바뀐다면 버튼이 바뀌어야함
            InboxPopup popup = PopupManager.Instance.GetPopUp<InboxPopup>("inbox");
            popup.button_Close.onClick.AddListener(HomePopUpCheck);
        }

        public void HomePopUpCheck()
        {
            //튜토가 종료되고 우편함을 닫으면 기존에 체크해서 띄워야했던 팝업창을 띄워줌
            //예: 이벤트 공지, 스킨
            InboxPopup popup = PopupManager.Instance.GetPopUp<InboxPopup>("inbox");
            popup.button_Close.onClick.RemoveListener(HomePopUpCheck);

            LobbyManager.Instance.CheckActivePopup();
        }

        public void TutorialEnd()
        {
            //튜토 끝
            nextSequence = () =>
            {
                dic_RectParent.Clear();
                dic_Slibling.Clear();

                SetTutorialMessage(TutorialMessageType.NONE_MESSAGE, null);
                SetArrow(TutorialArrowType.NONE_ARROW, Vector2.zero, Vector2.zero);
                SetUnMask(UnMaskActiveType.UNMASK_OFF, Vector2.zero, Vector2.one, defalutDuration);

                invisibleDim.SetActive(false);
                object_Tutorial.SetActive(false);

                nextSequence = null;
            };
        }

        public void FinishedTutorial()
        {
            //튜토 보상지급 및 유저 업데이트
            if (!UserInfoManager.Instance.userState.finishedTutorial)
            {
                UserInfoManager.Instance.userState.finishedTutorial = true;

                Dictionary<string, string> updateData = new()
                {
                    { "userId", UserInfoManager.Instance.userId },
                    { "finishedTutorial", "true" }
                };

                _ = NetworkManager.Instance.UpdateUserState(updateData, Finish);
            }
        }

        public void Finish()
        {
            //D_tutorial
            AdjustInitializer.TrackEvent("36a857");

            //인게임
            //UIManager.Instance.GameOver();
            LobbyManager.Instance.GetEnergyValue();

            Debug.Log("Finish");
        }

        public void SetTutorialMessage(TutorialMessageType type, string text, UnityAction action = null)
        {
            //튜토리얼 메세지 셋
            //1. 타입 2.텍스트 (포커스 메세지인경우 null넣어주면 디폴트가 들어감) 3.콜백함수 
            object_FocusMessage.SetActive(false);
            object_Message.SetActive(false);

            if (TypingSequence != null)
            {
                StopCoroutine(TypingSequence);
                TypingSequence = null;
            }

            switch (type)
            {
                case TutorialMessageType.NORMAL_MESSAGE:
                    TypingSequence = TypingAction(text_TutorialMessage, object_Message, text, action);
                    StartCoroutine(TypingSequence);
                    break;
                case TutorialMessageType.FOCUS_MESSAGE:
                    if (null == text)
                    {
                        text = LanguageManager.Instance.GetStringData("TMSG_Tutorial_Fail");
                        text_FocusTutorialMessage.text = text;
                    }
                    nextSequence = null;
                    nextSequence = () =>
                    {
                        ToastMessageFadeInout();
                    };

                    // TypingSequence = TypingAction(text_FocusTutorialMessage, object_FocusMessage, text, action);
                    // StartCoroutine(TypingSequence);
                    break;
            }
        }

        public void SetArrow(TutorialArrowType type, Vector2 position, Vector2 sizeDelta)
        {
            //손가락 화살표
            //1.위 화살표 아래화살표 타입 2.포지션 3.unmask사이즈 (거리를 조정하기 위해)
            rect_UpArrow.gameObject.SetActive(false);
            rect_DownArrow.gameObject.SetActive(false);
            rect_DownArrow.position = Vector2.zero;
            rect_UpArrow.position = Vector2.zero;

            switch (type)
            {
                case TutorialArrowType.UP_ARROW:
                    rect_UpArrow.gameObject.SetActive(true);
                    rect_UpArrow.position = position;
                    break;
                case TutorialArrowType.DOWN_ARROW:
                    rect_DownArrow.gameObject.SetActive(true);
                    rect_DownArrow.position = position;
                    break;

            }
        }

        public void SetUnMask(UnMaskActiveType type, Vector2 position, Vector2 sizeDelta, float duration, UnityAction action = null)
        {
            //언 마스크
            //1. 활성화 및 비활성화 2.포지션 3.범위 4.언마스크 완료되는시간 5.콜백함수
            switch (type)
            {
                case UnMaskActiveType.UNMASK_ON:
                    image_unmask.gameObject.SetActive(true);
                    image_unmask.rectTransform.position = position;

                    image_unmask.rectTransform.DOSizeDelta(new Vector2(defalutRectSize, defalutRectSize), 0);
                    image_unmask.rectTransform.DOSizeDelta(sizeDelta, duration).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        action?.Invoke();
                    });
                    break;
                case UnMaskActiveType.UNMASK_OFF:
                    image_unmask.gameObject.SetActive(false);
                    image_unmask.rectTransform.position = position;
                    break;
            }
        }


        public IEnumerator TypingAction(TextMeshProUGUI textMeshPro, GameObject objectMessage, string text, UnityAction action)
        {
            //메세지 무한 클릭 방지로인한 코루틴 함수

            button_NextSequence.interactable = false;

            //textMeshPro.text = originText;
            textMeshPro.text = text;
            yield return new WaitForSeconds(0.2f);
            objectMessage.SetActive(true);

            button_NextSequence.interactable = true;
            action?.Invoke();
        }


        public void SetButtonListener(ButtonListenerType type, Button button, string parentKey, UnityAction action)
        {
            //버튼 액션 추가 및 삭제
            //1.액션 추가 및 삭제, 2.버튼 3.버튼 부모키지정(추가 할때랑 삭제할때 부모를 셋해주기위함 현재는 tutorialpivot자식만 클릭이 가능하기에) 4.콜백함수
            Transform parent;

            switch (type)
            {
                case ButtonListenerType.ADDLISTENER:
                    parent = button.transform.parent;
                    dic_RectParent.Add(parentKey, parent);
                    button.transform.SetParent(tutorialPivot);
                    button.onClick.AddListener(action);
                    break;
                case ButtonListenerType.REMOVELISTENER:
                    parent = dic_RectParent[parentKey];
                    button.transform.SetParent(parent);
                    button.onClick.RemoveListener(action);
                    break;
            }
        }

        public void SetButtonListener(ButtonListenerType type, ButtonComponent button, string parentKey, UnityAction action)
        {
            Transform parent;

            switch (type)
            {
                case ButtonListenerType.ADDLISTENER:
                    parent = button.transform.parent;
                    dic_RectParent.Add(parentKey, parent);
                    button.transform.SetParent(tutorialPivot);
                    button.onPointerUp += action;
                    break;
                case ButtonListenerType.REMOVELISTENER:
                    parent = dic_RectParent[parentKey];
                    button.transform.SetParent(parent);
                    button.onPointerUp -= action;
                    break;
            }
        }

        public void LobbyTutorialFailed()
        {
            object_Tutorial.SetActive(false);
            invisibleDim.gameObject.SetActive(false);
            button_NextSequence.onClick.RemoveAllListeners();
        }

        public void ToastMessageFadeInout()
        {
            sequenceImageFadeInOut.Restart();
            sequenceTextFadeInOut.Restart();
            //LobbyManager.Instance.toastMessage.Sequence("hahaha");
        }

        public void SetToastMessageSequence()
        {
            Color color = Color.white;
            color.a = 0f;
            image_Focus.color = color;

            Color textColor = text_FocusTutorialMessage.color;
            textColor.a = 0f;
            text_FocusTutorialMessage.color = textColor;

            sequenceImageFadeInOut = DOTween.Sequence()
                .SetAutoKill(false).
                OnRewind(() =>
                {
                    object_FocusMessage.SetActive(true);
                })
                .Append(image_Focus.DOFade(fadeInOutAlpha, fadeInOutDuration))
                .Append(image_Focus.DOFade(0f, fadeInOutDuration))
                .OnComplete(() =>
                {
                    image_Focus.color = color;
                    object_FocusMessage.SetActive(false);
                });

            sequenceTextFadeInOut = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(text_FocusTutorialMessage.DOFade(1.0f, fadeInOutDuration))
                .Append(text_FocusTutorialMessage.DOFade(0f, fadeInOutDuration))
                .OnComplete(() =>
                {
                    text_FocusTutorialMessage.color = textColor;
                });
        }
    }
}