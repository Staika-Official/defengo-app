using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;
using Framework.GameData.Defense;

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

        public override void FieldBossInitialize(BossData bossData)
        {
            monsterType = MonsterType.FIELD_BOSS_MONSTER;
            isBoss = true;
            transform.name = "Trush";
            speed = bossData.monsterSpeed;
            health = bossData.health + GameManager.Instance.tempBossAddHealth + (waveIndex - 1) / 5 * bossData.healthFactor;
            SetBossMove();
        }
    }
}
