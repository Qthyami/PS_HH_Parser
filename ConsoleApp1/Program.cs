using System;
using System.Linq;
using System.Collections.Generic;
using ConsoleApp1;

namespace HandHistoryParser
{
    class Program
    {
        public static void Main(string[] args)
        {
            //var hands = PokerHandReader.ReadHands(@"C:\MyHandsArchive_H2N\2025\8\12\Impala IV.txt");
            var hand = "PokerStars Hand #93405882771:  Hold'em No Limit ($0.10/$0.25 USD) - 2013/02/03 1:16:19 EET [2013/02/02 18:16:19 ET]\r\nTable 'Stobbe III' 6-max Seat #4 is the button\r\nSeat 1: VakaLuks ($26.87 in chips) \r\nSeat 2: BigBlindBets ($29.73 in chips) \r\nSeat 3: Jamol121 ($17.66 in chips) \r\nSeat 4: ubbikk ($26.06 in chips) \r\nSeat 5: RicsiTheKid ($25 in chips) \r\nSeat 6: angrypaca ($26.89 in chips) \r\nRicsiTheKid: posts small blind $0.10\r\nangrypaca: posts big blind $0.25\r\n*** HOLE CARDS ***\r\nDealt to angrypaca [6d As]\r\nVakaLuks: folds \r\nBigBlindBets: folds \r\nJamol121: calls $0.25\r\nubbikk: folds \r\nRicsiTheKid: folds \r\nangrypaca: checks \r\n*** FLOP *** [5s Qs 3c]\r\nangrypaca: checks \r\nJamol121: checks \r\n*** TURN *** [5s Qs 3c] [8d]\r\nangrypaca: checks \r\nJamol121: bets $0.25\r\nangrypaca: folds \r\nUncalled bet ($0.25) returned to Jamol121\r\nJamol121 collected $0.57 from pot\r\n*** SUMMARY ***\r\nTotal pot $0.60 | Rake $0.03 \r\nBoard [5s Qs 3c 8d]\r\nSeat 1: VakaLuks folded before Flop (didn't bet)\r\nSeat 2: BigBlindBets folded before Flop (didn't bet)\r\nSeat 3: Jamol121 collected ($0.57)\r\nSeat 4: ubbikk (button) folded before Flop (didn't bet)\r\nSeat 5: RicsiTheKid (small blind) folded before Flop\r\nSeat 6: angrypaca (big blind) folded on the Turn";

            //foreach (var hh in hands)
            PSHandParser.ParseHand(hand);

            //// LINQ: для каждой раздачи выбираем номер
            //var handNumbers = hands
            //    .Select(hand =>
            //    {
            //        // найти первую строку, которая начинается с "PokerStars Hand #"
            //        var firstLine = hand.Split('\n').FirstOrDefault(l => l.StartsWith("PokerStars Hand #"));
            //        if (firstLine != null)
            //        {
            //            // взять всё после "#" и до первого пробела
            //            int hashIndex = firstLine.IndexOf('#');
            //            int spaceIndex = firstLine.IndexOf(' ', hashIndex);
            //            if (hashIndex >= 0 && spaceIndex > hashIndex)
            //            {
            //                return firstLine.Substring(hashIndex + 1, spaceIndex - hashIndex - 1);
            //            }
            //        }
            //        return null;
            //    })
            //    .Where(n => n != null) // убираем null
            //    .ToList();

            //// выводим номера
            //foreach (var number in handNumbers)
            //{
            //    Console.WriteLine(number);
            //}
        }
    }
}