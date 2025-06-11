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
    public class HighnickelProjectile : Projectile
    {
        public float stunDuration;

        private void Start()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => isMove)
                .Subscribe(_ => TrackingTarget());
        }

        public void Initialize(Character character, Monster target, bool isCriticalHit, float moveSpeed, string effectName, float secValue)
        {
            this.character = character;
            this.effectName = effectName;
            targetMonster = target;
            this.moveSpeed = moveSpeed;
            this.isCriticalHit = isCriticalHit;
            isSplashHit = strikeRange != 0;
            stunDuration = secValue;
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

            GameManager.Instance.buffManager.HighnickelStun(targetMonster, stunDuration);
            targetMonster.HitDamage(attackValue, damageType, criticalDamageRate);
            SoundManager.Instance.PlaySound(SoundKey.SF_ELECTRONIC_BASIC2);
            ReturnObjectPool();
        }

        public override void MissedTarget()
        {
            if (character.targetedMonster.Count == 0)
            {
                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                particle.PlayParticle(transform.localPosition, 0);
                SoundManager.Instance.PlaySound(SoundKey.SF_ELECTRONIC_BASIC2);
                ReturnObjectPool();
            }
            else
            {
                if (!character.gameObject.activeSelf)
                {
                    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                    particle.PlayParticle(transform.localPosition, 0);
                    SoundManager.Instance.PlaySound(SoundKey.SF_ELECTRONIC_BASIC2);
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
