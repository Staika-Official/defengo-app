using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Framework.GameData.Defense;
using Framework.Sound;

namespace Framework.Game.Defense
{
    public class ArrowProjectile : Projectile
    {
        public float arrowRaineDamage;
        public float arrowRainAddtiveDamage;

        public float arrowRainDuration;

        public float arrowCriticalRange;
        public float arrowCriticalDamageRate;

        public float arrowRainRange;

        void Start()
        {
            //몬스터를 추격하는 함수
            this.FixedUpdateAsObservable()
               .Where(_ => isMove)
               .Subscribe(_ => TrackingTargetBezierCurves(true));
        }

        public void Initialize(Character character, Monster targetMonster, float moveSpeed)
        {
            this.character = character;
            this.targetMonster = targetMonster;
            this.moveSpeed = moveSpeed;

            effectName = "ArcherHit";
        }

        public override void CloseTarget()
        {
            //함수 들어오는 조건 : 거리가 0.2보다 작아질 경우
            //몬스터가 살아있다면
            if (targetMonster.IsAlive)
            {
                //히트
                HitMonster();
            }
            else
            {
                //죽었다면 타겟 초기화 및 반환
                ReturnObjectPool();
            }
            isMove = false;

            //죽어있으니 타켓 몬스터 삭제
            RemoveMonster();
        }

        public override void FireProjectile()
        {
            throw new System.NotImplementedException();
        }

        public override void HitMonster()
        {
            //몬스터를 히트 했을 경우 함수가 호출
            YuriArrowRain arrowRain = GameManager.Instance.objectPoolManager.GetObject<YuriArrowRain>(effectName);
            arrowRain.arrowRaineDamage = arrowRaineDamage;
            arrowRain.arrowRainAddtiveDamage = arrowRainAddtiveDamage;
            arrowRain.arrowCriticalRange = criticalRange;
            arrowRain.arrowCriticalDamageRate = criticalDamageRate;
            arrowRain.arrowRainRange = arrowRainRange;

            arrowRain.Initialize(targetMonster, arrowRainDuration);

            //투사체 반환
            ReturnObjectPool();
        }

        public override void MissedTarget()
        {
            //함수 호출 시점 타겟 몬스터가 죽었다면 불리는 함수

            //캐릭터의 타켓몬스터가 한마리도없으면 반환해줘야함
            if (character.targetedMonster.Count == 0)
            {
                ReturnObjectPool();
            }
            else
            {
                //그게아니고 현재 캐릭터가 activeSelf 가된다면 다시 반환해줘야함
                if (!character.gameObject.activeSelf)
                {
                    ReturnObjectPool();
                }
                else
                {
                    //그것도 아니라면 캐릭터타겟 몬스터의 0번째 몬스터를 쳐다보게함
                    targetMonster = character.targetedMonster[0];
                }
            }
        }

      
    }
}
