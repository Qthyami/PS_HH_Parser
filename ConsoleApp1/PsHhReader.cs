using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public static class PsHhReader
    {
        public static List<string> ReadHands(string path)
        {
            var hands = new List<string>();
            var currentHand = new StringBuilder();


            bool startHand = false;

            bool summary = false;
            bool totalPotLine = false;
            bool summaryCompleted = false;
            int seatLineCointer = 0;


            foreach (var line in File.ReadLines(path))
            {
                if (string.IsNullOrEmpty(line)) continue;


                if (line.StartsWith("PokerStars Hand #"))
                {
                    if (startHand && summaryCompleted&& currentHand.Length > 0)
                    {
                        hands.Add(currentHand.ToString().Trim());
                        currentHand.Clear();

                    }
                    startHand = true;
                    summary = false;
                    totalPotLine = false;
                    seatLineCointer = 0;
                    currentHand.AppendLine(line);
                    continue;


                }

                if (startHand)
                {
                    currentHand.AppendLine(line);
                }
                else continue;
            

                if (line.StartsWith("*** SUMMARY ***"))
                {
                    summary = true;
                    totalPotLine = false;
                    seatLineCointer = 0;
                    continue;
                }

                if (summary && !totalPotLine && line.StartsWith("Total pot"))
                {
                    totalPotLine = true;
                    continue;
                }

                if (summary && totalPotLine && line.StartsWith($"Seat "))
                {
                    seatLineCointer++;

                    if (seatLineCointer >= 2 && seatLineCointer <= 10)
                    {
                        summaryCompleted = true;

                    }


                }
               
            }

            if (startHand && summaryCompleted)
            {

                hands.Add(currentHand.ToString().Trim());
            }

            return hands;
        }


    }
}
