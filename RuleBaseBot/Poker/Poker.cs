using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Suit
    {
        [JsonProperty("C")]
        Club,
        [JsonProperty("D")]
        Diamonds,
        [JsonProperty("H")]
        Heart,
        [JsonProperty("S")]
        Spades
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Numbers
    {

        [JsonProperty("A")]
        Ace = 1,
        [JsonProperty("2")]
        Two,
        [JsonProperty("3")]
        Three,
        [JsonProperty("4")]
        Four,
        [JsonProperty("5")]
        Five,
        [JsonProperty("6")]
        Six,
        [JsonProperty("7")]
        Seven,
        [JsonProperty("8")]
        Eight,
        [JsonProperty("9")]
        Nine,
        [JsonProperty("T")]
        Ten,
        [JsonProperty("J")]
        Jack,
        [JsonProperty("Q")]
        Queen,
        [JsonProperty("K")]
        King
    }

    public class Poker
    {
        public Poker(Suit suit, Numbers number)
        {
            this.Suit = suit;
            this.Number = number;
        }

        public Suit Suit { get; private set; }

        public Numbers Number { get; private set; }

        public static Dictionary<char, Numbers> NumsDict = new Dictionary<char, Numbers>()
        {
            //{'A',"1" },{'2',"2"},{'3',"3"},{'4',"4"},{'5',"5"},{'6',"6"},{'7',"7"},
            //{'8',"8"},{'9',"9"},{'T',"10"},{'J',"11"},{'Q',"12"},{'K',"13"},
            //{'H', }
            {'A',Numbers.Ace },{'2',Numbers.Two},{'3',Numbers.Three},{'4',Numbers.Four},{'5',Numbers.Five},
            {'6',Numbers.Six},{ '7',Numbers.Seven},{'8',Numbers.Eight},{'9',Numbers.Nine},{'T',Numbers.Ten},
            {'J',Numbers.Jack},{'Q',Numbers.Queen},{'K',Numbers.King},

        };

        public static Dictionary<char, Suit> SuitsDict = new Dictionary<char, Suit>()
        {
            {'C',Suit.Club},{'D',Suit.Diamonds},{'H',Suit.Heart},{'S', Suit.Spades}
        };

        private static List<Poker> pokers = new List<Poker>();

        public static List<Poker> Pokers
        {
            get
            {
                if (pokers.Count == 0)
                {
                    pokers = NumsDict.Values.Select(n => SuitsDict.Values.Select(s => new Poker(s, n)))
                            .SelectMany(p => p)
                            .OrderBy(p => p.Suit)
                            .ThenBy(p => p.Number)
                            .ToList();
                }

                return pokers;

            }
        }


        public override string ToString()
        {
            //FieldInfo fi = this.Suit.GetType().GetField(this.Suit.ToString());
            //JsonPropertyAttribute[] attributes = (JsonPropertyAttribute[])fi.GetCustomAttributes(typeof(JsonPropertyAttribute), false);
            //Enumerations.GetEnumDescription((Suit)value);
            return $"{this.Suit.GetJsonAttribute()}{this.Number.GetJsonAttribute()}";
        }

        public static bool operator ==(Poker c1, Poker c2)
        {
            return c1.Suit == c2.Suit && c1.Number == c2.Number;
        }

        public static bool operator !=(Poker c1, Poker c2)
        {
            return !(c1.Suit == c2.Suit && c1.Number == c2.Number);
        }

        public int CompareTo(Poker other)
        {
            if (this.Suit == other.Suit && this.Number == other.Number)
                return 0;
            else
                return -1;
        }
    }

    public static class PokerExtension
    {
        public static string GetJsonAttribute(this Enum enumObj)
        {
            FieldInfo fi = enumObj.GetType().GetField(enumObj.ToString());
            JsonPropertyAttribute[] attributes = (JsonPropertyAttribute[])fi.GetCustomAttributes(typeof(JsonPropertyAttribute), false);
            return attributes[0].PropertyName;
        }

        public static int CompareTo(this Numbers numbers)
        {
            return 0;
        }
    }
}
