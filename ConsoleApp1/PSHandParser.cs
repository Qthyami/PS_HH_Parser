using PSHhParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConsoleApp1
{
    public class PSHandParser
    {

        public class PlayerInfo
        {
            public int seatNumber {  get; set; }
            public string nickName { get; set; } = string.Empty;
            public decimal stack { get; set; }
            public bool? Hero {  get; set; }
            public string? heroCards { get; set; }

        }
        public class HandResults
        {
            public long handNumber {  get; set; }

            public List<PlayerInfo> playerInfos { get; set; }=new List<PlayerInfo>();         

        }

       
           public static HandResults ParseHand (string textHand)
            {
            var HandResultsFinal = new HandResults();
            HandResultsFinal.playerInfos = new List<PlayerInfo>();
         
          

            var lines = textHand.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            var selectedNumber = from line in lines
                                 where line.StartsWith("PokerStars Hand #")
                                 from part in line.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                 where part.StartsWith("#") && part.EndsWith(":")
                                 select part.Substring(1, part.Length - 2);

            foreach (string num in selectedNumber)
            {
             
                HandResultsFinal.handNumber=StringToNumberParser.ParserHelper.ParseNumber<long>(num);
               

            }

            List<string> seatsLines = (from line in lines
                                      where line.StartsWith("Seat ") && line.Contains("in chips")
                                      select line).ToList();
            foreach (string strSeat in seatsLines) {
                 var playerTemp = new PlayerInfo();


                var strParts= strSeat.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (strParts.Length < 6)
                {
                    Console.WriteLine($"Skipped invalid seat line: {strSeat}");
                    continue;
                }


                if (strParts.Length>=6 && strParts[0].Contains("Seat") && strParts[5].Contains("chips)"))
                    {
                        playerTemp.nickName = strParts[2];
                        
                        string stackStr = strParts[3].Trim().Trim('(', '$');
                     
                        playerTemp.stack = StringToNumberParser.ParserHelper.ParseNumber<decimal>(stackStr);


                        string seatNum = strParts[1].Trim().TrimEnd(':');
                        playerTemp.seatNumber = StringToNumberParser.ParserHelper.ParseNumber<int>(seatNum);
                    

                    HandResultsFinal.playerInfos.Add(playerTemp);
                     }

               



            }
         




            var dealLine = from line in lines
                               where line.StartsWith("Dealt to")
                               select line.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            foreach (var subline in dealLine)
            {
              var heroNick= subline[2];
                var heroPlayer= HandResultsFinal.playerInfos.SingleOrDefault(p=>p.nickName == heroNick);



                string hC = "";

                for (int i = 0; i < subline.Length; i++)
                {
                    if (i + 1 < subline.Length && subline[i].StartsWith("[") && subline[i + 1].EndsWith("]"))
                    {
                        hC = (subline[i] + subline[i + 1]).Substring(1, 4);
                        
                       
                    }
                }

                if (heroPlayer != null)
                {
                    heroPlayer.Hero = true;
                    heroPlayer.heroCards = hC;
                }

                 
            }

            Console.WriteLine($"Hand Number: {HandResultsFinal.handNumber}");
            foreach (var player in HandResultsFinal.playerInfos)
            {
                Console.WriteLine($"Seat: {player.seatNumber}, Nick: {player.nickName}, Stack: {player.stack}, Hero: {player.Hero}, HeroCards: {player.heroCards}");
            }

            return HandResultsFinal;
            


        }

        

        }


    }

