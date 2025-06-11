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
    public class SlowProjectile : Projectile
    {
        private void Start()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => isMove)
                .Subscribe(_ => TrackingTarget());
        }

        public void Initialize(Character character, Monster target, bool isCriticalHit, float moveSpeed, float strikeRange, string effectName, float buffValue, float buffDuration)
        {
            this.character = character;
            this.effectName = effectName;
            targetMonster = target;
            this.moveSpeed = moveSpeed;
            this.isCriticalHit = isCriticalHit;
            this.strikeRange = strikeRange;
            isSplashHit = strikeRange != 0;
            this.buffValue = buffValue;
            this.buffDuration = buffDuration;

            SetProjectileParticle();
        }

        public float timeCount;

        public ParticleSystem[] particles;

        public override void CloseTarget()
        {
            StartCoroutine(SlowDownSequence());
        }

        public override void FireProjectile()
        {

        }

        public override void HitMonster()
        {
            DamageType damageType = isCriticalHit ? DamageType.CRITICAL : DamageType.NORMAL;

            targetMonster.HitDamage(attackValue, damageType, criticalDamageRate);
            SoundKey soundKey = objectName == "DarkSlowProjectile" ? SoundKey.SF_CHARACTER_DARKRAZY : SoundKey.SF_CHARACTER_RAZY;
            SoundManager.Instance.PlaySound(soundKey);
        }

        public override void SetProjectileParticle()
        {
            base.SetProjectileParticle();
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Play();
            }
        }

        public void InactiveParticle()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Stop();
            }
        }

        public override void MissedTarget()
        {
            if (character.targetedMonster.Count == 0)
            {
                transform.SetParent(null);
                ReturnObjectPool();
                timeCount = 0.0f;
            }
            else
            {
                if (!character.gameObject.activeSelf)
                {
                    transform.SetParent(null);
                    ReturnObjectPool();
                    timeCount = 0.0f;
                }
                else
                {
                    targetMonster = character.targetedMonster[0];
                }
            }
        }

        public void EndOfUse()
        {

        }

        public IEnumerator SlowDownSequence()
        {
            isMove = false;
            transform.SetParent(targetMonster.transform);
            transform.localPosition = Vector2.zero;
            HitMonster();
            //targetMonster.SlowDownSequence(true, buffValue, this);
            targetMonster.SlowDownSequence(buffValue, buffDuration, this);

            yield return null;
            //while (true)
            //{
            //    timeCount += Time.fixedDeltaTime;

            //    if (!targetMonster.IsAlive)
            //    {
            //        transform.SetParent(null);
            //        InactiveParticle();
            //        //ReturnObjectPool();
            //        timeCount = 0.0f;
            //    }

            //    if (timeCount >= buffDuration)
            //    {
            //        //targetMonster.SlowDownSequence(false, buffValue, this);
            //        transform.SetParent(null);
            //        InactiveParticle();
            //        //ReturnObjectPool();
            //        timeCount = 0.0f;
            //        break;
            //    }
            //}
        }

    }
}

