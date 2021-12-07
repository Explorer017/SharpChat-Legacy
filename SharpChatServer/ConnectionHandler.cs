using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace SharpChatServer{
    class conHandle{
        static byte[] pubKey;
        static RSAParameters parameters;
        public static User connect(TcpClient client){
            // streamToMessage - discussed later
            genRSA();
            Server.sendBytes(pubKey, client);
            Console.WriteLine("User atempting authentication");
            string message = DataManipulation.streamToMessage(client.GetStream());
            auth Auth = new auth(false,null);
            if (message == "Reg"){
                Console.WriteLine("User is attempting to register");
                Auth = Reg(client,DataManipulation.streamToMessage(client.GetStream()));
            } else if (message == "LogIn") {
                Console.WriteLine("User is attempting to log in");
                Auth = LogIn(client,DataManipulation.streamToMessage(client.GetStream()));
            } else {
                Console.WriteLine("User is not attempting to register or log in (this should not be here, if it is then something has gone wrong (probably on the client side))");
            }
            User user = new User(client, Auth.name);
            Console.WriteLine("Encrypting Connection...");
            user.Key = rsaByteDecoder(DataManipulation.byteReciver(client.GetStream()));
			user.IV = rsaByteDecoder(DataManipulation.byteReciver(client.GetStream()));
			return user;
        }

        private static auth LogIn(TcpClient client, string requestt, string usrnam = null){
            if (requestt != null)
            {
                usrnam = requestt;
                usrnam = Regex.Replace(usrnam, @"\s", "");
                usrnam = Regex.Replace(usrnam, @"\n", "");
            }
            byte[] request = DataManipulation.byteReciver(client.GetStream());
            if (request != null && usrnam != null)
            {
                //Console.WriteLine(DataManipulation.passwordHasher(rsaDecoder(request)));
                bool auth = Database.pswdChecker(usrnam,DataManipulation.passwordHasher(rsaDecoder(request)));
                if (auth){
                    //Server.sendMessage($"authenticated as {usrnam}", client);
                    Console.WriteLine($"user {usrnam} has authenticated");
                    return new auth(true,usrnam);
                } else{
                    // Server.sendMessage("authenication failed", client);
                    Console.WriteLine($"user {usrnam} failed authentiation");
                    return new auth(false,usrnam);
                }
            }
            return new auth(false,usrnam);
        }

        public static auth Reg(TcpClient client, string requestt, string usrnam = null){
            Console.WriteLine(requestt);
            if (requestt != null)
            {
                usrnam = requestt;
                usrnam = Regex.Replace(usrnam, @"\s", "");
                usrnam = Regex.Replace(usrnam, @"\n", "");
            }
            Console.WriteLine($"User {usrnam} is attempting to register");
            Console.WriteLine("Waiting for password setup...");
            byte[] request = DataManipulation.byteReciver(client.GetStream());
            string pswd = DataManipulation.passwordHasher(rsaDecoder(request));
            Console.WriteLine($"User {usrnam} has set up password");
            // TODO: add username duplicate checker
            Database.addUser(usrnam, pswd);
            Console.WriteLine($"User {usrnam} has been added to the database");
            Console.WriteLine($"user {usrnam} has authenticated");
            return new auth(true,usrnam);

        }
        private static RSA genRSA(){
            using (var rsa = RSA.Create()){
                rsa.KeySize = 2048;
                pubKey = rsa.ExportRSAPublicKey();
                parameters = rsa.ExportParameters(true);
                //Console.WriteLine(BitConverter.ToString(pubKey,0));
                return rsa;
            }
            
        }

        public static string rsaDecoder(byte[] encryptedString){
            RSA rsa = RSA.Create();
            rsa.ImportParameters(parameters);
            byte[] decrypted = rsa.Decrypt(encryptedString,RSAEncryptionPadding.Pkcs1);
            // convert message byte array to the message string using the encoding
            string message = Encoding.UTF8.GetString(decrypted);
            string result = null;
            foreach (var c in message)
                if (c != '\0')
                    result += c;
            return result;
        }

        public static byte[] rsaByteDecoder(byte[] encryptedBytes){
            RSA rsa = RSA.Create();
            rsa.ImportParameters(parameters);
            byte[] decrypted = rsa.Decrypt(encryptedBytes,RSAEncryptionPadding.Pkcs1);
            return decrypted;
        }



    }
}