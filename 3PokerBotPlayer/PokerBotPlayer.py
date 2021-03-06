#coding=UTF-8
from pokereval.card import Card
#from deuces import Card, Evaluator
from pokereval.hand_evaluator import HandEvaluator
from websocket import create_connection
import math
import random
import json
import numpy as np
import time
import logging.config
import os
import logging

LOG_CONF = os.path.join('GhostInDoraemon\\3PokerBotPlayer','conf', 'log.conf')
with open(LOG_CONF) as log_f:
    logging.config.dictConfig(json.load(log_f))

LOG = logging.getLogger('ghost')

def getCard(card):
    card_type = card[1]
    cardnume_code = card[0]
    card_num = 0
    card_num_type = 0

    if card_type == 'S':
        card_num_type = 1
    elif card_type == 'H':
        card_num_type = 2
    elif card_type == 'D':
        card_num_type = 3
    else:
        card_num_type = 4

    if cardnume_code == 'T':
        card_num = 10
    elif cardnume_code == 'J':
        card_num = 11
    elif cardnume_code == 'Q':
        card_num = 12
    elif cardnume_code == 'K':
        card_num = 13
    elif cardnume_code == 'A':
        card_num = 14
    else:
        card_num = int(cardnume_code)

    return Card(card_num,card_num_type)

class PokerBot(object):
    def declareAction(self,hole, board, round, my_Raise_Bet, my_Call_Bet,Table_Bet,number_players,raise_count,bet_count,my_Chips,total_bet):
        err_msg = self.__build_err_msg("declare_action")
        raise NotImplementedError(err_msg)
    def game_over(self,isWin,winChips,data):
        err_msg = self.__build_err_msg("game_over")
        raise NotImplementedError(err_msg)

