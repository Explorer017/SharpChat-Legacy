﻿using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SharpChat
{
    class Client
    {
        public static string username;
        public static TcpClient connection;
        public static byte[] rsakey;
        static void Main(string[] args)
        {
            Console.WriteLine("SharpChat Basic Client\n--------------------\nVersion 0.1");
            // Connection creator - creates a new connection to the server and authenticates the user
            bool inputed = false;
            string IP = "localhost";
            int Port = 8081;
            if (!inputed){
                try{
                    Console.Write("What is the ip you would like to connect too: ");
                    IP = Console.ReadLine();
                    Console.Write("What is the port you would like to connect too: ");
                    Port = Convert.ToInt32(Console.ReadLine());
                    inputed = true;
                } catch (Exception e){
                    Console.WriteLine("Invalid input, please try again");
                    inputed = false;
                }
            }
            #nullable enable
            TcpClient? client = connect(new TcpClient(IP, Port));

            if (client == null)
            {
                Console.WriteLine("Failed to connect to server");
                return;
            }
            #nullable disable

            Aes aes = DataManipulation.AesSender(client.GetStream(), rsakey);
            Connection go = new Connection(client, aes.Key, aes.IV, username);
            Console.WriteLine("Press enter to send a message");
            Task reciever = new Task(new Action(go.Receiver()));
            reciever.Start();
            while (true){
                if (Console.KeyAvailable){
                    if (Console.ReadKey().Key == ConsoleKey.Enter)
                    {
                        Console.Write("ME >>> ");
                        string message = Console.ReadLine();
                        Task.Run(() => go.send(message));
                    }
                }
                // check if the reciever is still running
                if (!reciever.IsCompleted)
                {
                    continue;
                } else
                {
                    Console.WriteLine("The Connection has been lost");
                    break;
                }
            }
        }
    
    
        public static TcpClient connect(TcpClient client)
        {
            Console.WriteLine("Atempting to connect...");
            client.NoDelay = false;
            NetworkStream stream = client.GetStream();
            rsakey = DataManipulation.keyReciver(stream);
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
            username = Console.ReadLine();
            stream.Write(DataManipulation.messageToByteArray(username));
            Console.WriteLine("What is the password?");
            byte[] pswd = DataManipulation.passwordEncrypter(rsakey,(Console.ReadLine()));
            stream.Write(pswd,0,pswd.Length);
            bool response = DataManipulation.BoolReader(stream);
            if (!response){
                Console.WriteLine("Invalid username or password - closing connection");
                client.Close();
                return null;
            }
            return client;
        }
        

    }
}
