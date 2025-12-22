using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using static ConsoleApp1.example;

namespace ConsoleApp1
{
    internal class example
    {
        public class Player
        {
            public int SeatNumber { get; set; }
            public string Nickname { get; set; } = string.Empty;
            public decimal Stack { get; set; }
            public string? Cards { get; set; }
            public bool? Hero { get; set; }
        }

        public class HandHistory
        {
            public string HandId { get; set; } = string.Empty;
            public List<Player> Players { get; set; } = new();

            public override string ToString()
            {
                return $"Hand #{HandId}\n" +
                       string.Join("\n",
                           from player in Players
                           orderby player.SeatNumber
                           select $"Seat {player.SeatNumber}: {player.Nickname} (${player.Stack:0.00})" +
                                  (player.Cards != null ? $" [{player.Cards}]" : ""));
            }
        }

        public class SimpleQuerySyntaxParser
        {
            public HandHistory Parse(string text)
            {
                // Разбиваем на строки
                var lines =
                    from rawLine in text.Replace("\r", "").Split('\n')
                    let line = rawLine.Trim()
                    where !string.IsNullOrWhiteSpace(line)
                    select line;

                // Получаем номер раздачи
                var handIdLine =
                    from line in lines
                    where line.Contains("Hand #")
                    select line;

                var handId = ParseHandIdFromLine(handIdLine.FirstOrDefault() ?? "");

                // Получаем игроков
                var seatLines =
                    from line in lines
                    where line.StartsWith("Seat ")
                    select line;

                // Получаем карты
                var cardLines =
                    from line in lines
                    where line.Contains("Dealt to", StringComparison.OrdinalIgnoreCase)
                    select line;

                // Парсим игроков
                var players =
                    from seatLine in seatLines
                    let player = ParsePlayer(seatLine)
                    where player != null
                    select player;

                // Парсим карты
                var cards =
                    from cardLine in cardLines
                    let cardData = ParseCards(cardLine)
                    where cardData.nickname != null
                    select cardData;

                // Объединяем
                var playersWithCards =
                    from player in players
                    join card in cards on player.Nickname equals card.nickname into cardGroup
                    from matchedCard in cardGroup.DefaultIfEmpty()
                    select new Player
                    {
                        SeatNumber = player.SeatNumber,
                        Nickname = player.Nickname,
                        Stack = player.Stack,
                        Cards = matchedCard.cards
                    };

                return new HandHistory
                {
                    HandId = handId,
                    Players = playersWithCards.OrderBy(p => p.SeatNumber).ToList()
                };
            }

            private string ParseHandIdFromLine(string line)
            {
                if (string.IsNullOrEmpty(line)) return "";

                var start = line.IndexOf("Hand #");
                if (start == -1) return "";

                start += 6;
                var end = line.IndexOf(':', start);

                return end > start ? line[start..end].Trim() : "";
            }

            private Player? ParsePlayer(string line)
            {
                var parts = line.Split(':', '(');
                if (parts.Length < 3) return null;

                // Номер места
                var seatText = parts[0]["Seat ".Length..].Trim();
                if (!int.TryParse(seatText, out int seat)) return null;

                // Ник
                var nickname = parts[1].Trim();

                // Стек
                var stackText = parts[2]
                    .Replace(" in chips)", "")
                    .Replace("$", "")
                    .Trim();

                if (!decimal.TryParse(stackText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal stack))
                    return null;

                return new Player
                {
                    SeatNumber = seat,
                    Nickname = nickname,
                    Stack = stack
                };
            }

            private (string? nickname, string? cards) ParseCards(string line)
            {
                var start = line.IndexOf("Dealt to", StringComparison.OrdinalIgnoreCase);
                if (start == -1) return (null, null);

                start += 8;
                var bracketStart = line.IndexOf('[', start);
                var bracketEnd = line.IndexOf(']', bracketStart + 1);

                if (bracketStart == -1 || bracketEnd == -1)
                    return (null, null);

                var nickname = line[start..bracketStart].Trim();
                var cards = line[(bracketStart + 1)..bracketEnd].Trim();

                return (nickname, cards);
            }
        }
    }


