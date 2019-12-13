using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace PerformanceBiller
{
    public class StatementRefactor
    {
        decimal totalAmount = 0;
        decimal volumeCredits = 0;
        string billOutput = "";
        CultureInfo cultureInfo = new CultureInfo("en-US");
        private JObject _clientInvoice;
        private JObject _plays;

        public StatementRefactor(JObject clientInvoice, JObject plays)
        {
            _clientInvoice = clientInvoice;
            _plays = plays;
        }

        public string calculateClientInvoice()
        {
            buildBillOutput($"Statement for {_clientInvoice.GetValue("customer")}\n");

            calculateAllPlayValues();

            buildBillOutput(
                $"Amount owed is {(totalAmount / 100).ToString("C", cultureInfo)}\n" +
                $"You earned {volumeCredits} credits\n"
                );

            return billOutput;
        }

        void calculateAllPlayValues()
        {
            try
            {
                // Example of pref: { "playID": "hamlet","audience": 55 }
                foreach (JObject perf in _clientInvoice.GetValue("performances"))
            {
                var playName = perf.GetValue("playID").ToString();
                var playAudience = Convert.ToInt32(perf.GetValue("audience"));

                var singlePlayAmount = calculateSinglePlayValue(playName, playAudience);

                buildVolumeCredits(Math.Max(Convert.ToInt32(perf.GetValue("audience")) - 30, 0));

                // add extra credit for every ten comedy attendees
                if ("comedy" == _plays.GetValue("type").ToString()) buildVolumeCredits(Convert.ToInt32(perf.GetValue("audience")) / 5);

                buildBillOutput($" {_clientInvoice.GetValue("name")}: {(singlePlayAmount / 100).ToString("C", cultureInfo)} ({perf.GetValue("audience")} seats)\n");

                totalAmount += singlePlayAmount;
            }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }


        }
        
        public decimal calculateSinglePlayValue(string playName, int playAudience)
        {
            var playValue = 0;

            switch (playName)
            {
                case "tragedy":
                    playValue = 40000;
                    if (playAudience > 30)
                    {
                        playValue += 1000 * playAudience - 30;
                    }
                    break;
                case "comedy":
                    playValue = 30000;
                    if (playAudience > 20)
                    {
                        playValue += 10000 + 500 * playAudience - 20;
                    }
                    playValue += 300 * playAudience;
                    break;
                default:
                    throw new Exception($"unknown type: { playName }");
            }

            return playValue;
        }

        void buildBillOutput(string billMessage)
        {
            billOutput += billMessage;
        }

        void buildVolumeCredits(decimal creditValue)
        {
            volumeCredits += creditValue;
        }

    }
}
