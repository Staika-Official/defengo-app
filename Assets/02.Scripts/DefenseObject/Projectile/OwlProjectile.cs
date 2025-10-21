using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Sound;
using UniRx;
using UniRx.Triggers;
using Framework.GameData.Defense;
using Framework.Util;

namespace Framework.Game.Defense
{
    public class OwlProjectile : Projectile
    {
        public DamageType damageType;
        public Dictionary<Monster, Coroutine> dic_HitMonsters = new();
        public float addtiveDamage;
        public float stormRange;
        public IEnumerator fireAction;
        public void SetRotationProjectile()
        {
            Vector3 targetPos = targetMonster.transform.position;
            Vector3 moveDir = targetPos - transform.position;

            moveDir.Normalize();
            float rotationZ = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(rotationZ - 90.0f, Vector3.forward);
        }
        
        public void Initialize(Character character, Monster target, bool isCriticalHit, float moveSpeed, float strikeRange)
        {
            this.character = character;
            targetMonster = target;
            this.moveSpeed = moveSpeed;
            this.isCriticalHit = isCriticalHit;
            this.strikeRange = strikeRange;
            isSplashHit = strikeRange != 0;
            stormRange = transform.localScale.magnitude;
            
            damageType = isCriticalHit ? DamageType.CRITICAL : DamageType.NORMAL;
            
            SetRotationProjectile();
            
            if (null != fireAction)
            {
                StopCoroutine(fireAction);
                fireAction = null;
            }
            
            fireAction = FireTarget();
            StartCoroutine(fireAction);
        }

        public IEnumerator FireTarget()
        {
            //StartCoroutine(WaitForDuration());
            StartCoroutine(ValidataMapBounds());
            
            while (true)
            {
                transform.Translate(moveSpeed * Time.deltaTime * Vector2.up);

                for (int i = 0; i < GameManager.Instance.monsterSpawner.monsters.Count; i++)
                {
                    Monster monster = GameManager.Instance.monsterSpawner.monsters[i];
                    
                    Vector2 pos = transform.position;
                    Vector2 monPos = monster.transform.position;
                    
                    float dis = Vector2.Distance(monPos, pos);

                    if (monster.IsAlive && dis < stormRange)
                    {
                        if (!dic_HitMonsters.ContainsKey(monster))
                        {
                            Coroutine coroutine = StartCoroutine(monster.OwlrusStormSequence(attackValue, addtiveDamage,
                                damageType, criticalDamageRate, character));
                            dic_HitMonsters.Add(monster, coroutine);
                        }
                    }
                    else
                    {
                        if (dic_HitMonsters.ContainsKey(monster))
                        {
                            StopCoroutine(dic_HitMonsters[monster]);
                            dic_HitMonsters.Remove(monster);
                        }
                    }
                }
                
                yield return new WaitForFixedUpdate();
            }
        }

        public IEnumerator ValidataMapBounds()
        {
            Vector2 firstCellPosition = GridManager.Instance.cellDatas[0].cellPosition;
            Vector2 lastCellPosition = GridManager.Instance.cellDatas[^1].cellPosition;

            firstCellPosition.x -= 2;
            firstCellPosition.y += 4;
            
            lastCellPosition.x += 2;
            lastCellPosition.y -= 4;

            while (true)
            {
                if (firstCellPosition.x > transform.position.x || lastCellPosition.x < transform.position.x ||
                    lastCellPosition.y > transform.position.y || firstCellPosition.y < transform.position.y)
                {
                    if (null != fireAction)
                    {
                        StopCoroutine(fireAction);
                        fireAction = null;
                    }
                    
                    foreach (var monster in dic_HitMonsters)
                    {
                        StopCoroutine(monster.Value);
                    }
                    dic_HitMonsters.Clear();
                    
                    ReturnObjectPool();
                    
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
        }
        

        public override void CloseTarget()
        {
        }

        public override void FireProjectile()
        {
        }

        public override void HitMonster()
        {
        }

        public override void MissedTarget()
        {
        }
    }
}
