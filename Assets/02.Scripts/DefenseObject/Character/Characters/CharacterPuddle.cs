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
    public class CharacterPuddle : Character
    {
        public float getGemTimeCount;
        public float getGemRate;
        public float getGemAmount;
        public int getBasicGemAmount;

        public override IEnumerator ActionSequence()
        {
            Debug.Log("Puddle Action Seq");
            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);

            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

            int rate = (int)(getGemRate * 100);

            bool isAddGem = Random.Range(0, 100) <= rate;

            int addGem = isAddGem ? (int)getGemAmount : 0;

            SoundKey soundKey = isAddGem ? SoundKey.SF_PUDDLE_CRITICAL : SoundKey.SF_PUDDLE_CRITICAL;
            Debug.Log("GemAnimation : " + transform.name);
            GemAnimation gemAnimation = GameManager.Instance.objectPoolManager.GetObject<GemAnimation>("PlusLifeSton");
            gemAnimation.transform.SetParent(UIManager.GetDynamicCanvasTransform());
            gemAnimation.transform.localScale = Vector3.one;
            gemAnimation.transform.position = transform.localPosition;
            gemAnimation.SetGemAnimation(getBasicGemAmount + addGem);

            GameManager.Instance.ChangeGem(getBasicGemAmount + addGem);
            SoundManager.Instance.PlaySound(soundKey);
            yield return new WaitForSpineAnimationComplete(entry);
            Debug.Log(GetDuration());
            anim.AnimationState.SetAnimation(0, "Idle", true);
        }

        public override void SetCharacterInfo()
        {
            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
            }

            CharacterAction = null;
            getGemTimeCount = characterData.characterUniqueValue[0]
                - (characterData.characterClassLevel - 1)
                * characterData.classUpFactor[0];

            buffCycle = 1 / getGemTimeCount;

            getGemRate = characterData.characterUniqueValue[1] * Mathf.Pow(characterData.starFactor[0], starGradeIndex);

            getGemAmount = characterData.characterUniqueValue[2] * Mathf.Pow(characterData.starFactor[1], starGradeIndex);

            getBasicGemAmount = Mathf.FloorToInt(characterData.characterUniqueValue[3] * Mathf.Pow(characterData.starFactor[2], starGradeIndex));


            CharacterAction = () =>
            {
                if(isSummoned)
                {
                    
                    StartCoroutine(ActionSequence());
                }
            };

            isSummoned = true;
            actionCoroutine = WaitActionSequence();
            StartCoroutine(actionCoroutine);

            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

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