using System.Collections;
using System.Collections.Generic;
using Framework.Sound;
using UnityEngine;

namespace Framework.Game.Defense
{
    public class ShieldBubble : MonoBehaviour
    {
        public string effectName;
        //쉴드 초기화
        public void Initialize(Character character, string effectName)
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_LULU_BUBBLE_ON);

            transform.SetParent(character.transform);
            transform.localPosition = Vector2.zero;
            this.effectName = effectName;
        }

        public void DestroyShield()
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_LULU_BUBBLE_OFF);

            GameManager.Instance.objectPoolManager.ReturnObject(this, "ShieldBubble");

            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>(effectName);
            particle.PlayParticle(transform.localPosition, 0);
        }
    }
}
