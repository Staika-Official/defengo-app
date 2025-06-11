using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SimpleJSON;
using Framework.Network;
using Framework.Util;
using System;
using Framework.UI;

namespace Framework.Login
{
    public class NoticePopup : PopupTemplate
    {
        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_Content;
        public TextMeshProUGUI text_Date;

        public override void ActivePopup()
        {
            Debug.Log("Notice Active");
            GetNoticeContent();
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(() => InActivePopup());
        }

        public async void GetNoticeContent()
        {
            await NetworkManager.Instance.GetNoticeContent(SuccessNotice);
        }

        public void SetNoticePopup(MaintenaceDetail data)
        {
            LoginManager.Instance.button_AppleLogin.onPointerUp = () =>
            {
                PopUpSequence(true);
            };
            LoginManager.Instance.button_GoogleLogin.onPointerUp = () =>
            {
                PopUpSequence(true);
            };
            LoginManager.Instance.button_GuestLogin.onPointerUp = () =>
            {
                PopUpSequence(true);
            };
            DateTime from = Calculator.DateTimeParse(double.Parse(data.fromDateTime));
            DateTime to = Calculator.DateTimeParse(double.Parse(data.toDateTime));

            text_Date.text = $"{from:HH:mm} ~ {to:HH:mm}";

            text_Title.text = Application.systemLanguage switch
            {
                SystemLanguage.Korean => data.titleKr,
                _ => data.titleEn
            };

        
            text_Content.text = Application.systemLanguage switch
            {
                SystemLanguage.Korean => data.descriptionKr,
                _ => data.descriptionEn
            };

            PopUpSequence(true);
        }

        public void SuccessNotice(string data)
        {
            Debug.Log(data);

            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            string contentKey = Application.systemLanguage switch
            {
                SystemLanguage.Korean => "contentKo",
                _ => "contentEn",
            };

            Debug.Log(data);

            JSONNode value = JSONNode.Parse(data);

            Debug.Log(value.Count);

            if (value.Count > 1)
            {
                for (int i = 0; i < value.Count; i++)
                {
                    if (value[i]["type"] == "INSPECTION")
                    {
                        string title = value[i]["label"];
                        Debug.Log(title);
                        string content = value[i][contentKey];
                        Debug.Log(content);
                        DateTime startDate = DateTime.Parse(value[i]["displayFromDate"]);
                        DateTime endDate = DateTime.Parse(value[i]["displayToDate"]);

                        string start = startDate.ToString("HH:mm");
                        string end = endDate.ToString("HH:mm");

                        text_Title.text = title;
                        text_Content.text = content;
                        text_Date.text = $"{start}~{end}";

                        double now = Calculator.GetTime(DateTime.Now);

                        double startTime = Calculator.GetTime(startDate);
                        //Debug.Log(startTime);
                        double endTime = Calculator.GetTime(endDate);
                        //Debug.Log(endTime);

                        if (now > startTime && now < endTime)
                        {
                            Debug.Log("Maintenance Now");
                            LoginManager.Instance.OnMainTenance();
                            PopUpSequence(true);
                        }
                        else if (now > endTime)
                        {
                            //Debug.Log("Maintenance End");
                        }
                        else if (now < startTime)
                        {
                            //Debug.Log("Before Maintenance");
                            PopUpSequence(true);
                        }

                        break;
                    }
                }
            }
            else
            {
                string title = value[0]["label"];
                Debug.Log(title);
                string[] contents = (value[0][contentKey].ToString()).Split(',');
                Debug.Log(contents.Length);

                DateTime startDate = DateTime.Parse(contents[1]);
                DateTime endDate = DateTime.Parse(contents[2].Replace('"', ' '));

                string start = startDate.ToString("HH:mm");
                string end = endDate.ToString("HH:mm");
                text_Title.text = title;
                text_Content.text = string.Format(contents[0].Replace('"', ' '), start, end);
                PopUpSequence(true);
                button_Close.onClick.AddListener(() => LoginManager.Instance.CheckToken());
            }
        }
    }
}
