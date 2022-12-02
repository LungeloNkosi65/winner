using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace winner
{
    public class winner
    {
        private static readonly string projectPath = System.IO.Directory.GetCurrentDirectory();
        //projectPath
        private static  string currentDirectoryPath = projectPath.Remove(projectPath.IndexOf("winner"))+ "winner"+"\\winner";
        //var actualPath = projectPath + $@"HISDatabase\\{schema}\Stored Procedures";
        
        static void Main(string[] args)
        {
            try
            {
                //string inputfile ="abc.txt";
                string inputfile = (string)args[1].ToString();
                //string outfile = "xyz.txt";
                string outfile = (string)args[3].ToString();

                Console.WriteLine(inputfile);
                Console.WriteLine(outfile);

                string filenamer = Path.Combine(currentDirectoryPath, inputfile);
                var playersInput = new Dictionary<string, int> { };
                using (var sr = new StreamReader(filenamer))
                {
                    string line, playerName;
                    List<string> scoreResults = new List<string>();
                    List<string> nameResults = new List<string>();
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!String.IsNullOrWhiteSpace(line))
                        {
                            int colonLocation = line.IndexOf(':', StringComparison.Ordinal);
                            if (colonLocation > 0)
                            {
                                playerName = line.Substring(0, colonLocation);

                                string Card1 = GetCard(colonLocation, 1, line);
                                List<char> Card1List = new List<char>();
                                Card1List.AddRange(Card1);

                                string Card2 = GetCard(colonLocation, 4, line);
                                List<char> Card2List = new List<char>();
                                Card2List.AddRange(Card2);

                                string Card3 = GetCard(colonLocation, 7, line);
                                List<char> Card3List = new List<char>();
                                Card3List.AddRange(Card3);

                                string Card4 = GetCard(colonLocation, 10, line);
                                List<char> Card4List = new List<char>();
                                Card4List.AddRange(Card4);

                                string Card5 = GetCard(colonLocation, 13, line);
                                List<char> Card5List = new List<char>();
                                Card5List.AddRange(Card5);

                                int cardNumberTotal = GetCardValue(Card1List[0].ToString()) + GetCardValue(Card2List[0].ToString()) +
                                    GetCardValue(Card3List[0].ToString()) + GetCardValue(Card4List[0].ToString()) + GetCardValue(Card5List[0].ToString());

                                 var CalcSuitTotal = GetSuitValue(Card1List[1].ToString()) + GetSuitValue(Card2List[1].ToString()) + GetSuitValue(Card3List[1].ToString())
                                    + GetSuitValue(Card4List[1].ToString()) + GetSuitValue(Card5List[1].ToString());

                                playersInput.Add(playerName + "#" + CalcSuitTotal, cardNumberTotal);

                                var cardTotalValueWinners = GetTotalWinners(playersInput);

                                if (cardTotalValueWinners.Count > 1)
                                {
                                    var suitTotals = SplitSuitPlayerResult(cardTotalValueWinners);
                                    cardTotalValueWinners = GetTotalWinners(suitTotals);
                                }

                                string _gameResults = ProcessResults(cardTotalValueWinners);

                                OutPutResult(_gameResults, outfile);
                            }
                            else
                            {
                                OutPutResult("ERROR", outfile);
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {

                 throw ex;
            }
         
        }

        public static string ProcessResults(Dictionary<string, int> playersKeyValuePair)
        {
            var results = new StringBuilder();
            int tiescore = playersKeyValuePair.Values.Max();
            if (playersKeyValuePair.Count > 1)
            {
                foreach (KeyValuePair<string, int> player in playersKeyValuePair)
                {
                    if (results.Length > 0)
                    {
                        results.Append($", {player.Key}");
                    }
                    else
                    {
                        results.Append($"{player.Key}");
                    }

                }
            }
            else
            {
                var winnerName = playersKeyValuePair.ElementAt(0).Key.ToString();
                results.Append($"{winnerName.Substring(0, winnerName.IndexOf("#"))}");
            }
            results.Append($": {tiescore}");
            return results.ToString();
        }
        public static Dictionary<string, int> GetTotalWinners(Dictionary<string, int> playersKeyValuePair)
        {
            var results = new Dictionary<string, int>();
            int highestCardTotal = playersKeyValuePair.Values.Max();

            foreach (KeyValuePair<string, int> player in playersKeyValuePair)
            {
                if (player.Value == highestCardTotal)
                {
                    results.Add(player.Key, player.Value);
                }
            }
            return results;
        }

       public static Dictionary<string, int> SplitSuitPlayerResult(Dictionary<string, int> keyValuePairs)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> kvp in keyValuePairs)
            {
                result.Add(kvp.Key.Split("#")[0], Int32.Parse(kvp.Key.Split("#")[1]));
            }
            return result;
        }

        public static void OutPutResult(string gameResuslt,string outputFile)
        {
            try
            {
                Console.WriteLine(gameResuslt);
                string filename = Path.Combine(currentDirectoryPath, outputFile);
                File.Delete(filename);
                StreamWriter sw = new StreamWriter(filename, true, Encoding.ASCII);

                if ("ERROR".Equals(gameResuslt))
                {
                    sw.Write(gameResuslt);
                }
                else
                {
                    sw.Write(gameResuslt);
                }
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally { }
        }

        public static int GetSuitValue(string CardGroup)
        {
            var suitValue = CardGroup.ToUpper() switch
            {
                "H" => 3,
                "S" => 4,
                "D" => 2,
                "C" => 1,
                _ => 0,
            };
            return suitValue;
        }
        static string GetCard(int colonLocation, int cardPposition, string lineItem)
        {
            return lineItem.Substring(colonLocation + cardPposition, 2);
        }
        public static int GetCardValue(string CardNumber)
        {
            int cardValue = 0;
            if (!(CardNumber.ToString().Equals("A")) && !(CardNumber.ToString().Equals("J")) && !(CardNumber.ToString().Equals("K")) && !(CardNumber.ToString().Equals("Q")))
            {
                cardValue = int.Parse(CardNumber.ToString());
            }
            else
            {
                switch (CardNumber)
                {
                    case "A":
                        cardValue = 1;
                        break;
                    case "J":
                        cardValue = 11;
                        break;
                    case "Q":
                        cardValue = 12;
                        break;
                    case "K":
                        cardValue = 13;
                        break;
                }
            }


            return cardValue;
        }
    }

   
}
