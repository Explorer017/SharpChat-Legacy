using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;

namespace SharpChat{
    class Connection{
        public TcpClient client { get; set; }
        public NetworkStream stream { get; set; }
        public string username { get; set; }
        public byte[] key { get; set; }
        public byte[] iv { get; set; }
        private Aes aes;

        public Connection(TcpClient Client, byte[] Key, byte[] Iv, string Username){
            client = Client;
            username = Username;
            stream = client.GetStream();
            key = Key;
            iv = Iv;
            aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            username = Username;
        }

        /// <summary>
        /// <para>Encrypts the given string and sends it to the server. Returns true if successful</para>
        /// </summary>
        public bool send(string message){
            try{
                Stream connection = client.GetStream();
                byte[] encrypted = Encrypt(DataManipulation.messageToByteArray(message));
                byte[] length = BitConverter.GetBytes(encrypted.Length);
                connection.Write(length, 0, length.Length);
                connection.Write(encrypted, 0, encrypted.Length);
                return true;
            } catch (Exception e){
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /// <summary>
        /// <para>Checks for incoming messages, decrypts them and returns them as a byte array</para>
        /// <para>Returns null if no message is available</para>
        /// </summary>
        public byte[] receive(Stream connection){
            //try{
                byte[] size = DataManipulation.AdvancedByteReciver(connection,4);
                int sizeInt = BitConverter.ToInt32(size,0);
                byte[] data = DataManipulation.AdvancedByteReciver(connection,sizeInt);
                byte[] decrypted = Decrypt(data);
                string message = System.Text.Encoding.UTF8.GetString(decrypted);
                Console.WriteLine(message);
                return decrypted;
            //} catch (Exception e){
            //    Console.WriteLine(e.Message);
            //    return null;
            //}
            
        }

        private byte[] Encrypt(byte[] data){
            return LocalEncryption.Encrypt(data, this.key, this.iv);
        }

        private byte[] Decrypt(byte[] data){
            return LocalEncryption.Decrypt(data, this.key, this.iv);
        }
        public Stream streamGenerator(){
            return client.GetStream();
        }
    }
}