using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeStage.AntiCheat.ObscuredTypes;
using Framework.Network;

namespace Framework.UI
{
    public class DiscordEvent : MonoBehaviour
    {
        public Button button_Connect;
        public string linkedUrl;
        public GameObject gemIcon;
        public TextMeshProUGUI text_Amount;
        public bool isEventJoined;

        private void Start()
        {
            button_Connect.onClick.AddListener(() =>
            {
                Application.OpenURL(linkedUrl);
                if(!isEventJoined)
                {
                    _ = NetworkManager.Instance.ConnectDiscord(SetEventInfo);
                }
            });
        }

        public void SetEventInfo(DiscordConnect data)
        {
            linkedUrl = data.linkedUrl;

            isEventJoined = data.eventJoined;

            if(data.eventJoined)
            {
                gemIcon.SetActive(false);
            }
            else
            {
                gemIcon.SetActive(true);
                text_Amount.text = $"<sprite=2>{data.amount}";
            }
        }
    }
}