using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace winner
{
    public class addemup
    {
        private static void Main(string[] args)
        {
            string inputFile = GetParamValue(args, "--in");
            string outFile = GetParamValue(args, "--out");

            try
            {
                if (!string.IsNullOrEmpty(inputFile) && !string.IsNullOrEmpty(outFile))
                {
                    IEnumerable<Player> players = GetPlayers(inputFile, outFile);

                    OutputResult(GetWinnerName(players), outFile);
                }
            }
            catch (Exception ex)
            {
                OutputResult($"Exception : {ex.Message}", outFile);
            }
            Console.ReadKey();
        }

        private static string GetParamValue(string[] parameters, string name)
        {
            try
            {
                if (parameters.Any(p => p.Equals(name)))
                {
                    int indexOfParamName = Array.IndexOf(parameters, name);
                    if (indexOfParamName == parameters.Length - 1)
                    {
                        Console.WriteLine($"Exception : Start-up param {name} has no value, please provide with value e.g. {name} pathToFile.txt");
                        return null;
                    }
                    else
                    {
                        return parameters[indexOfParamName + 1];
                    }
                }
                else
                {
                    Console.WriteLine($"Exception : Cannot find parameter {name}");
                }
            }

            catch (Exception ex) { Console.WriteLine($"Exception : {ex.Message}"); }
            return null;
        }

        private static List<Player> GetPlayers(string inputFilePath, string outputFilePath)
        {
            List<Player> players = new List<Player>();
            try
            {
                using (StreamReader sr = new StreamReader(inputFilePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            if (HasPlayerName(line))
                            {
                                Player player = new Player(GetPlayerName(line));

                                foreach (string card in GetPlayerCards(line))
                                {
                                    player.CardScores.Add(GetCardValue(card));
                                    player.SuitScores.Add(GetSuitValue(card));
                                }

                                players.Add(player);

                                //Console.WriteLine($"Player: {player.Name}, Card Total: {player.CardScoreTotal}, Suit Score: {player.SuitScoreTotal}, Cards :: {string.Join(", ", GetPlayerCards(line))}");
                            }
                            else
                            {
                                OutputResult("ERROR", outputFilePath);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { OutputResult($"An error occurred, Exception: {ex.Message}", outputFilePath); }

            return players;
        }

        private static string GetWinnerName(IEnumerable<Player> players)
        {
            string winnerName = "";
            //Sort by total card score then by suit score
            players = players.OrderByDescending(p => p.CardScoreTotal).ThenByDescending(p => p.SuitScoreTotal);

            //since players are ordered, the first in the list has to be the highest (by cards and suit),
            // in case of a tie, any other winner has to have the same card score and suit score 
            int suitScoreTotal = players.FirstOrDefault().SuitScoreTotal;

            int countWinners = 0;
            foreach (Player player in players)
            {
                if (player.CardScoreTotal == players.FirstOrDefault().CardScoreTotal
                    && player.SuitScoreTotal == suitScoreTotal)
                {
                    winnerName += (countWinners > 0) ? $",{player.Name}" : player.Name;
                    countWinners++;
                }
            }

            return $"{winnerName}:{players.FirstOrDefault().CardScoreTotal}";
        }

        private static string GetPlayerName(string line)
        {
            return line.Substring(0, line.IndexOf(':', StringComparison.Ordinal));
        }

        private static bool HasPlayerName(string line)
        {
            return line.IndexOf(':', StringComparison.Ordinal) > 0;
        }

        public static void OutputResult(string message, string outputFile)
        {
            try
            {
                Console.WriteLine(message);
                if (File.Exists(outputFile)) { File.Delete(outputFile); }

                StreamWriter sw = new StreamWriter(outputFile, true, Encoding.ASCII);

                sw.Write(message);

                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally { }
        }

        private static string[] GetPlayerCards(string line)
        {
            line = line.Substring(line.IndexOf(':', StringComparison.Ordinal) + 1);
            return line.Split(',');
        }

        private static bool IsNumber(string cardFace)
        {
            return int.TryParse(cardFace, out int n);
        }

        public static int GetCardValue(string card)
        {
            string cardNumber = card.Substring(0, card.Length - 1);

            if (IsNumber(cardNumber))
            {
                return int.Parse(cardNumber);
            }
            else
            {
                switch (cardNumber.ToUpper())
                {
                    case "A":
                    case "J": return 11;
                    case "Q": return 12;
                    case "K": return 13;
                    default: return 0;
                }
            }
        }

        public static int GetSuitValue(string card)
        {
            return card.Substring(card.Length - 1).ToUpper() switch
            {
                "H" => 3,
                "S" => 4,
                "D" => 2,
                "C" => 1,
                _ => 0,
            };
        }
    }

    public record Player
    {
        public string Name { get; set; }

        public List<int> CardScores = new List<int>();

        public List<int> SuitScores = new List<int>();

        public int CardScoreTotal => CardScores.Sum();

        public int SuitScoreTotal => SuitScores.Sum();

        public Player(string name)
        {
            Name = name;
        }
    }
}
