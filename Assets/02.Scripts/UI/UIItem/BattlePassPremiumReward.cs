using System.Collections;
using System.Collections.Generic;
using Framework.Network;
using UnityEngine;

namespace Framework.UI
{
    public class BattlePassPremiumReward : BattlePassItemReward
    {
        public bool isPremiumActive;

        public override void Initialize(BattlePassReward data, int userLevel)
        {
            base.Initialize(data, userLevel);

            lockReward.SetActive(!isPremiumActive);

            if (isPremiumActive)
            {
                button_GetItem.gameObject.SetActive(data.isAvailable);

                if (userLevel >= data.passLevel)
                {
                    //박스 레벨과 유저 레벨을 비교해서 박스레벨이 낮은경우
                    //true면 아직 못받은것 (data.isAvailable 변수 자체가 받을 수 있는지 없는지에 대한것)
                    //false 받았다고 표시해주는 ui 
                    checkReward.SetActive(!data.isAvailable);
                    inActiveObject.SetActive(false);
                }
                else
                {
                    checkReward.SetActive(data.isAvailable);
                    inActiveObject.SetActive(true);
                }
            }

        }

        public void PremiumLock(bool isPrimiumActive)
        {
            //프리미엄 활성화 체크
            isPremiumActive = isPrimiumActive;
        }
    }
}
