using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Util;
using UnityEngine;

namespace Framework.Game.Defense
{
    public class ToxicGround : DebuffGround
    {
        public float toxicDamage;
        public float criticalRange;
        public float criticalDamage;
        public int positionIndex;
        public float toxicDuration;
        public Vector2 positionVector;
        public float currentRange;
        public DamageType damageType;

        public List<Monster> toxicMonster = new();

        public void Initialize(int positionIndex, float toxicDamage, float toxicDuration, float toxicRange, float criticalRange, float criticalDamage)
        {
            //생성 위치 인덱스
            this.positionIndex = positionIndex;

            //독 장판 지속시간
            this.toxicDuration = toxicDuration;

            //데미지를 해당 오브젝트에서 주기때문에 데미지관련 벨류값 받아
            this.currentRange = Mathf.Pow(toxicRange, 2);
            this.criticalRange = criticalRange;
            this.criticalDamage = criticalDamage;
            this.toxicDamage = toxicDamage;

            //생성 위치 받아옴
            positionVector = Calculator.TransformationVector(positionIndex);

            //몬스터 데미지 코루틴함수 시작
            StartCoroutine(MonsterToxicDamage());
        }

        public IEnumerator MonsterToxicDamage()
        {
            //몬스터 데미지 주는 틱 타이밍과 별개로
            //장판 지속시간은 따로 돌려주려고 코루틴함수 실행
            StartCoroutine(ToxicGroundTimer());

            //toxicParticles 장판에 데미지를 입으면 해당 몬스터에게 파티클을
            //SetParent를 해주기 떄문에 toxicParticles딕셔너리 선언
            //key Monster value Particle

            while (true)
            {
                toxicMonster.Clear();

                //스폰된 몬스터 순회
                for (int i = 0; i < GameManager.Instance.monsterSpawner.monsters.Count; i++)
                {
                    //틱마다 크리티컬 데미지를 랜덤으로 뽑아와서 줘야하기 때문에 여기서 연산 진행
                    bool isCritical = false;
                    if (criticalRange > 0)
                    {
                        int criticalRangeValue = (int)(criticalRange * 100);
                        int RandomcriticalValue = Random.Range(0, 100);
                        isCritical = RandomcriticalValue <= criticalRangeValue;
                    }

                    DamageType damageType = isCritical ? DamageType.CRITICAL : DamageType.NORMAL;

                    Monster monster = GameManager.Instance.monsterSpawner.monsters[i];

                    Vector2 monsPos = Calculator.TransformationVector(monster.prevPositionIdx);

                    float dis = Calculator.DistanceCheck(positionVector - monsPos);

                    //몬스터가 살아있고 장판 범위에 들어가있다면 조건 성립
                    if (monster.IsAlive && dis <= currentRange)
                    {
                        ////몬스터에게 파티클이 달려있다면 새롭게 달아줄 필요가 없기에 이러한 조건 줌
                        //if (!toxicParticles.ContainsKey(monster))
                        //{
                        //    //파티클을 풀에서 얻어와서 몬스터 하위에 둠
                        //    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("ToxicMon");
                        //    particle.transform.SetParent(monster.transform);
                        //    particle.transform.localPosition = Vector2.zero;
                        //    toxicParticles.Add(monster, particle);
                        //}

                        if(!toxicMonster.Contains(monster))
                        {
                            toxicMonster.Add(monster);
                        }

                        //몬스터 색상 변경을 위한 함수
                        monster.ToxicSequence(true);
                        //몬스터에게 데미지 줌
                        monster.HitDamage(toxicDamage, damageType, criticalDamage, null);
                    }
                    else
                    {
                        //몬스터가 죽었거나 범위에 범어났다면 여기에 조건이 들어옴
                        //ObjectParticle particle;

                        //todo :: 다른방식으로
                        //지금 몬스터가 파티클이 달려있는지 확인하고 달려있다면 파티클을 얻어오는 조건
                        //if (toxicParticles.TryGetValue(monster, out particle))
                        //{
                        //    //파티클 반환
                        //    GameManager.Instance.objectPoolManager.ReturnObject(toxicParticles[monster], "ToxicMon");
                        //    //색상 디폴트값으로
                        //    toxicParticles.Remove(monster);
                        //}

                        if (toxicMonster.Contains(monster))
                        {
                            toxicMonster.Remove(monster);
                        }

                        monster.ToxicSequence(false);
                    }

                }

                //0.5초 틱으로 백마법사랑 동일하게 줌
                //TODO :: 틱 단위를 전체적으로 관리해야하는데 어디서 관리해야할지 고민
                yield return new WaitForSeconds(0.5f);
            }

        }

        public IEnumerator ToxicGroundTimer()
        {
            //해당 시간이 지나면 데미지를 주는 코루틴 함수 정지를 위한 코루틴함수
            float duration = 0f;
            while (true)
            {
                duration += Time.fixedDeltaTime;

                //시간이 다됬다면 조건성립
                if (duration >= toxicDuration)
                {
                    //공격 중지
                    StopCoroutine(MonsterToxicDamage());

                    //몬스터 하위에 파티클 달려있는걸 전부 반환
                    //foreach (var toxic in toxicParticles)
                    //{
                    //    GameManager.Instance.objectPoolManager.ReturnObject(toxic.Value, "ToxicMon");

                    //    //색상 디폴트 값으로
                    //    toxic.Key.ToxicSequence(false);
                    //}
                    //toxicParticles.Clear();

                    foreach(var monster in toxicMonster)
                    {
                        monster.ToxicSequence(false);
                    }
                    toxicMonster.Clear();

                    GameManager.Instance.objectPoolManager.ReturnObject(this, "ToxicGround");
                    break;
                }

                yield return new WaitForFixedUpdate();
            }
        }

    }
}
