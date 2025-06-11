using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using Framework.UI;

namespace Framework.Util
{
    public enum Language
    {
        EN,
        KO
    }

    public class LanguageManager : MonoBehaviour
    {
        public static LanguageManager Instance;
        public Language language;
        public string languageKey;
        public string nationalKey;
        public StringTable currentTable;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                Initialize();
            }
        }

        public string GetLanguageKey()
        {
            return nationalKey;
        }

        public void Initialize()
        {
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                language = Language.KO;
                languageKey = "ko-KR";
                nationalKey = "ko";
            }
            else
            {
                language = Language.EN;
                languageKey = "en-US";
                nationalKey = "en";
            }

            // language = Language.EN;
            // languageKey = "en-US";
            // nationalKey = "en";

            //SetLocalization();

            SetStringTable(languageKey);
        }

        public void TestInitialize()
        {

        }

        public void SetLocalization()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.English:
                    language = Language.EN;
                    languageKey = "en-US";
                    nationalKey = "en";
                    break;
                case SystemLanguage.Indonesian:
                    //language = Language.ID;
                    //languageKey = "";
                    //nationalKey = "id";
                    break;
                case SystemLanguage.Korean:
                    language = Language.KO;
                    languageKey = "ko-KR";
                    nationalKey = "ko";
                    break;
                case SystemLanguage.Portuguese:
                    //language = Language.PT;
                    //languageKey = "";
                    //nationalKey = "pt";
                    break;
                case SystemLanguage.Spanish:
                    //language = Language.ES;
                    //languageKey = "";
                    //nationalKey = "es";
                    break;
                case SystemLanguage.Thai:
                    //language = Language.TH;
                    //languageKey = "";
                    //nationalKey = "th";
                    break;
                case SystemLanguage.Vietnamese:
                    //language = Language.VN;
                    //languageKey = "";
                    //nationalKey = "vn";
                    break;
                case SystemLanguage.Unknown:
                    //필리핀 유니티에서 지원안해줌
                    //language = Language.TL;
                    //languageKey = "";
                    //nationalKey = "tl";
                    break;
                default:
                    language = Language.EN;
                    languageKey = "en-US";
                    nationalKey = "en";
                    break;
            }
        }

        // [MenuItem("Assets/Create/Localization/Tagalog Locale")]
        // public static void CreateTagalogLocale()
        // {
        //     var path = EditorUtility.SaveFilePanelInProject("Save Tagalog Locale", "Tagalog (tl)", "asset", "Save Tagalog Locale", "Assets/");
        //     if (string.IsNullOrEmpty(path))
        //         return;
        //
        //     var locale = ScriptableObject.CreateInstance<Locale>();
        //     locale.Identifier = "tl";
        //     locale.name = "Tagalog";
        //     AssetDatabase.CreateAsset(locale, path);
        // }

        public void SetLanguageStringTable(string key)
        {
            LocaleIdentifier localeCode = new(key);
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                Locale locale = LocalizationSettings.AvailableLocales.Locales[i];
                LocaleIdentifier identifier = locale.Identifier;

                if (identifier == localeCode)
                {
                    LocalizationSettings.SelectedLocale = locale;
                    break;
                }
            }
        }

        public async void SetStringTable(string key)
        {
            LocalizedStringTable table = new() { TableReference = "UI_Text" };
            var data = table.GetTableAsync();
            await data.Task;

            if (data.IsDone && data.Status == AsyncOperationStatus.Succeeded)
            {
                currentTable = data.Result;
            }

            SetLanguageStringTable(key);
        }

        public string GetStringData(string dataKey)
        {
            var data = LocalizationSettings.AvailableLocales.GetLocale(languageKey);

            string localData = LocalizationSettings.StringDatabase.GetLocalizedString("UI_Text", dataKey, data);
            return localData;
        }

        public bool Cotains(string dataKey)
        {
            string text = GetStringData(dataKey);
            return text.Contains("No translation found for") == false;
        }

        public string GetPriceStringData(GameData.Defense.MoneyType moneyType, float value)
        {
            switch (moneyType)
            {
                case GameData.Defense.MoneyType.STIK:
                    return "<sprite=14>" + value.ToString("F1");

                case GameData.Defense.MoneyType.ADMOB:
                    {
                        if (value == 0f)
                            return "<sprite=6>" + "Free";
                        else
                            return "<sprite=6>" + value.ToString();
                    }

                case GameData.Defense.MoneyType.GEM:
                    return "<sprite=2>" + value.ToString();

                default:
                    return value.ToString();
            }
        }
    }
}


