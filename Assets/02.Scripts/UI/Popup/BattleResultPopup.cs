using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Game.Defense;

namespace Framework.UI
{
    public class BattleResultPopup : PopupTemplate
    {
        public TextMeshProUGUI text_rank;
        public TextMeshProUGUI text_wave;

        public void SetResultInfo(int rank, int wave)
        {
            text_rank.text = $"{rank}";
            text_wave.text = $"{wave}";

            ActivePopup();
        }

        public override void ActivePopup()
        {
            PopUpSequence(true);
        }

        public override void InActivePopup()
        {
            UIManager.Instance.battleResultTablePopup.ActivePopup();
            PopUpSequence(false);
        }

        public override void Initialize()
        {
            button_Close.onClick.AddListener(()=>
            {
                InActivePopup();
            });  
        }
    }
}