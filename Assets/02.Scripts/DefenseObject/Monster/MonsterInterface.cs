using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UniRx;
//using UniRx.Triggers;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Framework.Util;

namespace Framework.Game.Defense
{
    public class MonsterInterface : MonoBehaviour
    {
        public Monster targetMonster;

        public float maxEnergyValue;
        public float currentEnergyValue;
        public TextMeshProUGUI text_CurrentEnergyValue;
        public Image image_energyValue;
        public Image image_DelayedEnergyValue;
        public Image image_backGround;
        public IEnumerator delayedHp;

        public bool isAlive = false;
        
        //private void Start()
        //{
        //    this.UpdateAsObservable()
        //        .Where(_ => isAlive)
        //        .Subscribe(_ => transform.position = SetWorldPosition());
        //}

        private void Update()
        {
            if(isAlive)
            {
                transform.position = SetWorldPosition();
            }
        }

        public Vector3 SetWorldPosition()
        {
            Vector3 setVector = new(targetMonster.transform.localPosition.x, targetMonster.transform.localPosition.y + 0.4f, 0);

            return setVector;
        }

        public void Initialize(float maxEnergy)
        {
            this.maxEnergyValue = maxEnergy;
            image_DelayedEnergyValue.fillAmount = 1;
            UpdateValue(maxEnergyValue);
            isAlive = true;
        }

        public void UpdateValue(float currentValue)
        {
            currentValue = Mathf.RoundToInt(currentValue);
            currentEnergyValue = currentValue;
            float scaleValue = CalcEnergyBarScaling();

            string hp = Calculator.TruncateValue(currentValue);

            text_CurrentEnergyValue.text = hp;
            image_energyValue.fillAmount = scaleValue;
            StartCoroutine(DelayedHp(scaleValue));
        }

        public float CalcEnergyBarScaling()
        {
            float currentScalingValue = currentEnergyValue / maxEnergyValue;

            return currentScalingValue;
        }

        public void ReturnObjectPool()
        {
            delayedHp = null;
            isAlive = false;
        }

        // ???????? ???? ?????? hp
        IEnumerator DelayedHp(float scaleValue)
        {
            yield return new WaitForSeconds(0.03f);
            while (isAlive)
            {
                image_DelayedEnergyValue.fillAmount -= Time.deltaTime;
                if (image_DelayedEnergyValue.fillAmount <= scaleValue) break;
                yield return null;
            }
           
        }
    }
}
