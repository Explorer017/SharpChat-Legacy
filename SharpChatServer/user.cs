using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace SharpChatServer
{
    public class User
    {
        public string Name { get; set; }
        public TcpClient Client { get; set; } 
        public NetworkStream Stream { get; set; }
        public Aes AES { get; set; }
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public Task runner { get; set; }
        public User(TcpClient client, string name)
        {
            Name = name;
            Client = client;
            Stream = client.GetStream();
        }

        public byte[] Encrypt(byte[] bytes){
            return LocalEncryption.Encrypt(bytes, Key, IV);
        }

        public byte[] Decrypt(byte[] bytes){
            return LocalEncryption.Decrypt(bytes, Key, IV);
        }

        public Action reciver(){
            return () => {
                while(true){
                    try{
                        // check if client is still connected
                        if(!clientConnected(this.Client)){
                            throw new Exception("Client disconnected");
                        }
                        byte[] size = DataManipulation.AdvancedByteReciver(Stream,4);
                        int sizeInt = BitConverter.ToInt32(size,0);
                        byte[] data = DataManipulation.AdvancedByteReciver(Stream,sizeInt);
                        byte[] decrypted = Decrypt(data);
                        string message = System.Text.Encoding.UTF8.GetString(decrypted);
                        Console.WriteLine($"{this.Name}: {message}");
                        Server.forwardToOtherClients(Server.users, this, message);
                    }
                    catch(Exception e){
                        Console.WriteLine(e.Message);
                        Console.WriteLine($"User {this.Name} Disconnected. Removing them from the users list...");
                        Server.users.Remove(this);
                        break;
                    }
                }
            };
        }
        public void send(string message){
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            byte[] encrypted = Encrypt(data);
            byte[] size = BitConverter.GetBytes(encrypted.Length);
            Stream.Write(size,0,size.Length);
            Stream.Write(encrypted,0,encrypted.Length);
        }

        bool clientConnected(TcpClient client)
        {
            bool part1 = client.Client.Poll(1000, SelectMode.SelectRead);
            bool part2 = (client.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
    
    }

}