class PokerSocket(object):
    ws = ""
    board = []
    hole = []
    my_Raise_Bet = 0
    my_Call_Bet = 0
    number_players = 0
    my_Chips=0
    Table_Bet=0
    playerGameName=None
    raise_count=0
    bet_count=0
    total_bet=0
    def __init__(self,playerName,connect_url, pokerbot):
        self.pokerbot=pokerbot
        self.playerName=playerName
        self.connect_url=connect_url

    def getAction(self,data):
        LOG.info('---------------PokerSocket--------------')
        round = data['game']['roundName']
        # time.sleep(2)
        players = data['game']['players']
        chips = data['self']['chips']
        hands = data['self']['cards']

        self.raise_count = data['game']['raiseCount']
        self.bet_count = data['game']['betCount']
        self.my_Chips=chips
        self.playerGameName=data['self']['playerName']

        self.number_players = len(players)
        self.my_Call_Bet = data['self']['minBet']
        self.my_Raise_Bet = int(chips / 4)
        self.hole = []
        for card in (hands):
            self.hole.append(getCard(card))


        LOG.info('my_Raise_Bet:{}'.format(self.my_Raise_Bet))
        LOG.info('board:{}'.format(self.board))
        LOG.info('total_bet:{}'.format(self.Table_Bet))
        LOG.info('hands:{}'.format(self.hole))

        if self.board == []:
            round = 'preflop'

        LOG.info("PokerSocket round:{}".format(round))
        #print "round:{}".format(round)


        #aggresive_Tight = PokerBotPlayer(preflop_threshold_Tight, aggresive_threshold)
        #tightAction, tightAmount = aggresive_Tight.declareAction(hole, board, round, my_Raise_Bet, my_Call_Bet,Table_Bet,number_players)
        action, amount= self.pokerbot.declareAction(self.hole, self.board, round, self.my_Raise_Bet,self.my_Call_Bet, self.Table_Bet, self.number_players,self.raise_count,self.bet_count,self.my_Chips,self.total_bet)
        self.total_bet += amount
        return action, amount

    def takeAction(self,action, data):
        # Get number of players and table info
        if action == "__show_action":
            table = data['table']
            players = data['players']
            boards = table['board']
            self.number_players = len(players)
            self.Table_Bet = table['totalBet']
            self.board = []
            for card in (boards):
                self.board.append(getCard(card))
            
            LOG.info('number_players:{}'.format(self.number_players))
            LOG.info('board:{}'.format(self.board))
            LOG.info('total_bet:{}'.format(self.Table_Bet))
        elif action == "__bet":
            action, amount=self.getAction(data)
            LOG.info("action: {}".format(action))
            LOG.info("action amount: {}".format(amount))
            self.ws.send(json.dumps({
                "eventName": "__action",
                "data": {
                    "action": action,
                    "playerName": self.playerName,
                    "amount": amount
                }}))
        elif action == "__action":
            action,amount=self.getAction(data)
            LOG.info("action: {}".format(action))
            LOG.info("action amount: {}".format(amount))

            self.ws.send(json.dumps({
                "eventName": "__action",
                "data": {
                    "action": action,
                    "playerName": self.playerName,
                    "amount": amount
                }}))
        elif action == "__round_end":
            LOG.info("Game Over")
            self.total_bet=0
            players=data['players']
            isWin=False
            winChips=0
            for player in players:
                winMoney=player['winMoney']
                playerid=player['playerName']
                if (self.playerGameName == playerid):
                    if (winMoney==0):
                        isWin = False
                    else:
                        isWin = True
                    winChips=winMoney
            LOG.info("winPlayer:{}".format(isWin))
            LOG.info("winChips:{}".format(winChips))
            self.pokerbot.game_over(isWin, winChips,data)

    def doListen(self):
        try:
            self.ws = create_connection(self.connect_url)
            self.ws.send(json.dumps({
                "eventName": "__join",
                "data": {
                    "playerName": self.playerName
                }
            }))
            while 1:
                result = self.ws.recv()
                msg = json.loads(result)
                event_name = msg["eventName"]
                data = msg["data"]
                LOG.info('event_name:{}'.format(event_name))
                LOG.info('data : {}'.format(data))                
                self.evtHandler(event_name, data)
        except Exception, ex:
            LOG.exception("Uncaught exception (%s) %s", type(ex), ex)
            self.doListen()

    def evtHandler(self, event_name, data):
        if event_name == "__new_round":
            self.takeAction(event_name, data)
        if event_name == "__show_action":
            self.takeAction(event_name, data)            
        if event_name == "__action":
            self.takeAction(event_name, data)
        if event_name == "__deal":
            self.takeAction(event_name, data)
        if event_name == "__round_end":      
            self.takeAction(event_name, data)      
        elif event_name == "__game_over":
            LOG.info('__game_over')  
            time.sleep(8)
            self.doListen()


