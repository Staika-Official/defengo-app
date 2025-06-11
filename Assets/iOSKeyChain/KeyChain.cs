#if UNITY_IPHONE
using UnityEngine;
using System.Runtime.InteropServices;

public class KeyChain
{

    [DllImport("__Internal")]
    private static extern string getKeyChainUser();

    public static string BindGetKeyChainUser()
    {
        return getKeyChainUser();
    }

    [DllImport("__Internal")]
    private static extern void setKeyChainUser(string userId, string uuid);

    public static void BindSetKeyChainUser(string userId, string uuid)
    {
        setKeyChainUser(userId, uuid);
    }

    [DllImport("__Internal")]
    private static extern void deleteKeyChainUser();

    public static void BindDeleteKeyChainUser()
    {
        deleteKeyChainUser();
    }

    [DllImport("__Internal")]
    private static extern string getKeyChainValue(string key);
    [DllImport("__Internal")]
    private static extern void setKeyChainValue(string key, string value);
    [DllImport("__Internal")]
    private static extern void deleteKeyChainValue(string key);


    public static string GetValue(string key)
    {
        return getKeyChainValue(key);
    }
    public static void SetValue(string key, string value)
    {
        setKeyChainValue(key, value);
    }

    public static void DeleteValue(string key)
    {
        deleteKeyChainValue(key);
    }


}
#endif
