using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker
{
    public enum SetKinds
    {
        RoyalFlush,
        StraightFlush,
        TKey,
        FullHouse,
        Flush,
        Straight,
        Triple,
        TwoPair,
        Pair,
        HighCard
    }

    public class PokerSet : IComparable<PokerSet>
    {
        public PokerSet(Poker p1, Poker p2, Poker p3, Poker p4, Poker p5)
        {
            this.P1 = p1;
            this.P2 = p2;
            this.P3 = p3;
            this.P4 = p4;
            this.P5 = p5;

            this.PokerList = new List<Poker>()
            {
                p1,p2,p3,p4,P5
            };

            this.SuitList = new List<Suit>()
            {
                p1.Suit,p2.Suit,p3.Suit,p4.Suit,p5.Suit
            };

            this.NumberList = new List<Numbers>()
            {
                p1.Number,p2.Number,p3.Number,p4.Number,p5.Number,
            };

            this.SetKind = CheckSet();
        }

        public Poker P1 { get; set; }
        public Poker P2 { get; set; }
        public Poker P3 { get; set; }
        public Poker P4 { get; set; }
        public Poker P5 { get; set; }

        public List<Poker> PokerList { get; }
        public List<Suit> SuitList { get; private set; }
        public List<Numbers> NumberList { get; private set; }

        public SetKinds SetKind { get; set; }

        public SetKinds CheckSet()
        {
            if (this.IsRoyalFlush())
                return SetKinds.RoyalFlush;
            else if (this.IsStraightFlush())
                return SetKinds.StraightFlush;
            else if (this.IsTKey())
                return SetKinds.TKey;
            else if (this.IsFullHouse())
                return SetKinds.FullHouse;
            else if (this.IsFlush())
                return SetKinds.Flush;
            else if (this.IsStraight())
                return SetKinds.Straight;
            else if (this.IsTriple())
                return SetKinds.Triple;
            else if (this.IsTwoPair())
                return SetKinds.TwoPair;
            else if (this.IsPair())
                return SetKinds.Pair;
            else
                return SetKinds.HighCard;
        }

        public bool IsFlush()
        {
            return this.SuitList.GroupBy(s => s).Count() == 1;
        }

        public bool IsFullHouse()
        {
            var result = this.NumberList.GroupBy(n => n).ToList();
            //.Select(g => new { NumberList = g.Key, Count = g.Count() }).ToList();

            var hasTriple = result.Where(r => r.Count() == 3).Count() == 1;
            var hasPair = result.Where(r => r.Count() == 2).Count() == 1;

            return hasTriple && hasPair;
        }

        public bool IsRoyalFlush()
        {
            if (!IsFlush())
                return false;

            return this.P1.Number == Numbers.Ace && this.P2.Number == Numbers.Ten
                && this.P3.Number == Numbers.Jack && this.P4.Number == Numbers.Queen
                && this.P5.Number == Numbers.King;
        }

        public bool IsStraightFlush()
        {
            if (!IsFlush())
                return false;

            return this.P1.Number + 1 == this.P2.Number && this.P2.Number + 1 == this.P3.Number
                && this.P3.Number + 1 == this.P4.Number && this.P4.Number + 1 == this.P5.Number;
        }

        public bool IsTKey()
        {
            var result = this.NumberList.GroupBy(n => n)
                .Max(r => r.Count());

            return result == 4;
        }

        public bool IsStraight()
        {
            return this.P1.Number + 1 == this.P2.Number && this.P2.Number + 1 == this.P3.Number
                && this.P3.Number + 1 == this.P4.Number && this.P4.Number + 1 == this.P5.Number;
        }

        public bool IsTriple()
        {
            var result = this.NumberList.GroupBy(n => n)
               .Max(r => r.Count());

            return result == 3;
        }

        public bool IsTwoPair()
        {
            var result = this.NumberList.GroupBy(n => n).Count();

            return result == 3;
        }

        public bool IsPair()
        {
            var result = this.NumberList.GroupBy(n => n).Count();
            return result == 4;
        }

        public Numbers MaxNumber =>
             this.NumberList.Contains(Numbers.Ace) ? Numbers.Ace : this.NumberList.Max();

        public Numbers MaxPair
        {
            get
            {
                var pairs = this.NumberList.GroupBy(n => n).Where(g => g.Count() == 2)
                    .Select(n => n.Key);

                return pairs.Contains(Numbers.Ace) ? Numbers.Ace : pairs.Max();
            }
        }

        public Numbers MaxTriple =>
          this.NumberList.GroupBy(n => n).Where(g => g.Count() == 3).Select(n => n.Key).First();

        public int CompareTo(PokerSet other)
        {
            if (this.SetKind < other.SetKind)
                return -1;
            else if (this.SetKind > other.SetKind)
                return 1;

            switch (this.SetKind)
            {
                case SetKinds.RoyalFlush:
                    return 0;
                case SetKinds.TKey:
                case SetKinds.StraightFlush:
                    return 0;
                case SetKinds.FullHouse:
                case SetKinds.Triple:
                    return SingleCardCompare(this.MaxTriple, other.MaxTriple);
                case SetKinds.TwoPair:
                    return SingleCardCompare(this.MaxPair, other.MaxPair);
                default:
                case SetKinds.HighCard:
                    return SingleCardCompare(this.MaxNumber, other.MaxNumber);

            }

            int SingleCardCompare(Numbers n1, Numbers n2)
            {
                if (n1 == n2)
                    return 0;
                else if (n1 == Numbers.Ace)
                    return -1;
                else if (n2 == Numbers.Ace)
                    return 1;
                else
                    return n1 > n2 ? -1 : 1;
            }
        }
    }

}
