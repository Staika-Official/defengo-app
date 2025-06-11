using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;
using Framework.GameData.Defense;
using Framework.Network;
using Framework.UI;
using Framework.Sound;

public class SingleToneManager : MonoBehaviour
{
    public static SingleToneManager Instance;

    void Start()
    {
        if(Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void SetLogout()
    {
        Instance = null;
        Destroy(gameObject);
        SetNull();

        //SceneLoadManager.onCompleteLoadScene = () =>
        //{
           
        //};
    }

    public void SetNull()
    {
        UserInfoManager.Instance = null;
        LanguageManager.Instance = null;
        AssetManager.Instance = null;
        DataLoadManager.Instance = null;
        DataManager.Instance = null;
        NetworkManager.Instance = null;
        IAPManager.Instance = null;
        SequenceManager.Instance = null;
        CharacterClassManager.Instance = null;
        AdmobManager.Instance = null;
        FirebaseInitializer.Instance = null;
        SoundManager.Instance = null;
    }
}
