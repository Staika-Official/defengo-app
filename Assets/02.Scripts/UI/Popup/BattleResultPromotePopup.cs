using System.Collections;
using System.Collections.Generic;
using Framework.Game.Defense;
using UnityEngine;

namespace Framework.UI
{
    public class BattleResultPromotePopup : PopupTemplate
    {
        public Animator animator;
        
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
            button_Close.onClick.AddListener(() =>
            {
                InActivePopup();
            });
        }
    }
}

