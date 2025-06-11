using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using Framework.Util;


namespace Framework.Game.Defense
{
    public class OrbStackInterface : MonoBehaviour
    {
        public bool isOperation;
        public Monster targetMonster;

        public ParticleSystem[] particleSystems;

        public void Initialize(Monster monster)
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].gameObject.SetActive(false);
            }

            transform.SetParent(monster.transform);
            transform.localPosition = Vector2.zero;
        }

        public void HitMonster(int orbStackCount)
        {
            int count = orbStackCount - 1;

            if(count >= 3)
            {
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    particleSystems[i].gameObject.SetActive(false);
                }

                count = 0;
            }
            particleSystems[count].gameObject.SetActive(true);
        }

        public void ReturnObject()
        {
            GameManager.Instance.objectPoolManager.ReturnObject(this, "OrbStackInterface");
        }
    }
}