using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class AESUtil
{
    private static byte[] _key;
    private static byte[] _iv;

    public static void SetKeyAndIV(string key, string iv)
    {
        _key = Encoding.UTF8.GetBytes(key);
        _iv = Encoding.UTF8.GetBytes(iv);
        
        // if (_key.Length != 32)
            // Debug.LogError("AES 키의 길이가 다름 현재 길이: " + _key.Length);

        // if (_iv.Length != 16)
            // Debug.LogError("AES IV의 길이가 다름. 현재 길이: " + _iv.Length);
    }

    public static string Encrypt(string plainText)
    {
        if (_key == null || _iv == null)
            throw new InvalidOperationException("AES 키와 IV가 설정되지 않았습니다.");

        using Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);

        sw.Write(plainText);
        sw.Flush();
        cs.FlushFinalBlock();

        return Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string encryptedText)
    {
        if (_key == null || _iv == null)
            throw new InvalidOperationException("AES 키와 IV가 설정되지 않았습니다.");

        byte[] buffer = Convert.FromBase64String(encryptedText);

        using Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var ms = new MemoryStream(buffer);
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}