using Newtonsoft.Json.Linq;
using PerformanceBiller.Infrastructure;
using PerformanceBiller.Models;
using PerformanceBiller.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PerformanceBiller
{
    public class StatementRefactor

    {
        decimal totalAmount = 0;
        decimal volumeCredits = 0;
        string billOutput = "";
        CultureInfo cultureInfo = new CultureInfo("en-US");

        Invoices invoice = new Invoices();
        List<Play> plays = new List<Play>();


        public StatementRefactor()
        {
            // Use JsonReader.cs to populate invoice and plays vars.
        }

        public string calculateClientInvoice()
        {
            buildBillOutput($"Statement for {invoice.customerName}\n");

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
                foreach (Performace perf in invoice.performList)
                {
                    var playName = perf.playID;
                    var playAudience = perf.audience;

                    var singlePlayAmount = calculateSinglePlayValue(playName, playAudience);

                    buildVolumeCredits(Math.Max(playAudience - 30, 0));

                    if (playName == "as-like")
                    {
                        buildVolumeCredits(playAudience / 5);
                    }
                    
                    buildBillOutput($" {invoice.customerName}: {(singlePlayAmount / 100).ToString("C", cultureInfo)} ({playAudience} seats)\n");

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
