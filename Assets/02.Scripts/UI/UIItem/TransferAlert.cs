using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Framework.Util;
using System;

namespace Framework.UI
{
    public class TransferAlert : MonoBehaviour
    {
        public TextMeshProUGUI text_AlertMessage;
        public TextMeshProUGUI text_StikBalance;
        public TextMeshProUGUI text_StatusMessage;

        public GameObject successMark;
        public GameObject FailedMark;

        public IEnumerator SetCount()
        {
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
        }

        public void SetSwapAlert(string data, bool isKey)
        {
            successMark.SetActive(true);
            FailedMark.SetActive(false);
            text_AlertMessage.gameObject.SetActive(false);
            text_StikBalance.gameObject.SetActive(false);
            text_StatusMessage.gameObject.SetActive(true);

            if (isKey)
            {
                string message = LanguageManager.Instance.GetStringData(data);
                text_StatusMessage.text = message;
            }
            else
            {
                text_StatusMessage.text = data;
            }
        }

        public void SetFailedAlert(string data, bool isKey)
        {
            successMark.SetActive(false);
            FailedMark.SetActive(true);
            text_AlertMessage.gameObject.SetActive(false);
            text_StikBalance.gameObject.SetActive(false);
            text_StatusMessage.gameObject.SetActive(true);

            if(isKey)
            {
                string message = LanguageManager.Instance.GetStringData(data);
                text_StatusMessage.text = message;
            }
            else
            {
                text_StatusMessage.text = data;
            }
        }

        public void SetSuccessAlert(string data, double balance, bool isKey)
        {
            successMark.SetActive(true);
            FailedMark.SetActive(false);
            text_AlertMessage.gameObject.SetActive(true);
            text_StikBalance.gameObject.SetActive(true);
            text_StatusMessage.gameObject.SetActive(false);

            string stikBalance = string.Format("{0:0.#######0}", balance);

            if (isKey)
            {
                string message = LanguageManager.Instance.GetStringData(data);
                text_AlertMessage.text = message;
                text_StikBalance.text = $"-{stikBalance} Tik";
            }
            else
            {
                text_AlertMessage.text = data;
                text_StikBalance.text = $"-{stikBalance} Tik";
            }
        }

        public void EndAnim()
        {
            gameObject.SetActive(false);
        }
    }
}