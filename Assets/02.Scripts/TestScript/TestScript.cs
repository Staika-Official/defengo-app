using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
using Framework.Network;
using System.Data;

public enum TestEnum
{
    ALL,
    WINS,
    LOVE
}

public class CharacterStatus
{
    public int hp;
    public int mp;

    public CharacterStatus(int hp, int mp)
    {
        this.hp = hp;
        this.mp = mp;
    }
}

public class TestScript : MonoBehaviour
{


    public LineRenderer lineRenderer;
    public float count = 360;

    public string data;
    public TotalUserData totalUserData;

    //public SHA256 sha256;

    private void Start()
    {
        // totalUserData = JsonUtility.FromJson<TotalUserData>(data);
        //
        // count = 90;
        //
        // string a = TestEnum.ALL.ToString();
        // Debug.Log(a);
        //
        // DateTime date1 = new(2024, 03, 07);
        // DateTime date2 = new(2024, 03, 07);
        //
        // int compare = DateTime.Compare(date1, date2);
        // Debug.Log(compare);
        //
        // CharacterStatus characterStatus = new CharacterStatus(2, 1);
        // string serialize = JsonUtility.ToJson(characterStatus);
        // Debug.Log(serialize);
        //
        // CharacterStatus characterStatus1 = JsonUtility.FromJson<CharacterStatus>(serialize);
        //
        // Debug.Log(characterStatus1.hp);
        string appVersion = "2.4.2";
        string userId = "1004";
        string jwt = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJzYW11ZWwiLCJhdXRoIjoiUk9MRV9BRE1JTixST0xFX1NVUEVSX0FETUlOIiwiZXhwIjoxNzE5OTAzNjk5LCJ1c2VySWQiOiIxOTQifQ.nH5I_5k2YyunTZHBZl4I-nSIVgI2uk9UHXQXV3qBqoeZF19_MUD-XTjhSC4yarThAbipWkFmrNLtr9YEMK5c6Q";

        //Debug.Log(GetFingerPrint(appVersion, userId, jwt));

        Debug.Log(Crypto(appVersion, userId, jwt));

        double mill = 1728691200000;

        DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(mill).ToUniversalTime();
        Debug.Log(date);
        DateTime date2 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(mill).ToLocalTime();
        Debug.Log(date2);

        //TimeZoneInfo zone = TimeZoneInfo.ConvertTime
        // DateTime date3 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(mill).To
        // Debug.Log(date3);
        //Debug.Log(DateTimeParse(mill));
    }

    public static string Crypto(string appVersion, string userId, string jwt)
    {
        string data = appVersion + userId + jwt;

        var bytes = Encoding.UTF8.GetBytes(data);
        var hash = new SHA256CryptoServiceProvider().ComputeHash(bytes);
        var encryptedData = new StringBuilder();

        foreach (var h in hash)
        {
            encryptedData.AppendFormat("{0:x2}", h);
        }

        return encryptedData.ToString();
    }

    private static SHA256 sha256;
    public string GetFingerPrint(string appVersion, string userId, string jwt)
    {
        string data = appVersion + userId + jwt;

        if (sha256 == null)
        {
            sha256 = new SHA256Managed();
        }
        Debug.Log(jwt);
        Debug.Log(data);
        Byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(appVersion + userId + jwt));
        string convert = Convert.ToBase64String(hash);
        return convert;
    }

    private void Update()
    {
        // count -= Time.deltaTime * 50;
        //
        // if(count <= 0)
        // {
        //     count = 360;
        // }
        //
        // float x = Mathf.Cos(count * Mathf.Deg2Rad) * 5;
        // float y = Mathf.Sin(count * Mathf.Deg2Rad) * 5;
        // Vector3 dir = new(x, y, 0);
        //
        // lineRenderer.SetPosition(0, dir);
    }

}
