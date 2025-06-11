using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Network;
using TMPro;
using UnityEngine.UI;
using Framework.GameData.Defense;

namespace Framework.UI
{
    public class BattlePassLevel : MonoBehaviour
    {
        public Image image_Level_Shadow;

        public Image image_Level_EndOutBottomFrame;
        public Image image_Level_EndOutUpFrame;

        public Image image_Level_MiddleOutBottomFrame;
        public Image image_Level_FirstFrame;
        public Image image_Level_MainFrame;

        //public Material mat_NextLevel;

        public TextMeshProUGUI text_Level;

        public void Initialize(BattlePassLevelType levelType, int level)
        {
            PassLevelColorInfo colorInfo = DataManager.Instance.uiPropertyData.dic_BattlePassLevelColor[levelType];

            image_Level_Shadow.color = colorInfo.color_Shadow;
            image_Level_EndOutBottomFrame.color = colorInfo.color_EndOutBottomFrame;
            image_Level_EndOutUpFrame.color = colorInfo.color_EndOutUpFrame;
            image_Level_MiddleOutBottomFrame.color = colorInfo.color_MiddleOutBottom;
            image_Level_FirstFrame.color = colorInfo.color_FirstFrame;
            image_Level_MainFrame.color = colorInfo.color_MainFrame;

            text_Level.color = colorInfo.color_Text;
            text_Level.text = $"{level}";
            text_Level.fontMaterial = colorInfo.mat_Text;
        }
    }
}
