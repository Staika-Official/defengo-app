using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Sound;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Framework.Game.Defense
{
    public class CardProjectile : Projectile
    {
        void Start()
        {
            //몬스터를 추격하는 함수
            this.FixedUpdateAsObservable()
               .Where(_ => isMove)
               .Subscribe(_ => TrackingTarget());
        }

        public void Initialize(Character character, Monster target, bool isCriticalHit, float moveSpeed, float strikeRange, string effectName)
        {
            this.character = character;
            this.effectName = effectName;

            //쳐다보는 몬스터
            targetMonster = target;
            this.moveSpeed = moveSpeed;
            this.isCriticalHit = isCriticalHit;
            this.strikeRange = strikeRange;

            //스플레시 데미지를 줄것인가 아닌가
            isSplashHit = strikeRange != 0;
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
            //TODO :: 궁금점 ReturnObjectPool 에서 targetMonster = null을해주면
            //RemoveMonster 함수내에서 targetMonster 를 Remove해준다 null을 Remove하는데 무슨뜻일까요..?
            RemoveMonster();
        }

        public override void FireProjectile()
        {
        }

        public override void HitMonster()
        {
            //몬스터를 히트 했을 경우 함수가 호출

            //타겟 몬스터에게 데미지를 입힘 파티클 생성 및 발사체 반환
            DamageType damageType = isCriticalHit ? DamageType.CRITICAL : DamageType.NORMAL;

            //몬스터에게 데미지 가격
            targetMonster.HitDamage(attackValue, damageType, criticalDamageRate, character);

            //히트 파티클 생성
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
            particle.PlayParticle(transform.localPosition, 0);
            SoundManager.Instance.PlaySound(SoundKey.SF_TRICKSTER_ATTACK);

            //투사체 반환
            ReturnObjectPool();
        }

        public override void MissedTarget()
        {
            //함수 호출 시점 타겟 몬스터가 죽었다면 불리는 함수

            //캐릭터의 타켓몬스터가 한마리도없으면 반환해줘야함
            if (character.targetedMonster.Count == 0)
            {
                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                particle.PlayParticle(transform.localPosition, 0);
                SoundManager.Instance.PlaySound(SoundKey.SF_TRICKSTER_ATTACK);
                ReturnObjectPool();
            }
            else
            {
                //그게아니고 현재 캐릭터가 activeSelf 가된다면 다시 반환해줘야함
                if (!character.gameObject.activeSelf)
                {
                    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
                    particle.PlayParticle(transform.localPosition, 0);
                    SoundManager.Instance.PlaySound(SoundKey.SF_TRICKSTER_ATTACK);
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
