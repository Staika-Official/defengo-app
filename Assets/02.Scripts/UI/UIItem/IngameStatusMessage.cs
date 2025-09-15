using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Framework.UI
{
    public enum MessageState
    {
        WAVE,
        CHARACTER
    }

    public class IngameStatusMessage : MonoBehaviour
    {
        public Animation openAnimation;
        public TextMeshProUGUI text_message;
        public TextMeshProUGUI text_nickname;
        public MessageState messageState;

        public void SetMessage(string message, string nickname)
        {
            openAnimation.Play();
            switch (messageState)
            {
                case MessageState.WAVE:
                    break;
                case MessageState.CHARACTER:
                    break;
            }
            text_message.text = message;
            text_nickname.text = nickname;
        }

        public void OnCompleteAnim()
        {
            gameObject.SetActive(false);
        }
    }
}