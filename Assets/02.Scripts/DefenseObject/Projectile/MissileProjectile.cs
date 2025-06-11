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
    public class MissileProjectile : Projectile
    {
        private void Start()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => isMove)
                .Subscribe(_ => TrackingTarget());
        }

        public void Initialize(Character character, Monster target, bool isCriticalHit, float moveSpeed, float strikeRange, string effectName)
        {
            this.character = character;
            this.effectName = effectName;
            targetMonster = target;
            this.moveSpeed = moveSpeed;
            this.isCriticalHit = isCriticalHit;
            this.strikeRange = strikeRange;
            isSplashHit = strikeRange != 0;
            //Debug.Log(isCriticalHit);
        }

        public override void CloseTarget()
        {
            //Debug.Log("Close Target");
            List<Monster> targetCache = GameManager.Instance.monsterSpawner.monsters;

            for (int i = 0; i < targetCache.Count; i++)
            {
                //Debug.Log("In list : " + targetCache[i].transform.name);
                Monster monster = targetCache[i];

                Vector2 tarMonsterVector = Calculator.TransformationVector(targetMonster.prevPositionIdx);
                Vector2 monsterVector = Calculator.TransformationVector(monster.prevPositionIdx);

                if (Calculator.DistanceCheck(tarMonsterVector - monsterVector) < strikeRange)
                {
                    if (monster.IsAlive)
                    {
                        DamageType damageType = isCriticalHit ? DamageType.CRITICAL : DamageType.NORMAL;

                        monster.HitDamage(attackValue, damageType, criticalDamageRate);
                        //Debug.Log(monster.transform.name);
                        ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                        particle.PlayParticle(transform.localPosition, 0);
                        SoundManager.Instance.PlaySound(SoundKey.SF_CHARACTER_FORTIS);
                    }
                }
            }
            RemoveMonster();
            ReturnObjectPool();
        }

        public override void FireProjectile()
        {

        }

        public override void HitMonster()
        {
            
        }

        public override void MissedTarget()
        {
            if (character.targetedMonster.Count == 0)
            {
                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                particle.PlayParticle(transform.localPosition, 0);
                SoundManager.Instance.PlaySound(SoundKey.SF_CHARACTER_FORTIS);
                ReturnObjectPool();
            }
            else
            {
                targetMonster = character.targetedMonster[0];
            }

            //ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
            //particle.PlayParticle(transform.localPosition, 0);
            //SoundManager.Instance.PlaySound(SoundKey.SF_CHARACTER_FORTIS);
            //ReturnObjectPool();
        }
    }
}
