using System;
using System.Security.Cryptography;
using System.IO;

namespace SharpChatServer{
    class LocalEncryption{

        // Encrypts a byte array using AES
        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv){
            using(Aes aes = Aes.Create()){
                aes.Key = key;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using(MemoryStream ms = new MemoryStream()){
                    using(CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)){
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }

    
        // Decrypts a byte array using AES
        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv){
            using(Aes aes = Aes.Create()){
                aes.Key = key;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using(MemoryStream ms = new MemoryStream()){
                    using(CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write)){
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }
    }
}