using System;

namespace SharpChatServer{
    class auth{
        public bool authed { get; set; }
        public string name { get; set; }

        public auth(bool Authed,string Name){
            name = Name;
            authed = Authed;
        }
    }
}