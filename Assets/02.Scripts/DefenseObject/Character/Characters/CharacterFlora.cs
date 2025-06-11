using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using Framework.GameData.Defense;
using Framework.Util;
using Framework.Sound;
using UniRx.Triggers;
using UniRx;
using Spine.Unity;
using Spine;

namespace Framework.Game.Defense
{
    public class CharacterFlora : Character
    {
        public float naturalHealingCoolTime;
        public ObscuredFloat gemValue;

        public override IEnumerator ActionSequence()
        {
            while (isSummoned)
            {
                TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);

                yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

                SoundManager.Instance.PlaySound(SoundKey.SF_FLORA_SPELL);
                
                NaturalHealingAction();

                yield return new WaitForSpineAnimationComplete(entry);
                entry.TimeScale = 1;
                //애니메이션이 끝난다면 아이들 상태
                anim.AnimationState.SetAnimation(0, "Idle", true);
                
                yield return new WaitForSeconds(naturalHealingCoolTime);
            }
        }
        
        public override void SetCharacterInfo()
        {
            if (null != actionCoroutine)
            {
                StopCoroutine(actionCoroutine);
                actionCoroutine = null;
            }
            
            float starGradeValue = starGradeIndex == 0 ? 0f : 1f / starGradeIndex;

            gemValue = characterData.characterUniqueValue[1];
            
            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);
            isAttackType = false;

            naturalHealingCoolTime = ((characterData.characterUniqueValue[0]
                                       - (characterData.characterClassLevel - 1)
                                       * characterData.classUpFactor[0])
                                      - (upgradeIndex - 1) * characterData.powerFactor[0])
                                    * starGradeValue * characterData.starFactor[0];

            
            //todo Test
            //Debug.Log($"CheckFloraCoolTime : {naturalHealingCoolTime}");
            //naturalHealingCoolTime = 10f;
            
            maxStarGradeValue = characterData.maxStarGradeValue;

            int idx = GridManager.Instance.GetBorkenGlacierIdx();
            particleName = idx == -1 ? "ShieldBubbleBurst" : "TileHeal";
            
            isSummoned = true;

            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }
            
            SoundManager.Instance.PlaySound(SoundKey.SF_FLORA_APEAR);
            
            GemAction();
            
            bool isAppear = starGradeIndex == 0;

            AppearAction(HealingAction, isAppear);
        }

        public void HealingAction()
        {
            anim.AnimationState.SetAnimation(0, "Idle", true);

            if (starGradeIndex >= 4)
            {
                actionCoroutine = ActionSequence();
                StartCoroutine(actionCoroutine);
            }
        }
        
        public void GemAction()
        {
            GemAnimation gemAnimation = GameManager.Instance.objectPoolManager.GetObject<GemAnimation>("PlusLifeSton");
            gemAnimation.transform.SetParent(UIManager.GetDynamicCanvasTransform());
            gemAnimation.transform.localScale = Vector3.one;
            gemAnimation.transform.position = transform.localPosition;
            gemAnimation.SetGemAnimation((int)gemValue);
            GameManager.Instance.ChangeGem((int)gemValue);
        }

        public void NaturalHealingAction()
        {
            int idx = GridManager.Instance.GetBorkenGlacierIdx();

            particleName = idx == -1 ? "ShieldBubbleBurst" : "TileHeal";

            if (idx != -1)
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_TILE_HEAL);
                
                Glacier glacier = GridManager.Instance.glaciersTiles[idx];
                
                glacier.glacierState = GlacierState.NORMAL;
                glacier.anim.AnimationState.SetAnimation(0, "Appear_Normal", false);

                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(particleName);
                particle.PlayParticle(glacier.transform.localPosition, 0);
            }
            else
            {
                CharacterSpawner spawner = GameManager.Instance.characterSpawner;
                Character character = spawner.GetNonShieldRandomCharacter();

                if (character != null)
                {
                    character.ShieldSequence();
                }
            }
        }

        public override void SynthesisCharacter()
        {
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;
            
            float starGradeValue = starGradeIndex == 0 ? 0f : 1f / starGradeIndex;
            
            naturalHealingCoolTime = ((characterData.characterUniqueValue[0]
                                       - (characterData.characterClassLevel - 1)
                                       * characterData.classUpFactor[0])
                                      - (upgradeIndex - 1) * characterData.powerFactor[0])
                                    * starGradeValue * characterData.starFactor[0];

            //todo Test
            //Debug.Log($"CheckFloraCoolTime : {naturalHealingCoolTime}");

            //naturalHealingCoolTime = 10f;
        }
    }
}
