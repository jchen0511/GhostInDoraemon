using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
    public class PokerPlayer
    {
        public GameConnection Conn { get; set; }

        public int Chips { get; private set; }

        public List<Poker> Cards { get; set; }

        public Table GameTable { get; set; }

        public event EventHandler<GameParm> AfterDecideAction;

        public event EventHandler<GameParm> OnJoinGame;

        public void ReciveGameMessage(object sender, GameParm gameMsg)
        {
            switch (gameMsg.EventName)
            {
                case PokerEvent.NewRound:
                case PokerEvent.ShowAction:
                    UpdateGameInfo(gameMsg.Data);
                    break;
                case PokerEvent.Action:
                    AnalyzeDoWhichActin(gameMsg.Data);
                    break;
                default:
                    Console.WriteLine(JsonConvert.SerializeObject(gameMsg));
                    break;
            }
        }

        private void AnalyzeDoWhichActin(GameDataParm data)
        {
            int level = PokerLevelCalculator.CalculateCardLevel(this.Cards, data.TableForAction.Board);

            if(level <= 198180)
            {
                DoAllIn();
            }
            else if(level <= 1000000)
            {
                DoCall();
            }
            else
            {
                DoFold();
            }
        }

        private void UpdateGameInfo(GameDataParm data)
        {
            var me = data.Players.Where(p => p.PlayerName == "0df09d36ae7bd590fd516684dd4f89f3").First();
            this.Cards = me.PokerCards;
            this.Chips = me.Chips;
            this.GameTable = data.TableInfo;
        }

        #region Actions

        public void DoBet(int chips)
        {
            var parm = new GameParm()
            {
                EventName = PokerEvent.Action,
                Data = new GameDataParm()
                {
                    Aciton = ActionEnum.Bet,
                    Amount = chips
                }
            };

            AfterDecideAction(this, parm);
        }

        public void DoCall()
        {
            var parm = new GameParm()
            {
                EventName = PokerEvent.Action,
                Data = new GameDataParm()
                {
                    Aciton = ActionEnum.Call
                }
            };

            AfterDecideAction(this, parm);
        }

        public void DoRaise()
        {
            var parm = new GameParm()
            {
                EventName = PokerEvent.Action,
                Data = new GameDataParm()
                {
                    Aciton = ActionEnum.Raise
                }
            };

            AfterDecideAction(this, parm);
        }

        public void DoCheck()
        {
            var parm = new GameParm()
            {
                EventName = PokerEvent.Action,
                Data = new GameDataParm()
                {
                    Aciton = ActionEnum.Check
                }
            };

            AfterDecideAction(this, parm);
        }

        public void DoFold()
        {
            var parm = new GameParm()
            {
                EventName = PokerEvent.Action,
                Data = new GameDataParm()
                {
                    Aciton = ActionEnum.Fold
                }
            };

            AfterDecideAction(this, parm);
        }

        public void DoAllIn()
        {
            var parm = new GameParm()
            {
                EventName = PokerEvent.Action,
                Data = new GameDataParm()
                {
                    Aciton = ActionEnum.AllIn
                }
            };

            AfterDecideAction(this, parm);
        }

        public void DoReload()
        {
            var parm = new GameParm()
            {
                EventName = PokerEvent.Reload
            };

            AfterDecideAction(this, parm);
        }

        internal void JoinGame()
        {
            var parm = new GameParm()
            {
                EventName = PokerEvent.Join,
                Data = new GameDataParm() { PlayerName = "amu" }
            };

            OnJoinGame(this, parm);
        }

        #endregion
    }
}
