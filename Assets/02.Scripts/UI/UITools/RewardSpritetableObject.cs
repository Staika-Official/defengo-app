using Framework.GameData.Defense;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardSpritetableObject", menuName = "ScriptableObject/RewardSprite")]
public class RewardSpritetableObject : ScriptableObject
{
    public SerializableDictionary<string, Sprite> dic_RewardSprite;
}

