#if UNITY_IPHONE
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyChainManager : MonoBehaviour
{


    public TMP_InputField inputField_KeyChainData;
    public string inputFieldData;
    public TextMeshProUGUI text_keyChainData;

    public static void SetKeyChainData(string key, string value)
    {
        KeyChain.SetValue(key, value);
    }

    public static string GetKeyChain(string key)
    {
        string value = KeyChain.GetValue(key);

        return value;
    }

    public void SetValue(string value)
    {
        SetKeyChainData("key", value);
    }

    public void GetValue()
    {
        string value = GetKeyChain("key");

        text_keyChainData.text = value;
        Debug.Log("Get Key Chain Value!!!! : " + value);
    }

    public void OnComplete_Edit()
    {
        string data = inputField_KeyChainData.text;

        SetValue(data);
    }

}
#endif
