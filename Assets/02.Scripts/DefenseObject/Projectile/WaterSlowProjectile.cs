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
    public class WaterSlowProjectile : Projectile
    {
        public float slowDownValue;
        public float slowDownDuration;

        private void Start()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => isMove)
                .Subscribe(_ => TrackingTarget());
        }

        public ObjectParticle particle;

        public void Initialize(Character character, Monster target, bool isCriticalHit, float moveSpeed, float slowDownValue, float duration, string effectName)
        {
            this.character = character;
            this.effectName = effectName;
            this.slowDownValue = slowDownValue;
            this.slowDownDuration = duration;
            targetMonster = target;
            this.moveSpeed = moveSpeed;
            this.isCriticalHit = isCriticalHit;
            isSplashHit = strikeRange != 0;
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

            targetMonster.HitDamage(attackValue, damageType, criticalDamageRate, character);
            targetMonster.SlowDownSequence(slowDownValue, slowDownDuration);
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
            particle.PlayParticle(transform.localPosition, 0);
            SoundManager.Instance.PlaySound(SoundKey.SF_CHARACTER_SHELLFISH);
            ReturnObjectPool();
        }

        public override void MissedTarget()
        {
            if (character.targetedMonster.Count == 0)
            {
                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                particle.PlayParticle(transform.localPosition, 0);
                SoundManager.Instance.PlaySound(SoundKey.SF_CHARACTER_SHELLFISH);
                ReturnObjectPool();
            }
            else
            {
                targetMonster = character.targetedMonster[0];
            }
        }
    }
}
