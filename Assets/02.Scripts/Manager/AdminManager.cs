using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RegistRewardRuleDTO
{
    public string code;
    public string memo;
    public int rewardGo;
    public int rewardGoMax;
}

public class AdminManager : MonoBehaviour
{
    public List<RegistRewardRuleDTO> rewardRuleList = new();

    private void Start()
    {
        
    }
}
