using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Network;
using SimpleJSON;

namespace Framework.UI
{
    public enum DocumentType
    {
        TERMS,
        PRIVACY,
        MARKETING
    }

    public class JoinPopup : PopupTemplate
    {
        public GameObject terms;
        public GameObject privacy;
        public GameObject marketing;

        public GameObject[] openObject;
        public GameObject[] closeObject;

        public TextMeshProUGUI text_TermsOfUse;
        public TextMeshProUGUI text_PrivacyPolicy;
        public TextMeshProUGUI text_Marketing;

        public RectTransform rect_Terms;
        public RectTransform rect_Privacy;
        public RectTransform rect_Marketing;
        public RectTransform rect_ScrollView;

        public bool isTermActive = false;
        public Button button_Terms;
        public Button button_TermsCheck;

        public bool isPrivacyActive = false;
        public Button button_Privacy;
        public Button button_PrivacyCheck;

        public bool isMarketingActive = false;
        public Button button_Marketing;
        public Button button_MarketingCheck;

        public Button button_AllAccept;
        public Button button_Accept;

        public bool[] isAccept = new bool[3];
        public GameObject[] acceptCheck;

        public override void ActivePopup()
        {

        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            button_Terms.onClick.AddListener(() => OnClick_Collapse(DocumentType.TERMS));
            button_TermsCheck.onClick.AddListener(() => OnClick_Check(DocumentType.TERMS));
            button_Privacy.onClick.AddListener(() => OnClick_Collapse(DocumentType.PRIVACY));
            button_PrivacyCheck.onClick.AddListener(() => OnClick_Check(DocumentType.PRIVACY));
            button_Marketing.onClick.AddListener(() => OnClick_Collapse(DocumentType.MARKETING));
            button_MarketingCheck.onClick.AddListener(() => OnClick_Check(DocumentType.MARKETING));

            button_AllAccept.onClick.AddListener(() => OnClick_AllAccept());
            button_Accept.onClick.AddListener(() => OnClick_Accept());
        }

        public void OnClick_Check(DocumentType document)
        {
            int idx = (int)document;

            isAccept[idx] = !isAccept[idx];
            acceptCheck[idx].SetActive(isAccept[idx]);
            CheckAccept();
        }

        public void OnClick_AllAccept()
        {
            for (int i = 0; i < isAccept.Length; i++)
            {
                isAccept[i] = true;
                acceptCheck[i].SetActive(true);
            }

            SetHistoryData();
        }

        public void OnClick_Accept()
        {
            SetHistoryData();
        }

        public string Encoding(string data)
        {
            string encodedData = UnityEngine.Networking.UnityWebRequest.EscapeURL(data);
            return encodedData;
        }

        public void SetHistoryData()
        {
            List<TermsHistory> termsHistories = new();

            if (isAccept[0])
            {
                TermsHistory termsData = new()
                {
                    terms = UserInfoManager.Instance.dic_BoardList["TERMS"].title,
                    boardType = "TERMS",
                    postId = UserInfoManager.Instance.dic_BoardList["TERMS"].postId,
                    activated = isAccept[0],
                    clientId = "DEFENGO"
                };
                termsHistories.Add(termsData);
            }

            if (isAccept[1])
            {
                TermsHistory termsData = new()
                {
                    terms = UserInfoManager.Instance.dic_BoardList["PRIVACY"].title,
                    boardType = "PRIVACY",
                    postId = UserInfoManager.Instance.dic_BoardList["PRIVACY"].postId,
                    activated = isAccept[1],
                    clientId = "DEFENGO"
                };
                termsHistories.Add(termsData);
            }

            if (isAccept[2])
            {
                TermsHistory termsData = new()
                {
                    terms = UserInfoManager.Instance.dic_BoardList["MARKETING"].title,
                    boardType = "MARKETING",
                    postId = UserInfoManager.Instance.dic_BoardList["MARKETING"].postId,
                    activated = isAccept[2],
                    clientId = "DEFENGO"
                };
                termsHistories.Add(termsData);
            }

            string test = "";

            for (int i = 0; i < termsHistories.Count; i++)
            {
                string json = JsonUtility.ToJson(termsHistories[i]);
                if (i != termsHistories.Count - 1)
                {
                    json += ',';
                }
                test += json;
            }
            string data = $"[{test}]";

            _ = NetworkManager.Instance.TermsHistory(data);
        }

