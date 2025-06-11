using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;

namespace Framework.Game.Defense
{
    public class ObjectParticle : MonoBehaviour
    {
        public string particleName;
        public ParticleSystem[] particleSystems;
        public bool isBossGo;

        public void Initialize()
        {

        }

        public void SimplePlay()
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play();
            }
        }

        public void SimplePlay(Transform target)
        {
            transform.localPosition = target.localPosition;
            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play();
            }
        }

        public IEnumerator CoinSequence(Vector3 targetPos)
        {
            while (true)
            {
                if(Vector2.Distance(transform.position, targetPos) <= 0.25f)
                {
                    UIManager.Instance.ClosedGoIcon(isBossGo);
                    GameManager.Instance.objectPoolManager.ReturnObject(this, particleName);
                    break;
                }

                Vector3 moverDir = targetPos - transform.position;
                transform.Translate(4f * Time.deltaTime * moverDir);
                
                yield return null;
            }
        }


        public void PlayParticle(Vector2 pos, float buffDuration)
        {
            StartCoroutine(PlayParticleAsync(pos, buffDuration));
        }

        public IEnumerator PlayParticleAsync(Vector2 pos, float buffDuration)
        {
            bool isLoop = Mathf.Approximately(0, buffDuration);
            transform.localPosition = pos;
            if (!isLoop)
            {
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    particleSystems[i].Play();
                }
                yield return new WaitForSeconds(buffDuration);

                GameManager.Instance.objectPoolManager.ReturnObject(this, particleName);
            }
            else
            {
                float duration = particleSystems[0].main.duration;
                
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    particleSystems[i].Play();
                }
                yield return new WaitForSeconds(duration + 0.1f);

                GameManager.Instance.objectPoolManager.ReturnObject(this, particleName);
            }     
        }
    }
}
