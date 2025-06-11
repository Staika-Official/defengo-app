using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Util;

namespace Framework.UI
{
    public class StatusInfo : MonoBehaviour
    {
        public string displayInfo;
        public Image image_StatusIcon;
        public GameObject increaseIcon;
        public TextMeshProUGUI text_Status;
        public TextMeshProUGUI text_StatusValue;
        public TextMeshProUGUI text_IncreaseValue;
        public bool isUpgradeValue = true;
        public Animator animator;

        //newdongmin
        public Image backFrontGround_Image;
        public Image backGround_Image;

        public GameObject levelUp_UiObject;
        public GameObject levelUp_ScaleUiObject;

        public bool isChanged;
        public bool isLevelUp;

        public void IncreaseProperty(bool isIncrease)
        {
            increaseIcon.SetActive(isIncrease);
            text_IncreaseValue.gameObject.SetActive(isIncrease);

            if (!isIncrease && !isLevelUp)
            {
                animator.Rebind();
                isChanged = false;
            }
        }

        public void FieldBuffInitiailize(FieldBuffType type, CharacterData data)
        {
            int classLevel = data.characterClassLevel == 0 ? (int)data.characterGrade + 1 : data.characterClassLevel;

            IncreaseStruct increaseStruct = new IncreaseStruct(0, 0, classLevel, classLevel, false, false, false);

            LogicFieldBuffInitiailize(type, data, increaseStruct);
        }

        public void ReinForceMentFieldBuffInitialize(FieldBuffType type, CharacterData data, int increaseLevelStat)
        {
            int classLevel = data.characterClassLevel == 0 ? (int)data.characterGrade + 1 : data.characterClassLevel;

            IncreaseStruct increaseStruct = new IncreaseStruct(0, 0, classLevel, increaseLevelStat, false, false, false);

            LogicFieldBuffInitiailize(type, data, increaseStruct);
        }


        public void LogicFieldBuffInitiailize(FieldBuffType type, CharacterData data, IncreaseStruct increaseStruct)
        {
            //string stringKey = LanguageManager.Instance.GetStringData($"UI_FieldBuff_{type}");
            //text_Status.text = stringKey;
            bool isLevelupUsed = increaseStruct.classLevel != increaseStruct.increaseClassLevel;

            switch (type)
            {
                case FieldBuffType.NONE:
                    break;
                case FieldBuffType.INCREASE_CRITICAL_RATE:
                    float CriticalRate = 0.01f + (increaseStruct.classLevel - 1) * 0.01f;
                    string critical = LanguageManager.Instance.GetStringData("UI_FieldBuff_INCREASE_CRITICAL_RATE");
                    string criticalValue = ((int)(CriticalRate * 100f)).ToString();
                    text_StatusValue.text = string.Format(critical, $"<color=#9EFF6B>{criticalValue}%</color>");

                    if (isLevelupUsed)
                    {
                        float increaseCriticalRate = 0.01f + (increaseStruct.increaseClassLevel - 1) * 0.01f;
                        string increaseCriticalValue = ((int)(increaseCriticalRate * 100f)).ToString();
                        text_IncreaseValue.text = $"<color=#9EFF6B>{increaseCriticalValue}%</color>";
                        ReinForcementUp(isLevelupUsed);
                    }
                    IncreaseProperty(isLevelupUsed);
                    break;
                case FieldBuffType.DECREASE_DEFENSE_VALUE:
                    break;
                case FieldBuffType.INCREASE_MISSION_REWARD:
                    break;
                case FieldBuffType.INCREASE_BONUS_RATE:
                    break;
                case FieldBuffType.INCREASE_BONUS_SPEED:
                    break;
                default:
                    break;
            }
        }

        public void ClassUpSequence(CharacterData data, StatusType type)
        {
            StartCoroutine(ClassUpSequenceAsync(data, type));
        }

        public IEnumerator ClassUpSequenceAsync(CharacterData data, StatusType type)
        {
            yield return new WaitForSeconds(0.15f);

            //로직상 레벨업인지 아닌지 IncreaseProperty 에서 확인을 하기에 LevelUp값을 변경해줌

            //레벨업 true -> IncreaseProperty에서 레벨업인지 확인해서 애니메이션 값을 초기화 해줄지 말지 결정 -> 다시 레벨업 초기화

            //애니메이션 값을 초기화를 해주는 이유는 업그레이드 애니메이션 진행하고 그 애니메이션 값을 유지하고있어야 원하는 표현이 완성이됨
            //또한 다른 애니메이션을 진행하려면 애니메이션 값을 초기화해줘야하기때문에 인크리스 부분에서 초기화 진행
            isLevelUp = true;

            Initialize(type, data);

            isLevelUp = false;

        }

        public void ReinForcementUp(bool isBtnUsed)
        {
            if (isBtnUsed && !isChanged)
            {
                isChanged = true;
                animator.Rebind();
                animator.SetTrigger("ReinForceMent_Click_GaugeUp");
            }
            else if (isBtnUsed && isChanged)
            {
                animator.Rebind();
                animator.SetTrigger("ReinForceMent_Click_Scale");
            }
        }

        public void SetLevelUp()
        {
            if (isUpgradeValue)
            {
                animator.Rebind();
                animator.SetTrigger("LevelUp");
            }

        }

        public void Initialize(StatusType type, CharacterData characterData)
        {
            bool isInActive = characterData.characterClassLevel == 0;
            int classLevel = isInActive ? (int)characterData.characterGrade + 1 : characterData.characterClassLevel;

            image_StatusIcon.sprite = DataManager.Instance.uiPropertyData.dic_StatusInfo[type].sprite_Icon;
            string stringKey = LanguageManager.Instance.GetStringData($"UI_Stat_{type}");
            text_Status.text = stringKey;

            IncreaseStruct increaseStruct = new IncreaseStruct(0, 1, classLevel, classLevel, false, false, false);

            LogicInitialize(type, characterData, increaseStruct);
        }


        public void ReinforcementInitalize(StatusType type, CharacterData characterData, IncreaseStruct sendStruct)
        {
            bool isInActive = characterData.characterClassLevel == 0;
            int classLevel = isInActive ? (int)characterData.characterGrade + 1 : characterData.characterClassLevel;

            IncreaseStruct reciveStruct = sendStruct;

            int increaseClassLevel;

            if (reciveStruct.increaseClassLevel != classLevel)
                increaseClassLevel = reciveStruct.increaseClassLevel;
            else
                increaseClassLevel = classLevel;

            IncreaseStruct increaseStruct = new IncreaseStruct(reciveStruct.star, reciveStruct.upgrade, classLevel, increaseClassLevel,
                reciveStruct.isStarUsed, reciveStruct.isUpgradeUsed, reciveStruct.isLevelupUsed);

            LogicInitialize(type, characterData, increaseStruct);
        }


