using System;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace SharpChatServer
{
    public class User
    {
        public string Name { get; set; }
        public TcpClient Client { get; set; } 
        public NetworkStream Stream { get; set; }
        public Aes AES { get; set; }
        public byte[] Key { get; set; }
        public User(TcpClient client, string name)
        {
            Name = name;
            Client = client;
            Stream = client.GetStream();
        }

        public byte[] Encrypt(byte[] bytes){
            return LocalEncryption.Encrypt(bytes, AES.Key, AES.IV);
        }

        public byte[] Decrypt(byte[] bytes){
            return LocalEncryption.Decrypt(bytes, AES.Key, AES.IV);
        }

    
    }

}