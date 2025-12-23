using PSHhParser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;


namespace PSHhParser
{
    public class PSHandParser
    {

        public class PlayerInfo  // структура для сохранения выходных данных про игрока, складывается в list List<PlayerInfo>
        {
            public int seatNumber {  get; set; }
            public string nickName { get; set; } = string.Empty;
            public decimal stack { get; set; }
            public bool? Hero {  get; set; }
            public string? heroCards { get; set; }

        }
        public class HandResults  // структура для сохранения выходных данных об 1 раздаче, номер и вложенный List<PlayerInfo>
        {
            public long handNumber {  get; set; }

            public List<PlayerInfo> playerInfos { get; set; }=new List<PlayerInfo>();         

        }

        /// <summary>
        /// Функция ParseHand парсит текст одной раздачи PokerStars и возвращает структурированные данные формата HandResults
        /// </summary>

        public static HandResults ParseHand (string textHand)
            {
            // Финальный объект результата
            var HandResultsFinal = new HandResults(); 

            HandResultsFinal.playerInfos = new List<PlayerInfo>();

            // Разбиваем текст раздачи на строки
            var lines = textHand.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);


            //--- 1.Парсинг номера раздачи---
            var selectedNumber = from line in lines
                                 where line.StartsWith("PokerStars Hand #")
                                 from part in line.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                 where part.StartsWith("#") && part.EndsWith(":")
                                 select part.Substring(1, part.Length - 2);

            foreach (string num in selectedNumber)
            {             
                HandResultsFinal.handNumber=StringToNumberParser.ParserHelper.ParseNumber<long>(num);   //полученное числовое string значение парсим кастомной функцией типа tryParse, сохраням в HandResultsFinal         

            }

            //---2. Парсинг строк Seat (игроки и стеки)---
            List<string> seatsLines = (from line in lines
                                      where line.StartsWith("Seat ") && line.Contains("in chips")
                                      select line).ToList();
            foreach (string strSeat in seatsLines) {
                 var playerTemp = new PlayerInfo();  // временный объект с данными по каждому seat который потом очистим, а данные добавим в HandResultsFinal.playerInfos


                var strParts= strSeat.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (strParts.Length < 6)  //формат строки типа Seat 5: RicsiTheKid ($25 in chips) строгий, поэтому можно не итерироваться, а сразу обращаться по индексам, но все равно проверка на всякий случай
                {
                    Console.WriteLine($"Skipped invalid seat line: {strSeat}");
                    continue;
                }

                //сразу находим по индексам, но если истории бывают битые не проблема сделать через еще 1 цикл
                if (strParts.Length>=6 && strParts[0].Contains("Seat") && strParts[5].Contains("chips)"))
                    {
                        playerTemp.nickName = strParts[2];
                        
                        string stackStr = strParts[3].Trim().Trim('(', '$');
                     
                        playerTemp.stack = StringToNumberParser.ParserHelper.ParseNumber<decimal>(stackStr); //наши, перепарсили, записали 

                        string seatNum = strParts[1].Trim().TrimEnd(':');
                        playerTemp.seatNumber = StringToNumberParser.ParserHelper.ParseNumber<int>(seatNum);
                    

                    HandResultsFinal.playerInfos.Add(playerTemp);
                     }
            }

            // ---3. Определение хиро и его карт---

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

            //TODO : this coommented lines of code just for showing functionality
            Console.WriteLine($"=== Hand #{HandResultsFinal.handNumber} ===");
            foreach (var player in HandResultsFinal.playerInfos)
            {
                Console.WriteLine($"Seat: {player.seatNumber}, Nick: {player.nickName}, stack: {player.stack.ToString(CultureInfo.InvariantCulture)}, Hero: {player.Hero}, HeroCards: {player.heroCards}", CultureInfo.InvariantCulture);
            }

            return HandResultsFinal;
            


        }

        

        }


    }

