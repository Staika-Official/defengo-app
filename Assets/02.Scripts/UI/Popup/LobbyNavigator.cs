using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.GameData.Defense;
using DG.Tweening;
using Framework.Sound;

namespace Framework.UI
{
    [System.Serializable]
    public class BottomProperty
    {
        public MenuType menuType;
        public Button button;
        public GameObject image_Menu;
        public Transform icon_Pos;
        public GameObject onNoticeObject;
        public GameObject offNoticeObject;
    }

    public class LobbyNavigator : MonoBehaviour
    {
        public static LobbyNavigator Instance;
        public SerializableDictionary<MenuType, BottomProperty> dic_bottomProperty;
        public Ease setEase;

        public float maxUpperPosition;
        public float duration;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            foreach (BottomProperty item in dic_bottomProperty.Values)
            {
                item.button.onClick.AddListener(() => OnClick_Screen(item.menuType));
            }
        }

        public void Notification(MenuType menuType, bool isActive)
        {
            dic_bottomProperty[menuType].offNoticeObject.SetActive(isActive);
            dic_bottomProperty[menuType].onNoticeObject.SetActive(isActive);

        }

        public void OnClick_RankingScreen(MenuType menuType)
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            LobbyManager.Instance.ActiveScreen(menuType);
        }

        public void OnClick_Screen(MenuType menuType)
        {
            SoundManager.Instance.PlaySound(SoundKey.SF_CLICK);
            LobbyManager.Instance.ActiveScreen(menuType);

            foreach (BottomProperty item in dic_bottomProperty.Values)
            {
                bool isActive = item.menuType == menuType;
                item.image_Menu.SetActive(isActive);
            }
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}
