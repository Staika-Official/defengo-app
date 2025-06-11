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
    public class BoomerangProjectile : Projectile
    {
        
        //turnEnd -> 던지는 모션을 유지를해야하기때문에 체크용으로 들고있음
        public bool isTurnEnd;
        public Dictionary<Monster, Coroutine> dic_HitMonsters = new();
        public IEnumerator boomerangSequence;
        
        public void Initialize(Character character, Monster target, float moveSpeed, string effectName)
        {
            isTurnEnd = false;
            this.character = character;
            this.effectName = effectName;
            targetMonster = target;
            this.moveSpeed = moveSpeed;
            isSplashHit = strikeRange != 0;

            if (null != boomerangSequence)
            {
                StopCoroutine(boomerangSequence);
                boomerangSequence = null;
            }

            boomerangSequence = BoomerangPathWithTimeControl(targetMonster.transform.position);
            StartCoroutine(boomerangSequence);
        }


        public override void HitMonster()
        {
        }

        public override void CloseTarget()
        {
        }

        public override void MissedTarget()
        {
           
        }

        public void SpinSuccess()
        {
            isMove = false;
            isTurnEnd = true;
            
            foreach (var monster in dic_HitMonsters)
            {
                StopCoroutine(monster.Value);
            }
            dic_HitMonsters.Clear();
            
            if (null != boomerangSequence)
            {
                StopCoroutine(boomerangSequence);
                boomerangSequence = null;
            }

            ReturnObjectPool();
            RemoveMonster();
        }
        
        public override void FireProjectile()
        {
            throw new System.NotImplementedException();
        }

        
        public IEnumerator BoomerangPathWithTimeControl(Vector3 targetMonsterPosition)
        {
            Vector3 playerPos = transform.position;
            Vector3 playerToMonsterDir = targetMonsterPosition - playerPos;
            Vector3 monsterToPlayerDir = playerPos - targetMonsterPosition;
            
            float distance = Vector3.Distance(playerPos, targetMonsterPosition);
            float ratioDistance = distance * 0.8f;
            playerToMonsterDir.Normalize();
            monsterToPlayerDir.Normalize();
            
            //float attackSpeed = 2f; // 임시 공격 속도
            //float uniqueValue = 2f; // 임시 유니크 벨류
            
            float totalTime = distance / moveSpeed;
            float throwTime = totalTime * 0.6f;
            float returnTime = totalTime * 0.4f;
            float elapsedTime = 0f; // 경과 시간

            Debug.Log($"거리 Distance : {distance}");
            Debug.Log($"몬스터에게 가는 시간 : {throwTime}");
            Debug.Log($"돌아올때 걸리는 시간 : {returnTime}");
            
            float upPivotAngle = 30f;
            float downPivotAngle = 340f;
            
            bool isThrowPhase = true;
            bool isEnd = false;
            
            Quaternion upRotation = Quaternion.AngleAxis(upPivotAngle, Vector3.forward);
            Quaternion downRotation = Quaternion.AngleAxis(downPivotAngle, Vector3.forward);
  
            Vector3 playerPoint0 = playerPos;
            Vector3 playerPoint1 = targetMonsterPosition + (upRotation * monsterToPlayerDir) * ratioDistance;
            Vector3 playerPoint2 = playerPos + (downRotation * playerToMonsterDir)  * ratioDistance;
            Vector3 playerPoint3 = targetMonsterPosition;
            
            Vector3 monsterPoint0 = targetMonsterPosition;
            Vector3 monsterPoint1 = playerPos + (upRotation * playerToMonsterDir)  * ratioDistance;
            Vector3 monsterPoint2 = targetMonsterPosition + (downRotation * monsterToPlayerDir)  * ratioDistance;
            Vector3 monsterPoint3 = playerPos;

            //나누기 방어코드
            throwTime = Mathf.Max(0.001f, throwTime);
            returnTime = Mathf.Max(0.001f, returnTime);

            while (elapsedTime < totalTime)
            {
                if (isThrowPhase)
                {
                    float t = elapsedTime / throwTime;
                    //Debug.Log($"elapsedTime : {elapsedTime}");
                    //Debug.Log($"Time : {t}");
                    Vector3 currentPos = Calculator.BezierCurves(playerPoint0, playerPoint1, playerPoint2, playerPoint3, t);
                    transform.position = currentPos;

                    if (elapsedTime >= throwTime) //부메랑돌아오기
                    {
                        //Debug.Log($"elapsedTime111111 : {elapsedTime}");
                        isThrowPhase = false;
                        elapsedTime = 0f;
                    }
                }
                else
                {
                    float t = elapsedTime / returnTime;
                    Vector3 currentPos = Calculator.BezierCurves(monsterPoint0, monsterPoint1, monsterPoint2,
                        monsterPoint3, t);
                    transform.position = currentPos;

                    if (elapsedTime >= returnTime)
                    {
                        isEnd = true;
                        SpinSuccess();
                        yield break;
                    }
                }

                elapsedTime += Time.deltaTime;
                FinderHitMonster(0.5f);
                yield return new WaitForFixedUpdate();
            }

            //방어코드
            if (!isEnd)
            {
                SpinSuccess();
            }
        }

        public void FinderHitMonster(float hitDistance)
        {
            for (int i = 0; i < GameManager.Instance.monsterSpawner.monsters.Count; i++)
            {
                bool isCritical = false;
                if (criticalRange > 0)
                {
                    int criticalRangeValue = (int)(criticalRange * 100);
                    int RandomcriticalValue = Random.Range(0, 100);
                    isCritical = RandomcriticalValue <= criticalRangeValue;
                }
                DamageType damageType = isCritical ? DamageType.CRITICAL : DamageType.NORMAL;
                
                Monster monster = GameManager.Instance.monsterSpawner.monsters[i];
                Vector2 pos = transform.position;
                Vector2 monPos = monster.transform.position;
                    
                float dis = Vector2.Distance(monPos, pos);

                if (monster.IsAlive && dis < hitDistance)
                {
                    if (!dic_HitMonsters.ContainsKey(monster))
                    {
                        Coroutine coroutine = StartCoroutine(monster.BoomerangSequence(attackValue,
                            damageType, criticalDamageRate, PlayHitEffect));
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
        }

        public void PlayHitEffect(Transform targetTransform)
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_RIO_HTTING);
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
            particle.PlayParticle(transform.localPosition, 0);
        }

        #region anotherRogic

        // public IEnumerator TrackingTargetBoomerang(float hitDistance, Vector3 targetMonsterPosition)
        // {
        //     Vector3 position = transform.position;
        //     Vector3 playerToMonsterDir = targetMonsterPosition - position;
        //     Vector3 monsterToPlayerDir = position - targetMonsterPosition;
        //
        //     float distance = playerToMonsterDir.magnitude;
        //     playerToMonsterDir.Normalize();
        //     monsterToPlayerDir.Normalize();
        //
        //     float pivotAngle = 40f; // 40도에서 0도로 변화
        //     float attackSpeed = 2f;
        //     float uniqueValue = 2f;
        //
        //     // 총 이동 시간 및 던질 때와 돌아올 때 시간 비율 설정
        //     float totalTime = distance / (attackSpeed * uniqueValue);
        //     float throwTime = totalTime * 0.4f;
        //     float returnTime = totalTime * 0.6f;
        //
        //     // 각도 변화 속도 설정 (던질 때)
        //     float angularSpeedThrow = pivotAngle / throwTime;
        //     float angularSpeedReturn = pivotAngle / returnTime;
        //
        //     bool isFirstPhase = true;
        //     float currentAngle = pivotAngle;
        //
        //     while (true)
        //     {
        //         Vector3 rotatedVector = Vector2.zero;
        //
        //         // 던질 때와 돌아올 때 각도 변화 계산
        //         currentAngle -= (isFirstPhase ? angularSpeedThrow : angularSpeedReturn) * Time.deltaTime;
        //
        //         // 각도에 맞는 이동 속도 설정
        //         float currentMoveSpeed = distance / totalTime;
        //
        //         // 첫 번째 던질 때, 각도 변화 및 회전
        //         if (isFirstPhase)
        //         {
        //             Quaternion rotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);
        //             rotatedVector = rotation * playerToMonsterDir;
        //             Debug.DrawLine(position, playerToMonsterDir, Color.red);
        //         }
        //         else
        //         {
        //             Quaternion rotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);
        //             rotatedVector = rotation * monsterToPlayerDir;
        //             Debug.DrawLine(position, monsterToPlayerDir, Color.green);
        //         }
        //
        //         // 이동 방향 및 회전 설정
        //         Debug.DrawLine(position, rotatedVector, Color.blue);
        //         float rotationZ = Mathf.Atan2(rotatedVector.y, rotatedVector.x) * Mathf.Rad2Deg;
        //         transform.rotation = Quaternion.AngleAxis(rotationZ - 90.0f, Vector3.forward);
        //         transform.Translate(currentMoveSpeed * Time.deltaTime * Vector2.up);
        //         
        //         if (isFirstPhase && currentAngle <= 0f)
        //         {
        //             isFirstPhase = false;
        //             currentAngle = pivotAngle;
        //         }
        //         
        //         if (!isFirstPhase && currentAngle <= 0f)
        //         {
        //             MissedTarget();
        //             yield break;
        //         }
        //
        //         yield return null;
        //     }
        // }
        #endregion

    }
}
