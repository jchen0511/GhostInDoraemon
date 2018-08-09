using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker
{
    internal class PokerLevelCalculator
    {

        internal static int CalculateCardLevel(List<Poker> myCards, List<Poker> boardCards)
        {
            switch (boardCards.Count)
            {
                case 3:
                    var cardSets = myCards.Concat(boardCards).OrderBy(c => c.Suit).ThenBy(c => c.Number).ToList();
                    var pokerSet = new PokerSet(cardSets[0], cardSets[1], cardSets[2], cardSets[3], cardSets[4]);
                    var strenth = ListPokers.PokerSetLevel[pokerSet];
                    return strenth;
                case 4:
                    return CalculateCardLevel6Cards(myCards, boardCards);
                case 5:
                    return CalculateCardLevel7Cards(myCards, boardCards);
            }

            return 0;
        }

        private static int CalculateCardLevel7Cards(List<Poker> myCards, List<Poker> boardCards)
        {
            var strenthList = new List<int>();

            for (int i = 0; i < 3; i++)
            {
                List<Poker> cardSets = new List<Poker>();
                cardSets = cardSets.Concat(myCards).ToList();

                for (int j = i + 1; j < 4; j++)
                {
                    for (int k = j + 1; k < 5; k++)
                    {
                        cardSets.Add(boardCards[i]);
                    }
                }

                cardSets.OrderBy(c => c.Suit).ThenBy(c => c.Number);
                var pokerSet = new PokerSet(cardSets[0], cardSets[1], cardSets[2], cardSets[3], cardSets[4]);

                var strenth = ListPokers.PokerSetLevel[pokerSet];
                strenthList.Add(strenth);
            }

            return strenthList.Min();
        }

        private static int CalculateCardLevel6Cards(List<Poker> myCards, List<Poker> boardCards)
        {
            var strenthList = new List<int>();

            for (var skipIndex = 4; skipIndex >= 0; skipIndex--)
            {
                List<Poker> cardSets = new List<Poker>();
                cardSets = cardSets.Concat(myCards).ToList();

                for (int i = 0; i < boardCards.Count; i++)
                {
                    if (i == skipIndex)
                        continue;
                    cardSets.Add(boardCards[i]);
                }

                cardSets.OrderBy(c => c.Suit).ThenBy(c => c.Number);
                var pokerSet = new PokerSet(cardSets[0], cardSets[1], cardSets[2], cardSets[3], cardSets[4]);

                var strenth = ListPokers.PokerSetLevel[pokerSet];
                strenthList.Add(strenth);
            }

            return strenthList.Min();
        }
    }
}