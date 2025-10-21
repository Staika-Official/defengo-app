using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;
using Framework.Sound;
using UniRx;
using UniRx.Triggers;
using Framework.GameData.Defense;

namespace Framework.Game.Defense
{
    public class LegendIceProjectile : Projectile
    {
        public float debuffSpeedRate;
        public float debuffBoardTime;
        public float debuffBoardSize;

        private void Start()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => isMove)
                .Subscribe(_ => TrackingTarget(0.3f));
        }

        public void Initialize(Character character, Monster target, bool isCriticalHit, float moveSpeed, string effectName, float debuffSpeedRate, float debuffBoardTime, float debuffBoardSize)
        {
            this.character = character;
            this.effectName = effectName;
            targetMonster = target;
            this.moveSpeed = moveSpeed;
            this.isCriticalHit = isCriticalHit;
            isSplashHit = false;
            this.debuffSpeedRate = debuffSpeedRate;
            this.debuffBoardTime = debuffBoardTime;
            this.debuffBoardSize = debuffBoardSize;
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
            throw new System.NotImplementedException();
        }

        public override void HitMonster()
        {
            DamageType damageType = isCriticalHit ? DamageType.CRITICAL : DamageType.NORMAL;

            targetMonster.HitDamage(attackValue, damageType, criticalDamageRate, character);
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
            particle.PlayParticle(transform.localPosition, 0);
            SoundManager.Instance.PlaySound(SoundKey.SF_ICICLE_HIT_BURST);

            IceGround iceGround = GameManager.Instance.objectPoolManager.GetObject<IceGround>("IceGround");
            GameManager.Instance.renderSortManager.AddDebuffSortLayer(iceGround);
            iceGround.Initialize(targetMonster.prevPositionIdx, debuffBoardTime, debuffSpeedRate, debuffBoardSize);
            iceGround.transform.position = transform.position;
            ReturnObjectPool();
        }

        public override void MissedTarget()
        {
            if (character.targetedMonster.Count == 0)
            {
                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                particle.PlayParticle(transform.localPosition, 0);
                SoundManager.Instance.PlaySound(SoundKey.SF_ICICLE_HIT_BURST);
                ReturnObjectPool();
            }
            else
            {
                if (!character.gameObject.activeSelf)
                {
                    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                    particle.PlayParticle(transform.localPosition, 0);
                    SoundManager.Instance.PlaySound(SoundKey.SF_ICICLE_HIT_BURST);
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