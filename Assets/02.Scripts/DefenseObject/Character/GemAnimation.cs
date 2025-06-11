using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using TMPro;

namespace Framework.Game.Defense
{
    public class GemAnimation : MonoBehaviour
    {
        public Animator animator;
        public TextMeshProUGUI text_Gem;

        public void SetGemAnimation(int gem)
        {
            animator.SetTrigger("SetAnim");
            text_Gem.text = $"+{gem}";
        }

        public void AnimationEnd()
        {
            GameManager.Instance.objectPoolManager.ReturnObject(this, "PlusLifeSton");
        }

    }
}