class MontecarloPokerBot(PokerBot):

    def __init__(self, simulation_number):
       LOG.info("------------MontecarloPokerBot __init__---------------------")
       self.simulation_number=simulation_number

    def game_over(self, isWin,winChips,data):
        LOG.info("------------MontecarloPokerBot game_over---------------------")
        pass

    def declareAction(self,hole, board, round, my_Raise_Bet, my_Call_Bet,Table_Bet,number_players,raise_count,bet_count,my_Chips,total_bet):
        LOG.info("------------MontecarloPokerBot declareAction---------------------")
        win_rate =self.get_win_prob(hole,board,number_players)
        LOG.info("Win Rate:{}".format(win_rate))
        if win_rate > 0.5:
            if win_rate > 0.7:
                # If it is extremely likely to win, then raise as much as possible
                action = 'raise'
                amount = my_Raise_Bet
            else:
                # If there is a chance to win, then call
                action = 'call'
                amount=my_Call_Bet
        else:
            action = 'fold'
            amount=0
        return action,amount

    def getCardID(self,card):
        #LOG.info("------------MontecarloPokerBot getCardID---------------------")
        rank=card.rank
        suit=card.suit
        suit=suit-1
        id=(suit*13)+rank
        return id

    def genCardFromId(self,cardID):
        #LOG.info("------------MontecarloPokerBot genCardFromId---------------------")
        if int(cardID)>13:
            rank=int(cardID)%13
            if rank==0:
                suit=int((int(cardID)-rank)/13)
            else:
                suit = int((int(cardID) - rank) / 13) + 1

            if(rank==0):
                rank=14
            else:
                rank+=1
            return Card(rank,suit)
        else:
            suit=1
            rank=int(cardID)
            if (rank == 0):
                rank = 14
            else:
                rank+=1
            return Card(rank,suit)

    def _pick_unused_card(self,card_num, used_card):
        #LOG.info("------------MontecarloPokerBot _pick_unused_card---------------------")
        used = [self.getCardID(card) for card in used_card]
        unused = [card_id for card_id in range(1, 53) if card_id not in used]
        choiced = random.sample(unused, card_num)
        return [self.genCardFromId(card_id) for card_id in choiced]

    def get_win_prob(self,hand_cards, board_cards,num_players):
        """Calculate the win probability from your board cards and hand cards by using simple Monte Carlo method.

        Args:
            board_cards: The board card list.
            hand_cards: The hand card list

        Examples:
            >>> get_win_prob(["8H", "TS", "6C"], ["7D", "JC"])
        """
        LOG.info("------------MontecarloPokerBot get_win_prob---------------------")
        win = 0
        round=0
        evaluator = HandEvaluator()

        for i in range(self.simulation_number):
            board_cards_to_draw = 5 - len(board_cards)  # 2
            LOG.info("board_cards_to_draw:{}".format(board_cards_to_draw))
            board_sample = board_cards + self._pick_unused_card(board_cards_to_draw, board_cards + hand_cards)
            LOG.info("board_sample:{}".format(board_sample))
            unused_cards = self._pick_unused_card((num_players - 1) * 2, hand_cards + board_sample)
            opponents_hole = [unused_cards[2 * i:2 * i + 2] for i in range(num_players - 1)]
            LOG.info("opponents_hole:{}".format(opponents_hole))
            hand_sample = self._pick_unused_card(2, board_sample + hand_cards)
            LOG.info("hand_sample:{}".format(hand_sample))

            try:
                opponents_score = [evaluator.evaluate_hand(hole, board_sample) for hole in opponents_hole]
                LOG.info("opponents_score:{}".format(opponents_score))
                my_rank = evaluator.evaluate_hand(hand_cards, board_sample)
                LOG.info("my_rank:{}".format(my_rank))
                if my_rank >= max(opponents_score):
                    win += 1
                rival_rank = evaluator.evaluate_hand(hand_sample, board_sample)
                LOG.info("rival_rank:{}".format(rival_rank))
                round+=1
            except Exception, e:
                print "get_win_prob exception:{},{}".format(i,e)
                continue

        LOG.info("Win:{}".format(win))
        win_prob = win / float(round)
        LOG.info("Win_prob:{}".format(win_prob))
        return win_prob


if __name__ == '__main__':
        aggresive_threshold = 0.5
        passive_threshold = 0.7
        preflop_threshold_Loose = 0.2
        preflop_threshold_Tight = 0.5
        bet_tolerance=0.2
        # Aggresive -loose
        #myPokerBot=PotOddsPokerBot(preflop_threshold_Loose,aggresive_threshold,bet_tolerance)
        #myPokerBot=PotOddsPokerBot(preflop_threshold_Tight,aggresive_threshold,bet_tolerance)
        #myPokerBot=PotOddsPokerBot(preflop_threshold_Loose,passive_threshold,bet_tolerance)
        #myPokerBot=PotOddsPokerBot(preflop_threshold_Tight,passive_threshold,bet_tolerance)

        print "----------------Start Game----------------------"
        LOG.info("----------------Start Game----------------------")
        playerName="3dedee18681d436ba401bf10b7ba2cbc"
        #playerName="jerry001"
        #connect_url="ws://iskf.org:80/"
        connect_url="ws://poker-battle.vtr.trendnet.org:3001/"
        #connect_url="ws://poker-dev.wrs.club:3001/"
        simulation_number=200
        
        #myPokerBot=FreshPokerBot()
        myPokerBot=MontecarloPokerBot(simulation_number)
        #myPokerBot=PotOddsPokerBot(preflop_threshold_Tight,aggresive_threshold,bet_tolerance)
        myPokerSocket=PokerSocket(playerName,connect_url,myPokerBot)
        myPokerSocket.doListen()