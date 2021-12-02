using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Net.Sockets;

namespace SharpChat{

    class DataManipulation{
        // using UTF8 encoding for the messages
        static Encoding encoding = Encoding.UTF8;
        public static byte[] messageToByteArray(string message)
        {
            // get the size of original message
            byte[] messageBytes = encoding.GetBytes(message);
            int messageSize = messageBytes.Length;
            // add content length bytes to the original size
            int completeSize = messageSize + 4;
            // create a buffer of the size of the complete message size
            byte[] completemsg = new byte[completeSize];
        
            // convert message size to bytes
            byte[] sizeBytes = BitConverter.GetBytes(messageSize);
            // copy the size bytes and the message bytes to our overall message to be sent 
            sizeBytes.CopyTo(completemsg, 0);
            messageBytes.CopyTo(completemsg, 4);
            return completemsg;
        }
        
        public static string streamToMessage(Stream stream)
        {
            // size bytes have been fixed to 4
            byte[] sizeBytes = new byte[4];
            // read the content length
            stream.Read(sizeBytes, 0, 4);
            int messageSize = BitConverter.ToInt32(sizeBytes, 0);
            // create a buffer of the content length size and read from the stream
            byte[] messageBytes = new byte[messageSize];
            stream.Read(messageBytes, 0, messageSize);
            // convert message byte array to the message string using the encoding
            string message = encoding.GetString(messageBytes);
            string result = null;
            foreach (var c in message)
                if (c != '\0')
                    result += c;
        
            return result;
        }

        public static string byteToMessage(byte[] message)
        {
            // convert message byte array to the message string using the encoding
            string messageString = encoding.GetString(message);
            string result = null;
            foreach (var c in messageString)
                if (c != '\0')
                    result += c;
            return result;
        }
        public static byte[] keyReciver(Stream stream){
            byte[] sizeBytes = new byte[270];
            stream.Read(sizeBytes,0,270);
            //Console.WriteLine(BytesToReadable(sizeBytes));
            return sizeBytes;
        }
        public static string BytesToReadable(byte[] bytes){
            string hex = BitConverter.ToString(bytes,0);
            return hex;
        }

        public static byte[] passwordEncrypter(byte[] pubKey, string Password){
            using(var rsa = RSA.Create()){
                Console.WriteLine("Encrypting...");
                rsa.ImportRSAPublicKey(pubKey,out _);
                
                //Console.WriteLine(BitConverter.ToString(rsa.Encrypt(messageToByteArray(Password),RSAEncryptionPadding.Pkcs1)));
                return rsa.Encrypt(messageToByteArray(Password),RSAEncryptionPadding.Pkcs1);
            }
        }

        public static Aes AesSender(NetworkStream stream, byte[] rsaPubKey){
            Console.WriteLine("Sending AES key...");
            Console.WriteLine("Re-Importing RSA...");
            RSA rsa = RSA.Create();
            rsa.ImportRSAPublicKey(rsaPubKey,out _);
            Aes aes = Aes.Create();
            aes.KeySize = 256;
			aes.Padding = PaddingMode.PKCS7;
            Console.WriteLine("Generating AES key...");
            aes.GenerateKey();
            Console.WriteLine("Encrypting AES IV...");
            aes.GenerateIV();
            byte[] aesKey = aes.Key;
			Console.WriteLine("AES Key: " + BytesToReadable(aes.Key));
			Console.WriteLine("AES IV: " + BytesToReadable(aes.IV));
            byte[] aesIV = aes.IV;
            byte[] aesKeyEncrypted = rsa.Encrypt(aesKey, RSAEncryptionPadding.Pkcs1);
            byte[] aesIVEncrypted = rsa.Encrypt(aesIV, RSAEncryptionPadding.Pkcs1);
            stream.Write(aesKeyEncrypted,0,aesKeyEncrypted.Length);
            stream.Write(aesIVEncrypted,0,aesIVEncrypted.Length);
            return aes;
        }
        public static byte[] AdvancedByteReciver(Stream stream, int size){
            byte[] sizeBytes = new byte[size];
            stream.Read(sizeBytes,0,size);
            return sizeBytes;
        }

    }
}