    public class ExampleProgram
    {
        public static void RunExample()
        {
            var handHistory = @"


PokerStars Hand #251984295394:  Hold'em No Limit ($1/$2 USD) - 2024/08/15 20:56:11 ET
Table 'Sasin' 6-max Seat #1 is the button
Seat 2: MakoEv ($200 in chips) is sitting out
Seat 1: LamanJohn ($20 in chips)
Seat 6: alwayslastcl ($58.47 in chips)
LamanJohn: posts small blind $1
alwayslastcl: posts big blind $2
*** HOLE CARDS ***
Dealt to LamanJohn [Kd 2h]
LamanJohn: raises $18 to $20 and is all-in
alwayslastcl: calls $18
*** FLOP *** [9c 7d Kh]
*** TURN *** [9c 7d Kh] [Kc]
*** RIVER *** [9c 7d Kh Kc] [2d]
*** SHOW DOWN ***
alwayslastcl: shows [As 6s] (a pair of Kings)
LamanJohn: shows [Kd 2h] (a full house, Kings full of Deuces)
LamanJohn collected $38.75 from pot
*** SUMMARY ***
Total pot $40 | Rake $1.25
Board [9c 7d Kh Kc 2d]
Seat 2: MakoEv
Seat 1: LamanJohn (button) (small blind) showed [Kd 2h] and won ($38.75) with a full house, Kings full of Deuces
Seat 6: alwayslastcl (big blind) showed [As 6s] and lost with a pair of Kings

PokerStars Hand #251984298300:  Hold'em No Limit ($1/$2 USD) - 2024/08/15 20:56:32 ET
Table 'Sasin' 6-max Seat #6 is the button
Seat 1: LamanJohn ($38.75 in chips)
Seat 2: MakoEv ($200 in chips)
Seat 6: alwayslastcl ($38.47 in chips)
LamanJohn: posts small blind $1
MakoEv: posts big blind $2
*** HOLE CARDS ***
Dealt to LamanJohn [Td 6s]
alwayslastcl: folds
LamanJohn: folds
Uncalled bet ($1) returned to MakoEv
MakoEv collected $2 from pot
MakoEv: doesn't show hand
*** SUMMARY ***
Total pot $2 | Rake $0
Seat 1: LamanJohn (small blind) folded before Flop
Seat 2: MakoEv (big blind) collected ($2)
Seat 6: alwayslastcl (button) folded before Flop (didn't bet)

PokerStars Hand #251984300476:  Hold'em No Limit ($1/$2 USD) - 2024/08/15 20:56:49 ET
Table 'Sasin' 6-max Seat #1 is the button
Seat 1: LamanJohn ($37.75 in chips)
Seat 2: MakoEv ($201 in chips)
Seat 6: alwayslastcl ($38.47 in chips)
MakoEv: posts small blind $1
alwayslastcl: posts big blind $2
*** HOLE CARDS ***
Dealt to LamanJohn [9c 2c]
LamanJohn: folds
MakoEv: folds
Uncalled bet ($1) returned to alwayslastcl
alwayslastcl collected $2 from pot
alwayslastcl: doesn't show hand
*** SUMMARY ***
Total pot $2 | Rake $0
Seat 1: LamanJohn (button) folded before Flop (didn't bet)
Seat 2: MakoEv (small blind) folded before Flop
Seat 6: alwayslastcl (big blind) collected ($2)

PokerStars Hand #251984302545:  Hold'em No Limit ($1/$2 USD) - 2024/08/15 20:57:05 ET
Table 'Sasin' 6-max Seat #2 is the button
Seat 1: LamanJohn ($37.75 in chips)
Seat 2: MakoEv ($200 in chips)
Seat 6: alwayslastcl ($39.47 in chips)
alwayslastcl: posts small blind $1
LamanJohn: posts big blind $2
*** HOLE CARDS ***
Dealt to LamanJohn [Qh 4c]
MakoEv: raises $3 to $5
alwayslastcl: folds
LamanJohn: folds
Uncalled bet ($3) returned to MakoEv
MakoEv collected $5 from pot
MakoEv: doesn't show hand
*** SUMMARY ***
Total pot $5 | Rake $0
Seat 1: LamanJohn (big blind) folded before Flop
Seat 2: MakoEv (button) collected ($5)
Seat 6: alwayslastcl (small blind) folded before Flop

PokerStars Hand #251984304561:  Hold'em No Limit ($1/$2 USD) - 2024/08/15 20:57:19 ET
Table 'Sasin' 6-max Seat #1 is the button
Seat 2: MakoEv ($203 in chips) is sitting out
Seat 1: LamanJohn ($35.75 in chips)
Seat 6: alwayslastcl ($38.47 in chips)
LamanJohn: posts small blind $1
MakoEv: is sitting out
alwayslastcl: posts big blind $2
*** HOLE CARDS ***
Dealt to LamanJohn [9s Jc]
MakoEv leaves the table
LamanJohn: raises $2 to $4
alwayslastcl: calls $2
*** FLOP *** [4c Ks 5h]
alwayslastcl: checks
LamanJohn: bets $2.52
alwayslastcl: calls $2.52
*** TURN *** [4c Ks 5h] [9d]
alwayslastcl: checks
LamanJohn: bets $6
alwayslastcl: folds
Uncalled bet ($6) returned to LamanJohn
LamanJohn collected $12.39 from pot
LamanJohn: doesn't show hand
*** SUMMARY ***
Total pot $13.04 | Rake $0.65
Board [4c Ks 5h 9d]
Seat 2: MakoEv
Seat 1: LamanJohn (button) (small blind) collected ($12.39)
Seat 6: alwayslastcl (big blind) folded on the Turn

PokerStars Hand #251984312164:  Hold'em No Limit ($1/$2 USD) - 2024/08/15 20:58:16 ET
Table 'Sasin' 6-max Seat #6 is the button
Seat 1: LamanJohn ($41.62 in chips)
Seat 6: alwayslastcl ($31.95 in chips)
alwayslastcl: posts small blind $1
LamanJohn: posts big blind $2
*** HOLE CARDS ***
Dealt to LamanJohn [4s Qh]
alwayslastcl: folds
Uncalled bet ($1) returned to LamanJohn
LamanJohn collected $2 from pot
LamanJohn: doesn't show hand
*** SUMMARY ***
Total pot $2 | Rake $0
Seat 1: LamanJohn (big blind) collected ($2)
Seat 6: alwayslastcl (button) (small blind) folded before Flop

PokerStars Hand #251984313223:  Hold'em No Limit ($1/$2 USD) - 2024/08/15 20:58:24 ET
Table 'Sasin' 6-max Seat #1 is the button
Seat 1: LamanJohn ($42.62 in chips)
Seat 6: alwayslastcl ($30.95 in chips)
LamanJohn: posts small blind $1
alwayslastcl: posts big blind $2
*** HOLE CARDS ***
Dealt to LamanJohn [2d 9c]
LamanJohn: folds
Uncalled bet ($1) returned to alwayslastcl
alwayslastcl collected $2 from pot
*** SUMMARY ***
Total pot $2 | Rake $0
Seat 1: LamanJohn (button) (small blind) folded before Flop
Seat 6: alwayslastcl (big blind) collected ($2)

PokerStars Hand #251984314213:  Hold'em No Limit ($1/$2 USD) - 2024/08/15 20:58:31 ET
Table 'Sasin' 6-max Seat #6 is the button
Seat 1: LamanJohn ($41.62 in chips)
Seat 6: alwayslastcl ($31.95 in chips)
alwayslastcl: posts small blind $1
LamanJohn: posts big blind $2
*** HOLE CARDS ***
Dealt to LamanJohn [Td 3h]
alwayslastcl: calls $1
LamanJohn: checks
*** FLOP *** [4s 4h Kd]
LamanJohn: checks
alwayslastcl: checks
*** TURN *** [4s 4h Kd] [Ad]
LamanJohn: bets $2.85
alwayslastcl: folds
Uncalled bet ($2.85) returned to LamanJohn
LamanJohn collected $3.80 from pot
LamanJohn: doesn't show hand
*** SUMMARY ***
Total pot $4 | Rake $0.20
Board [4s 4h Kd Ad]
Seat 1: LamanJohn (big blind) collected ($3.80)
Seat 6: alwayslastcl (button) (small blind) folded on the Turn
";

            //Console.WriteLine("=== Версия 1: Основной парсер ===");
            //var parser1 = new PokerStarsHandHistoryParser();
            //var result1 = parser1.Parse(handHistory);
            //Console.WriteLine(result1);

            //Console.WriteLine("\n=== Версия 2: Query Syntax Only ===");
            //var parser2 = new QuerySyntaxOnlyParser();
            //var result2 = parser2.Parse(handHistory);
            //Console.WriteLine(result2);

            Console.WriteLine("\n=== Версия 3: Простой Query Syntax ===");
            var parser3 = new SimpleQuerySyntaxParser();
            var result3 = parser3.Parse(handHistory);
            Console.WriteLine(result3);
        }
    }

}

