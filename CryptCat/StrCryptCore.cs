using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CryptCat
{
    internal static class StrCryptCore
    {
        public static byte[] GetHash128(string key)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] hashBytes = MD5.HashData(keyBytes);
            return hashBytes;
        }

        public static byte[] Encrypt(byte[] dataToEncrypt, byte[] key)
        {
            using Aes aesAlg = Aes.Create();//创建AES加密算法实例
            aesAlg.Key = key;
            aesAlg.Mode = CipherMode.CBC;//设置加密模式为CBC
            aesAlg.Padding = PaddingMode.PKCS7;//设置填充模式为PKCS7

            //生成随机IV，最后将IV拷贝到result数组的前16个字节
            aesAlg.GenerateIV();
            byte[] iv = aesAlg.IV;

            //创建加密器实例
            using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                //将加密后的数据写入到msEncrypt流中
                using var bwEncrypt = new BinaryWriter(csEncrypt);
                bwEncrypt.Write(dataToEncrypt);
            }

            byte[] encryptedData = msEncrypt.ToArray();
            byte[] result = new byte[iv.Length + encryptedData.Length];
            //将IV和加密后的数据拷贝到result数组
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encryptedData, 0, result, iv.Length, encryptedData.Length);

            return result;
        }

        public static byte[] Decrypt(byte[] dataToDecrypt, byte[] key)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            //从dataToDecrypt数组中分离出IV和密文
            byte[] iv = new byte[aesAlg.BlockSize / 8];
            byte[] cipherText = new byte[dataToDecrypt.Length - iv.Length];
            Buffer.BlockCopy(dataToDecrypt, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(dataToDecrypt, iv.Length, cipherText, 0, cipherText.Length);

            aesAlg.IV = iv;

            using var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using var msDecrypt = new MemoryStream(cipherText);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var brDecrypt = new BinaryReader(csDecrypt);
            byte[] decryptedData = brDecrypt.ReadBytes(cipherText.Length);
            return decryptedData;
        }
    }
}
