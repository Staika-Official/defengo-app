using System.Collections;
using System.Collections.Generic;
using Framework.GameData.Defense;
using Framework.Util;
using UnityEngine;

namespace Framework.Game.Defense
{
    public class YuriArrowRain : MonoBehaviour
    {
        public float arrowRaineDamage;
        public float arrowRainAddtiveDamage;

        public float arrowRainDuration;

        public float arrowCriticalRange;
        public float arrowCriticalDamageRate;

        public float arrowRainRange;

        public Vector2 positionVector;

        public Dictionary<Monster, Coroutine> dic_HitMonsters = new();

        public void Initialize(Monster targetMonster, float arrowRainDuration)
        {
            CellData data = GridManager.Instance.cellDatas[targetMonster.prevPositionIdx];
            Vector2 pos = data.cellPosition;

            this.arrowRainDuration = arrowRainDuration;

            transform.position = pos;

            positionVector = Calculator.TransformationVector(targetMonster.prevPositionIdx);

            StartCoroutine(MonsterArrowRainDamage());
        }

        public IEnumerator MonsterArrowRainDamage()
        {
            StartCoroutine(WaitForArrowRainDuration());

            while (true)
            {
                for (int i = 0; i < GameManager.Instance.monsterSpawner.monsters.Count; i++)
                {
                    Monster monster = GameManager.Instance.monsterSpawner.monsters[i];

                    Vector2 monsPos = Calculator.TransformationVector(monster.prevPositionIdx);

                    float dis = Calculator.DistanceCheck(positionVector - monsPos);

                    if (monster.IsAlive && dis <= arrowRainRange)
                    {
                        if(!dic_HitMonsters.ContainsKey(monster))
                        {
                            Coroutine coroutine = StartCoroutine(monster.arrowRainSequence(arrowRaineDamage, arrowRainAddtiveDamage, arrowCriticalRange, arrowCriticalDamageRate, null));
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

        public IEnumerator WaitForArrowRainDuration()
        {
            float time = 0;
            while(true)
            {
                time += Time.deltaTime;

                if(time >= arrowRainDuration)
                {
                    StopCoroutine(MonsterArrowRainDamage());

                    foreach(var monster in dic_HitMonsters)
                    {
                        StopCoroutine(monster.Value);
                    }
                    dic_HitMonsters.Clear();

                    GameManager.Instance.objectPoolManager.ReturnObject(this, "ArcherHit");
                    break;
                }

                yield return new WaitForFixedUpdate();
            }

        }



    }
}
