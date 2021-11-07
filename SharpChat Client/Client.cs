using System;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace SharpChat
{
    class Client
    {
    // client.cs
    static void Main(string[] args)
    {
        Console.WriteLine("SharpChat Basic Client\n--------------------\nVersion 0.1");
        //Console.ReadLine();
        connect();
    }
    
    public static void connect()
    {
        byte[] response;
        //try
        //{
            Console.WriteLine("Atempting to connect...");
            TcpClient client = new TcpClient("127.0.0.1", 8081); // Create a new connection  
            client.NoDelay = true; // please check TcpClient for more optimization
            // messageToByteArray- discussed later
            //byte[] messageBytes = DataManipulation.messageToByteArray(message);
            Console.WriteLine("Preparing to recive key");
            using (NetworkStream stream = client.GetStream())
            {
                response = DataManipulation.keyReciver(stream);
                Console.WriteLine("Key recived");
                Console.WriteLine("Would you like to log in, or register?[l/r]");
                string choice = Console.ReadLine().ToLower();
                if (choice == "r")
                {
                    stream.Write(DataManipulation.messageToByteArray("Reg"));
                }
                else if (choice == "l")
                {
                    stream.Write(DataManipulation.messageToByteArray("LogIn"));
                }
                else
                {
                    Console.WriteLine("Invalid choice! Atempting into Log In.");
                    stream.Write(DataManipulation.messageToByteArray("LogIn"));
                }
                Console.WriteLine("What is the username");
                stream.Write(DataManipulation.messageToByteArray(Console.ReadLine()));
                Console.WriteLine("What is the password?");
                byte[] pswd = DataManipulation.passwordEncrypter(response,(Console.ReadLine()));
                stream.Write(pswd,0,pswd.Length);
                //Console.WriteLine(stream);
                Console.WriteLine(DataManipulation.streamToMessage(stream));
                Aes aes = DataManipulation.AesSender(stream, response);
                Console.WriteLine("Aes key sent");
                Console.WriteLine("Sending Test Message...");
                byte[] test = DataManipulation.messageToByteArray("Test");
                byte[] testEncrypted = LocalEncryption.Encrypt(test, aes.Key, aes.IV);
                stream.Write(BitConverter.GetBytes(testEncrypted.Length),0,4);
                Console.WriteLine(testEncrypted.Length);
                stream.Write(LocalEncryption.Encrypt(test, aes.Key, aes.IV));
            }
            client.Close();
        //}
        //catch (Exception e) { Console.WriteLine(e.Message); }
        }
    }
}
