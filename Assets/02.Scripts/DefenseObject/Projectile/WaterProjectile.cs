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
    public class WaterProjectile : Projectile
    {
        private void Start()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => isMove)
                .Subscribe(_ => TrackingTarget());
        }

        public ObjectParticle particle;
        public SoundKey soundKey;

        public void Initialize(Character character, Monster target, bool isCriticalHit, float moveSpeed, float strikeRange, string effectName, SoundKey soundKey)
        {
            this.character = character;
            this.effectName = effectName;
            targetMonster = target;
            this.moveSpeed = moveSpeed;
            this.isCriticalHit = isCriticalHit;
            this.strikeRange = strikeRange;
            isSplashHit = strikeRange != 0;
            this.soundKey = soundKey;
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
            particle.SimplePlay();
        }

        public override void HitMonster()
        {
            DamageType damageType = isCriticalHit ? DamageType.CRITICAL : DamageType.NORMAL;

            targetMonster.HitDamage(attackValue, damageType, criticalDamageRate);
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
            particle.PlayParticle(transform.localPosition, 0);
            SoundManager.Instance.PlaySound(soundKey);
            ReturnObjectPool();
        }

        public override void MissedTarget()
        {
            if (character.targetedMonster.Count == 0)
            {
                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                particle.PlayParticle(transform.localPosition, 0);
                SoundManager.Instance.PlaySound(soundKey);
                ReturnObjectPool();
            }
            else
            {
                targetMonster = character.targetedMonster[0];
            }

            //ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
            //particle.PlayParticle(transform.localPosition, 0);
            //SoundManager.Instance.PlaySound(SoundKey.SF_CHARACTER_SHELLFISH);
            //ReturnObjectPool();
        }
    }
}
