using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.GameData.Defense
{
    [Serializable]
    public class ClassUpRule
    {
        public int classLevel;
        public int gemPrice;
        public double stikPrice;
        public int normalQuantity;
        public int rareQuantity;
        public int uniqueQuantity;
        public int epicQuantity;
        public int legendQuantity;
    }

    public class CharacterClassManager : MonoBehaviour
    {
        public static CharacterClassManager Instance;

        private Dictionary<int, ClassUpRule> classUpRuleDic;
        private int maxClassLevel = 0;
        public int GetMaxClassLevel() => maxClassLevel;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        
        public void Initialize(ClassUpRule[] classUpRule)
        {
            if (classUpRule != null)
            {
                classUpRuleDic = classUpRule.ToDictionary(x => x.classLevel, x => x);
                maxClassLevel = classUpRuleDic.Keys.Max() + 1; // MaxLevel
            }
        }

        public int GetClassUpGemPrice(int level)
        {
            if (level == 0)
                return classUpRuleDic[1].gemPrice;
            else if (classUpRuleDic.ContainsKey(level))
                return classUpRuleDic[level].gemPrice;
            else
                return 0;
        }
        
        public double GetClassUpStikPrice(int level)
        {
            if (level == 0)
                return classUpRuleDic[1].stikPrice;
            else if (classUpRuleDic.ContainsKey(level))
                return classUpRuleDic[level].stikPrice;
            else
                return 0;
        }

        public int GetClassUpInfo(CharacterGrade grade, int level)
        {
            if (classUpRuleDic.ContainsKey(level))
            {
                switch (grade)
                {
                    case CharacterGrade.COMMON:
                        return classUpRuleDic[level].normalQuantity;
                    case CharacterGrade.UNCOMMON:
                        return classUpRuleDic[level].rareQuantity;
                    case CharacterGrade.RARE:
                        return classUpRuleDic[level].uniqueQuantity;
                    case CharacterGrade.EPIC:
                        return classUpRuleDic[level].epicQuantity;
                    case CharacterGrade.LEGENDARY:
                        return classUpRuleDic[level].legendQuantity;
                    default:
                        return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}
