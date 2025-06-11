using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Game.Defense
{
    public class RenderSortManager : MonoBehaviour
    {
        public readonly int defalutSortorderLayer = 2;
        public readonly int sortOrderLayer = 3;
        public Stack<DebuffGround> debuffGrounds = new();


        public void Initialized()
        {
        }

        public void AddDebuffSortLayer(DebuffGround debuffGround)
        {
            //장판이 1개라도 있다면 이전 장판의 레이어를 정상적으로 돌려준다
            if (debuffGrounds.Count > 0)
                debuffGrounds.Pop().SortedSprite(defalutSortorderLayer);

            //장판을 관리하면서 새로운 장판이 생성될 때 새로운 장판의 레이어를 최신으로 바꿔줌
            debuffGrounds.Push(debuffGround);
            debuffGrounds.Peek().SortedSprite(sortOrderLayer);
        }

        //public void RemoveDebuffSortLayer()
        //{
        //    //장판이 사라질 때 불리는 함수 인데 방어코드로 디폴트값으로 돌려주기위함
        //    //잘못생각함 위에 add부분에서 지우고 넣어주고 지우고넣어주고하는데 여기서 리무브가불리면
        //    //최신으로 설정된 오더 레이어값도 2가 됌 
        //    //if (debuffGrounds.Count > 0)
        //        //debuffGrounds.Pop().SortedSprite(defalutSortorderLayer);
        //}

    }
}
