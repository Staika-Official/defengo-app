using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;
using Framework.Sound;
using UniRx;
using UniRx.Triggers;
using Framework.GameData.Defense;

namespace Framework.Game.Defense
{
    public class KanadeBomb : MonoBehaviour
    {
        public Animator animator;

        public void Initialize(float waitTime, int pivotPos, float damage, float strikeRange)
        {
            CellData data = GridManager.Instance.cellDatas[pivotPos];

            Vector2 pos = data.cellPosition;

            transform.position = pos;

            animator.Rebind();
            //waitTime
            StartCoroutine(WaitForExplosion(waitTime, pivotPos, damage, strikeRange));
        }

        public IEnumerator WaitForExplosion(float waitTime, int pivotPos, float damage, float strikeRange)
        {
            yield return new WaitForSeconds(waitTime);

            for (int i = 0; i < GameManager.Instance.monsterSpawner.monsters.Count; i++)
            {
                Monster monster = GameManager.Instance.monsterSpawner.monsters[i];

                Vector2 tarMonsterVector = Calculator.TransformationVector(pivotPos);
                Vector2 monsterVector = Calculator.TransformationVector(monster.prevPositionIdx);

                if (Calculator.DistanceCheck(tarMonsterVector - monsterVector) < strikeRange)
                {
                    if (monster.IsAlive)
                    {
                        monster.HitDamage(damage, DamageType.NORMAL, 0, null);
                    }
                }
            }

            SoundManager.Instance.PlaySound(SoundKey.SF_CHARACTER_FORTIS);
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("KanadeBombHit");
            particle.PlayParticle(transform.localPosition, 0);

            GameManager.Instance.objectPoolManager.ReturnObject(this, "KanadeBomb");
        }
    }
}