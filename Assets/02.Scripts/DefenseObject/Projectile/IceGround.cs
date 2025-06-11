using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using Framework.Util;

namespace Framework.Game.Defense
{
    public class IceGround : DebuffGround
    {
        //public List<Character> withinRange;
        public IEnumerator slowDownSeqeunce;
        public float slowDownValue;
        public int positionIdx;
        public Vector2 positionVector;
        public float duration;
        public float slowValue;
        public float range;
        public float currentRange;

        public void Initialize(int positionIdx, float duration, float value, float range)
        {
            this.positionIdx = positionIdx;
            this.duration = duration;
            this.slowValue = value;
            this.range = range;
            currentRange = Mathf.Pow(range, 2);
            positionVector = Calculator.TransformationVector(positionIdx);
            //transform.localPosition = positionVector;
            StartCoroutine(MonsterSlowDown());
        }

        public IEnumerator MonsterSlowDown()
        {
            float timer = 0;

            while (true)
            {
                for (int i = 0; i < GameManager.Instance.monsterSpawner.monsters.Count; i++)
                {
                    Monster monster = GameManager.Instance.monsterSpawner.monsters[i];

                    if (!monster.IsAlive) continue;
                    Vector2 monsPos = Calculator.TransformationVector(monster.prevPositionIdx);

                    float dis = Calculator.DistanceCheck(positionVector - monsPos);
                    if(dis <= currentRange)
                    {
                        monster.SlowDownSequence(slowValue, currentRange, null, this);
                    }
                }

                yield return new WaitForFixedUpdate();
                timer += Time.fixedDeltaTime;
                if (timer >= duration)
                {
                    break;
                }
            }
            ReturnObject();
        }

        public void ReturnObject()
        {
            GameManager.Instance.objectPoolManager.ReturnObject(this, "IceGround");
        }

        public void EndOfUse()
        {
            StopCoroutine(slowDownSeqeunce);
        }
    }
}