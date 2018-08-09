using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
    public class ListPokers
    {
        //public static List<PokerSet> PokerSets { get; private set; } = new List<PokerSet>();

        public static Dictionary<PokerSet, int> PokerSetLevel { get; set; } = new Dictionary<PokerSet, int>();

        public static List<PokerSet> pokerSets = new List<PokerSet>();

        public static List<PokerSet> PokerSets
        {
            get
            {
                if (pokerSets.Count != 0)
                    return pokerSets;

                var pokers = Poker.Pokers;

                for (int i = 0; i < pokers.Count; i++)
                {
                    for (int j = i + 1; j < pokers.Count; j++)
                    {
                        for (int k = j + 1; k < pokers.Count; k++)
                        {
                            for (int l = k + 1; l < pokers.Count; l++)
                            {
                                for (int m = l + 1; m < pokers.Count; m++)
                                {
                                    var pks = new PokerSet(pokers[i], pokers[j], pokers[k], pokers[l], pokers[m]);
                                    Console.WriteLine($"{pks.P1.ToString()},{pks.P2.ToString()},{pks.P3.ToString()},{pks.P4.ToString()},{pks.P5.ToString()}");
                                    //Console.WriteLine(JsonConvert.SerializeObject(pks));
                                    pokerSets.Add(pks);
                                }
                            }
                        }
                    }
                }

                pokerSets.Sort();

                return pokerSets;
            }
        }

        public static void SetPokerLevelFlush()
        {           
            var index = 1;
            PokerSetLevel = pokerSets.ToDictionary(p => p, p => index++);           
        }
    }   
}
