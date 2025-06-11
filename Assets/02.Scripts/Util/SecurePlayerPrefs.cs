using System;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class SecurePlayerPrefs
{
    private static readonly byte[] encryptionKey = GenerateKey("MySuperSecureKey1234567890!@#$"); // 32바이트 키

    private static byte[] GenerateKey(string passphrase)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(passphrase)); // 32바이트 고정
        }
    }
#if UNITY_EDITOR
    [MenuItem("Tools/Clear PlayerPrefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs 데이터가 모두 삭제되었습니다!");
    }
#endif

    /// <summary>
    /// AES를 사용하여 문자열을 암호화합니다.
    /// </summary>
    private static string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = encryptionKey;
            aes.IV = new byte[16]; // 고정 IV (보안을 강화하려면 랜덤 IV 사용)

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }
    }

    /// <summary>
    /// AES를 사용하여 암호화된 문자열을 복호화합니다.
    /// </summary>
    private static string Decrypt(string cipherText)
    {
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = encryptionKey;
                aes.IV = new byte[16];

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] encryptedBytes = Convert.FromBase64String(cipherText);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
        catch (Exception)
        {
            Debug.LogWarning("복호화 실패: 잘못된 키 또는 데이터 손상");
            return null;
        }
    }

    /// <summary>
    /// 암호화된 문자열을 저장합니다.
    /// </summary>
    public static void SetString(string key, string value)
    {
        string encryptedValue = Encrypt(value);
        PlayerPrefs.SetString(key, encryptedValue);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 암호화된 문자열을 불러옵니다.
    /// </summary>
    public static string GetString(string key, string defaultValue = "")
    {
        if (PlayerPrefs.HasKey(key))
        {
            string encryptedValue = PlayerPrefs.GetString(key);
            string decryptedValue = Decrypt(encryptedValue);
            return decryptedValue ?? defaultValue;
        }
        return defaultValue;
    }

    /// <summary>
    /// 암호화된 float 값을 저장합니다.
    /// </summary>
    public static void SetFloat(string key, float value)
    {
        SetString(key, value.ToString());
    }

    /// <summary>
    /// 암호화된 float 값을 불러옵니다.
    /// </summary>
    public static float GetFloat(string key, float defaultValue = 0f)
    {
        string stringValue = GetString(key, defaultValue.ToString());
        if (float.TryParse(stringValue, out float result))
        {
            return result;
        }
        return defaultValue;
    }

    /// <summary>
    /// 암호화된 int 값을 저장합니다.
    /// </summary>
    public static void SetInt(string key, int value)
    {
        SetString(key, value.ToString());
    }

    /// <summary>
    /// 암호화된 int 값을 불러옵니다.
    /// </summary>
    public static int GetInt(string key, int defaultValue = 0)
    {
        string stringValue = GetString(key, defaultValue.ToString());
        if (int.TryParse(stringValue, out int result))
        {
            return result;
        }
        return defaultValue;
    }

    /// <summary>
    /// 특정 키의 데이터를 삭제합니다.
    /// </summary>
    public static void DeleteKey(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
        }
    }

    /// <summary>
    /// 모든 PlayerPrefs 데이터를 삭제합니다.
    /// </summary>
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}