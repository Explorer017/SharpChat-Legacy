using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace SharpChatServer{

    class DataManipulation{
        // using UTF8 encoding for the messages
        static Encoding encoding = Encoding.UTF8;
        //TODO: GET RID OF messageToByteArray AND REPLACE WITH SOMTHING BETTER
        // TODO: MAKE IT NOT BREAK ON WINDOWS
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
        public static byte[] byteReciver(Stream stream){
            byte[] sizeBytes = new byte[256];
            stream.Read(sizeBytes,0,256);
            return sizeBytes;
        }
        public static byte[] KeyReciver(Stream stream){
            byte[] sizeBytes = new byte[526];
            stream.Read(sizeBytes,0,526);
            return sizeBytes;
        }

        public static String passwordHasher(String password){
            using (SHA512 sha512Hash = SHA512.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash;
            }
        }
        public static byte[] AdvancedByteReciver(Stream stream, int size){
            byte[] sizeBytes = new byte[size];
            stream.Read(sizeBytes,0,size);
            return sizeBytes;
        }

        public static int IntReciver(Stream stream){
            byte[] sizeBytes = new byte[4];
            stream.Read(sizeBytes,0,4);
            return BitConverter.ToInt32(sizeBytes,0);
        }


    }
}