using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Framework.GameData.Defense;

namespace Framework.Util
{
    public static class Calculator
    {
        public static Vector2 TransformationVector(int idx)
        {
            float x = idx % 20;
            float y = idx / 20;

            Vector2 vector = new(x, y);

            return vector;
        }

        public static float DistanceCheck(Vector2 normalVec)
        {
            float dis = normalVec.sqrMagnitude;

            return dis;
        }

        public static float GetAnimationSpeed(float attackSpeed, float duration)
        {
            float animSpeed;

            if ((1 / attackSpeed) > duration)
            {
                animSpeed = 1;
            }
            else
            {
                animSpeed = attackSpeed;
            }
            return animSpeed;
        }

        public static string TruncateValue(float value)
        {
            string valueUnit = "";
            int unitState = value < 1000 ? 0 : value >= 1000000 ? 2 : 1;
            float truncatedValue = 0;
            switch (unitState)
            {
                case 0:
                    truncatedValue = Mathf.RoundToInt(value);
                    valueUnit = "";
                    break;
                case 1:
                    truncatedValue = value / 1000;
                    truncatedValue = (float)Math.Truncate(truncatedValue * 10) / 10;
                    valueUnit = "K";
                    break;
                case 2:
                    truncatedValue = value / 1000000;
                    truncatedValue = (float)Math.Truncate(truncatedValue * 10) / 10;
                    valueUnit = "M";
                    break;
            }

            string truncateValue = truncatedValue.ToString() + valueUnit;
            return truncateValue;
        }

        public static ObscuredFloat SynthesisAttackValue(ObscuredInt grade, ObscuredFloat attackDamage) // 별 등급
        {
            double value = attackDamage * (Math.Pow(ConfigData.STARGRADE_FACTOR, grade));

            return (float)value;
        }

        public static float UpgradeAttackValue(ObscuredInt upgradeIndex, ObscuredFloat upgradeFactor, ObscuredInt starGrade) //레
        {
            float value = ((upgradeIndex - 1) * upgradeFactor * starGrade) + (upgradeIndex - 1);
            return value;
        }

        public static double GetTime(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public static DateTime DateTimeParse(double milliseconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds).ToLocalTime();
        }


        public static Vector3 BezierCurves(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float time)
        {
            Vector3 m0 = Vector3.Lerp(p0, p1, time);
            Vector3 m1 = Vector3.Lerp(p1, p2, time);
            Vector3 m2 = Vector3.Lerp(p2, p3, time);

            Vector3 b0 = Vector3.Lerp(m0, m1, time);
            Vector3 b1 = Vector3.Lerp(m1, m2, time);

            return Vector3.Lerp(b0, b1, time);
        }
        
        public static int[] GetExpiredTime(DateTime dateTime)
        {
            TimeSpan expiredDate = dateTime - DateTime.Now;

            int[] temp = new int[3];
            temp[0] = expiredDate.Days;
            temp[1] = expiredDate.Hours;
            temp[2] = expiredDate.Minutes;

            return temp;
        }
    }
}


