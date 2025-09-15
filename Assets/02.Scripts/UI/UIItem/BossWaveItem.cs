using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Framework.Game.Defense;
using Framework.GameData.Defense;

namespace  Framework.UI
{
    public class BossWaveItem : MonoBehaviour
    {
        public Animator animator;
        public TextMeshProUGUI text_bossHealth;
        
        public void Initialize(float addhealth)
        {
            Debug.Log($"{gameObject.name} init SetBossSummon - Add Health: {addhealth}");
            animator.Rebind();
            
            text_bossHealth.text = $"+{addhealth}";
            animator.SetTrigger("SetBossSummon");
        }

        public void OnCompleteAnim()
        {
            Debug.Log($"{gameObject.name} OnCompleteAnim");
            gameObject.SetActive(false);
            UIManager.Instance.bossSelectPopup.ActivePopup();
        }
    }
}

