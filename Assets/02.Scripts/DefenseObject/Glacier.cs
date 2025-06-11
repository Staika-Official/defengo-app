using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Framework.GameData.Defense;
using Framework.Util;
using Framework.Sound;
using DG.Tweening;

namespace Framework.Game.Defense
{
    public class Glacier : MonoBehaviour
    {
        public SkeletonAnimation anim;
        public int glacierIndex;
        public Vector2 centerPivot;
        public GlacierState glacierState;
        public bool isImpossibleSummon = false;
        public int orderLayer = 2;
        public MeshRenderer meshRenderer;
        public GlacierSummonDirectionType glacierDirType;

        public List<int> boundaryIndexes = new();

        public void Initialize()
        {
            glacierState = GlacierState.NORMAL;
            meshRenderer.sortingOrder = 2;
        }

        public void SummonCharacter()
        {
            isImpossibleSummon = true;
        }

        public void FocusTile(bool isFocus)
        {
            if (isFocus)
            {

            }
            else
            {

            }
        }

        public IEnumerator AppearAction()
        {
            string animKey = glacierState switch
            {
                GlacierState.NORMAL => "Normal",
                GlacierState.BROKEN => "Damaged",
                GlacierState.DESTROYED => "",
                _=> ""
            };

            TrackEntry entry = anim.AnimationState.SetAnimation(0, $"Appear_{animKey}", false);

            yield return new WaitForSpineAnimationComplete(entry);

            anim.AnimationState.SetAnimation(0, animKey, true);
        }

        public bool Shield()
        {
            return GameManager.Instance.characterSpawner.ShieldTile(glacierIndex);
        }

        public void Damage()
        {
            switch (glacierState)
            {
                case GlacierState.NORMAL:
                    SoundManager.Instance.PlaySound(SoundKey.SF_GLACIER_DAMAGED);
                    anim.AnimationState.SetAnimation(0, "Damaged", true);
                    gameObject.transform.DOScale(1f, 0f).SetAutoKill(true);
                    gameObject.transform.DOScale(1.4f, 0.12f).SetEase(Ease.OutQuart).SetAutoKill(true).SetLoops(2, LoopType.Yoyo);
                    break;
                case GlacierState.BROKEN:
                    SoundManager.Instance.PlaySound(SoundKey.SF_GLACIER_BROKEN);
                    DestroyedTile();

                    ObjectParticle particle1 = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("FallingWater");
                    Vector2 pos = new(transform.localPosition.x, transform.localPosition.y + 0.4f);
                    particle1.PlayParticle(pos, 0);

                    anim.AnimationState.SetAnimation(0, "Broken", true);
                    break;
                default:
                    break;
            }
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("IceBreak");
            particle.PlayParticle(transform.localPosition, 0);
            glacierState++;
        }

        public void DestroyedTile()
        {
            isImpossibleSummon = true;

            GameManager.Instance.characterSpawner.DestroyedTile(glacierIndex);
            GameManager.Instance.characterSpawner.SummonSubCharacter(this);
        }

        public int GlacierDirectionTypeToIndex()
        {
            int directionIndex = 0;

            switch (glacierDirType)
            {
                case GlacierSummonDirectionType.NONE:
                    return default;
                case GlacierSummonDirectionType.NEAR_LEFT:
                    directionIndex -= 3;
                    break;
                case GlacierSummonDirectionType.NEAR_RIGHT:
                    directionIndex += 3;
                    break;
                case GlacierSummonDirectionType.NEAR_UP:
                    directionIndex -= 60;
                    break;
                case GlacierSummonDirectionType.NEAR_DOWN:
                    directionIndex += 60;
                    break;
                case GlacierSummonDirectionType.FAR_LEFT:
                    directionIndex -= 6;
                    break;
                case GlacierSummonDirectionType.FAR_RIGHT:
                    directionIndex += 6;
                    break;
                case GlacierSummonDirectionType.FAR_UP:
                    directionIndex -= 120;
                    break;
                case GlacierSummonDirectionType.FAR_DOWN:
                    directionIndex += 120;
                    break;
            }

            return directionIndex;
        }
    }
}