using System;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpChatServer
{
    class Server
    {
        // server.cs
        static void Main(string[] args)
        {
            Console.WriteLine("Server starting !");
        
            // IP Address to listen on. Loopback in this case
            IPAddress ipAddr = IPAddress.Loopback;
            // Port to listen on
            int port = 8081;
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
                    Thread t = new Thread(() => user = conHandle.connect(listener.AcceptTcpClient()));
                    // Create a new thread for the client
                    //ThreadedServer clientThread = new ThreadedServer(client);
                    //clientThread.Start();
                }
                conHandle.connect(listener.AcceptTcpClient());
            }
        }
        
        public static void sendMessage(string message, TcpClient client)
        {
            // messageToByteArray- discussed later
            byte[] bytes = DataManipulation.messageToByteArray(message);
            client.GetStream().Write(bytes, 0, bytes.Length);
        }
        public static void sendBytes(byte[] bytes, TcpClient client){
            client.GetStream().Write(bytes,0,bytes.Length);
        }
        
        public static string MessageHandler(string id)
        {
            return "";
        }
    }
}
