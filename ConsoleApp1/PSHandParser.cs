using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class PSHandParser
    {

        public class PlayerInfo
        {
            public int seatNumber {  get; set; }
            public string nickName{ get; set; }
            public decimal stack { get; set; }
            public bool? Hero {  get; set; }
            public string? heroCards { get; set; }

        }
        public class HandResults
        {
            public long handNumber {  get; set; }

            public List<PlayerInfo> playerInfos { get; set; }          

        }

       
           public static void ParseHand (string textHand)
            {
                var lines = textHand.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                var selectedHashNumber = from line in lines
                                         where line.Contains("PokerStars Hand #")
                                         from part in line.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                         where part.StartsWith("#") && part.EndsWith(":")
                                         select part.Substring(1, part.Length - 2);


                foreach (var temp in selectedHashNumber) {
                    Console.Write(temp);
                }

                                



            }

        

        }


    }

