using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using Framework.Network;

namespace Framework.UI
{
    public class TermsPopup : PopupTemplate
    {
        public TextMeshProUGUI text_Terms;
        public TextMeshProUGUI text_privacy;
        public TextMeshProUGUI text_Marketing;

        public Button button_Terms;
        public Button button_Privacy;
        public Button button_Marketing;

        public bool isTermsActive;
        public RectTransform rect_Terms;
        public bool isPrivacyActive;
        public RectTransform rect_Privacy;
        public bool isMarketingActive;
        public RectTransform rect_Marketing;
        public RectTransform rect_ScrollView;
        public bool hasTermsData = false;

        public override void ActivePopup()
        {
            if (hasTermsData)
            {
                PopUpSequence(true);
            }
            else
            {
                _ = NetworkManager.Instance.GetBoardContent(OnSuccessGetData, OnFailedGetData);
            }
        }

        public void OnSuccessGetData()
        {
            SetTerms();
            PopUpSequence(true);
        }

        public void OnFailedGetData()
        {
            Debug.Log("Failed");
        }

        public override void InActivePopup()
        {

        }

        public override void Initialize()
        {
            hasTermsData = UserInfoManager.Instance.dic_BoardList.ContainsKey("TERMS");

            button_Close.onClick.AddListener(() => PopUpSequence(false));

            rect_Terms.gameObject.SetActive(true);
            rect_Privacy.gameObject.SetActive(true);
            rect_Marketing.gameObject.SetActive(true);

            rect_Terms.gameObject.SetActive(false);
            rect_Privacy.gameObject.SetActive(false);
            rect_Marketing.gameObject.SetActive(false);

            button_Terms.onClick.AddListener(() => OnClick_Terms());
            button_Privacy.onClick.AddListener(() => OnClick_Privacy());
            button_Marketing.onClick.AddListener(() => OnClick_Marketing());
        }

        public void SetTerms()
        {
            text_Terms.text = UserInfoManager.Instance.dic_BoardList["TERMS"].content;
            text_privacy.text = UserInfoManager.Instance.dic_BoardList["PRIVACY"].content;
            text_Marketing.text = UserInfoManager.Instance.dic_BoardList["MARKETING"].content;
            rect_Terms.sizeDelta = new(850, text_Terms.preferredHeight);
            rect_Privacy.sizeDelta = new(850, text_privacy.preferredHeight);
            rect_Marketing.sizeDelta = new(850, text_Marketing.preferredHeight);
        }

        public void OnClick_Terms()
        {
            isTermsActive = !isTermsActive;

            if (isTermsActive)
            {
                rect_Terms.gameObject.SetActive(isTermsActive);
                rect_ScrollView.sizeDelta = new(850, rect_ScrollView.sizeDelta.y + rect_Terms.sizeDelta.y);
            }
            else
            {
                rect_Terms.gameObject.SetActive(isTermsActive);
                rect_ScrollView.sizeDelta = new(850, rect_ScrollView.sizeDelta.y - rect_Terms.sizeDelta.y);
            }
        }

        public void OnClick_Privacy()
        {
            isPrivacyActive = !isPrivacyActive;
            if (isPrivacyActive)
            {
                rect_Privacy.gameObject.SetActive(isPrivacyActive);
                rect_ScrollView.sizeDelta = new(850, rect_ScrollView.sizeDelta.y + rect_Privacy.sizeDelta.y);
            }
            else
            {
                rect_Privacy.gameObject.SetActive(isPrivacyActive);
                rect_ScrollView.sizeDelta = new(850, rect_ScrollView.sizeDelta.y - rect_Privacy.sizeDelta.y);
            }
        }

        public void OnClick_Marketing()
        {
            isMarketingActive = !isMarketingActive;

            if (isMarketingActive)
            {
                rect_Marketing.gameObject.SetActive(isMarketingActive);
                rect_ScrollView.sizeDelta = new(850, rect_ScrollView.sizeDelta.y + rect_Marketing.sizeDelta.y);
            }
            else
            {
                rect_Marketing.gameObject.SetActive(isMarketingActive);
                rect_ScrollView.sizeDelta = new(850, rect_ScrollView.sizeDelta.y - rect_Marketing.sizeDelta.y);
            }
        }
    }
}