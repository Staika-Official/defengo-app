using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Framework.Util;
using Framework.GameData.Defense;

namespace Framework.Game.Defense
{
    public class BuffManager : MonoBehaviour
    {
        public Dictionary<string, UnityAction<float,float,float, int>> dicBuffAction = new();
        public float increaseCriticalRate;

        private void Start()
        {
            dicBuffAction.Add("attackValue", Temp);
            dicBuffAction.Add("attackSpeed", AttackDamageBuff);
        }

        public void Initialize()
        {

        }

        public void AttackDamageBuff(float range, float value, float duration, int glacierIdx)
        {
            StartCoroutine(AttackDamageBuffSync(range, value, duration, glacierIdx));
        }

        public IEnumerator AttackDamageBuffSync(float range, float value, float duration, int glacierIdx)
        {
            float currentRange = Mathf.Pow(range, 2);
            List<Character> characters = new();
            Vector2 pos = Calculator.TransformationVector(glacierIdx);

            for (int i = 0; i < GameManager.Instance.characterSpawner.summonedCharacters.Count; i++)
            {
                Character character = GameManager.Instance.characterSpawner.summonedCharacters[i];
                float distance = Calculator.DistanceCheck(pos - character.currentGridPos);
                if(distance <= currentRange && character.isAttackType)
                {
                    characters.Add(character);
                    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("GetBuff");
                    particle.PlayParticle(character.transform.localPosition, duration);
                    character.AttackValueBuff(true, value);
                }
            }

            yield return new WaitForSeconds(duration);

            for (int i = 0; i < characters.Count; i++)
            {
                characters[i].AttackValueBuff(false, value);
            }
        }

        public void SetFieldBuff(CharacterData data)
        {
            switch (data.fieldBuffType)
            {
                case FieldBuffType.NONE:
                    break;
                case FieldBuffType.INCREASE_CRITICAL_RATE:
                    increaseCriticalRate = 0.01f * data.characterClassLevel;
                    break;
                case FieldBuffType.DECREASE_DEFENSE_VALUE:
                    break;
                case FieldBuffType.INCREASE_MISSION_REWARD:
                    break;
                case FieldBuffType.INCREASE_BONUS_RATE:
                    break;
                case FieldBuffType.INCREASE_BONUS_SPEED:
                    break;
                default:
                    break;
            }
        }

        public void HighnickelStun(Monster target, float seconds)
        {
            StartCoroutine(HighnickelSequence(target, seconds));
        }

        public IEnumerator HighnickelSequence(Monster target, float seconds)
        {
            if (target.IsAlive)
            {
                ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("ElectroHit");
                particle.PlayParticle(target.transform.localPosition, seconds);

                target.IsMove = false;

                yield return new WaitForSeconds(seconds);
                if (target.IsAlive)
                {
                    target.IsMove = true;
                }
            }
            else
            {
                yield return null;
            }
        }

        public void Temp(float range, float value, float duration, int glacierIdx)
        {
            Debug.Log($"{range} {value}");
        }
    }
}
