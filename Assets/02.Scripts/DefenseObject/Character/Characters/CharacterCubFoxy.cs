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
    public class CharacterCubFoxy : Character
    {
        public override void SetCharacterInfo()
        {
            characterIndex = characterData.characterIndex;
            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);
            CharacterAction = null;

            isAttackType = false;
            buffValue = characterData.buffValue + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0];
            buffCycle = characterData.buffCycle;
            buffRange = characterData.buffRange;
            buffDuration = characterData.buffDuration;
            projectileName = characterData.projectileName;
            upgradeFactor = characterData.powerFactor[0];
            SetBuffValue();

            gameObject.name = $"{characterData.characterName}_{glacierIdx}";

            CharacterAction = () => BuffCharacter();
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

        public void BuffCharacter()
        {
            if (GameManager.Instance.gameState == GameState.PLAY) 
                StartCoroutine(ActionSequence());
        }

        public override IEnumerator ActionSequence()
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CHARACTER_FOXY);
            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Attack_FrontSide", false);
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("DoBuff");
            particle.PlayParticle(transform.localPosition, 0);
            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");
            GameManager.Instance.buffManager.AttackDamageBuff(buffRange, totalBuffValue, buffDuration, constructibleIdx);
            yield return new WaitForSpineAnimationComplete(entry);

            anim.AnimationState.SetAnimation(0, "Idle", true);
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;
            SetBuffValue();
        }

        public override void SynthesisCharacter()
        {
            
        }
    }
}
