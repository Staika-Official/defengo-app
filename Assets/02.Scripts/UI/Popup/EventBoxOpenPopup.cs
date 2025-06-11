using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Network;
using Framework.Util;
using Framework.Sound;
using System.Globalization;

namespace Framework.UI
{
    public class EventBoxOpenPopup : PopupTemplate
    {
        public Animator animator;
        public TextMeshProUGUI text_SymbolName;
        public TextMeshProUGUI text_Amount;
        public TextMeshProUGUI text_Event;
        public GameObject buttonGroupObject;

        // private void Update()
        // {
        //     if(Input.GetKeyDown(KeyCode.G))
        //     {
        //         StartCoroutine(OpenBoxSequence(1.0f));
        //     }
        // }

        public override void ActivePopup()
        {
            animator.Rebind();
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() =>
            {
                InActivePopup();
            });
        }

        public void OpenRandomBox(EventBoxResult data)
        {
            Debug.Log("stik Value : " + data.value);
        
            ActivePopup();
            StartCoroutine(OpenBoxSequence(data.value));
        }

        public IEnumerator OpenBoxSequence(string value)
        {
            buttonGroupObject.SetActive(false);
            //SoundManager.Instance.PlaySound(SoundKey.SF_BOX_ENTER);
            animator.SetTrigger("EnterBoxToIdle");
            animator.SetTrigger("IdleToOpen");

            text_Amount.text = value + "<color=#FF992F> STIK</color>";
                //$"{value:N3}<color=#FF992F> STIK</color>";
            text_Event.text = "Event";

            animator.SetTrigger("OpenToIdleOpen");
            while (true)
            {
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "IdleOpenBox 1") break;
                yield return null;
            }

            buttonGroupObject.SetActive(true);
            //Debug.Log("Activate Button");
        }
    }
}