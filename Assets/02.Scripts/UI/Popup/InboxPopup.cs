using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Framework.Util;
using Framework.GameData.Defense;
using Framework.Network;
using Spine.Unity;
using System;

namespace Framework.UI
{
    public class InboxPopup : PopupTemplate
    {
        public Button button_AllClaim;
        public Button button_Delete;
        public RectTransform scrollRect;
        public RectTransform inactive;
        public List<InboxItem> activeInboxItem = new();
        public Queue<InboxItem> inboxItems = new();

        [Header("Mail Popup")]
        public GameObject mailPopup;
        public TextMeshProUGUI text_Title;
        public TextMeshProUGUI text_content;
        public Button button_MailPopupExit;

        [Header("Delete Popup")]
        public GameObject deletePopup;
        public Button button_DeleteAll;
        public Button button_Cancel;

        public GameObject[] deleteAll;

        public GameObject emptyObject;


        public override void ActivePopup()
        {
            mailPopup.SetActive(false);
            deletePopup.SetActive(false);
            RefreshInbox();
            SetInboxData(true);
        }

        public override void InActivePopup()
        {
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Delete.onClick.AddListener(() =>
            {
                deletePopup.SetActive(true);
            });

            button_Close.onClick.AddListener(() =>
            {
                InActivePopup();
            });

            button_DeleteAll.onClick.AddListener(() =>
            {
                OnClick_DeleteAll();
            });

            button_Cancel.onClick.AddListener(() =>
            {
                OnClick_DeleteCancel();
            });

            button_MailPopupExit.onClick.AddListener(() =>
            {
                OnClick_MailExit();
            });
        }

        public void SetInboxData(bool isActive)
        {
            _ = NetworkManager.Instance.GetUserInboxList(isActive, SetInboxContent, Failed);
        }

        public void SetInboxContent(UserInboxInfo data, bool isActive)
        {
            int count = 0;

            DateTime now = DateTime.Now;
            
            for (int i = 0; i < data.userInboxes.Length; i++)
            {
                DateTime expDate = DateTime.Parse(data.userInboxes[i].expiredDate);

                if (DateTime.Compare(expDate, now) < 0)
                {
                    continue;
                }
                else
                {
                    count++;
                }
            }

            RefreshInbox();
            float y = count * 175f;
            scrollRect.sizeDelta = new Vector2(scrollRect.sizeDelta.x, y);


            for (int i = 0; i < data.userInboxes.Length; i++)
            {
                DateTime expDate = DateTime.Parse(data.userInboxes[i].expiredDate);

                if (DateTime.Compare(expDate, now) < 0)
                {

                    continue;
                }

                InboxItem inboxItem = GetInboxItem();
                inboxItem.transform.SetParent(scrollRect);
                inboxItem.gameObject.SetActive(true);
                inboxItem.transform.localScale = Vector3.one;
                inboxItem.Initialize(data.userInboxes[i]);
                activeInboxItem.Add(inboxItem);
            }

            if(isActive)
            {
                PopUpSequence(true);
            }

            bool isNewPost = activeInboxItem.Count != 0;
            emptyObject.SetActive(!isNewPost);
            deleteAll[0].SetActive(isNewPost);
            deleteAll[1].SetActive(!isNewPost);
            button_Delete.interactable = isNewPost;
            button_DeleteAll.interactable = isNewPost;

            HomeScreen.Instance.postNoticeObject.SetActive(isNewPost);
        }
        
        public void GetRandomBox(string data, string value)
        {
            RandomBoxIndex randomBoxInbox = JsonUtility.FromJson<RandomBoxIndex>(data);

            RandomBoxPopup popup = PopupManager.Instance.GetPopUp<RandomBoxPopup>("randomBoxOpen");
            popup.ActivePopup();
            popup.RandomBoxOpen(randomBoxInbox, value);
            SetInboxData(true);
        }

        public void GetMail(string title, string data)
        {
            DetailInbox detailInbox = JsonUtility.FromJson<DetailInbox>(data);
            text_content.text = LanguageManager.Instance.languageKey == "ko-KR" ? detailInbox.contentKo : detailInbox.contentEn;
            text_Title.text = title;

            mailPopup.SetActive(true);
            SetInboxData(false);
        }

        public void GetAsset(string data)
        {
            DetailInbox detailInbox = JsonUtility.FromJson<DetailInbox>(data);
            InboxType inboxType = (InboxType)Enum.Parse(typeof(InboxType), detailInbox.inboxType);

            EarnPopup earnPopup = PopupManager.Instance.GetPopUp<EarnPopup>("earn");
            earnPopup.ActivePopup();
            earnPopup.SetAmountValue(inboxType, detailInbox.value, detailInbox.quantity);
            SetInboxData(false);
        }

        public void GetEventRandomBox(string data)
        {
            EventBoxResult eventBoxResult = JsonUtility.FromJson<EventBoxResult>(data);

            EventBoxOpenPopup popup = PopupManager.Instance.GetPopUp<EventBoxOpenPopup>("eventBoxOpen");

            popup.OpenRandomBox(eventBoxResult);
            _ = NetworkManager.Instance.GetUserWalletHistory();
            SetInboxData(false);
        }

        public InboxItem GetInboxItem()
        {
            if(inboxItems.Count == 0)
            {
                GameObject obj = Instantiate(activeInboxItem[0].gameObject);
                InboxItem inboxItem = obj.GetComponent<InboxItem>();
                return inboxItem;
            }
            else
            {
                return inboxItems.Dequeue();
            }
        }

        public void RefreshInbox()
        {
            for (int i = 0; i < activeInboxItem.Count; i++)
            {       
                activeInboxItem[i].gameObject.SetActive(false);
                //activeInboxItem[i].transform.SetParent(null);
                activeInboxItem[i].transform.SetParent(inactive);
                inboxItems.Enqueue(activeInboxItem[i]);
            }

            activeInboxItem.Clear();
        }

        public void Failed()
        {
            Debug.Log("Ranking Failed");
        }

        public void OnClick_MailExit()
        {
            mailPopup.SetActive(false);
        }

        public void OnClick_DeleteCancel()
        {
            deletePopup.SetActive(false);
        }

        public void OnClick_DeleteAll()
        {
            _ = NetworkManager.Instance.DeleteUserInbox(SetInboxContent, OnFailedDelete);
            deletePopup.SetActive(false);
        }

        //public void OnSuccessDelete()
        //{
        //    SetInboxContent()
        //}

        public void OnFailedDelete()
        {
            
        }
    }
}