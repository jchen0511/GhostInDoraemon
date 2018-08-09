using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Poker
{
    public class GameDataParm 
    {
        [JsonProperty("playerName")]
        public string PlayerName { get; set; }

        [JsonProperty("tableNumber")]
        public int TableNumber { get; set; }

        [JsonProperty("self")]
        public PlayerInfo Self { get; set; }

        [JsonProperty("game")]
        public Table TableForAction { get; set; }

        [JsonProperty("table")]
        public Table TableForDeal { get; set; }

        public Table TableInfo => this.TableForAction != null ? this.TableForAction : this.TableForDeal;


        [JsonProperty("players")]
        public List<PlayerInfo> Players { get; set; }

        [JsonProperty("action")]
        public ActionEnum Aciton { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }
    }

    public class Table
    {
        [JsonProperty("board")]
        public List<Poker> Board { get; set; }

        [JsonProperty("roundName")]
        public Round RoundName { get; set; }

        [JsonProperty("roundCount")]
        public int RoundCound { get; set; }

        [JsonProperty("raiseCount")]
        public int RaiseCount { get; set; }

        [JsonProperty("betCount")]
        public int BetCount { get; set; }

        [JsonProperty("totalBet")]
        public int TotalBet { get; set; }

        [JsonProperty("initChips")]
        public int InitChips { get; set; }

        [JsonProperty("maxReloadCount")]
        public int MaxReloadCount { get; set; }

        [JsonProperty("players")]
        public List<PlayerInfo> Players { get; set; }

        [JsonProperty("smallBlind")]
        public Blind SmallBlind { get; set; }

        [JsonProperty("bigBlind")]
        public Blind BigBlind { get; set; }

    }

    public class BlindBet
    {
        [JsonProperty("playerName")]
        public string PlayerName { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }
    }

    public class GameParm : EventArgs
    {
        [JsonProperty("eventName")]
        public PokerEvent EventName { get; set; }

        [JsonProperty("data")]
        public GameDataParm Data { get; set; }
    }

    public class ActionParm
    {
        [JsonProperty("action")]
        public ActionEnum Action { get; set; }

        [JsonProperty("playerName")]
        public string PlayerName { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("chips")]
        public int Chips { get; set; }
    }

    public class GameMessage
    {
        [JsonProperty("eventName")]
        public string EventName { get; set; }

        //[JsonProperty("data")]
        //public List<string> Data { get; set; }
    }

    public class ActionMessage
    {
        [JsonProperty("tableNumber")]
        public int TableNumber { get; set; }

        [JsonProperty("self")]
        public PlayerInfo Self { get; set; }

        [JsonProperty("game")]
        public GameInfo Game { get; set; }
    }

    public class GameInfo
    {
        public List<string> board { get; set; }

        public List<Poker> Board =>
            this.board.Select(card => new Poker(Poker.SuitsDict[card[1]], Poker.NumsDict[card[0]])).ToList();

        [JsonProperty("roundName")]
        public string RoundName { get; set; }

        [JsonProperty("roundCound")]
        public int roundCound { get; set; }

        [JsonProperty("raiseCount")]
        public int RaiseCound { get; set; }

        [JsonProperty("betCount")]
        public int BetCount { get; set; }

        [JsonProperty("players")]
        public List<PlayerInfo> players { get; set; }

        [JsonProperty("smallblind")]
        public Blind SmallBind { get; set; }

        [JsonProperty("BigBlind")]
        public Blind BigBlind { get; set; }

    }

    public class Blind
    {
        [JsonProperty("playerName")]
        public string playerName { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }
    }

    public class PlayerInfo
    {
        [JsonProperty("playerName")]
        public string PlayerName { get; set; }

        [JsonProperty("chips")]
        public int Chips { get; set; }

        [JsonProperty("folded")]
        public bool Folded { get; set; }

        [JsonProperty("allIn")]
        public bool AllIn { get; set; }

        [JsonProperty("isSurvive")]
        public bool IsSurvive { get; set; }

        [JsonProperty("reloadCount")]
        public bool ReloadCount { get; set; }

        [JsonProperty("roundBet")]
        public int RoundBet { get; set; }

        [JsonProperty("bet")]
        public int Bet { get; set; }

        [JsonProperty("minBet")]
        public int MinBet { get; set; }

        [JsonProperty("isOnline")]
        public bool IsOnline { get; set; }

        [JsonProperty("isHuman")]
        public bool IsHuman { get; set; }

        public List<string> cards { get; set; }

        public List<Poker> PokerCards => this.cards.Select(card => new Poker(Poker.SuitsDict[card[1]], Poker.NumsDict[card[0]])).ToList();

        public int SmallBlind { get; set; }

        public int BigBlind { get; set; }

    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActionEnum
    {
        [EnumMember(Value = "bet")]
        Bet,

        [EnumMember(Value = "call")]
        Call,

        [EnumMember(Value = "raise")]
        Raise,

        [EnumMember(Value = "check")]
        Check,

        [EnumMember(Value = "fold")]
        Fold,

        [EnumMember(Value = "allin")]
        AllIn
    }


    [JsonConverter(typeof(StringEnumConverter))]
    public enum PokerEvent
    {
        [EnumMember(Value = "_join")]
        Join,

        [EnumMember(Value = "__new_peer")]
        NewPeer,

        [EnumMember(Value = "__game_prepare")]
        GamePrepare,

        [EnumMember(Value = "__game_start")]
        GameStart,

        [EnumMember(Value = "__new_round")]
        NewRound,

        [EnumMember(Value = "__show_action")]
        ShowAction,

        [EnumMember(Value = "__start_reload")]
        StartReload,

        [EnumMember(Value = "__reload")]
        Reload,

        [EnumMember(Value = "__deal")]
        Deal,

        [EnumMember(Value = "__action")]
        Action,

        [EnumMember(Value = "__round_end")]
        RoundEnd,

        [EnumMember(Value = "__game_over")]
        GameOver
    }

    public enum Round
    {
        Deal,
        Flop,
        Turn,
        River
    }
}