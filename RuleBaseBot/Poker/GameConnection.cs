using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Poker
{
    public class GameConnection
    {
        public static List<string> msgs = new List<string>();

        public WebSocket Ws { get; set; } = new WebSocket("ws://poker-dev.wrs.club:3001");

        public event EventHandler<GameParm> OnReciveGameMessage;

        public void DoListen(GameParm parm)
        {
            Ws.OnMessage += OnMessage;

            Ws.OnError += (sender, e) =>
                Console.WriteLine("Error: " + e.Message);

            Ws.Connect();
            var p = @"{
                            ""eventName"" : ""__join"",
                            ""data"" : {""playerName"" : ""amurtart""}
                        }";
            Ws.Send(JsonConvert.SerializeObject(parm));
        }

        public void Send(GameParm parm)
        {
            this.Ws.Send(JsonConvert.SerializeObject(parm));
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Data.Contains("__new_peer"))
                return;

            var msg = JsonConvert.DeserializeObject<GameParm>(e.Data);
            OnReciveGameMessage(this, msg);



            //File.WriteAllText($@"F:\AI Contest\logs\{++i}_{msg.EventName}.json",e.Data);
        }

        public static int i = 0;

    }
}
