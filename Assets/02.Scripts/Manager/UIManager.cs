using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.Sound;
using Framework.UI;
using Framework.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Game.Defense
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public CanvasGroup canvasGroup_ButtonGroup;
        public TextMeshProUGUI text_TimeCount;
        public Transform topLocation;
        public UISequences uISequences_WaveCountTimer;

        public ButtonComponent button_SummonCharacter;
        public SummonButton summonButton;
        public ButtonComponent button_Relocation;
        public ButtonComponent button_SummonBoss;
        public ButtonComponent button_Pause;
        public ButtonComponent button_MisstionCheck;
        public ButtonComponent button_rank;
        public ButtonComponent button_giveUp;

        public TextMeshProUGUI text_Gem;
        public TextMeshProUGUI text_Go;

        public TextMeshProUGUI text_RelocationCost;
        public TextMeshProUGUI text_SummonBossLevel;
        public TextMeshProUGUI text_bossAmount;
        public TextMeshProUGUI text_BossReward;
        public TextMeshProUGUI text_SummonCharacterCost;
        public TextMeshProUGUI text_Interest;
        public TextMeshProUGUI text_WaveIdx;
        public GameObject interestAction;

        public GameObject objectBossMessage;
        public TextMeshProUGUI text_bossMessageLevel;

        public Transform canvas;
        public Transform dynamicCanvas;
        public Transform dynamicFocusCanvas;
        public Transform interestParticlePos;
        public ObjectParticle interestParticle;
        public GameObject[] icon_BossActive;
        public GameObject icon_SummonCharacterActive;

        public Transform goScoreDestination;
        public RectTransform goScorePanel;

        public DecButton[] buttons_Upgrade;

        public PausePopup pausePopup;
        public RoulettePopup roulettePopup;
        public GameOverPopup gameOverPopup;
        public MissionPopup missionPopup;
        public SellPopup sellPopup;
        public InGameRankPopup inGameRankPopup;
        public BossSelectPopup bossSelectPopup;
        public FieldBossRewardPopup fieldBossRewardPopup;
        public BattleResultPopup battleResultPopup;
        public BattleResultTablePopup battleResultTablePopup;
        public BattleResultPromotePopup battleResultPromotePopup;
        public GameObject sellPanel;
        public Image[] image_BossCoolTime;
        public Image image_WaveCountFill;
        public int bossAmount;
        public IngameStatusMessage ingameStatusMessage;

        public BossWaveItem BossWaveItem;

        public Animation anim_Interest;
        public Animation anim_Upgrade;
        public Animation anim_UpgradeFail;
        public Animation anim_UpgradeReset;
        public CharacterCard characterCard;
        public CharacterCard characterCard_Fail;
        public CharacterCard characterCard_Reset;


        public TextMeshProUGUI text_AnimInterest;
        public TextMeshProUGUI text_AnimUpgrade;
        public TextMeshProUGUI text_AnimUpgradeFail;
        public TextMeshProUGUI text_AnimUpgradeReset;

        public IEnumerator BossCoolTimer;

        public List<ObjectParticle> goParticles = new();

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            if (!pausePopup.gameObject.activeSelf)
            {
                pausePopup.gameObject.SetActive(true);
            }
            pausePopup.Initialize();
            pausePopup.PopUpSequence(false);
        }

        public void WaveEndSequence()
        {
            StartCoroutine(WaveEndSequenceCo());
        }

        public IEnumerator WaveEndSequenceCo()
        {
            for (int i = 0; i < goParticles.Count; i++)
            {
                StartCoroutine(goParticles[i].CoinSequence(GoIconPos()));
                yield return new WaitForSeconds(0.05f);
            }
            goParticles.Clear();
        }

        public void ClosedGoIcon(bool isBossGo)
        {
            if (GameManager.Instance.gameState == GameState.GAME_OVER) return;
            int rewardValue = isBossGo ? GameManager.Instance.dic_RewardRules[$"BOSS_{GameManager.Instance.monsterSpawner.bossIdx}"].rewardGo : GameManager.Instance.currentRewardGoValue;
            bool isHundred = rewardValue >= 100;

            if (isHundred)
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_GO_BIG);
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_GO_SMALL);
            }
            GameManager.Instance.ChangeGo(rewardValue);
            goScorePanel.DOScale(Vector3.one * 1.2f, 0.1f).OnComplete(() => goScorePanel.DOScale(Vector3.one, 0.1f));
        }

        public Vector3 GoIconPos()
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(goScoreDestination.position);
            return pos;
        }

        public void SetPossibleRelocation(bool isPossible)
        {
            button_Relocation.SetInterectible(isPossible);
        }

        public void SetPossibleSummon(bool isPossible)
        {
            // button_SummonCharacter.SetInterectible(isPossible);
            // icon_SummonCharacterActive.SetActive(isPossible);
            summonButton.SetPossibleSummon(isPossible);
        }

        // public void SetUpgradeAnim(CharacterIndex index, int level)
        // {
        //     CharacterData data = DataManager.Instance.dic_CharacterData[index];
        //     characterCard.Initialize(data);
        //     text_AnimUpgrade.text = $"{level}";
        //     if (!anim_Upgrade.gameObject.activeSelf)
        //     {
        //         anim_Upgrade.gameObject.SetActive(true);
        //     }
        //     anim_Upgrade.Rewind();
        //     anim_Upgrade.Play();
        // }

        public void SetUpgradeAnim(CharacterIndex index, int level, UpgradeType type)
        {
            CharacterData data = DataManager.Instance.dic_CharacterData[index];
            Debug.Log($"characterLevel{data.characterClassLevel}");
            switch (type)
            {
                case UpgradeType.SUCCESS:
                    characterCard.Initialize(data);
                    text_AnimUpgrade.text = $"{level}";
                    anim_UpgradeFail.gameObject.SetActive(false);
                    anim_UpgradeReset.gameObject.SetActive(false);
                    if (!anim_Upgrade.gameObject.activeSelf)
                    {
                        anim_Upgrade.gameObject.SetActive(true);
                    }
                    anim_Upgrade.Rewind();
                    anim_Upgrade.Play();
                    break;
                case UpgradeType.FAILED:
                    characterCard_Fail.Initialize(data);
                    text_AnimUpgradeFail.text = $"{level}";
                    anim_Upgrade.gameObject.SetActive(false);
                    anim_UpgradeReset.gameObject.SetActive(false);
                    if (!anim_UpgradeFail.gameObject.activeSelf)
                    {
                        anim_UpgradeFail.gameObject.SetActive(true);
                    }
                    anim_UpgradeFail.Rewind();
                    anim_UpgradeFail.Play();
                    break;
                case UpgradeType.RESET:
                    characterCard_Reset.Initialize(data);
                    text_AnimUpgradeReset.text = $"{level}";
                    anim_Upgrade.gameObject.SetActive(false);
                    anim_UpgradeFail.gameObject.SetActive(false);
                    if (!anim_UpgradeReset.gameObject.activeSelf)
                    {
                        anim_UpgradeReset.gameObject.SetActive(true);
                    }
                    anim_UpgradeReset.Rewind();
                    anim_UpgradeReset.Play();
                    break;
            }

        }


        public void SetPossibleUpgrade(int gem)
        {
            for (int i = 0; i < buttons_Upgrade.Length; i++)
            {
                buttons_Upgrade[i].CalcPossibleCost(gem);
            }
        }

        public void ActiveTimeCount(bool isActive)
        {
            uISequences_WaveCountTimer.gameObject.SetActive(isActive);
            uISequences_WaveCountTimer.ExcuteSeqeunce();
        }

        public void TutorialInitialize()
        {
            button_SummonCharacter.onPointerUp = () =>
            {
                OnClick_TutorialSummon();
            };

            button_Relocation.onPointerUp = () =>
            {
                OnClick_Relocation();
            };

            button_SummonBoss.onPointerUp = () =>
            {
                OnClick_TutorialSummonBoss();
            };

            button_MisstionCheck.onPointerUp = () =>
            {
                OnClick_MissionCheck();
            };

            button_Pause.onPointerUp = () =>
            {
                OnClick_PauseButton();
            };

            Vector2 pos = Camera.main.ScreenToWorldPoint(interestParticlePos.position);
            interestParticle.transform.position = pos;

            text_WaveIdx.text = $"{1}";
            text_Interest.text = "0";
            text_Go.text = "0";
            text_Gem.text = "0";
            bossAmount = 0;
            InitPopup();
            SetBossSummon();
        }

        bool IsBattleMode = false;
        public void Initialize(bool isBattleMode)
        {
            IsBattleMode = isBattleMode;

            if (IsBattleMode)
            {
                if (!inGameRankPopup.gameObject.activeSelf)
                {
                    inGameRankPopup.gameObject.SetActive(true);
                    inGameRankPopup.PopUpSequence(false);
                }
                inGameRankPopup.Initialize();

                if (!fieldBossRewardPopup.gameObject.activeSelf)
                {
                    fieldBossRewardPopup.gameObject.SetActive(true);
                    fieldBossRewardPopup.PopUpSequence(false);
                }
                fieldBossRewardPopup.Initialize();

                if (!bossSelectPopup.gameObject.activeSelf)
                {
                    bossSelectPopup.gameObject.SetActive(true);
                    bossSelectPopup.PopUpSequence(false);
                }
                bossSelectPopup.Initialize();

                if (!battleResultTablePopup.gameObject.activeSelf)
                {
                    battleResultTablePopup.gameObject.SetActive(true);
                    battleResultTablePopup.PopUpSequence(false);
                }
                battleResultTablePopup.Initialize();

                if (!battleResultPopup.gameObject.activeSelf)
                {
                    battleResultPopup.gameObject.SetActive(true);
                    battleResultPopup.PopUpSequence(false);
                }
                battleResultPopup.Initialize();

                if (!battleResultPromotePopup.gameObject.activeSelf)
                {
                    battleResultPromotePopup.gameObject.SetActive(true);
                    battleResultPromotePopup.PopUpSequence(false);
                }
                battleResultPromotePopup.Initialize();

                button_Pause.gameObject.SetActive(false);
                button_rank.gameObject.SetActive(isBattleMode);
                button_rank.onPointerUp = OnClick_RankButton;
                button_giveUp.gameObject.SetActive(false);
                button_giveUp.onPointerUp = OnClick_GiveUpButton;

                goScorePanel.gameObject.SetActive(!NetworkConnect.Instance.isFriendlyMatch);
            }

            // button_SummonCharacter.onPointerUp = () =>
            // {
            //     OnClick_SummonCharacter();
            // };
            summonButton.Initilize();

            button_Relocation.onPointerUp = () =>
            {
                OnClick_Relocation();
            };

            button_SummonBoss.onPointerUp = () =>
            {
                OnClick_SummonBoss();
            };

            button_MisstionCheck.onPointerUp = () =>
            {
                OnClick_MissionCheck();
            };

            button_Pause.onPointerUp = () =>
            {
                OnClick_PauseButton();
            };

            Vector2 pos = Camera.main.ScreenToWorldPoint(interestParticlePos.position);
            interestParticle.transform.position = pos;

            SetInterestValue(0);

            text_WaveIdx.text = $"{1}";
            text_Interest.text = "0";
            text_Go.text = "0";
            text_Gem.text = "0";
            bossAmount = 0;
            InitPopup();
            SetBossSummon();
        }

        public void SetInterestValue(int interestValue)
        {

            bool isActive = interestValue > 0;
            text_Interest.text = $"+{interestValue}";
            text_Interest.gameObject.SetActive(isActive);
            interestAction.SetActive(isActive);
            Vector2 pos = isActive ? new Vector2(-26f, -75f) : new Vector2(-1f, -75f);
            text_Gem.rectTransform.localPosition = pos;

        }

        public void InitPopup()
        {
            if (!gameOverPopup.gameObject.activeSelf)
            {
                gameOverPopup.gameObject.SetActive(true);
            }
            gameOverPopup.Initialize();
            gameOverPopup.PopUpSequence(false);

            if (!missionPopup.gameObject.activeSelf)
            {
                missionPopup.gameObject.SetActive(true);
            }

            roulettePopup.Initialize();
        }

        public void SetBossSummon()
        {
            // icon_BossActive[bossAmount].SetActive(true);

            // image_BossCoolTime[bossAmount].fillAmount = 1;
            // button_SummonBoss.SetInterectible(true);

            // int level = GameManager.Instance.monsterSpawner.bossIdx;
            // int levelIdx = level >= GameManager.Instance.bossRewardData.rewards.Length ? GameManager.Instance.bossRewardData.rewards.Length - 1 : level;
            // SetBossInfo(level + 1, GameManager.Instance.bossRewardData.rewards[levelIdx].rewardValue);
            icon_BossActive[bossAmount].SetActive(true);

            image_BossCoolTime[bossAmount].fillAmount = 1;
            button_SummonBoss.SetInterectible(true);

            int level = GameManager.Instance.monsterSpawner.bossIdx;
            int levelIdx = level >= GameManager.Instance.bossRewardData.rewards.Length ? GameManager.Instance.bossRewardData.rewards.Length - 1 : level;
            SetBossInfo(level + 1, GameManager.Instance.bossRewardData.rewards[levelIdx].rewardValue);
            bossAmount++;

            if (bossAmount == 2)
            {
                icon_BossActive[0].SetActive(false);
            }

            bool isBossAmount = bossAmount > 0;

            text_bossAmount.gameObject.SetActive(isBossAmount);
            text_bossAmount.text = $"x{bossAmount}";
            if (bossAmount == 1)
            {
                if (BossCoolTimeCo == null)
                {
                    BossCoolTimeCo = BossCoolTime();

                    StartCoroutine(BossCoolTimeCo);
                }
            }
        }

        public void SetBossInfo(int bossLevel, int bossReward)
        {
            string bossLeveldata = $"Lv.{bossLevel}";
            string bossRewardData = $"{bossReward}";

            text_SummonBossLevel.text = bossLeveldata;
            text_BossReward.text = bossRewardData;
        }

        public ObscuredFloat bossCoolTimeBuffValue = 0;
        public int BOSS_COOLTIME => (int)(ConfigData.BOSS_COOLTIME - ConfigData.BOSS_COOLTIME * bossCoolTimeBuffValue);
        public IEnumerator BossCoolTime()
        {
            float timer = 0;
            // button_SummonBoss.SetInterectible(false);
            image_BossCoolTime[bossAmount].gameObject.SetActive(true);
            image_BossCoolTime[bossAmount].fillAmount = 0;

            while (true)
            {
                timer += Time.deltaTime;
                image_BossCoolTime[bossAmount].fillAmount = timer / BOSS_COOLTIME;
                if (timer >= BOSS_COOLTIME)
                {
                    SoundManager.Instance.PlaySound(SoundKey.SF_BOSS_COOLTIME);
                    SetBossSummon();
                    break;
                }
                yield return Time.deltaTime;
            }
        }

        public static Transform GetDynamicCanvasTransform()
        {
            return Instance.dynamicCanvas;
        }

        public static Transform GetDynamicFocusTransform()
        {
            return Instance.dynamicFocusCanvas;
        }

        public static Transform GetCanvasTransform()
        {
            return Instance.canvas;
        }

        public void RoulettePopup(ReqRouletteGroupData data)
        {
            roulettePopup.SetRoulette(data);
        }

        public void GameOver(int bounsWave = 0)
        {
            StartCoroutine(GameOverSequence(bounsWave));
        }

        private IEnumerator GameOverSequence(int bounsWave)
        {
            yield return null;
            gameOverPopup.PopUpSequence(true);
            gameOverPopup.SetGameOver(GameManager.Instance.Go, GameManager.Instance.waveIdx, bounsWave);
        }

        public void SetTimeCount(int timeValue)
        {
            int time = timeValue + 1;

            string timer = time.ToString("0");
            text_TimeCount.text = $"{timer}";
        }

        public void SetInterestParticle(int gemValue)
        {
            interestParticle.SimplePlay();

            if (missionPopup.missionMessage.activeSelf) return;

            text_AnimInterest.text = $"x{gemValue}";
            if (!anim_Interest.gameObject.activeSelf)
            {
                anim_Interest.gameObject.SetActive(true);
            }
            anim_Interest.Rewind();
            anim_Interest.Play();
        }

        public void OnClick_PauseButton()
        {
            pausePopup.ActivePopup();
        }

        public void OnClick_RankButton()
        {
            inGameRankPopup.ActivePopup();
        }
        public void OnClick_GiveUpButton()
        {
            SystemNoticePopup popup = PopupManager.Instance.GetPopUp<SystemNoticePopup>("systemNotice");
            popup.SetYesNoPopupMessage(LanguageManager.Instance.GetStringData("MSG_Surrender"), () => GameManager.Instance.FieldBossDamage());
        }

        public void OnClick_Relocation()
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            GameManager.Instance.Relocation();
            GameManager.Instance.CalcPossibleCostAction();
        }

        public void OnClick_SummonBoss()
        {

            // SoundManager.Instance.PlaySound(SoundKey.SF_BOSS_SUMMON);
            // image_BossCoolTime.fillAmount = 0;
            // icon_BossActive.SetActive(false);
            // StartCoroutine(BossCoolTime());
            // GameManager.Instance.SummonBoss();


            // int level = GameManager.Instance.monsterSpawner.bossIdx;

            // StartCoroutine(SetBossMessage(level));
            // int levelIdx = level >= GameManager.Instance.bossRewardData.rewards.Length ? GameManager.Instance.bossRewardData.rewards.Length - 1 : level;
            // SetBossInfo(level + 1, GameManager.Instance.bossRewardData.rewards[levelIdx].rewardValue);

            int tempBossAmount = bossAmount == 2 ? 1 : bossAmount;
            //icon_BossActive[tempBossAmount].SetActive(false);
            image_BossCoolTime[tempBossAmount].gameObject.SetActive(false);

            bossAmount--;

            bool isBossAmount = bossAmount > 0;
            if (isBossAmount)
            {
                icon_BossActive[0].SetActive(true);
            }


            text_bossAmount.gameObject.SetActive(isBossAmount);
            text_bossAmount.text = $"x{bossAmount}";
            button_SummonBoss.SetInterectible(bossAmount > 0);
            SoundManager.Instance.PlaySound(SoundKey.SF_BOSS_SUMMON);
            //image_BossCoolTime[bossAmount].fillAmount = 0;

            if (BossCoolTimeCo == null)
            {
                BossCoolTimeCo = BossCoolTime();
                StartCoroutine(BossCoolTimeCo);
            }
            else
            {
                StopCoroutine(BossCoolTimeCo);
                BossCoolTimeCo = BossCoolTime();
                StartCoroutine(BossCoolTimeCo);
            }

            //StartCoroutine(BossCoolTime());
            GameManager.Instance.SummonBoss();

            icon_BossActive[bossAmount].SetActive(false);

            int level = GameManager.Instance.monsterSpawner.bossIdx;

            StartCoroutine(SetBossMessage(level));
            int levelIdx = level >= GameManager.Instance.bossRewardData.rewards.Length ? GameManager.Instance.bossRewardData.rewards.Length - 1 : level;
            SetBossInfo(level + 1, GameManager.Instance.bossRewardData.rewards[levelIdx].rewardValue);
        }

        public IEnumerator BossCoolTimeCo;
        public IEnumerator SetBossMessage(int level)
        {
            text_bossMessageLevel.text = $"Lv.{level}";
            objectBossMessage.SetActive(true);
            yield return new WaitForSeconds(1);
            objectBossMessage.SetActive(false);
        }

        public void OnClick_TutorialSummonBoss()
        {
            //TutorialManager.Instance.nextSequence?.Invoke();

            TutorialManager.Instance.BossSummonSequence();
            SoundManager.Instance.PlaySound(SoundKey.SF_BOSS_SUMMON);
            image_BossCoolTime[0].fillAmount = 0;
            icon_BossActive[bossAmount].SetActive(false);
            StartCoroutine(BossCoolTime());
            GameManager.Instance.TutorialSummonBoss();

            int level = GameManager.Instance.monsterSpawner.bossIdx;
            StartCoroutine(SetBossMessage(level));
            int levelIdx = level >= GameManager.Instance.bossRewardData.rewards.Length ? GameManager.Instance.bossRewardData.rewards.Length - 1 : level;
            SetBossInfo(level + 1, GameManager.Instance.bossRewardData.rewards[levelIdx].rewardValue);

            //TutorialManager.Instance.ResetBossSequence();
        }

        public void FillWaveCount(float amount)
        {
            image_WaveCountFill.fillAmount = amount;
        }

        public void OnClick_TutorialSummon()
        {
            TutorialSummonData data = TutorialManager.Instance.GetSummonData();
            CharacterIndex characterIndex = data.characterIndex;
            int glacierIdx = data.glacierIdx;

            SoundManager.Instance.PlaySound(SoundKey.SF_SUMMON);
            GameManager.Instance.TutorialSummon(characterIndex, glacierIdx);
            GameManager.Instance.CalcPossibleCostAction();
        }

        public void OnClick_SummonCharacter()
        {
            //Todo : Check Gem, Random Index Value
            SoundManager.Instance.PlaySound(SoundKey.SF_SUMMON);
            if (GameManager.Instance.isTestMode)
            {
                GameManager.Instance.characterSpawner.SummonCharacter();
            }
            else
            {
                GameManager.Instance.SummonCharacter();
            }

            GameManager.Instance.CalcPossibleCostAction();
        }

        public void OnClick_MissionCheck()
        {
            missionPopup.OnClick_MissionButton(true);
        }

        public void ChangeWaveValue(int changeValue)
        {
            text_WaveIdx.text = $"{changeValue}";

            //test code
        }

        public void ChangeValueSequnce(float target, float current, TextMeshProUGUI text)
        {
            StartCoroutine(ChangeValueSequenceAsync(target, current, text));
        }

        public IEnumerator ChangeValueSequenceAsync(float target, float current, TextMeshProUGUI text)
        {
            float duration = 0.3f; // 카운팅에 걸리는 시간 설정.
            float temp = target - current;

            if (temp >= 0)
            {
                float offset = (target - current) / duration;

                while (current < target)
                {
                    current += offset * Time.deltaTime;
                    text.text = ((int)current).ToString();
                    yield return null;
                }
            }
            else
            {
                float offset = (current - target) / duration;

                while (current > target)
                {
                    current -= offset * Time.deltaTime;
                    text.text = ((int)current).ToString();
                    yield return null;
                }

            }

            current = target;

            text.text = ((int)current).ToString();
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}
