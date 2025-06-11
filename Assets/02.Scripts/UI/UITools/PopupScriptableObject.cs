using Framework.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "PopupScriptableObject", menuName = "ScriptableObject/Popup")]
public class PopupScriptableObject : ScriptableObject
{
    public SerializableDictionary<string, PopupTemplate> dic_Popup;
}
