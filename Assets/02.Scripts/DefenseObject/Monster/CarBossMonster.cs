using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;
using Framework.GameData.Defense;

namespace Framework.Game.Defense
{
    public class CarBossMonster : Monster
    { 
        public float bossSpeed;
        public float bossHealth;
        public int monCloneId;
        public float monCloneHealth;

        public IEnumerator cloneSequence;
       
        public override void Abillity()
        {
            cloneSequence = CarBossAbillitySequence();
            StartCoroutine(cloneSequence);
        }

        public IEnumerator CarBossAbillitySequence()
        {
            while (IsAlive)
            {
                int idx = CurrentArrayIdx;
                yield return new WaitForSeconds(ConfigData.CREATE_CLONEMONSTER_TIME);
                GameManager.Instance.monsterSpawner.SetCloneMonster(monCloneHealth, speed - ConfigData.CREATE_CLONEMONSTER_TIME, monCloneId, idx);
            }
        }

        public override void DeathSequence()
        {
            
        }

        public override void EndOfUse()
        {
            if (null != deadTimerCoroutine)
            {
                StopCoroutine(deadTimerCoroutine);
                deadTimerCoroutine = null;
            }

            StopCoroutine(cloneSequence);
            cloneSequence = null;
            monsterInterface.transform.SetParent(transform);
            monsterInterface.ReturnObjectPool();
            IsAlive = false;
            GameManager.Instance.monsterSpawner.RemoveMonster(this);
            GameManager.Instance.objectPoolManager.ReturnObject(this, "Boss_101");
        }
    }
}
