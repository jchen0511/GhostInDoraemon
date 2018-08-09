using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Threading;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Poker
{
    public class T1
    {
        public Numbers N { get; set; }
    }

    class Program
    {
        public static List<Poker> InialCards()
        {
            var pokers = new List<Poker>();

            Dictionary<Suit, int> cards = new Dictionary<Suit, int>()
            {
                {Suit.Club,13 },{Suit.Diamonds,13},{Suit.Heart,13},{Suit.Spades,13}
            };

            for (var j = 0; j < 4; j++)
            {
                for (var i = 1; i <= 13; i++)
                {
                    pokers.Add(new Poker((Suit)j, (Numbers)i));
                }
            }

            return pokers;
        }

        static void Main(string[] args)
        {
            ListPokers.SetPokerLevelFlush();           

            var game = new GameConnection();
            var player = new PokerPlayer();
            player.OnJoinGame += (sender, e) => { game.DoListen(e); };
            player.AfterDecideAction += (sender, e) => { game.Send(e); };
            game.OnReciveGameMessage += player.ReciveGameMessage;
            player.JoinGame();

            Console.ReadKey(true);          

        }

    }
}