        public void LogicInitialize(StatusType type, CharacterData characterData, IncreaseStruct increaseStruct)
        {
            isUpgradeValue = true;

            bool isBtnUsed = false;
            bool isUsed = false;
            bool isStarUsed = increaseStruct.star != 0;
            bool isUpgradeUsed = increaseStruct.upgrade != 1;
            bool isLevelupUsed = increaseStruct.classLevel != increaseStruct.increaseClassLevel;

            switch (type)
            {
                case StatusType.ATTACK_DAMAGE:
                    if (characterData.characterType == CharacterType.SUB)
                    {
                        text_StatusValue.text = characterData.attackDamage.ToString();
                        IncreaseProperty(false);
                    }
                    else
                    {
                        float attackDamage = (characterData.attackDamage +
                                              (increaseStruct.classLevel - 1) * characterData.classUpFactor[0]);
                        text_StatusValue.text = ((int)attackDamage).ToString();

                        isUsed = isStarUsed || isUpgradeUsed || isLevelupUsed;
                        isBtnUsed = increaseStruct.isStarUsed || increaseStruct.isUpgradeUsed ||
                                    increaseStruct.isLevelupUsed;
                        if (isUsed)
                        {
                            float increaseAttackDamage = (int)(characterData.attackDamage +
                                                               (increaseStruct.increaseClassLevel - 1) *
                                                               characterData.classUpFactor[0]);
                            float calcStarDamage = 1;
                            float calcUpgradeLevelupDamage = 0;

                            calcStarDamage = (Mathf.Pow(ConfigData.STARGRADE_FACTOR, increaseStruct.star));
                            calcUpgradeLevelupDamage =
                                ((increaseStruct.upgrade - 1) * characterData.powerFactor[0] * increaseStruct.star) +
                                (increaseStruct.upgrade - 1);

                            float TotalIncreaseAttackDamage =
                                (increaseAttackDamage * calcStarDamage) + calcUpgradeLevelupDamage;
                            TotalIncreaseAttackDamage = Mathf.Floor(TotalIncreaseAttackDamage);

                            text_IncreaseValue.text = TotalIncreaseAttackDamage.ToString();
                            ReinForcementUp(isBtnUsed);
                        }

                        IncreaseProperty(isUsed);
                    }

                    break;
                case StatusType.ATTACK_SPEED:
                    float attackSpeed = characterData.attackSpeed;
                    text_StatusValue.text = attackSpeed.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.INCREASE_ATTACK_SPEED:
                    //캐릭터마다 고유의 공격속도를 지니고있기에 따로 함수로 파줌
                    SwitchForAttackSpeed(characterData.characterIndex, characterData, increaseStruct);

                    break;
                case StatusType.DETECT_RANGE:
                    float detectRange = characterData.detectRange;
                    text_StatusValue.text = detectRange.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.INCREASE_DETECT_RANGE:
                    //call two ->detect_range -> increase_detect_range
                    float DetectRange = characterData.detectRange +
                                        (increaseStruct.classLevel - 1) * characterData.classUpFactor[1];
                    text_StatusValue.text = DetectRange.ToString();

                    //
                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;

                    if (isUsed)
                    {
                        float increaseDetectRange = characterData.detectRange +
                                                    (increaseStruct.increaseClassLevel - 1) *
                                                    characterData.classUpFactor[1];
                        text_IncreaseValue.text = increaseDetectRange.ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;

                case StatusType.TARGET_COUNT:
                    int targetCount = (int)characterData.targetCount;
                    text_StatusValue.text = targetCount.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.STRIKE_RANGE:
                    float strikeRange = characterData.strikeRange;
                    text_StatusValue.text = strikeRange.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.BUFF_VALUE:
                    float buffValue = (characterData.buffValue +
                                       (increaseStruct.classLevel - 1) * characterData.classUpFactor[0]);
                    text_StatusValue.text = ((int)buffValue).ToString();

                    isUsed = isStarUsed || isUpgradeUsed || isLevelupUsed;
                    isBtnUsed = increaseStruct.isStarUsed || increaseStruct.isUpgradeUsed ||
                                increaseStruct.isLevelupUsed;

                    if (isUsed)
                    {
                        float increaseBuffValue = (int)(characterData.buffValue +
                                                        (increaseStruct.increaseClassLevel - 1) *
                                                        characterData.classUpFactor[0]);
                        float calcStarDamage = 1;
                        float calcUpgradeLevelupDamage = 0;

                        calcStarDamage = Mathf.Pow(ConfigData.STARGRADE_FACTOR, increaseStruct.star);
                        calcUpgradeLevelupDamage =
                            ((increaseStruct.upgrade - 1) * characterData.powerFactor[0] * increaseStruct.star) +
                            (increaseStruct.upgrade - 1);

                        float TotalIncreaseAttackDamage =
                            (increaseBuffValue * calcStarDamage) + calcUpgradeLevelupDamage;
                        TotalIncreaseAttackDamage = Mathf.Floor(TotalIncreaseAttackDamage);

                        text_IncreaseValue.text = TotalIncreaseAttackDamage.ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.BUFF_CYCLE:
                    float buffCycle = characterData.buffCycle;
                    text_StatusValue.text = buffCycle.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.BUFF_DURATION:
                    float buffDuration = characterData.buffDuration;
                    text_StatusValue.text = buffDuration.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.BUFF_RANGE:
                    float buffRange = characterData.buffRange;
                    text_StatusValue.text = buffRange.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.WAVE_DAMAGE:
                    float waveDamage = characterData.characterUniqueValue[0];
                    text_StatusValue.text = waveDamage.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.SLOW_RATE:
                    float slowRate = characterData.characterUniqueValue[0] * 100;
                    text_StatusValue.text = $"{slowRate}%";
                    IncreaseProperty(false);
                    break;
                case StatusType.SLOW_TIME:
                    float slowTime = characterData.buffDuration;
                    text_StatusValue.text = slowTime.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.ADD_CRITICAL_RATE:
                    float criticalRate = characterData.criticalRange;
                    text_StatusValue.text = $"{(int)(criticalRate * 100)}%";

                    isUsed = isUpgradeUsed;
                    isBtnUsed = increaseStruct.isUpgradeUsed;
                    if (isUsed)
                    {
                        float increaseCriticalRate = characterData.criticalRange
                                                     + (increaseStruct.upgrade - 1) * characterData.powerFactor[1];
                        text_IncreaseValue.text = $"{(int)(increaseCriticalRate * 100)}%";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.GET_GEM_VALUE:
                    float gemValue = characterData.characterUniqueValue[1] +
                                     (characterData.characterClassLevel - 1) * characterData.classUpFactor[1];
                    text_StatusValue.text = gemValue.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.BOMB_DAMAGE:
                    float bombDamage = characterData.characterUniqueValue[2];
                    text_StatusValue.text = bombDamage.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.STUN_TIME:
                    float stunTime = characterData.characterUniqueValue[0];
                    text_StatusValue.text = stunTime.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.EXCUTION_HEALTH_RATE:
                    float oneShotCondition = characterData.characterUniqueValue[0] + 0 * characterData.starFactor[0]
                        + (1 - 1) * characterData.powerFactor[1];
                    text_StatusValue.text = $"{oneShotCondition * 100}%";

                    isUsed = isStarUsed || isUpgradeUsed;
                    isBtnUsed = increaseStruct.isStarUsed || increaseStruct.isUpgradeUsed;

                    if (isUsed)
                    {
                        float increaseStarOnshot = 0;
                        float increaseUpgradeOneShot = 0;

                        increaseStarOnshot = characterData.characterUniqueValue[0] +
                                             increaseStruct.star * characterData.starFactor[0];

                        increaseUpgradeOneShot = (increaseStruct.upgrade - 1)
                                                 * characterData.powerFactor[1];

                        float increaseTotalCondition = increaseStarOnshot + increaseUpgradeOneShot;

                        text_IncreaseValue.text = $"{increaseTotalCondition * 100}%";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.BERSERKER_ATTACK_DAMAGE:

                    float berserkerDamage = characterData.characterUniqueValue[0] +
                                            (increaseStruct.classLevel - 1) * characterData.classUpFactor[1]
                                            + (1 - 1) * characterData.powerFactor[1] * 0 + (1 - 1);


                    text_StatusValue.text = berserkerDamage.ToString();

                    isUsed = isUpgradeUsed || isStarUsed;
                    isBtnUsed = increaseStruct.isStarUsed || increaseStruct.isUpgradeUsed;

                    if (isUsed)
                    {
                        float AddtiveBerserkerDamage = characterData.characterUniqueValue[0] +
                                                       increaseStruct.star * characterData.starFactor[0];

                        //0으로 두는 이유 어차피 계산식 상 0이 나오기 때문
                        float AddtiveBerserkerPowerUpDamage = 0;

                        AddtiveBerserkerPowerUpDamage = (increaseStruct.upgrade - 1) * characterData.powerFactor[1];

                        float TotalIncreaseAttackDamage = AddtiveBerserkerDamage + AddtiveBerserkerPowerUpDamage;

                        text_IncreaseValue.text = Mathf.Floor(TotalIncreaseAttackDamage).ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.BERSERKER_ATTACK_SPEED:
                    float berserkerSpeed = characterData.characterUniqueValue[1] + characterData.starFactor[0] * 0;
                    text_StatusValue.text = berserkerSpeed.ToString();

                    isUsed = isStarUsed;
                    isBtnUsed = increaseStruct.isStarUsed;

                    if (isUsed)
                    {
                        //공식계산기 시트상 스타펙터 2가있는데 데이터상 스타펙터는 1밖에없음 임시로 1넣음
                        //광분 계산식 기존공속 + 유니크 벨류 + 스타펙터[2] * 현재 성급
                        float increaseberserkerSpeed = characterData.characterUniqueValue[1] +
                                                       characterData.starFactor[0] * increaseStruct.star;
                        text_IncreaseValue.text = increaseberserkerSpeed.ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.BLACKMAGIC_DAMAGE:
                    float blackDamage =
                        characterData.characterUniqueValue[0] + (increaseStruct.classLevel - 1) *
                                                              characterData.classUpFactor[1]
                                                              + (1 - 1) * characterData.powerFactor[1] * 0 + (1 - 1);

                    text_StatusValue.text = blackDamage.ToString();

                    isUsed = isStarUsed || isUpgradeUsed;
                    isBtnUsed = increaseStruct.isUpgradeUsed || increaseStruct.isStarUsed;

                    if (isUsed)
                    {
                        //float increaseBlackDamage = characterData.attackDamage + (increaseStruct.increaseClassLevel - 1) * characterData.classUpFactor[0];

                        float totalIncreaseBlackDamage = 0;
                        float LevelUpIncreaseBlackDamage = 0;
                        float UpgradeIncreaseBlackDamage = 0;

                        LevelUpIncreaseBlackDamage = characterData.characterUniqueValue[0] +
                                                     (increaseStruct.increaseClassLevel - 1) *
                                                     characterData.classUpFactor[1];

                        UpgradeIncreaseBlackDamage = (increaseStruct.upgrade - 1) * characterData.powerFactor[1] *
                                                     increaseStruct.star
                                                     + (increaseStruct.upgrade - 1);

                        totalIncreaseBlackDamage = LevelUpIncreaseBlackDamage + UpgradeIncreaseBlackDamage;
                        text_IncreaseValue.text = totalIncreaseBlackDamage.ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.TRANSFER_DAMAGE_RATE:

                    float totalTransferDamage = characterData.characterUniqueValue[0] +
                                                (increaseStruct.classLevel - 1) * characterData.classUpFactor[1]
                                                + (1 - 1) * characterData.powerFactor[1];
                    text_StatusValue.text = totalTransferDamage.ToString("F2");


                    isUsed = isUpgradeUsed || isLevelupUsed;
                    isBtnUsed = increaseStruct.isUpgradeUsed || increaseStruct.isLevelupUsed;

                    if (isUsed)
                    {
                        float increaseTransferDamage = characterData.characterUniqueValue[0] +
                                                       (increaseStruct.increaseClassLevel - 1) *
                                                       characterData.classUpFactor[1]
                                                       + (increaseStruct.upgrade - 1) * characterData.powerFactor[1];
                        text_IncreaseValue.text = increaseTransferDamage.ToString("F2");
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.TRANSFER_COUNT:
                    int transferCount = (int)characterData.characterUniqueValue[1];
                    text_StatusValue.text = transferCount.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.SLOW_BOARD_TIME:
                    float slowBoardTime = characterData.characterUniqueValue[1];
                    text_StatusValue.text = slowBoardTime.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.SLOW_BOARD_RATE:
                    float slowBoardRate = characterData.characterUniqueValue[0];
                    text_StatusValue.text = $"{(int)(slowBoardRate * 100)}%";
                    IncreaseProperty(false);
                    break;
                case StatusType.TRANSFORM_TIME:
                    //패트롤 변신 지속시간
                    float transformTime = characterData.characterUniqueValue[1] +
                                          (increaseStruct.classLevel - 1) * characterData.classUpFactor[1];
                    text_StatusValue.text = transformTime.ToString();

                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;

                    if (isUsed)
                    {
                        float tempTransformTime = characterData.characterUniqueValue[1] +
                                                  (increaseStruct.increaseClassLevel - 1) *
                                                  characterData.classUpFactor[1];
                        text_IncreaseValue.text = tempTransformTime.ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.TRANSFORM_CYCLE:
                    //패트롤 변신 주기
                    float transformCycle = characterData.characterUniqueValue[0];
                    text_StatusValue.text = transformCycle.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.TRANSFORM_ADDITIVE_ATTACK_SPEED:
                    float transformAttackSpeed =
                        characterData.characterUniqueValue[2] + (1 - 1) * characterData.powerFactor[1];
                    text_StatusValue.text = transformAttackSpeed.ToString();

                    isUsed = isUpgradeUsed;
                    isBtnUsed = increaseStruct.isUpgradeUsed;

                    if (isUsed)
                    {
                        float increaseTransformAttackSpeed = characterData.characterUniqueValue[2] +
                                                             (increaseStruct.upgrade - 1) *
                                                             characterData.powerFactor[1];
                        text_IncreaseValue.text = increaseTransformAttackSpeed.ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.GAIN_GEM_RATE:
                    float gainGemRate = characterData.characterUniqueValue[0] +
                                        (increaseStruct.classLevel - 1) * characterData.classUpFactor[1];
                    text_StatusValue.text = gainGemRate.ToString();

                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;

                    if (isUsed)
                    {
                        float increasegainGemRate = characterData.characterUniqueValue[0] +
                                                    (increaseStruct.increaseClassLevel - 1) *
                                                    characterData.classUpFactor[1];
                        text_IncreaseValue.text = increasegainGemRate.ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.KORRA_WAVE:
                    text_StatusValue.text = $"{characterData.characterUniqueValue[0]}";
                    IncreaseProperty(false);
                    break;
                case StatusType.POLARIS_STRIKE_RANGE:
                    float range = characterData.strikeRange;
                    text_StatusValue.text = range.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.POLARIS_SEC:
                    float sec = characterData.characterUniqueValue[1] - (increaseStruct.classLevel - 1)
                                * characterData.classUpFactor[1]
                                + (0 * characterData.starFactor[1]);
                    text_StatusValue.text = $"{sec}";

                    isUsed = isLevelupUsed || isStarUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed || increaseStruct.isStarUsed;

                    if (isUsed)
                    {
                        float increaseSec = characterData.characterUniqueValue[1] -
                                            (increaseStruct.increaseClassLevel - 1)
                                            * characterData.classUpFactor[1]
                                            + (increaseStruct.star * characterData.starFactor[1]);

                        text_IncreaseValue.text = $"{increaseSec}";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.DAVI_ATK:
                    //davi 추가 데미지
                    //맨 앞의 1 의미 다비는 절구를 강화해서 강화를 기본 1로 둠
                    float daviAtk = 0 * (characterData.characterUniqueValue[0] + (increaseStruct.classLevel - 1)
                            * characterData.classUpFactor[1]) + (1 - 1)
                        * characterData.powerFactor[1] * 0;

                    text_StatusValue.text = $"{daviAtk}";

                    isUsed = isUpgradeUsed || isLevelupUsed || isStarUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed || increaseStruct.isUpgradeUsed || isStarUsed;

                    if (isUsed)
                    {
                        float increasedaviAttack = 0
                            * (characterData.characterUniqueValue[0] + (increaseStruct.increaseClassLevel - 1)
                                * characterData.classUpFactor[1]) + (increaseStruct.upgrade - 1)
                            * characterData.powerFactor[1] * increaseStruct.star;

                        text_IncreaseValue.text = $"{increasedaviAttack}";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.DAVI_ATS:
                    //davi 추가 공격속도
                    float daviAts = characterData.attackSpeed + (1 * characterData.characterUniqueValue[1]);
                    text_StatusValue.text = $"{daviAts}";

                    isUsed = isUpgradeUsed;
                    isBtnUsed = increaseStruct.isUpgradeUsed;

                    if (isUsed)
                    {
                        float increasedaviAts = characterData.attackSpeed +
                                                (increaseStruct.upgrade * characterData.characterUniqueValue[1]);
                        text_IncreaseValue.text = $"{increasedaviAts}";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.DAVI_SEC:
                    //davi 강화 주기
                    float daviSec = characterData.characterUniqueValue[3] -
                                    (increaseStruct.classLevel - 1) * characterData.classUpFactor[2];
                    text_StatusValue.text = $"{daviSec}";

                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;

                    if (isUsed)
                    {
                        float increaseDaviSec = characterData.characterUniqueValue[3] -
                                                (increaseStruct.increaseClassLevel - 1) *
                                                characterData.classUpFactor[2];
                        text_IncreaseValue.text = $"{increaseDaviSec}";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.KYLE_ATK:
                    float kyleBuff = (characterData.characterUniqueValue[0] +
                                      (increaseStruct.classLevel - 1) * characterData.classUpFactor[1])
                        * (Mathf.Pow(ConfigData.STARGRADE_FACTOR + characterData.characterUniqueValue[1], 0)) +
                        (1 - 1) * characterData.powerFactor[1] * (0 + characterData.characterUniqueValue[1])
                        + 1 - 1;
                    text_StatusValue.text = $"{Mathf.Floor(kyleBuff)}";

                    isUsed = isLevelupUsed || isUpgradeUsed || isStarUsed;
                    isBtnUsed = increaseStruct.isStarUsed || increaseStruct.isUpgradeUsed ||
                                increaseStruct.isLevelupUsed;

                    if (isUsed)
                    {
                        float increaseKyleBuff = (characterData.characterUniqueValue[0] +
                                                  (increaseStruct.increaseClassLevel - 1) *
                                                  characterData.classUpFactor[1])
                                                 * (Mathf.Pow(
                                                     ConfigData.STARGRADE_FACTOR +
                                                     characterData.characterUniqueValue[1], increaseStruct.star)) +
                                                 (increaseStruct.upgrade - 1) * characterData.powerFactor[1] *
                                                 (increaseStruct.star + characterData.characterUniqueValue[1])
                                                 + (increaseStruct.upgrade - 1);
                        text_IncreaseValue.text = $"{Mathf.Floor(increaseKyleBuff)}";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.BARD_SPEED_BUFF:
                    float bardSpeedBuff = 1 + (characterData.characterUniqueValue[0]
                                               + (characterData.characterClassLevel - 1)
                                               * characterData.classUpFactor[0])
                                            * Mathf.Pow(ConfigData.STARGRADE_FACTOR - characterData.starFactor[0], 0)
                                            + (1 - 1) * characterData.powerFactor[0];
                    //text_StatusValue.text = $"{(bardSpeedBuff * 100):#.#}%";
                    text_StatusValue.text = $"{(bardSpeedBuff):#.##}%";

                    isUsed = isLevelupUsed || isStarUsed || isUpgradeUsed;
                    isBtnUsed = increaseStruct.isStarUsed || increaseStruct.isUpgradeUsed ||
                                increaseStruct.isLevelupUsed;

                    if (isUsed)
                    {
                        float increaseBardSpeedBuff = 1 + (characterData.characterUniqueValue[0]
                                                           + (increaseStruct.increaseClassLevel - 1)
                                                           * characterData.classUpFactor[0])
                                                        * Mathf.Pow(
                                                            ConfigData.STARGRADE_FACTOR - characterData.starFactor[0],
                                                            increaseStruct.star)
                                                        + (increaseStruct.upgrade - 1) * characterData.powerFactor[0];

                        //text_IncreaseValue.text = $"{(increaseBardSpeedBuff * 100):#.#}%";
                        text_IncreaseValue.text = $"{(increaseBardSpeedBuff):#.##}%";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.DARKWOLF_INSTANTKILL_RATE:
                    float instantDeathRate = characterData.characterUniqueValue[0]
                                             + characterData.starFactor[0]
                                             * 0;
                    text_StatusValue.text = $"{(instantDeathRate * 100):0.##}%";
                    //text_StatusValue.text = $"{(instantDeathRate):0.##}%";

                    isUsed = isStarUsed;
                    isBtnUsed = increaseStruct.isStarUsed;

                    if (isUsed)
                    {
                        float increaseInstantDeathRate = characterData.characterUniqueValue[0]
                                                         + characterData.starFactor[0]
                                                         * increaseStruct.star;

                        text_IncreaseValue.text = $"{(increaseInstantDeathRate * 100):0.##}%";
                        //text_IncreaseValue.text = $"{(increaseInstantDeathRate):0.##}%";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.DARKWOLF_GET_GEM_RATE:
                    float darkWolfGetGemRate = (characterData.characterUniqueValue[1]
                                                + characterData.classUpFactor[2]
                                                * characterData.characterClassLevel)
                                               * 100;

                    //Debug.Log(darkWolfGetGemRate);
                    text_StatusValue.text = $"{darkWolfGetGemRate:#.##}%";

                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;

                    if (isUsed)
                    {
                        float increaseDarkWolfGetGemRate = (characterData.characterUniqueValue[1]
                                                            + characterData.classUpFactor[2]
                                                            * (increaseStruct.increaseClassLevel))
                                                           * 100;

                        //Debug.Log(increaseDarkWolfGetGemRate);
                        text_IncreaseValue.text = $"{increaseDarkWolfGetGemRate:#.##}%";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.MARRIER_ADDITIVE_DAMAGE:

                    float marrierDamage = (1 + characterData.characterUniqueValue[0]
                                             + (increaseStruct.classLevel - 1) * characterData.classUpFactor[1])
                                          * (500 * 0.1f)
                                          * characterData.starFactor[0]
                                          * 0;

                    //text_StatusValue.text = $"{(int)MarrierTotalIncreaseAttackDamage + marrierDamage}";
                    text_StatusValue.text = Mathf.Floor(marrierDamage).ToString();

                    isUsed = isStarUsed;

                    if (isUsed)
                    {
                        //합성이 되야 값이 오르기때문에 버튼은 합성이 될 시 레벨업과 합성을 클릭 시 애니메이션 진행
                        isBtnUsed = increaseStruct.isStarUsed || increaseStruct.isLevelupUsed;

                        float MarrierincreaseAddtiveDamage = (1 + characterData.characterUniqueValue[0]
                                                                + (increaseStruct.increaseClassLevel - 1) *
                                                                characterData.classUpFactor[1])
                                                             * (500 * 0.1f)
                                                             * characterData.starFactor[0]
                                                             * increaseStruct.star;

                        //MarrierTotalIncreaseAttackDamage = (MaeeierAddtiveAttackDamage * MarrierCalcStarDamage) + MarrierCalcUpgradeLevelupDamage + MarrierincreaseAddtiveDamage;

                        text_IncreaseValue.text = Mathf.Floor(MarrierincreaseAddtiveDamage).ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.MARRIER_LIMIT_GEM:
                    isUpgradeValue = false;
                    int marrierLimitGem = (int)characterData.characterUniqueValue[1];
                    text_StatusValue.text = $"{marrierLimitGem}";
                    IncreaseProperty(false);
                    break;
                case StatusType.PUDDLE_GET_GEM_TIMECOUNT:
                    float getGemTimeCount = characterData.characterUniqueValue[0]
                                            - (characterData.characterClassLevel - 1)
                                            * characterData.classUpFactor[0];
                    text_StatusValue.text = $"{getGemTimeCount:0.#}";

                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;

                    if (isUsed)
                    {
                        float increaseGetGemTimeCount = characterData.characterUniqueValue[0]
                                                        - (increaseStruct.increaseClassLevel - 1)
                                                        * characterData.classUpFactor[0];
                        text_IncreaseValue.text = $"{increaseGetGemTimeCount:0.#}";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.PUDDLE_GET_GEM_RATE:
                    float puddleGetGemRate = characterData.characterUniqueValue[1]
                                             * Mathf.Pow(characterData.starFactor[0], 1);
                    text_StatusValue.text = $"{puddleGetGemRate.ToString("F2")}%";

                    isUsed = isStarUsed;
                    isBtnUsed = increaseStruct.isStarUsed;

                    if (isUsed)
                    {
                        float increasePuddleGetGemRate = characterData.characterUniqueValue[1]
                                                         * Mathf.Pow(characterData.starFactor[0], increaseStruct.star);
                        text_IncreaseValue.text = $"{increasePuddleGetGemRate.ToString("F2")}%";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.PUDDLE_GET_GEM_AMOUNT:
                    float puddleGetGemAmount = characterData.characterUniqueValue[2]
                                               * Mathf.Pow(characterData.starFactor[1], 1);
                    text_StatusValue.text = $"{(int)puddleGetGemAmount}";

                    isUsed = isStarUsed;
                    isBtnUsed = increaseStruct.isStarUsed;

                    if (isUsed)
                    {
                        float increasePuddleGetGemAmount = characterData.characterUniqueValue[2]
                                                           * Mathf.Pow(characterData.starFactor[1],
                                                               increaseStruct.star);
                        text_IncreaseValue.text = $"{(int)increasePuddleGetGemAmount}";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.CRITICAL_DAMAGE:
                    SwitchForCriticalDamage(characterData.characterIndex, characterData, increaseStruct, false);
                    SwitchForCriticalDamage(characterData.characterIndex, characterData, increaseStruct, true);
                    break;
                case StatusType.CARONAPENGO_ADDTIVE_DAMAGE:
                    float addtiveDamage =
                        characterData.characterUniqueValue[0] + (1 - 1) * characterData.powerFactor[1];
                    text_StatusValue.text = addtiveDamage.ToString("F2");

                    isUsed = isUpgradeUsed;
                    isBtnUsed = increaseStruct.isUpgradeUsed;

                    if (isUsed)
                    {
                        float increaseAddtiveDamage = characterData.characterUniqueValue[0] +
                                                      (increaseStruct.upgrade - 1) * characterData.powerFactor[1];
                        text_IncreaseValue.text = increaseAddtiveDamage.ToString("F2");
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.GUARDIAN_BARRIER_CYCLE:
                    float barrierCycle = characterData.characterUniqueValue[0]
                                         - (increaseStruct.classLevel - 1)
                                         * characterData.classUpFactor[0];

                    text_StatusValue.text = barrierCycle.ToString("F2");

                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;
                    if (isUsed)
                    {
                        float increaseBarrierCycle = characterData.characterUniqueValue[0]
                                                     - (increaseStruct.increaseClassLevel - 1)
                                                     * characterData.classUpFactor[0];

                        text_IncreaseValue.text = increaseBarrierCycle.ToString("F2");
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.GUARDIAN_BARRIER_DURATION:
                    float barrierDuration = characterData.characterUniqueValue[1]
                                            + 0 * characterData.starFactor[0];

                    text_StatusValue.text = barrierDuration.ToString("F2");

                    isUsed = isStarUsed;
                    isBtnUsed = increaseStruct.isStarUsed;
                    if (isUsed)
                    {
                        float increaseBarrierDuration = characterData.characterUniqueValue[1]
                                                        + increaseStruct.star * characterData.starFactor[0];

                        text_IncreaseValue.text = increaseBarrierDuration.ToString("F2");
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.TRICKSTER_GLACIER_CHANGE:
                    float glacierChangeStunTime = characterData.characterUniqueValue[0]
                                                  - (increaseStruct.classLevel - 1)
                                                  * characterData.classUpFactor[1];

                    text_StatusValue.text = glacierChangeStunTime.ToString("F2");

                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;
                    if (isUsed)
                    {
                        float increaseGlacierChangeStunTime = characterData.characterUniqueValue[0]
                                                              - (increaseStruct.increaseClassLevel - 1)
                                                              * characterData.classUpFactor[1];

                        text_IncreaseValue.text = increaseGlacierChangeStunTime.ToString("F2");
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.LEMMINGTOXIN_TOXIC_RATETIME:
                    float toixcRateTime = characterData.characterUniqueValue[0] + 0 * characterData.starFactor[0];
                    text_StatusValue.text = toixcRateTime.ToString();

                    isUsed = isStarUsed;
                    isBtnUsed = increaseStruct.isStarUsed;
                    if (isUsed)
                    {
                        float increasetoixcRateTime = characterData.characterUniqueValue[0]
                                                      + increaseStruct.star
                                                      * characterData.starFactor[0];

                        text_IncreaseValue.text = increasetoixcRateTime.ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.LULU_BUBBLE:
                    float temp = characterData.characterUniqueValue[0];
                    text_StatusValue.text = temp.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.YURI_PER_DAMAGE:
                    float yuriPerDamage = ((characterData.characterUniqueValue[0]
                                            + (increaseStruct.classLevel - 1)
                                            * characterData.classUpFactor[1])
                                           + (1 - 1) * characterData.powerFactor[1]) * 0;

                    text_StatusValue.text = $"{yuriPerDamage:0.##%}";

                    isUsed = isStarUsed;
                    if (isUsed)
                    {
                        isBtnUsed = increaseStruct.isLevelupUsed || increaseStruct.isUpgradeUsed ||
                                    increaseStruct.isStarUsed;

                        float increaseYuriPerDamage = ((characterData.characterUniqueValue[0]
                                                        + (increaseStruct.increaseClassLevel - 1)
                                                        * characterData.classUpFactor[1])
                                                       + (increaseStruct.upgrade - 1)
                                                       * characterData.powerFactor[1]) * increaseStruct.star;

                        text_IncreaseValue.text = $"{increaseYuriPerDamage:0.##%}";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.YURI_DURATION:
                    float duration = characterData.characterUniqueValue[2] +
                                     (increaseStruct.classLevel - 1) * characterData.classUpFactor[2];
                    text_StatusValue.text = duration.ToString();

                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;
                    if (isUsed)
                    {
                        float increaseArrowRainDuration = characterData.characterUniqueValue[2]
                                                          + (increaseStruct.increaseClassLevel - 1)
                                                          * characterData.classUpFactor[2];

                        text_IncreaseValue.text = increaseArrowRainDuration.ToString("F2");
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.YURI_COOLDOWN:
                    float coolDown = characterData.characterUniqueValue[3];
                    text_StatusValue.text = coolDown.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.HIFIVE_EXTRA_DAMAGE:
                    float extraDamage = 1 + (5 - 1)
                        * (characterData.characterUniqueValue[0] + (increaseStruct.classLevel - 1)
                            * characterData.classUpFactor[1]);



                    text_StatusValue.text = extraDamage.ToString();

                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;
                    if (isUsed)
                    {
                        float increaseExtraDamage = 1 + (5 - 1)
                            * (characterData.characterUniqueValue[0] + (increaseStruct.increaseClassLevel - 1)
                                * characterData.classUpFactor[1]);

                        text_IncreaseValue.text = increaseExtraDamage.ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.HIFIVE_RANGE_UP:
                    float rangeUp = characterData.detectRange + characterData.characterUniqueValue[1];
                    text_StatusValue.text = rangeUp.ToString();
                    IncreaseProperty(false);
                    break;
                case StatusType.OWLRUS_DAMAGE:
                    float owlrusAddtiveDamage = ((characterData.characterUniqueValue[0]
                                                  + (increaseStruct.classLevel - 1) * characterData.classUpFactor[1])
                                                 + (1 - 1) * characterData.powerFactor[1])
                                                * (0 + characterData.starFactor[0]);

                    text_StatusValue.text = $"{(owlrusAddtiveDamage * 100):0.##}%";

                    isUsed = isLevelupUsed || isUpgradeUsed || isStarUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed || increaseStruct.isUpgradeUsed ||
                                increaseStruct.isStarUsed;
                    if (isUsed)
                    {
                        float increaseOwlAddtiveDamage = ((characterData.characterUniqueValue[0]
                                                           + (increaseStruct.increaseClassLevel - 1) *
                                                           characterData.classUpFactor[1])
                                                          + (increaseStruct.upgrade - 1) * characterData.powerFactor[1])
                                                         * (increaseStruct.star + characterData.starFactor[0]);

                        text_IncreaseValue.text = $"{(increaseOwlAddtiveDamage * 100):0.##}%";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.FLORA_DAMAGE:
                    float floraCoolTime = ((characterData.characterUniqueValue[0]
                                            - (increaseStruct.classLevel - 1) * characterData.classUpFactor[0])
                                           - (1 - 1) * characterData.powerFactor[0])
                                          * 0 * characterData.starFactor[0];

                    text_StatusValue.text = Mathf.RoundToInt(floraCoolTime).ToString();

                    if (increaseStruct.star >= 4)
                    {
                        isUsed = isLevelupUsed || isUpgradeUsed || isStarUsed;
                        isBtnUsed = increaseStruct.isLevelupUsed || increaseStruct.isUpgradeUsed ||
                                    increaseStruct.isStarUsed;

                        if (isUsed)
                        {
                            float starGradeValue = increaseStruct.star == 0 ? 0f : 1f / increaseStruct.star;

                            float increasefloraCoolTime = ((characterData.characterUniqueValue[0]
                                                            - (increaseStruct.increaseClassLevel - 1) *
                                                            characterData.classUpFactor[0])
                                                           - (increaseStruct.upgrade - 1) *
                                                           characterData.powerFactor[0])
                                                          * starGradeValue * characterData.starFactor[0];
                            text_IncreaseValue.text = Mathf.RoundToInt(increasefloraCoolTime).ToString();

                            ReinForcementUp(isBtnUsed);
                        }
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.GUARDIAN_BARRIER_CYCLE_MAX:
                    float barrierMaxCycle = characterData.characterUniqueValue[3]
                                            - (increaseStruct.classLevel - 1)
                                            * characterData.classUpFactor[1];

                    text_StatusValue.text = barrierMaxCycle.ToString("F2");

                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;
                    if (isUsed)
                    {
                        float increaseBarrierMaxCycle = characterData.characterUniqueValue[3]
                                                        - (increaseStruct.increaseClassLevel - 1)
                                                        * characterData.classUpFactor[1];

                        text_IncreaseValue.text = increaseBarrierMaxCycle.ToString("F2");
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.VEGAS_MIN_DAMAGE:
                    float totalMinDamage = (characterData.attackDamage +
                                            (increaseStruct.classLevel - 1) * characterData.classUpFactor[0]) *
                                           characterData.characterUniqueValue[0];
                    text_StatusValue.text = ((int)totalMinDamage).ToString();

                    isUsed = isStarUsed || isUpgradeUsed || isLevelupUsed;
                    isBtnUsed = increaseStruct.isStarUsed || increaseStruct.isUpgradeUsed ||
                                increaseStruct.isLevelupUsed;
                    if (isUsed)
                    {
                        float increaseAttackDamage = (int)(characterData.attackDamage +
                                                           (increaseStruct.increaseClassLevel - 1) *
                                                           characterData.classUpFactor[0]);
                        float calcStarDamage = 1;
                        float calcUpgradeLevelupDamage = 0;

                        calcStarDamage = (Mathf.Pow(ConfigData.STARGRADE_FACTOR, increaseStruct.star));
                        calcUpgradeLevelupDamage = ((increaseStruct.upgrade - 1)
                                                    * characterData.powerFactor[0] * increaseStruct.star) +
                                                   (increaseStruct.upgrade - 1);

                        float TotalIncreaseAttackDamage =
                            (increaseAttackDamage * calcStarDamage) + calcUpgradeLevelupDamage;
                        TotalIncreaseAttackDamage =
                            Mathf.Floor(TotalIncreaseAttackDamage * characterData.characterUniqueValue[0]);

                        text_IncreaseValue.text = TotalIncreaseAttackDamage.ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.VEGAS_MAX_DAMAGE:

                    float damage = (characterData.attackDamage +
                                    (increaseStruct.classLevel - 1) * characterData.classUpFactor[0]);

                    float maxDamage = characterData.characterUniqueValue[1]
                                      + ((increaseStruct.classLevel - 1)
                                         * characterData.classUpFactor[1]) +
                                      (0 * characterData.starFactor[0])
                                      + ((1 - 1) * characterData.powerFactor[1]);

                    float totalMaxDamage = Mathf.Floor(damage * maxDamage);


                    text_StatusValue.text = ((int)totalMaxDamage).ToString();

                    isUsed = isStarUsed || isUpgradeUsed || isLevelupUsed;
                    isBtnUsed = increaseStruct.isStarUsed || increaseStruct.isUpgradeUsed ||
                                increaseStruct.isLevelupUsed;
                    if (isUsed)
                    {
                        float increaseAttackDamage = (int)(characterData.attackDamage +
                                                           (increaseStruct.increaseClassLevel - 1) *
                                                           characterData.classUpFactor[0]);
                        float calcStarDamage = 1;
                        float calcUpgradeLevelupDamage = 0;

                        calcStarDamage = (Mathf.Pow(ConfigData.STARGRADE_FACTOR, increaseStruct.star));
                        calcUpgradeLevelupDamage =
                            ((increaseStruct.upgrade - 1) * characterData.powerFactor[0] * increaseStruct.star) +
                            (increaseStruct.upgrade - 1);

                        float vegasAttackDamage = (increaseAttackDamage * calcStarDamage) + calcUpgradeLevelupDamage;

                        float increaseMaxDamage = characterData.characterUniqueValue[1]
                                                  + ((increaseStruct.increaseClassLevel - 1)
                                                     * characterData.classUpFactor[1]) +
                                                  (increaseStruct.star * characterData.starFactor[0])
                                                  + ((increaseStruct.upgrade - 1) * characterData.powerFactor[1]);

                        float totalIncreaseMaxAttackDamage = Mathf.Floor(vegasAttackDamage * increaseMaxDamage);

                        text_IncreaseValue.text = ((int)totalIncreaseMaxAttackDamage).ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.VEGAS_SPECIAL_EFFECT:
                    float totalDamage = (characterData.attackDamage +
                                         (increaseStruct.classLevel - 1) * characterData.classUpFactor[0]) *
                                        characterData.characterUniqueValue[2];
                    text_StatusValue.text = ((int)totalDamage).ToString();

                    isUsed = isStarUsed || isUpgradeUsed || isLevelupUsed;
                    isBtnUsed = increaseStruct.isStarUsed || increaseStruct.isUpgradeUsed ||
                                increaseStruct.isLevelupUsed;
                    if (isUsed)
                    {
                        float increaseAttackDamage = (int)(characterData.attackDamage +
                                                           (increaseStruct.increaseClassLevel - 1) *
                                                           characterData.classUpFactor[0]);
                        float calcStarDamage = 1;
                        float calcUpgradeLevelupDamage = 0;

                        calcStarDamage = (Mathf.Pow(ConfigData.STARGRADE_FACTOR, increaseStruct.star));
                        calcUpgradeLevelupDamage = ((increaseStruct.upgrade - 1)
                                                    * characterData.powerFactor[0] * increaseStruct.star) +
                                                   (increaseStruct.upgrade - 1);

                        float TotalIncreaseAttackDamage =
                            (increaseAttackDamage * calcStarDamage) + calcUpgradeLevelupDamage;
                        TotalIncreaseAttackDamage =
                            Mathf.Floor(TotalIncreaseAttackDamage * characterData.characterUniqueValue[1]) *
                            characterData.characterUniqueValue[2];

                        text_IncreaseValue.text = TotalIncreaseAttackDamage.ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.HAMMERING_ATTACK_RATE:
                    float hammerAttackRate = characterData.characterUniqueValue[0] +
                                             characterData.characterUniqueValue[0] *
                                             Mathf.Pow(0, characterData.starFactor[0]);

                    text_StatusValue.text = $"{(hammerAttackRate * 100):0.##}%";

                    isUsed = isStarUsed;
                    isBtnUsed = increaseStruct.isStarUsed;
                    if (isUsed)
                    {
                        float increaseHammerAttackRate = characterData.characterUniqueValue[0] +
                                                         characterData.characterUniqueValue[0] *
                                                         Mathf.Pow(increaseStruct.star, characterData.starFactor[0]);

                        text_IncreaseValue.text = $"{(increaseHammerAttackRate * 100):0.##}%";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.HAMMERING_ATTACK_DAMAGE:
                    float hammerAttackDamage = characterData.characterUniqueValue[1] +
                                               (increaseStruct.classLevel - 1) * characterData.classUpFactor[2];

                    text_StatusValue.text = Mathf.Floor(hammerAttackDamage).ToString();

                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;
                    if (isUsed)
                    {
                        float increaseHammerAttackDamage = characterData.characterUniqueValue[1] +
                                                           (increaseStruct.increaseClassLevel - 1) *
                                                           characterData.classUpFactor[2];

                        text_IncreaseValue.text = Mathf.Floor(increaseHammerAttackDamage).ToString();
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.HAMMERING_UPGRADE_DISCOUNT:
                    float upgradeCostValue = 1 - (characterData.characterUniqueValue[3] -
                                                  (increaseStruct.classLevel - 1)
                                                  * characterData.classUpFactor[1]);

                    text_StatusValue.text = $"{(upgradeCostValue * 100):0.##}%";

                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;
                    if (isUsed)
                    {
                        float increaseUpgradeCostValue = 1 - (characterData.characterUniqueValue[3] -
                                                              (increaseStruct.increaseClassLevel - 1)
                                                              * characterData.classUpFactor[1]);

                        text_IncreaseValue.text = $"{(increaseUpgradeCostValue * 100):0.##}%";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.HAMMERING_UPGRADE_RATE:
                    float hammerUpgradeRate = characterData.characterUniqueValue[4] *
                                              Mathf.Pow(characterData.powerFactor[1], 1 - 1);
                    text_StatusValue.text = $"{(hammerUpgradeRate * 100):0.##}%";

                    isUsed = isUpgradeUsed;
                    isBtnUsed = increaseStruct.isUpgradeUsed;
                    if (isUsed)
                    {
                        float increaseUpgradeResetRate = characterData.characterUniqueValue[4]
                                                         * Mathf.Pow(characterData.powerFactor[1],
                                                             increaseStruct.upgrade - 1);

                        text_IncreaseValue.text = $"{(increaseUpgradeResetRate * 100):0.##}%";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                    break;
                case StatusType.HAMMERING_PER_DAMAGE:
                    float perDamage = 0f;
                    float increasePerDamage = 0f;

                    bool isPerDamageActive = increaseStruct.upgrade < characterData.characterUniqueValue[7];

                    if (isPerDamageActive)
                    {
                        text_StatusValue.text = $"{(perDamage * 100):0.##}%";
                        text_IncreaseValue.text = $"{(increasePerDamage * 100):0.##}%";
                        IncreaseProperty(false);
                    }
                    else
                    {
                        perDamage = ((characterData.characterUniqueValue[6] +
                                      (increaseStruct.classLevel - 1)
                                      * characterData.classUpFactor[3]) +
                                     (characterData.characterUniqueValue[7] - 1) * characterData.powerFactor[3]) * 0;

                        text_StatusValue.text = $"{(perDamage * 100):0.##}%";

                        isUsed = isUpgradeUsed || isLevelupUsed || isStarUsed;
                        isBtnUsed = increaseStruct.isUpgradeUsed || increaseStruct.isLevelupUsed ||
                                    increaseStruct.isStarUsed;

                        if (isUsed)
                        {
                            increasePerDamage = ((characterData.characterUniqueValue[6]
                                                  + (increaseStruct.increaseClassLevel - 1)
                                                  * characterData.classUpFactor[3])
                                                 + (increaseStruct.upgrade - 1) * characterData.powerFactor[3]) *
                                                increaseStruct.star;


                            text_IncreaseValue.text = $"{(increasePerDamage * 100):0.##}%";
                            ReinForcementUp(isBtnUsed);
                        }

                        IncreaseProperty(isUsed);
                    }

                    break;

                case StatusType.TRICKSTER_GLACIER_CHANGE_RATE:
                {
                    float changeRate = characterData.characterUniqueValue[1] +
                                       (increaseStruct.classLevel - 1) * characterData.classUpFactor[2];

                    text_StatusValue.text = $"{(changeRate * 100):0.##}%";

                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;

                    if (isUsed)
                    {
                        float increaseRate = characterData.characterUniqueValue[1] +
                                             (increaseStruct.increaseClassLevel - 1) * characterData.classUpFactor[2];

                        text_IncreaseValue.text = $"{(increaseRate * 100):0.##}%";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                }

                    break;

                case StatusType.RIO_ATTACK_READY_COOLTIME:
                {
                    float rioAttackSpeed = (characterData.attackSpeed 
                                            +(increaseStruct.classLevel - 1) 
                                            * characterData.classUpFactor[1])
                                           + (1 - 1) * characterData.powerFactor[1];
                    
                    float attackCoolTime = characterData.characterUniqueValue[1] 
                                           - (rioAttackSpeed * (characterData.characterUniqueValue[3] 
                                                                + (increaseStruct.classLevel - 1) 
                                                                * characterData.classUpFactor[2]));
                    
                    text_StatusValue.text = $"{attackCoolTime:0.#}";
                    
                    
                    isUsed = isLevelupUsed || isUpgradeUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed || increaseStruct.isUpgradeUsed;

                    if (isUsed)
                    {
                        float increaseRioAttackSpeed = (characterData.attackSpeed
                                                        + (increaseStruct.increaseClassLevel - 1)
                                                        * characterData.classUpFactor[1])
                                                       + (increaseStruct.upgrade - 1) * characterData.powerFactor[1];
                        
                        float increaseAttackCoolTime = characterData.characterUniqueValue[1]
                                                       - (increaseRioAttackSpeed * (characterData.characterUniqueValue[3] 
                                                        + (increaseStruct.increaseClassLevel - 1) 
                                                    * characterData.classUpFactor[2]));
                        
                        text_IncreaseValue.text =  $"{increaseAttackCoolTime:0.#}";
                        
                        ReinForcementUp(isBtnUsed);
                    }
                    
                    IncreaseProperty(isUsed);
                }
                    break;
                case StatusType.CHAMY_MERGE_SUCCESS_RATE:
                {
                    float mergeRate = characterData.characterUniqueValue[0] - 0 * (characterData.starFactor[0] -
                        characterData.classUpFactor[0] * increaseStruct.classLevel);

                    text_StatusValue.text = $"{(mergeRate * 100):0.##}%";

                    isUsed = isLevelupUsed || isStarUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed || increaseStruct.isStarUsed;

                    if (isUsed)
                    {
                        float increaseRate = characterData.characterUniqueValue[0] - increaseStruct.star *
                            (characterData.starFactor[0] -
                             characterData.classUpFactor[0] * increaseStruct.increaseClassLevel);

                        text_IncreaseValue.text = $"{(increaseRate * 100):0.##}%";
                        ReinForcementUp(isBtnUsed);
                    }

                    IncreaseProperty(isUsed);
                }
                    break;
            }
        }

        public void SwitchForCriticalDamage(CharacterIndex type, CharacterData characterData, IncreaseStruct increaseStruct, bool isIncrease)
        {
            //크리티컬 데미지 보여주기용 = 기본공격력 * 크리티컬 데미지

            float CriticalDamage = characterData.criticalDamageRate;
            float TotalCriticalDamage = 0;

            bool isBtnUsed = false;
            bool isUsed = false;
            bool isStarUsed = increaseStruct.star != 0;
            bool isUpgradeUsed = increaseStruct.upgrade != 1;
            bool isLevelupUsed = increaseStruct.classLevel != increaseStruct.increaseClassLevel;


            switch (type)
            {
                case CharacterIndex.EMPEROR_PENGGO:
                    if (!isIncrease)
                    {
                        TotalCriticalDamage = CriticalDamage + (1 - 1) * characterData.powerFactor[2];
                        text_StatusValue.text = $"{TotalCriticalDamage:#.#}";
                        IncreaseProperty(false);
                    }
                    // else
                    // {
                    //     isUsed = isUpgradeUsed;
                    //     isBtnUsed = increaseStruct.isUpgradeUsed;
                    //
                    //     if (isUsed)
                    //     {
                    //         TotalCriticalDamage = CriticalDamage + (increaseStruct.upgrade - 1) * characterData.powerFactor[2];
                    //         text_IncreaseValue.text = $"{TotalCriticalDamage:#.#}";
                    //         ReinForcementUp(isBtnUsed);
                    //     }
                    //     IncreaseProperty(isUsed);
                    // }
                    break;
                case CharacterIndex.DARKWOLF:
                    if (!isIncrease)
                    {
                        TotalCriticalDamage = CriticalDamage + (increaseStruct.classLevel - 1) * characterData.classUpFactor[1];
                        text_StatusValue.text = $"{TotalCriticalDamage:#.#}";
                    }
                    else
                    {
                        isUsed = isLevelupUsed;
                        isBtnUsed = increaseStruct.isLevelupUsed;

                        if (isUsed)
                        {
                            TotalCriticalDamage = CriticalDamage + (increaseStruct.increaseClassLevel - 1) * characterData.classUpFactor[1];
                            text_IncreaseValue.text = $"{TotalCriticalDamage:#.#}";
                            ReinForcementUp(isBtnUsed);
                        }
                        IncreaseProperty(isUsed);
                    }
                    break;
            }
        }

        public void SwitchForAttackSpeed(CharacterIndex type, CharacterData characterData, IncreaseStruct increaseStruct)
        {
            //캐릭터마다 공격속도가 다르기에 나눠줌

            bool isBtnUsed = false;
            bool isUsed = false;
            bool isStarUsed = increaseStruct.star != 0;
            bool isUpgradeUsed = increaseStruct.upgrade != 1;
            bool isLevelupUsed = increaseStruct.classLevel != increaseStruct.increaseClassLevel;

            switch (type)
            {
                case CharacterIndex.KIRING:
                    float AttackSpeed = characterData.attackSpeed + (1 - 1) * characterData.powerFactor[1];
                    text_StatusValue.text = AttackSpeed.ToString("F2");

                    isUsed = isUpgradeUsed;
                    isBtnUsed = increaseStruct.isUpgradeUsed;
                    if (isUsed)
                    {
                        float increaseAttackSpeed = characterData.attackSpeed + (increaseStruct.upgrade - 1) * characterData.powerFactor[1];
                        text_IncreaseValue.text = increaseAttackSpeed.ToString("F2");
                        ReinForcementUp(isBtnUsed);
                    }
                    IncreaseProperty(isUsed);
                    break;
                case CharacterIndex.POLARIS:
                    float PolarrisAttackSpeed = characterData.attackSpeed + Mathf.Pow(0 * characterData.starFactor[0], ConfigData.STARGRADE_FACTOR);

                    text_StatusValue.text = PolarrisAttackSpeed.ToString("F2");

                    //
                    isUsed = isStarUsed;
                    isBtnUsed = increaseStruct.isStarUsed;

                    if (isUsed)
                    {
                        float PolarisIncreaseAttackSpeed = characterData.attackSpeed + Mathf.Pow(increaseStruct.star * characterData.starFactor[0], ConfigData.STARGRADE_FACTOR);
                        text_IncreaseValue.text = PolarisIncreaseAttackSpeed.ToString("F2");
                        ReinForcementUp(isBtnUsed);
                    }
                    IncreaseProperty(isUsed);
                    break;
                case CharacterIndex.BEOSK:
                    float BeoskAttackSpeed = characterData.attackSpeed + (increaseStruct.classLevel - 1) * characterData.powerFactor[1];
                    text_StatusValue.text = BeoskAttackSpeed.ToString("F2");

                    //
                    isUsed = isLevelupUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed;

                    if (isUsed)
                    {
                        float increaseBeoskAttackSpeed = characterData.attackSpeed + (increaseStruct.increaseClassLevel - 1) * characterData.powerFactor[1];
                        text_IncreaseValue.text = increaseBeoskAttackSpeed.ToString("F2");
                        ReinForcementUp(isBtnUsed);
                    }
                    IncreaseProperty(isUsed);

                    break;
                case CharacterIndex.BLACK_MAGICIAN_FOXY:
                    float blackMagicAttackSpeed = characterData.attackSpeed + 0 * characterData.starFactor[0];
                    text_StatusValue.text = blackMagicAttackSpeed.ToString("F1");

                    //
                    isUsed = isStarUsed;
                    isBtnUsed = increaseStruct.isStarUsed;

                    if (isUsed)
                    {
                        float increaseBeoskAttackSpeed = characterData.attackSpeed + increaseStruct.star * characterData.starFactor[0];
                        text_IncreaseValue.text = increaseBeoskAttackSpeed.ToString("F1");
                        ReinForcementUp(isBtnUsed);
                    }
                    IncreaseProperty(isUsed);
                    break;
                case CharacterIndex.RIO:
                    float rioAttackSpeed = (characterData.attackSpeed 
                                            +(increaseStruct.classLevel - 1) 
                                            * characterData.classUpFactor[1])
                                           + (1 - 1) * characterData.powerFactor[1];
                    
                    text_StatusValue.text = $"{rioAttackSpeed:0.##}";

                    isUsed = isLevelupUsed || isUpgradeUsed;
                    isBtnUsed = increaseStruct.isLevelupUsed || increaseStruct.isUpgradeUsed;

                    if (isUsed)
                    {
                        
                        float increaseRioAttackSpeed =  (characterData.attackSpeed 
                                                         +(increaseStruct.increaseClassLevel - 1) 
                                                         * characterData.classUpFactor[1])
                                                        + (increaseStruct.upgrade - 1) * characterData.powerFactor[1];
                        text_IncreaseValue.text = $"{increaseRioAttackSpeed:0.##}";
                        ReinForcementUp(isBtnUsed);
                    }
                    IncreaseProperty(isUsed);
                    break;
            }
        }


        #region 백업용
        //public void LogicInitialize(StatusType type, CharacterData characterData, int classLevel)
        //{
        //    isUpgradeValue = true;

        //    //캐릭터 데이터에서 관리
        //    //클래스 레벨

        //    //지금 변수가 캐릭터 레벨이 0일경우 체크해서 캐릭터를 안가지고있을 때만 인크리스를 비활성화했는데 이제 처음부터 비활성화하고
        //    //캐릭터 강화 버튼을 눌를경우에만 활성화를 해야합니다 


        //    //new temp dongmin
        //    //잘 변경되는지 테스트 하기위해 구성 이것도 변경예정

        //    switch (type)
        //    {
        //        case StatusType.ATTACK_DAMAGE:
        //            if (characterData.characterType == CharacterType.SUB)
        //            {
        //                text_StatusValue.text = characterData.attackDamage.ToString();
        //                IncreaseProperty(false);
        //            }
        //            else
        //            {
        //                float attackDamage = (characterData.attackDamage + (classLevel - 1) * characterData.classUpFactor[0]);
        //                text_StatusValue.text = ((int)attackDamage).ToString();
        //                float increaseAttackDamage = (int)(characterData.attackDamage + (classLevel) * characterData.classUpFactor[0]);
        //                text_IncreaseValue.text = increaseAttackDamage.ToString();
        //                IncreaseProperty(!isInActive);
        //            }

        //            break;
        //        case StatusType.ATTACK_SPEED:
        //            float attackSpeed = characterData.attackSpeed;
        //            text_StatusValue.text = attackSpeed.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.INCREASE_ATTACK_SPEED:
        //            float increaseAttackSpeed = characterData.attackSpeed + (classLevel - 1) * characterData.classUpFactor[1];
        //            text_StatusValue.text = increaseAttackSpeed.ToString();
        //            float tempAttackSpeed = characterData.attackSpeed + (classLevel) * characterData.classUpFactor[1];
        //            text_IncreaseValue.text = tempAttackSpeed.ToString();
        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.DETECT_RANGE:
        //            float detectRange = characterData.detectRange;
        //            text_StatusValue.text = detectRange.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.INCREASE_DETECT_RANGE:
        //            float increaseDetectRange = characterData.detectRange + (classLevel - 1) * characterData.classUpFactor[1];
        //            text_StatusValue.text = increaseDetectRange.ToString();
        //            float tempDetectRange = characterData.detectRange + (classLevel) * characterData.classUpFactor[1];
        //            text_IncreaseValue.text = tempDetectRange.ToString();
        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.TARGET_COUNT:
        //            int targetCount = (int)characterData.targetCount;
        //            text_StatusValue.text = targetCount.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.STRIKE_RANGE:
        //            float strikeRange = characterData.strikeRange;
        //            text_StatusValue.text = strikeRange.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.BUFF_VALUE:
        //            float buffValue = characterData.buffValue + (classLevel - 1) * characterData.classUpFactor[0];
        //            text_StatusValue.text = buffValue.ToString();
        //            float increaseBuffValue = characterData.buffValue + (classLevel) * characterData.classUpFactor[0];
        //            text_IncreaseValue.text = increaseBuffValue.ToString();
        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.BUFF_CYCLE:
        //            float buffCycle = characterData.buffCycle;
        //            text_StatusValue.text = buffCycle.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.BUFF_DURATION:
        //            float buffDuration = characterData.buffDuration;
        //            text_StatusValue.text = buffDuration.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.BUFF_RANGE:
        //            float buffRange = characterData.buffRange;
        //            text_StatusValue.text = buffRange.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.WAVE_DAMAGE:
        //            float waveDamage = characterData.characterUniqueValue[0];
        //            text_StatusValue.text = waveDamage.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.SLOW_RATE:
        //            float slowRate = characterData.characterUniqueValue[0] * 100;
        //            text_StatusValue.text = $"{slowRate}%";
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.SLOW_TIME:
        //            float slowTime = characterData.buffDuration;
        //            text_StatusValue.text = slowTime.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.ADD_CRITICAL_RATE:
        //            float criticalRate = characterData.criticalRange;
        //            text_StatusValue.text = $"{(int)(criticalRate * 100)}%";
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.GET_GEM_VALUE:
        //            float gemValue = characterData.characterUniqueValue[1] + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1];
        //            text_StatusValue.text = gemValue.ToString();

        //            //float additiveGemValue = data.characterUniqueValue[1] + data.characterClassLevel * data.classUpFactor[1];
        //            //text_IncreaseValue.text = additiveGemValue.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.BOMB_DAMAGE:
        //            float bombDamage = characterData.characterUniqueValue[2];
        //            text_StatusValue.text = bombDamage.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.STUN_TIME:
        //            float stunTime = characterData.characterUniqueValue[0];
        //            text_StatusValue.text = stunTime.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.EXCUTION_HEALTH_RATE:
        //            float oneShotCondition = characterData.characterUniqueValue[0];
        //            text_StatusValue.text = $"{oneShotCondition * 100}%";
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.BERSERKER_ATTACK_DAMAGE:
        //            float berserkerDamage = characterData.attackDamage + (classLevel - 1) * characterData.classUpFactor[0] +
        //                characterData.characterUniqueValue[0] + (classLevel - 1) * characterData.classUpFactor[1];
        //            text_StatusValue.text = berserkerDamage.ToString();
        //            float tempBerserkerDamage = characterData.attackDamage + (classLevel) * characterData.classUpFactor[0] +
        //                characterData.characterUniqueValue[0] + (classLevel) * characterData.classUpFactor[1];
        //            text_IncreaseValue.text = tempBerserkerDamage.ToString();
        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.BERSERKER_ATTACK_SPEED:
        //            float berserkerSpeed = characterData.attackSpeed + characterData.characterUniqueValue[1];
        //            text_StatusValue.text = berserkerSpeed.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.BLACKMAGIC_DAMAGE:
        //            float blackDamage = characterData.attackDamage + (classLevel - 1) * characterData.classUpFactor[0] +
        //                characterData.characterUniqueValue[0] + (classLevel - 1) * characterData.classUpFactor[1];
        //            text_StatusValue.text = blackDamage.ToString();

        //            float tempBlackDamage = characterData.attackDamage + (classLevel) * characterData.classUpFactor[0] +
        //                characterData.characterUniqueValue[0] + (classLevel) * characterData.classUpFactor[1];
        //            text_IncreaseValue.text = tempBlackDamage.ToString();
        //            IncreaseProperty(!isInActive);

        //            break;
        //        case StatusType.TRANSFER_DAMAGE_RATE:
        //            float totalTransferDamage = characterData.characterUniqueValue[0] + (classLevel - 1) * characterData.classUpFactor[1]
        //                + (1 - 1) * characterData.powerFactor[1];
        //            float tempTd = totalTransferDamage * (characterData.attackDamage + (classLevel - 1) * characterData.classUpFactor[0]);
        //            text_StatusValue.text = $"{Mathf.CeilToInt(tempTd)}";

        //            float increaseTransferDamage = characterData.characterUniqueValue[0] + (classLevel) * characterData.classUpFactor[1]
        //                + (1 - 1) * characterData.powerFactor[1];
        //            float tempItd = increaseTransferDamage * (characterData.attackDamage + (classLevel) * characterData.classUpFactor[0]);
        //            text_IncreaseValue.text = $"{Mathf.CeilToInt(tempItd)}";
        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.TRANSFER_COUNT:
        //            int transferCount = (int)characterData.characterUniqueValue[1];
        //            text_StatusValue.text = transferCount.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.SLOW_BOARD_TIME:
        //            float slowBoardTime = characterData.characterUniqueValue[1];
        //            text_StatusValue.text = slowBoardTime.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.SLOW_BOARD_RATE:
        //            float slowBoardRate = characterData.characterUniqueValue[0];
        //            text_StatusValue.text = $"{(int)(slowBoardRate * 100)}%";
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.TRANSFORM_TIME:
        //            float transformTime = characterData.characterUniqueValue[1] + (classLevel - 1) * characterData.classUpFactor[1];
        //            text_StatusValue.text = transformTime.ToString();
        //            float tempTransformTime = characterData.characterUniqueValue[1] + (classLevel) * characterData.classUpFactor[1];
        //            text_IncreaseValue.text = tempTransformTime.ToString();
        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.TRANSFORM_CYCLE:
        //            float transformCycle = characterData.characterUniqueValue[0];
        //            text_StatusValue.text = transformCycle.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.TRANSFORM_ADDITIVE_ATTACK_SPEED:
        //            float transformAttackSpeed = characterData.attackSpeed + characterData.characterUniqueValue[2];
        //            text_StatusValue.text = transformAttackSpeed.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.GAIN_GEM_RATE:
        //            float gainGemRate = characterData.characterUniqueValue[0] + (classLevel - 1) * characterData.classUpFactor[1];
        //            text_StatusValue.text = gainGemRate.ToString();

        //            float increasegainGemRate = characterData.characterUniqueValue[0] + (classLevel) * characterData.classUpFactor[1];
        //            text_IncreaseValue.text = increasegainGemRate.ToString();
        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.KORRA_WAVE:
        //            text_StatusValue.text = $"{characterData.characterUniqueValue[0]}";
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.POLARIS_STRIKE_RANGE:
        //            float range = characterData.strikeRange;
        //            text_StatusValue.text = range.ToString();
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.POLARIS_SEC:
        //            float sec = characterData.characterUniqueValue[1] - (classLevel - 1) * characterData.classUpFactor[1];
        //            text_StatusValue.text = $"{sec}";
        //            float increaseSec = characterData.characterUniqueValue[1] - classLevel * characterData.classUpFactor[1];
        //            text_IncreaseValue.text = increaseSec.ToString();
        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.DAVI_ATK:
        //            float daviAtk = 1 * ((characterData.characterUniqueValue[0] + (classLevel - 1)
        //                * characterData.classUpFactor[1]) + (1 - 1)
        //                * characterData.powerFactor[1]);

        //            text_StatusValue.text = $"{daviAtk}";
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.DAVI_ATS:
        //            float daviAts = characterData.attackSpeed + (1 * characterData.characterUniqueValue[1]);
        //            text_StatusValue.text = $"{daviAts}";
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.DAVI_SEC:
        //            float daviSec = characterData.characterUniqueValue[3] - (classLevel - 1) * characterData.classUpFactor[2];
        //            text_StatusValue.text = $"{daviSec}";
        //            float increaseDaviSec = characterData.characterUniqueValue[3] - (classLevel) * characterData.classUpFactor[2];
        //            text_IncreaseValue.text = $"{increaseDaviSec}";
        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.KYLE_ATK:
        //            float kyleBuff = (characterData.characterUniqueValue[0] + (classLevel - 1) * characterData.classUpFactor[1])
        //                * (Mathf.Pow(ConfigData.STARGRADE_FACTOR + characterData.characterUniqueValue[1], 1)) +
        //                (1 - 1) * characterData.powerFactor[1] * (1 + characterData.characterUniqueValue[1])
        //                + 1 - 1;
        //            text_StatusValue.text = $"{kyleBuff}";

        //            float increaseKyleBuff = (characterData.characterUniqueValue[0] + (classLevel) * characterData.classUpFactor[1])
        //                * (Mathf.Pow(ConfigData.STARGRADE_FACTOR + characterData.characterUniqueValue[1], 1)) +
        //                (1 - 1) * characterData.powerFactor[1] * (1 + characterData.characterUniqueValue[1])
        //                + 1 - 1;
        //            text_IncreaseValue.text = $"{increaseKyleBuff}";
        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.BARD_SPEED_BUFF:
        //            float bardSpeedBuff = (characterData.characterUniqueValue[0]
        //                + (characterData.characterClassLevel - 1)
        //                * characterData.classUpFactor[0])
        //                * Mathf.Pow(ConfigData.STARGRADE_FACTOR, 0);
        //            //+ (1 - 1) * characterData.powerFactor[0];
        //            text_StatusValue.text = $"{(bardSpeedBuff * 100):#.#}%";

        //            float increaseBardSpeedBuff = (characterData.characterUniqueValue[0]
        //              + (characterData.characterClassLevel)
        //              * characterData.classUpFactor[0])
        //              * Mathf.Pow(ConfigData.STARGRADE_FACTOR, 0);
        //            //+ (1 - 1) * characterData.powerFactor[0];
        //            text_IncreaseValue.text = $"{(increaseBardSpeedBuff * 100):#.#}%";
        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.DARKWOLF_INSTANTKILL_RATE:
        //            float instantDeathRate = characterData.characterUniqueValue[0]
        //                + characterData.starFactor[0]
        //                * 0;
        //            text_StatusValue.text = $"{(instantDeathRate * 100):0.##}%";
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.DARKWOLF_GET_GEM_RATE:
        //            float darkWolfGetGemRate = (characterData.characterUniqueValue[1]
        //                + characterData.classUpFactor[2]
        //                * characterData.characterClassLevel)
        //                * 100;
        //            Debug.Log(darkWolfGetGemRate);
        //            text_StatusValue.text = $"{darkWolfGetGemRate:#.##}%";

        //            float increaseDarkWolfGetGemRate = (characterData.characterUniqueValue[1]
        //             + characterData.classUpFactor[2]
        //             * (characterData.characterClassLevel + 1))
        //             * 100;

        //            Debug.Log(increaseDarkWolfGetGemRate);
        //            text_IncreaseValue.text = $"{increaseDarkWolfGetGemRate:#.##}%";
        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.MARRIER_ADDITIVE_DAMAGE:
        //            float marrierDamage = (1 + characterData.characterUniqueValue[0]
        //                + (characterData.characterClassLevel - 1) * characterData.classUpFactor[1])
        //                * (500 * 0.1f)
        //                * characterData.starFactor[0]
        //                * 0;

        //            text_StatusValue.text = $"{(int)marrierDamage}";

        //            float increaseMarrierDamage = (1 + characterData.characterUniqueValue[0]
        //              + (characterData.characterClassLevel) * characterData.classUpFactor[1])
        //              * (500 * 0.1f)
        //              * characterData.starFactor[0]
        //              * 0;

        //            text_IncreaseValue.text = $"{(int)increaseMarrierDamage}";

        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.MARRIER_LIMIT_GEM:
        //            isUpgradeValue = false;
        //            int marrierLimitGem = (int)characterData.characterUniqueValue[1];
        //            text_StatusValue.text = $"{marrierLimitGem}";
        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.PUDDLE_GET_GEM_TIMECOUNT:
        //            float getGemTimeCount = characterData.characterUniqueValue[0]
        //                - (characterData.characterClassLevel - 1)
        //                * characterData.classUpFactor[0];
        //            text_StatusValue.text = $"{getGemTimeCount:0.#}";
        //            float increaseGetGemTimeCount = characterData.characterUniqueValue[0]
        //                - (characterData.characterClassLevel)
        //                * characterData.classUpFactor[0];
        //            text_IncreaseValue.text = $"{increaseGetGemTimeCount:0.#}";
        //            IncreaseProperty(!isInActive);
        //            break;
        //        case StatusType.PUDDLE_GET_GEM_RATE:
        //            float puddleGetGemRate = characterData.characterUniqueValue[1]
        //                * Mathf.Pow(characterData.starFactor[0], 0);
        //            text_StatusValue.text = $"{puddleGetGemRate}%";
        //            float increasePuddleGetGemRate = characterData.characterUniqueValue[1]
        //                * Mathf.Pow(characterData.starFactor[0], 1);
        //            text_IncreaseValue.text = $"{increasePuddleGetGemRate}%";

        //            IncreaseProperty(false);
        //            break;
        //        case StatusType.PUDDLE_GET_GEM_AMOUNT:
        //            float puddleGetGemAmount = characterData.characterUniqueValue[2]
        //                * Mathf.Pow(characterData.starFactor[1], 0);
        //            text_StatusValue.text = $"{(int)puddleGetGemAmount}";
        //            float increasePuddleGetGemAmount = characterData.characterUniqueValue[2]
        //                * Mathf.Pow(characterData.starFactor[1], 1);
        //            text_IncreaseValue.text = $"{(int)increasePuddleGetGemAmount}";
        //            IncreaseProperty(false);
        //            break;
        //    }

        //    if (characterData.characterClassLevel == 15)
        //    {
        //        IncreaseProperty(false);
        //    }
        //}
        #endregion
    }
}
