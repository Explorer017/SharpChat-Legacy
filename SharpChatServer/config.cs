using System;

namespace SharpChatServer{
    public class Config{
        public string ServerName { get; set; }
        public string ip { get; set; }
        public int port { get; set; }

        // create a function to import config from a file
        public Config(string path){
            CreateConfig(path);
            // read the file
            string[] lines = System.IO.File.ReadAllLines(path);
            // loop through the lines
            foreach(string line in lines){
                // split the line into a key and value
                string[] split = line.Split('=');
                // check if the key is valid
                if(split.Length != 2){
                    continue;
                }
                // check if the key is valid
                if(!Enum.IsDefined(typeof(ConfigKey), split[0])){
                    continue;
                }
                // set the value
                switch((ConfigKey)Enum.Parse(typeof(ConfigKey), split[0])){
                    case ConfigKey.ServerName:
                        ServerName = split[1];
                        break;
                    case ConfigKey.ip:
                        ip = split[1];
                        break;
                    case ConfigKey.port:
                        port = int.Parse(split[1]);
                        break;
                }
            }
        }

        // create a new config file if it doesn't exist
        private static void CreateConfig(string path){
            // check if the file exists
            if(System.IO.File.Exists(path)){
                return;
            }
            Console.WriteLine("Config File not found, creating new one...");
            // create the file
            System.IO.File.Create(path).Close();
            // write the default config
            System.IO.File.WriteAllText(path, "ServerName=SharpChatServer\nip=127.0.0.1\nport=8081");
        }

    }
    public enum ConfigKey{
        ServerName,
        ip,
        port
    }
    
}