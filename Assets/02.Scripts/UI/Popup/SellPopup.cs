using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


namespace Framework.Game.Defense
{
    public class SellPopup : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler
    {
        public Character selectedCharacter;
        public TextMeshProUGUI text_SellPrice;
        

        public bool isMouseOver = false;

        public void SetSellPrice(Character character)
        {
            int price = (int)((Mathf.Pow(2, character.starGradeIndex) * character.sellPriceRatio) * 10);
            text_SellPrice.text = $"{price}";
            
        }

        public void OnMouseOver()
        {
            //Debug.Log("On Mouse Over");
            //Debug.Log("Sell Price");
            //Debug.Log(SetSellPrice(selectedCharacter));
            isMouseOver = false;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            isMouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isMouseOver = false;
        }

        public void SellCharacter()
        {
            if (selectedCharacter != null)
            {
                selectedCharacter.SellCharacter();
                selectedCharacter = null;
            }
        }
    }
}
