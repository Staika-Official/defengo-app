using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Sound;
using UniRx;
using UniRx.Triggers;
using Framework.GameData.Defense;

namespace Framework.Game.Defense
{
    public class BlackOrbProjectile : Projectile
    {
        public float additiveDamage;

        private void Start()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => isMove)
                .Subscribe(_ => TrackingTarget());
        }

        public void Initialize(Character character, Monster target, bool isCriticalHit, float moveSpeed, float strikeRange, string effectName, float additiveDamage)
        {
            this.character = character;
            this.effectName = effectName;
            targetMonster = target;
            this.moveSpeed = moveSpeed;
            this.isCriticalHit = isCriticalHit;
            this.strikeRange = strikeRange;
            this.additiveDamage = additiveDamage;
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
            throw new System.NotImplementedException();
        }

        public override void HitMonster()
        {
            //targetMonster.HitDamage(attackValue, isCriticalHit, criticalDamageRate);
            DamageType damageType = isCriticalHit ? DamageType.CRITICAL : DamageType.NORMAL;

            targetMonster.BlackOrbStackSequence(attackValue, damageType, criticalDamageRate, additiveDamage, character);

            string blackEffectName = targetMonster.blackOrbCount >= 4 ? "BlackHit2" : "BlackHit1";
            SoundKey soundKey = targetMonster.blackOrbCount >= 4 ? SoundKey.SF_DARK_CRITICAL : SoundKey.SF_DARK_HIT;
            SoundManager.Instance.PlaySound(soundKey);
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(blackEffectName);
            particle.PlayParticle(transform.localPosition, 0);
            ReturnObjectPool();
        }

        public override void MissedTarget()
        {
            if (character.targetedMonster.Count == 0)
            {
                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                SoundManager.Instance.PlaySound(SoundKey.SF_DARK_HIT);
                particle.PlayParticle(transform.localPosition, 0);
                ReturnObjectPool();
            }
            else
            {
                if (!character.gameObject.activeSelf)
                {
                    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                    SoundManager.Instance.PlaySound(SoundKey.SF_DARK_HIT);
                    particle.PlayParticle(transform.localPosition, 0);
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