using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Game.Defense
{
    public class DebuffGround : MonoBehaviour
    {
        public SpriteRenderer spriterenderer;

        public void SortedSprite(int sortLayerIndex)
        {
            spriterenderer.sortingOrder = sortLayerIndex;
        }
    }
}
