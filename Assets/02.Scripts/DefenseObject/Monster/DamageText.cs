using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Framework.Util;
using Framework.Game.Defense;
using Framework.GameData.Defense;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI text_DamageText;
    public float maxScaleValue;
    public float minScaleValue;
    public float scalingDuration;

    public void SetDamageText(float damageValue, DamageType damageType, Vector3 pos, Character character)
    {
        transform.position = pos;
        //Camera.main.WorldToScreenPoint(new Vector3(pos.x, pos.y + 1f, 0));
        float startY = transform.position.y;
        string textValue = Calculator.TruncateValue(damageValue);
        text_DamageText.color = Color.white;
        float maxValue = 0;
        switch (damageType)
        {
            case DamageType.NORMAL:
                text_DamageText.text = $"<color=#ffffff>{textValue}</color>";
                text_DamageText.fontSize = 40;
                maxValue = maxScaleValue;
                break;
            case DamageType.CRITICAL:
                text_DamageText.text = $"<sprite=4><color=#ff5b5b>{textValue}</color>";
                text_DamageText.fontSize = 55;

                maxValue = maxScaleValue * 1.2f;
                break;
            case DamageType.BLACKORB:
                text_DamageText.text = $"<sprite=5><color=#D19BFF>{textValue}</color>";
                maxValue = maxScaleValue * 1.2f;
                break;
            case DamageType.INSTANT_KILL:
                text_DamageText.text = $"<sprite=25><color=#e60002>{textValue}</color>";
                text_DamageText.fontSize = 55;
                maxValue = maxScaleValue * 1.2f;
                break;
            case DamageType.GAMBLING_KILL:
                text_DamageText.text = $"<sprite=28><color=#FFD648>{textValue}</color>";
                text_DamageText.fontSize = 55;
                maxValue = maxScaleValue * 1.2f;
                break;
        }

        if (character != null && character.IsInfected)
            text_DamageText.text = $"<sprite=28><color=#0EC24C>{textValue}</color>";

        transform.DOScale(minScaleValue, 0);
        transform.DOScale(maxValue, scalingDuration).SetEase(Ease.OutBounce);

        float yPos = Random.Range(0.7f, 1.3f);

        transform.DOMoveY(startY + yPos, 0.25f);

        text_DamageText.DOColor(Color.white, 0.26f).OnComplete(() => ReturnObjectPool());
    }

    public void ReturnObjectPool()
    {
        Color textColor = text_DamageText.color;
        transform.localScale = Vector2.one;
        text_DamageText.DOColor(new Color(textColor.r, textColor.g, textColor.b, 0), 0.25f).SetDelay(0.3f)
          .OnComplete(() => GameManager.Instance.objectPoolManager.ReturnObject(this, "DamageText"));
    }
}