        public void CheckAccept()
        {
            for (int i = 0; i < isAccept.Length - 1; i++)
            {
                if (!isAccept[i])
                {
                    button_Accept.interactable = false;
                    break;
                }

                button_Accept.interactable = true;
            }
        }

        public void OnClick_Collapse(DocumentType document)
        {
            switch (document)
            {
                case DocumentType.TERMS:
                    isTermActive = !isTermActive;
                    openObject[0].SetActive(!isTermActive);
                    closeObject[0].SetActive(isTermActive);
                    if (isTermActive)
                    {
                        terms.SetActive(isTermActive);
                        rect_Terms.sizeDelta = new(900, text_TermsOfUse.preferredHeight);
                        rect_ScrollView.sizeDelta = new(0, rect_ScrollView.sizeDelta.y + rect_Terms.sizeDelta.y);
                    }
                    else
                    {
                        terms.SetActive(isTermActive);
                        rect_ScrollView.sizeDelta = new(0, rect_ScrollView.sizeDelta.y - rect_Terms.sizeDelta.y);
                    }
                    break;
                case DocumentType.PRIVACY:
                    isPrivacyActive = !isPrivacyActive;
                    openObject[1].SetActive(!isPrivacyActive);
                    closeObject[1].SetActive(isPrivacyActive);
                    if (isPrivacyActive)
                    {
                        privacy.SetActive(isPrivacyActive);
                        rect_Privacy.sizeDelta = new(900, text_PrivacyPolicy.preferredHeight);
                        rect_ScrollView.sizeDelta = new(0, rect_ScrollView.sizeDelta.y + rect_Privacy.sizeDelta.y);
                    }
                    else
                    {
                        privacy.SetActive(isPrivacyActive);
                        rect_ScrollView.sizeDelta = new(0, rect_ScrollView.sizeDelta.y - rect_Privacy.sizeDelta.y);
                    }
                    break;
                case DocumentType.MARKETING:
                    isMarketingActive = !isMarketingActive;
                    openObject[2].SetActive(!isMarketingActive);
                    closeObject[2].SetActive(isMarketingActive);
                    if (isMarketingActive)
                    {
                        marketing.SetActive(isMarketingActive);
                        rect_Marketing.sizeDelta = new(900, text_Marketing.preferredHeight);
                        rect_ScrollView.sizeDelta = new(0, rect_ScrollView.sizeDelta.y + rect_Marketing.sizeDelta.y);
                    }
                    else
                    {
                        marketing.SetActive(isMarketingActive);
                        rect_ScrollView.sizeDelta = new(0, rect_ScrollView.sizeDelta.y - rect_Marketing.sizeDelta.y);
                    }
                    break;
            }
        }
        public void SetJoinPopup()
        {
            Dictionary<string, BoardList> data = UserInfoManager.Instance.dic_BoardList;

            text_TermsOfUse.text = data["TERMS"].content;
            text_PrivacyPolicy.text = data["PRIVACY"].content;
            text_Marketing.text = data["MARKETING"].content;

            rect_Terms.sizeDelta = new(900, text_TermsOfUse.preferredHeight);
            rect_Privacy.sizeDelta = new(900, text_PrivacyPolicy.preferredHeight);
            rect_Marketing.sizeDelta = new(900, text_Marketing.preferredHeight);

            terms.SetActive(false);
            privacy.SetActive(false);
            marketing.SetActive(false);
            CheckAccept();

            PopUpSequence(true);
        }

        public void SetJoinPopup(Dictionary<string, BoardList> data)
        {
            text_TermsOfUse.text = data["TERMS"].content;
            text_PrivacyPolicy.text = data["PRIVACY"].content;
            text_Marketing.text = data["MARKETING"].content;

            rect_Terms.sizeDelta = new(900, text_TermsOfUse.preferredHeight);
            rect_Privacy.sizeDelta = new(900, text_PrivacyPolicy.preferredHeight);
            rect_Marketing.sizeDelta = new(900, text_Marketing.preferredHeight);

            terms.SetActive(false);
            privacy.SetActive(false);
            marketing.SetActive(false);
            CheckAccept();
        }
    }
}
