using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;
using UniRx;
using UniRx.Triggers;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Detectors;
using Framework.GameData.Defense;

namespace Framework.Game.Defense
{
    public abstract class Projectile : MonoBehaviour
    {
        public Monster targetMonster;
        public string objectName;
        public ObscuredFloat moveSpeed;
        public bool isMove = false;
        public ObscuredFloat distance;
        public ObscuredInt provokedIndex;
        public ObscuredFloat attackValue;
        public ObscuredFloat criticalDamageRate;
        public ObscuredFloat criticalRange;
        public bool isCriticalHit = false;
        public bool isSplashHit = false;
        public ObscuredFloat buffValue;
        public ObscuredFloat buffDuration;
        public Character character;

        public string effectName;

        public ObscuredFloat strikeRange = 0.0f;
        public ObscuredFloat moveDelta;
        
        public abstract void FireProjectile();
        public abstract void HitMonster();
        public abstract void CloseTarget();
        public abstract void MissedTarget();

        public virtual void SetProjectileParticle()
        {

        }

        public void TrackingTargetFixed()
        {
            if (!targetMonster.IsAlive)
            {
                MissedTarget();
                return;
            }

            Vector3 targetPos = targetMonster.transform.position;
            Vector3 moveDir = targetPos - transform.position;

            moveDir.Normalize();

            transform.Translate(moveSpeed * Time.deltaTime * moveDir);

            distance = Vector2.Distance(targetPos, transform.position);

            if (distance < 0.2f)
            {
                CloseTarget();
            }
        }

        public void TrackingTarget(float hitDistance = 0.2f)
        {
            if (!targetMonster.IsAlive)
            {
                MissedTarget();
                return;
            }

            Vector3 targetPos = targetMonster.transform.position;
            Vector3 moveDir = targetPos - transform.position;

            moveDir.Normalize();
            float rotationZ = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(rotationZ - 90.0f, Vector3.forward);

            transform.Translate(moveSpeed * Time.deltaTime * Vector2.up);

            distance = Vector2.Distance(targetPos, transform.position);

            if (distance < hitDistance)
            {
                CloseTarget();
            }
        }
        
        public void TrackingTargetBezierCurves(bool isRotation)
        {
            if (!targetMonster.IsAlive)
            {
                MissedTarget();
                return;
            }

            //이동 값을 시간값으로 바꿔서 공식값에 대입하기위한 작업 16  -> 1.6
            float speedTime = moveSpeed * 0.1f;

            //이동값을 적용해줄 값 적용
            moveDelta += Time.deltaTime * speedTime;

            //몬스터 위치
            Vector3 targetPos = targetMonster.transform.position;

            //몬스터 방향
            Vector3 targetDir = targetPos - character.transform.position;

            //3차 베지어 곡선을 위해 점 4개를 얻어옴 
            Vector3 p0 = character.transform.position;

            //맨 위의 빙하나 맨아래의 빙하의 인덱스를 구할 수 없기에 임의의 점을 둠
            //바꿔야할수도
            Vector3 p1 = p0 + (Vector3.up * 2);

            //왼쪽인이 오른쪽인지 곱하기를 하면 얻을 수 있고
            //p1의 값을 더해 세번쩨 점을 구해줌
            Vector3 p2 = p1 + (Vector3.right * targetDir.x);

            //마지막 점은 타켓 위치를 정해줌
            Vector3 p3 = targetPos;

            Vector3 berzierPosition = Calculator.BezierCurves(p0, p1, p2, p3, moveDelta);

            //2차 베지어 함수
            //Vector3 bezierUpPosition = character.transform.position + (Vector3.up * moveDir.magnitude) + (Vector3.right * (moveDir.x / 2));
            //Vector3 BeizerPoint3 = Vector3.Lerp(character.transform.position, bezierUpPosition, moveDelta);
            //Vector3 BeizerPoint4 = Vector3.Lerp(bezierUpPosition, targetMonster.transform.position, moveDelta);
            //transform.position = Vector3.Lerp(BeizerPoint3, BeizerPoint4, moveDelta);

            if (isRotation)
            {
                Vector3 moveDir = berzierPosition - transform.position;
                moveDir.Normalize();
                float rotationZ = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(rotationZ - 90.0f, Vector3.forward);
            }

            transform.position = berzierPosition;
            distance = Vector2.Distance(targetPos, transform.position);

            if (distance < 0.2f)
            {
                //도착했다면 무브 델타값 초기화
                moveDelta = 0f;
                CloseTarget();
            }
        }

        public void ReturnObjectPool()
        {
            targetMonster = null;
            isMove = false;
            GameManager.Instance.objectPoolManager.ReturnObject(this, objectName);
        }

        public void RemoveMonster()
        {
            for (int i = 0; i < GameManager.Instance.characterSpawner.summonedCharacters.Count; i++)
            {
                if (GameManager.Instance.characterSpawner.summonedCharacters[i].glacierIdx == provokedIndex)
                {
                    GameManager.Instance.characterSpawner.summonedCharacters[i].targetedMonster.Remove(targetMonster);
                    break;
                }
            }
        }
    }
}
