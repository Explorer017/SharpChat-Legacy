using System;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SharpChatServer
{
    class Server
    {
        // server.cs
        public static List<User> users = new List<User>();
        static void Main(string[] args)
        {
            Console.WriteLine("Server starting !");
        
            // IP Address to listen on. Loopback in this case
            Console.WriteLine("IP Address to listen on: ");
            IPAddress ipAddr = IPAddress.Parse(Console.ReadLine());
            // Port to listen on
            Console.WriteLine("Port to listen on: ");
            int port = int.Parse(Console.ReadLine());
            // Create a network endpoint
            IPEndPoint ep = new IPEndPoint(ipAddr, port);
            // Create and start a TCP listener
            TcpListener listener = new TcpListener(ep);
            listener.Start();
            Console.WriteLine("Server listening on: {0}:{1}", ep.Address, ep.Port);
            //Database.initDatabase();
            // keep running
            while(true)
            {
                if (listener.Pending())
                {
                    // Accept the next connection
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");
                    // Create a thread for handling the authentication of the user.
                    User user;
                    Task.Run(() =>
                    {
                        user = conHandle.connect(client);
                        users.Add(user);
                        Console.WriteLine($"User {user.Name} joined, starting reciving thread...");
                        user.send("Server");
                        user.send($"Welcome to SharpChat, {user.Name}");
                        forwardToOtherClientsFromServer(users, $"{user.Name} has joined the chat.");
                        Task.Run(user.reciver());
                    });
                    

                }
                
            }
        }
        

        public static void sendBytes(byte[] bytes, TcpClient client){
            client.GetStream().Write(bytes,0,bytes.Length);
        }
        
        public static bool forwardToOtherClients(List<User> users, User user, string message){
            try {
            foreach(User u in users){
                if(u != user){
                    u.send(user.Name);
                    u.send(message);
                }
            }
            return true;
            }
            catch(Exception e){
                Console.WriteLine($"An error while forwarding messages: {e.Message}");
                return false;
            }
        }

        public static bool forwardToOtherClientsFromServer(List<User> users, string message){
            try {
            foreach(User u in users){
                u.send("Server");
                u.send(message);
            }
            return true;
            }
            catch(Exception e){
                Console.WriteLine($"An error while forwarding messages: {e.Message}");
                return false;
            }
        }

    }
}
