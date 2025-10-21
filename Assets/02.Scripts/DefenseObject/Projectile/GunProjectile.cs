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
    public class GunProjectile : Projectile
    {
        public bool isGambling;
        private void Start()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => isMove)
                .Subscribe(_ => TrackingTarget(0.3f));
        }

        public void Initialize(Character character, Monster target, bool isGamblingHit, float moveSpeed, float strikeRange, string effectName)
        {
            this.character = character;
            this.effectName = effectName;
            targetMonster = target;
            this.moveSpeed = moveSpeed;
            this.strikeRange = strikeRange;
            isSplashHit = strikeRange != 0;
            isGambling = isGamblingHit;
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
            DamageType damageType = !isGambling ? DamageType.NORMAL : DamageType.GAMBLING_KILL;

            targetMonster.HitDamage(attackValue, damageType, criticalDamageRate, character);
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
            particle.PlayParticle(transform.localPosition, 0);
            SoundManager.Instance.PlaySound(SoundKey.SF_COIN_HIT);
            ReturnObjectPool();
        }

        public override void MissedTarget()
        {
            if (character.targetedMonster.Count == 0)
            {
                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                particle.PlayParticle(transform.localPosition, 0);
                SoundManager.Instance.PlaySound(SoundKey.SF_COIN_HIT);
                ReturnObjectPool();
            }
            else
            {
                if (!character.gameObject.activeSelf)
                {
                    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                    particle.PlayParticle(transform.localPosition, 0);
                    SoundManager.Instance.PlaySound(SoundKey.SF_COIN_HIT);
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
