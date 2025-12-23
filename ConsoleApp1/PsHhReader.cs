using System;
using System.Collections.Generic;
using System.Text;

namespace PSHhParser
{

    //функция, читающая txt файл по заданному path. Нарезает непрерывный текст раздач в List<string>,
    // чтобы потом через цикл скормить их парсеру, обрабатывающий 1 раздачу  PSHandParser.ParseHand(hand);

    public static class PsHhReader  
    {
        public static List<string> ReadHands(string path)
        {
            var hands = new List<string>();
            var currentHand = new StringBuilder();  //временная переменная, очищаемая к концу кажого цикла после добавления в переменныю hands


            bool startHand = false;  //флаги, изначально зануленные и заполняемые по мере продвижения к концу раздачи. 
            bool summary = false;
            bool totalPotLine = false;
            bool summaryCompleted = false;
            int seatLineCointer = 0; //сколько строк seat уже прошли


            foreach (var line in File.ReadLines(path))
            {
                if (string.IsNullOrEmpty(line)) continue;


                if (line.StartsWith("PokerStars Hand #"))   // если встречаем начало раздачи и все флаги заполнены обрезаем заполение currentHand и пушим содержимое в hands
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
                 
                if (startHand)  //мы внутри раздачи добавляем строка за строкой в currentHand
                {
                    currentHand.AppendLine(line);
                }
                else continue; // Добавляем строки только после начала раздачи, остальное игнорируем


                if (line.StartsWith("*** SUMMARY ***")) //дальше по мере заполения флагов подходим к концу разадчи и когда опять встретим "PokerStars Hand #" обрезаем чтение
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

            if (startHand && summaryCompleted)  // случай конца txt файла - пушим в List накопленное, хоть и не встретили начало новой руки
            {

                hands.Add(currentHand.ToString().Trim());
            }

            return hands;
        }


    }
}
