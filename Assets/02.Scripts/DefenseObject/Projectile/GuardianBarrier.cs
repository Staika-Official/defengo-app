using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Sound;
using Framework.Util;
using UnityEngine;

namespace Framework.Game.Defense
{
    public class GuardianBarrier : DebuffGround
    {
        //현재 멈추게 하려는 몬스터를 들고있어야
        //다시 움직이게 할 수 있기에 리스트 변수 선언
        public List<Monster> stopMonsters = new List<Monster>(); 


        //베리어 초기화
        //셀 위치를 가져와서 셋 해줌
        public void Initialize(int pivotPos, float barrierDuration, float barreirBlockDistance)
        {
            CellData data = GridManager.Instance.cellDatas[pivotPos];

            Vector2 pos = data.cellPosition;

            transform.position = pos;

            //코루틴 함수 채택 이유
            //지속적으로 도는게아닌 일정 시간만 유지하기에 채택
            StartCoroutine(WaitForBarrierDuration(pivotPos, barrierDuration, barreirBlockDistance));

        }

        public IEnumerator WaitForBarrierDuration(int pivotPos, float barrierDuration, float barreirBlockDistance)
        {
            //이펙트 사운드 추가
            SoundManager.Instance.PlaySound(SoundKey.SF_GUARDIAN_GLACIER);

            stopMonsters.Clear();

            //Test
            //float duration = 3;

            //Develop
            float duration = barrierDuration;

            //duration 시간 만큼만 유지할꺼이기에 이러한 조건 넣어줌
            while (0f <= duration)
            {
                //연산해서 가지고온 그리드 셀 포지션을 얻어옴
                Vector2 barrierPosition = Calculator.TransformationVector(pivotPos);

                //스폰된 몬스터 리스트를 순회
                for (int i = 0; i < GameManager.Instance.monsterSpawner.monsters.Count; ++i)
                {
                    //각 몬스터를 얻어옴
                    Monster monster = GameManager.Instance.monsterSpawner.monsters[i];

                    //몬스터의 현재 위치를 얻어옴
                    Vector2 monsterPosition = Calculator.TransformationVector(monster.prevPositionIdx);

                    //방향벡터를 구하는게아닌 거리값만 구할꺼이기에 순서상관없이 넣었음
                    //거리를 구해옴
                    float distance = Calculator.DistanceCheck(barrierPosition - monsterPosition);


                    //현재 구해온 거리가 기획서에 지정해준 거리보다 짧으면 조건성립
                    if (distance <= barreirBlockDistance)
                    {
                        //!stopMonsters.Contains(monster) 조건 같은 몬스터를 또 추가해줄 이유는 없기에
                        if (!stopMonsters.Contains(monster))
                        {
                            //현재 멈추게한 몬스터를 얻어오기위함
                            stopMonsters.Add(monster);
                        }

                        //몬스터가 살아있고 멈춰있지 않다면 멈추게 해줘야함
                        if(monster.IsAlive && !monster.IsStop)
                        {
                            monster.IsStop = true;
                        }
                    }
                }

                //지속시간을 감소시켜줌
                duration -= Time.deltaTime;

                yield return null;
            }

            //현재 멈추게한 몬스터를 순회
            for (int i = 0; i < stopMonsters.Count; ++i)
            {
                //살아있다면 다시 움직이게함
                if (stopMonsters[i].IsAlive)
                {
                    stopMonsters[i].IsStop = false;
                }
            }

            //이펙트 반환
            GameManager.Instance.objectPoolManager.ReturnObject(this, "EffectBarrier");
        }


    }
}
