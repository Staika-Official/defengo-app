using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Util;

namespace Framework.Game.Defense
{
    public class TutorialData
    {
        public string message;
        public int actionCount;
    }

    [System.Serializable]
    public class TutorialSummonData
    {
        public CharacterIndex characterIndex;
        public int glacierIdx;
    }

    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance;
        public bool isTutorial = false;
        public GameObject messageObject;
        public GameObject focusMessageObject;
        public RectTransform gemTransform;
        public TextMeshProUGUI text_TutorialMessage;
        public TextMeshProUGUI text_FocusTutorialMessage;
        public Image image_SpeechBalloon;
        public Image image_unmask;
        public Image image_subUnmask;
        public Image image_Focus;
        public CanvasGroup dim;
        public Button button_NextSequence;
        public string originText;
        public string subText;
        public Transform tutorialPivot;
        public Transform menuButtonPos;
        public IEnumerator TypingSequence;
        public float unmaskDuration;
        public UnityAction nextSequence;
        public int summonCount;
        public TutorialData[] tutorialDatas;
        public GameObject summonArrow;
        public GameObject bossArrow;
        public GameObject missionArrow;
        public GameObject dragArrow;
        public GameObject clickNotice;
        public TutorialSummonData[] tutorialSummonDatas;
        public bool isWrongArea;

        
        public readonly float fadeInOutDuration = 0.3f;
        public Sequence sequenceImageFadeInOut;
        public Sequence sequenceTextFadeInOut;
        
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public Sequence sequence;

        
        public void ToastMessageFadeInout()
        {
            sequenceImageFadeInOut.Restart();
            sequenceTextFadeInOut.Restart();
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
                    focusMessageObject.SetActive(true);  
                })
                .Append(image_Focus.DOFade(0.2f, fadeInOutDuration))
                .Append(image_Focus.DOFade(0f, fadeInOutDuration))
                .OnComplete(() =>
                {
                    image_Focus.color = color;
                    focusMessageObject.SetActive(false);  
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
        
        public void OnClick_Focus()
        {
            Debug.Log("focus");
            clickNotice.SetActive(true);
            // if(sequence.active && sequence != null)
            // {
            //     sequence.Kill();
            // }
            sequence = DOTween.Sequence()
                .Append(clickNotice.transform.DOScale(Vector2.zero, 0))
                .Append(clickNotice.transform.DOScale(Vector2.one, 0.3f))
                .AppendInterval(1f)
                .Append(clickNotice.transform.DOScale(Vector2.zero, 0.1f));          
            sequence.Play();
        }

        public void Initiailize(bool isTutorial)
        {
            this.isTutorial = isTutorial;

            if (isTutorial)
            {
                summonCount = 0;
                dim.gameObject.SetActive(true);
                
                SetToastMessageSequence();

                button_NextSequence.onClick.AddListener(() =>
                {
                    nextSequence?.Invoke();
                });
                UserInfoManager.Instance.tutorial_State = TutorialProgressType.GAME_PROGRESS;
            }
            else
            {
                messageObject.SetActive(false);
                dim.gameObject.SetActive(false);
            }
        }

        public Sequence UnMaskSequence(Vector2 scale)
        {
            Sequence sequence = DOTween.Sequence()
                .Append(image_unmask.rectTransform.DOScale(Vector2.zero, 0))
                .Append(image_unmask.rectTransform.DOScale(scale, unmaskDuration));

            return sequence;
        }

        public Sequence SpeechBalloon()
        {
            Sequence sequence = DOTween.Sequence()
                .Append(image_SpeechBalloon.rectTransform.DOSizeDelta(Vector2.zero, 0))
                .Append(image_SpeechBalloon.rectTransform.DOSizeDelta(new Vector2(800, 600), 0.5f))
                .SetEase(Ease.OutBounce);

            return sequence;
        }

        public TutorialSummonData GetSummonData()
        {
            TutorialSummonData data = tutorialSummonDatas[summonCount];
            summonCount++;
            //if (summonCount == 1)
            //{
                //dim.DOFade(0, 0.1f);
            //}

            if (summonCount == 4)
            {
                UIManager.Instance.button_SummonCharacter.transform.SetParent(menuButtonPos);
                dim.DOFade(0, 0.1f);
                button_NextSequence.interactable = true;
                nextSequence = null;
                WaveStartSequence();
            }

            // if (summonCount == 5)
            // {
            //     dim.DOFade(0, 0.1f);
            // }

            if (summonCount == 6)
            {
                nextSequence = null;
                dim.DOFade(1, 0.1f);
                messageObject.SetActive(true);
                image_unmask.gameObject.SetActive(false);
                CombineMessage();
                button_NextSequence.interactable = true;
            }

            if (summonCount == 7)
            {
                BossSummonMessage();
            }

            if (summonCount == 8)
            {
                dim.DOFade(0, 0.1f);
            }

            //if (summonCount == 9)
            //{
            //    Debug.Log("mission Clear");
            //    dim.DOFade(1, 0.1f);
            //}
            
            if (summonCount == 10)
            {
                summonArrow.SetActive(false);
            }

            Debug.Log($"tutorial summonCount {summonCount}");

            return data;
        }

        public void OnClick_NextSequence()
        {
            Debug.Log("Next Sequence");
            nextSequence?.Invoke();
            nextSequence = null;
        }

        public IEnumerator TypingAction(UnityAction action)
        {
            button_NextSequence.interactable = false;
            text_TutorialMessage.text = originText;
            yield return new WaitForSeconds(0.2f);
            messageObject.SetActive(true);

            button_NextSequence.interactable = true;
            action?.Invoke();
        }

        public void Initialize()
        {
            Debug.Log("Initialize");
            messageObject.SetActive(false);
            image_subUnmask.gameObject.SetActive(false);
            dim.DOFade(1, 0);
            UIManager.Instance.uISequences_WaveCountTimer.gameObject.SetActive(true);
            originText = LanguageManager.Instance.GetStringData("UI_Tutorial_2");
            image_unmask.rectTransform.position = UIManager.Instance.text_TimeCount.rectTransform.position;
            image_unmask.rectTransform.DOSizeDelta(new Vector2(5000, 5000), 0);
            image_unmask.rectTransform.DOSizeDelta(new Vector2(150, 150), 0.45f).SetEase(Ease.OutQuad);
            TypingSequence = TypingAction(SummonTutorial);
            StartCoroutine(TypingSequence);
        }

        public void SummonTutorial()
        {
            nextSequence = () =>
            {
                Debug.Log("SummonTutorial");
                //button_NextSequence.interactable = false;
                
                text_FocusTutorialMessage.text = LanguageManager.Instance.GetStringData("TMSG_Tutorial_Fail");
                //focusMessageObject.SetActive(true);
                
                //todo dongmin
                nextSequence = () =>
                {
                    ToastMessageFadeInout();
                };
        
                //button_NextSequence.onClick.RemoveAllListeners();
                //button_NextSequence.onClick.AddListener(()=> OnClick_Focus());
                messageObject.SetActive(false);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(5000, 5000), 0);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(200, 300), 0.45f).SetEase(Ease.OutQuad);
                image_unmask.rectTransform.position = UIManager.Instance.button_SummonCharacter.transform.position;
                summonArrow.SetActive(true);
                CharacterSummonSequence();
                //button_NextSequence.interactable = false;
            };
        }

        public void CombineTutorial()
        {
            nextSequence = () =>
            {
                Debug.Log("CobineTutorial");
                messageObject.SetActive(false);

                UIManager.Instance.button_SummonCharacter.transform.SetParent(menuButtonPos);

                for (int i = 0; i < GameManager.Instance.characterSpawner.summonedCharacters.Count; i++)
                {
                    if (GameManager.Instance.characterSpawner.summonedCharacters[i].glacierIdx == 7)
                    {
                        GameManager.Instance.characterSpawner.summonedCharacters[i].isDraggable = true;
                    }
                }

                image_unmask.gameObject.SetActive(true);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(0, 0), 0);
                Vector2 firstPos = Camera.main.WorldToScreenPoint(GridManager.Instance.glaciersTiles[4].centerPivot);
                image_unmask.rectTransform.position = firstPos;
                image_unmask.rectTransform.DOSizeDelta(new Vector2(200, 200), 0.25f).SetEase(Ease.OutQuad);

                image_subUnmask.gameObject.SetActive(true);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(0, 0), 0);
                Vector2 secondPos = Camera.main.WorldToScreenPoint(GridManager.Instance.glaciersTiles[7].centerPivot);
                image_subUnmask.rectTransform.position = secondPos;
                image_unmask.rectTransform.DOSizeDelta(new Vector2(200, 200), 0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    dragArrow.SetActive(true);
                });
                nextSequence = null;
            };
        }

        public void BossSummonTutorial()
        {

        }

        public void CharacterSummonSequence()
        {
            UIManager.Instance.button_SummonCharacter.transform.SetParent(tutorialPivot);
        }

        public void WaveStartSequence()
        {
            Debug.Log("Wave Start Sequence");
            focusMessageObject.SetActive(false);
            summonArrow.SetActive(false);
            button_NextSequence.interactable = false;
            //button_NextSequence.onClick.AddListener(() => OnClick_Focus());
            GameManager.Instance.TutorialCountStart();

            nextSequence = () =>
            {
                FocusGem();
            };
        }

        public void FocusGem()
        {
            Debug.Log("Focus Gem");
            button_NextSequence.interactable = true;
            dim.DOFade(1, 0.2f);
            image_unmask.rectTransform.DOSizeDelta(new Vector2(5000, 5000), 0);
            image_unmask.rectTransform.DOSizeDelta(new Vector2(200, 200), 0.45f).SetEase(Ease.OutQuad);
            image_unmask.rectTransform.position = gemTransform.position;

            originText = LanguageManager.Instance.GetStringData("UI_Tutorial_3");
            //image_unmask.gameObject.SetActive(false);
            nextSequence = null;
            TypingSequence = TypingAction(CombineDetail);
            StartCoroutine(TypingSequence);
            //nextSequence = () =>
            //{

            //};
        }

        public void CombineDetail()
        {
            //messageObject.SetActive(false);

            nextSequence = () =>
            {
                image_unmask.gameObject.SetActive(false);
                
                text_FocusTutorialMessage.text = LanguageManager.Instance.GetStringData("TMSG_Tutorial_Fail");
                //focusMessageObject.SetActive(true);
                
                nextSequence = () =>
                {
                    ToastMessageFadeInout();
                };

                
                summonArrow.SetActive(true);
                Debug.Log("Combine Detail");
                //button_NextSequence.interactable = false;
                //button_NextSequence.onClick.AddListener(() => OnClick_Focus());
                messageObject.SetActive(false);
                image_unmask.gameObject.SetActive(true);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(5000, 5000), 0);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(200, 300), 0.45f).SetEase(Ease.OutQuad);
                image_unmask.rectTransform.position = UIManager.Instance.button_SummonCharacter.transform.position;
                CharacterSummonSequence();
            };
        }

        public void CombineMessage()
        {
            Debug.Log("Cobine Message");
            originText = LanguageManager.Instance.GetStringData("UI_Tutorial_4");
            focusMessageObject.SetActive(false);
            summonArrow.SetActive(false);
            messageObject.SetActive(true);
            image_unmask.gameObject.SetActive(false);
            UIManager.Instance.button_SummonCharacter.transform.SetParent(menuButtonPos);
            TypingSequence = TypingAction(CombineTutorial);
            StartCoroutine(TypingSequence);
        }

        public void BossSummonMessage()
        {
            Debug.Log("Boss Summon Message");
            dim.DOFade(1, 0.1f);
            dragArrow.SetActive(false);
            originText = LanguageManager.Instance.GetStringData("UI_Tutorial_5");

            messageObject.SetActive(true);
            image_unmask.gameObject.SetActive(false);
            image_subUnmask.gameObject.SetActive(false);

            nextSequence = null;
            TypingSequence = TypingAction(BossSummonDetail);
            StartCoroutine(TypingSequence);
        }

        public void BossSummonDetail()
        {
            nextSequence = () =>
            {
                Debug.Log("Boss Summon Detail");
                //button_NextSequence.interactable = false;
                //button_NextSequence.onClick.AddListener(() => OnClick_Focus());
                //dragArrow.SetActive(false);
                messageObject.SetActive(false);
                image_unmask.gameObject.SetActive(true);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(5000, 5000), 0);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(400, 320), 0.45f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    bossArrow.SetActive(true);
                    UIManager.Instance.button_SummonBoss.transform.SetParent(tutorialPivot);
                });
                image_unmask.rectTransform.position = UIManager.Instance.button_SummonBoss.transform.position;

                text_FocusTutorialMessage.text = LanguageManager.Instance.GetStringData("TMSG_Tutorial_Fail");
                nextSequence = () =>
                {
                    ToastMessageFadeInout();
                };
                //BossSummonSequence();
            };
        }

        public void BossSummonSequence()
        {
            Debug.Log("Boss Summon Seq");
            focusMessageObject.SetActive(false);
            bossArrow.SetActive(false);
            image_unmask.gameObject.SetActive(false);
            dim.DOFade(0, 0.1f);
            button_NextSequence.interactable = false;
            ResetBossSequence();
        }

        public void ResetBossSequence()
        {
            nextSequence = () =>
            {
                Debug.Log("Reset Boss Sequence");
                button_NextSequence.interactable = true;

                UIManager.Instance.button_SummonBoss.transform.SetParent(menuButtonPos);
                dim.DOFade(1, 0.1f);

                messageObject.SetActive(true);

                originText = LanguageManager.Instance.GetStringData("UI_Tutorial_6");

                TypingSequence = TypingAction(MissionMessage);
                StartCoroutine(TypingSequence);
            };
        }

        public void MissionMessage()
        {
            nextSequence = () =>
            {
                Debug.Log("Mission Message");
                //button_NextSequence.interactable = false;
                
                text_FocusTutorialMessage.text = LanguageManager.Instance.GetStringData("TMSG_Tutorial_Fail");
                //focusMessageObject.SetActive(true);

                nextSequence = () =>
                {
                    ToastMessageFadeInout();
                };
                
                summonArrow.SetActive(true);
                messageObject.SetActive(false);
                image_unmask.gameObject.SetActive(true);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(5000, 5000), 0);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(200, 300), 0.45f).SetEase(Ease.OutQuad);
                image_unmask.rectTransform.position = UIManager.Instance.button_SummonCharacter.transform.position;

                UIManager.Instance.button_SummonCharacter.transform.SetParent(tutorialPivot);

                //MissionDetail();
            };
        }

        public void MissionDetail()
        {
            Debug.Log("Mission Detail");
            button_NextSequence.interactable = true;
            dim.DOFade(1, 0.1f);
            originText = LanguageManager.Instance.GetStringData("UI_Tutorial_7");
            summonArrow.SetActive(false);
            focusMessageObject.SetActive(false);
            messageObject.SetActive(true);

            image_unmask.gameObject.SetActive(false);
            UIManager.Instance.button_SummonCharacter.transform.SetParent(menuButtonPos);
            
            TypingSequence = TypingAction(MissionButtonFocus);
            StartCoroutine(TypingSequence);
        }

        public void MissionButtonFocus()
        {
            nextSequence = () =>
            {
                Debug.Log("Mission Button Focus");
                //missionArrow.SetActive(true);
                dim.DOFade(1, 0.1f);
                //messageObject.SetActive(false);
                image_unmask.gameObject.SetActive(true);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(5000, 5000), 0);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(100, 100), 0.45f).SetEase(Ease.OutQuad);
                image_unmask.rectTransform.position = UIManager.Instance.button_MisstionCheck.transform.position;

                InterestMessage();
            };
        }

        public void InterestMessage()
        {
            nextSequence = () =>
            {
                Debug.Log("Interest Message");
                image_unmask.gameObject.SetActive(false);
                messageObject.SetActive(false);
                //UIManager.Instance.button_SummonCharacter.transform.SetParent(menuButtonPos);
                originText = LanguageManager.Instance.GetStringData("UI_Tutorial_8");
                TypingSequence = TypingAction(InterestDetail);
                StartCoroutine(TypingSequence);

                messageObject.SetActive(true);
            };
        }

        public void InterestDetail()
        {
            nextSequence = () =>
            {
                Debug.Log("Interest Detail");
                //messageObject.SetActive(false);
                originText = LanguageManager.Instance.GetStringData("UI_Tutorial_9");
                messageObject.SetActive(true);
                image_unmask.gameObject.SetActive(true);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(5000, 5000), 0);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(200, 200), 0.45f).SetEase(Ease.OutQuad);
                image_unmask.rectTransform.position = gemTransform.position;
                TypingSequence = TypingAction(DamageTutorial);
                StartCoroutine(TypingSequence);
                //DamageTutorial();
            };
        }

        public void DamageTutorial()
        {
            nextSequence = () =>
            {
                Debug.Log("Damage Tutorial");
                dim.DOFade(0, 0.1f);
                GameManager.Instance.TutorialCountStart();
                messageObject.SetActive(false);
                button_NextSequence.interactable = false;
                //button_NextSequence.onClick.AddListener(() => OnClick_Focus());


                DamageFocus();
            };
        }

        public void DamageFocus()
        {
            nextSequence = () =>
            {
                Debug.Log("Damage Focus");

                button_NextSequence.interactable = true;

                dim.DOFade(1, 0.1f);
                image_unmask.gameObject.SetActive(true);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(5000, 5000), 0);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(200, 200), 0.45f).SetEase(Ease.OutQuad);
                image_unmask.rectTransform.position = Camera.main.WorldToScreenPoint(GridManager.Instance.glaciersTiles[18].centerPivot);

                //image_unmask.gameObject.SetActive(false);
                originText = LanguageManager.Instance.GetStringData("UI_Tutorial_10");
                TypingSequence = TypingAction(FocusUpgrade);
                StartCoroutine(TypingSequence);

                messageObject.SetActive(true);

                //DamageDetail();
            };
        }

        public void FocusUpgrade()
        {
            nextSequence = () =>
            {
                Debug.Log("Focus Upgrade");
                //messageObject.SetActive(false);
                originText = LanguageManager.Instance.GetStringData("UI_Tutorial_11");


                //messageObject.SetActive(true);
                image_unmask.gameObject.SetActive(true);
                image_unmask.rectTransform.DOSizeDelta(new Vector2(1000, 300), 0.1f);
                image_unmask.rectTransform.position = UIManager.Instance.buttons_Upgrade[2].transform.position;

                TypingSequence = TypingAction(LastTip);
                StartCoroutine(TypingSequence);
                //LastTip();
            };
        }

        public void LastTip()
        {
            nextSequence = () =>
            {
                Debug.Log("Last Tip");
                messageObject.SetActive(true);

                image_unmask.gameObject.SetActive(false);

                originText = LanguageManager.Instance.GetStringData("UI_Tutorial_12");
                TypingSequence = TypingAction(GameEnd);
                StartCoroutine(TypingSequence);
                button_NextSequence.interactable = false;
                //button_NextSequence.onClick.AddListener(() => OnClick_Focus());
            };
        }

        public void GameEnd()
        {
            nextSequence = () =>
            {
                Debug.Log("Game End");
                originText = LanguageManager.Instance.GetStringData("UI_Tutorial_13");
                TypingSequence = TypingAction(ReturnLobby);
                StartCoroutine(TypingSequence);
            };
        }

        public void ReturnLobby()
        {
            nextSequence = () =>
            {
                dim.gameObject.SetActive(false);
                messageObject.SetActive(false);
                //Debug.Log("return Lobby");

                UserInfoManager.Instance.tutorial_State = TutorialProgressType.GAME_END;
                UIManager.Instance.GameOver();
            };
        }
    }
}