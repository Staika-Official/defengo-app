using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using UnityEngine;
using Spine;
using Spine.Unity;
using UniRx;
using UniRx.Triggers;
using Framework.Util;
using DG.Tweening;
using UnityEngine.Events;
using Framework.Sound;

namespace Framework.Game.Defense
{
    public enum ActionType
    {
        BOMB = 0,
        FAIL = 1,
        GEM = 2
    }

    public class CharacterKanade : Character
    {
        public int gemValue;
        public float bombDamage;
        public float explosionTime;
        public float bombStrikeRange;
        public UnityAction kanadeAction;
        public ActionType actionType;
        
        public override IEnumerator ActionSequence()
        {
            while (isSummoned && !IsLockdown)
            {
                int randomIndex = Random.Range(0, 3);

                actionType = (ActionType)randomIndex;

                string animKey = actionType switch
                {
                    ActionType.BOMB => "_Bomb",
                    ActionType.FAIL => "_Fail",
                    ActionType.GEM => "_LifeStone",
                    _ => "_Fail"
                };

                yield return new WaitForSeconds(attackSpeed);

                TrackEntry entry = anim.AnimationState.SetAnimation(0, $"Attack_FrontSide{animKey}", false);

                yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

                switch (actionType)
                {
                    case ActionType.BOMB:
                        BombAction();
                        break;
                    case ActionType.FAIL:
                        FailAction();
                        break;
                    case ActionType.GEM:
                        GemAction();
                        break;
                    default:
                        break;
                }

                yield return new WaitForSpineAnimationComplete(entry);
                anim.AnimationState.SetAnimation(0, "Idle", true);
            }
        }

        public void BombAction()
        {
            KanadeBomb bomb = GameManager.Instance.objectPoolManager.GetObject<KanadeBomb>("KanadeBomb");

            bomb.Initialize(explosionTime, CalcBombPosition(), bombDamage, bombStrikeRange);
        }

        public int CalcBombPosition()
        {
            //int pivot = constructibleIdx;

            //int[] temp = { pivot + 60, pivot - 60, pivot + 3, pivot - 3, pivot + 6, pivot - 6 };

            //for (int i = 0; i < temp.Length; i++)
            //{
            //    int a = temp[i];

            //    if (a < 0 || a > GridManager.Instance.cellDatas.Count) continue;

            //    if (GridManager.Instance.cellDatas[a].cellType == CellType.MOVABLE)
            //    {
            //        return a;
            //    }
            //}

            //return default;

            int pivot = constructibleIdx;
            int directionIndex = GridManager.Instance.glaciersTiles[glacierIdx].GlacierDirectionTypeToIndex();

            pivot += directionIndex;

            return pivot;
        }

        public void FailAction()
        {

        }

        public void GemAction()
        {
            GemAnimation gemAnimation = GameManager.Instance.objectPoolManager.GetObject<GemAnimation>("PlusLifeSton");
            gemAnimation.transform.SetParent(UIManager.GetDynamicCanvasTransform());
            gemAnimation.transform.localScale = Vector3.one;
            gemAnimation.transform.position = transform.localPosition;
            gemAnimation.SetGemAnimation(gemValue);
            GameManager.Instance.ChangeGem(gemValue);
        }

        public override void SetCharacterInfo()
        {
            CharacterAction = null;
            isAttackType = false;
            attackSpeed = characterData.attackSpeed;
            gemValue = (int)characterData.characterUniqueValue[1];
            bombDamage = characterData.characterUniqueValue[2];
            explosionTime = characterData.characterUniqueValue[3];
            bombStrikeRange = Mathf.Pow(characterData.characterUniqueValue[4], 2);
               
            StartCoroutine(ActionSequence());

            bool isAppear = starGradeIndex == 0;

            AppearAction("Idle", isAppear);
        }

        public override void SynthesisCharacter()
        {
            
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            
        }

    }
}