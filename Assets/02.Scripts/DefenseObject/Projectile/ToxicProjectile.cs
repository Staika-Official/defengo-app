using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Sound;
using Framework.Util;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Framework.Game.Defense
{
    public class ToxicProjectile : Projectile
    {
        public float toxicDuration;
        public float toxicBoardSize;

        void Start()
        {
            //베지어 커브로 몬스터 추격함수
            this.FixedUpdateAsObservable()
                .Where(_ => isMove)
                .Subscribe(_ => TrackingTargetBezierCurves(false));
        }

        public void Initialize(Character character, Monster target, float criticalRange, float moveSpeed, string effectName, float toxicDuration, float toxicBoardSize)
        {
            this.character = character;
            this.effectName = effectName;
            targetMonster = target;
            this.moveSpeed = moveSpeed;
            this.criticalRange = criticalRange;
            isSplashHit = false;
            this.toxicDuration = toxicDuration;
            this.toxicBoardSize = toxicBoardSize;
        }

        public override void CloseTarget()
        {
            if (targetMonster.IsAlive)
            {
                HitMonster();
            }
            else
            {
                ReturnObjectPool();
            }
            isMove = false;
            RemoveMonster();
        }

        public override void FireProjectile()
        {
        }

        public override void HitMonster()
        {
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
            particle.PlayParticle(transform.localPosition, 0);
            SoundManager.Instance.PlaySound(SoundKey.SF_GLASS_BROKEN_2);

            //독 장판을 얻어와서 생성 및 초기화
            ToxicGround toxicGround = GameManager.Instance.objectPoolManager.GetObject<ToxicGround>("ToxicGround");
            toxicGround.Initialize(targetMonster.prevPositionIdx, attackValue, toxicDuration, toxicBoardSize, criticalRange, criticalDamageRate);
            toxicGround.transform.position = transform.position;

            //해당 장판이 최신이면 제일 위로 올라와야하는 처리를 하기위해 만든 함수
            GameManager.Instance.renderSortManager.AddDebuffSortLayer(toxicGround);

            ReturnObjectPool();
        }

        public override void MissedTarget()
        {
            if (character.targetedMonster.Count == 0)
            {
                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                particle.PlayParticle(transform.localPosition, 0);
                SoundManager.Instance.PlaySound(SoundKey.SF_GLASS_BROKEN_2);
                ReturnObjectPool();
            }
            else
            {
                if (!character.gameObject.activeSelf)
                {
                    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                    particle.PlayParticle(transform.localPosition, 0);
                    SoundManager.Instance.PlaySound(SoundKey.SF_GLASS_BROKEN_2);
                    ReturnObjectPool();
                }
                else
                {
                    targetMonster = character.targetedMonster[0];
                }
            }
        }

     

    }
}
