using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

namespace Framework.Game.Defense
{
    public class CharacterKorra : Character
    {
        public int waveCount;

        public override IEnumerator ActionSequence()
        {
            yield return null;
        }

        public override void SetCharacterInfo()
        {
            waveCount = 5;

            bool isAppear = starGradeIndex == 0;

            AppearAction($"Idle_{waveCount}", isAppear);

            GameManager.onCompleteWave += WaveAction;
        }

        public IEnumerator RestorationTile()
        {
            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Change2Ice", false);
            yield return new WaitForSpineEvent(anim.AnimationState, "Change");

            Glacier glacier = GridManager.Instance.glaciersTiles[glacierIdx];

            glacier.isImpossibleSummon = false;
            glacier.glacierState = GameData.Defense.GlacierState.NORMAL;
            glacier.anim.AnimationState.SetAnimation(0, "Normal", true);
            
            GameManager.onCompleteWave -= WaveAction;
            
            yield return new WaitForSpineAnimationComplete(entry);
            UIManager.Instance.SetPossibleSummon(GridManager.Instance.IsPossibleSummon() && GameManager.Instance.Gem >= 10);
            GameManager.Instance.objectPoolManager.ReturnObject(this, characterName);
        }

        public void WaveAction()
        {
            waveCount--;

            if(waveCount == -1)
            {
                if (GameManager.Instance.gameState == GameState.GAME_OVER) return;
                StartCoroutine(RestorationTile());
            }
            else
            {
                anim.AnimationState.SetAnimation(0, $"Idle_{waveCount}", true);
                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("KorraUp");
                particle.transform.position = transform.position;
                particle.SimplePlay();
            }
        }

        public override void SynthesisCharacter()
        {
            
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            
        }
    }
}