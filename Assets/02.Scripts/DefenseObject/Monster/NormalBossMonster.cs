using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;

namespace Framework.Game.Defense
{
    public class NormalBossMonster : Monster
    {
        public float bossSpeed;
        public float bossHealth;

        public override void Abillity()
        {

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

            monsterInterface.transform.SetParent(transform);
            monsterInterface.ReturnObjectPool();
            IsAlive = false;
            GameManager.Instance.monsterSpawner.RemoveMonster(this);
            GameManager.Instance.objectPoolManager.ReturnObject(this, "Boss_105");
        }
    }
}
