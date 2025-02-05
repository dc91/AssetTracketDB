﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AssetTracketDB
{
    internal class LiveCurrency
    {
        public static List<KeyValuePair<string, decimal>> FetchRates()
        {
            List<KeyValuePair<string, decimal>> fetchedQuotes = [];
            string url = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

            XmlTextReader reader = new(url);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "currency" && (reader.Value == "USD" || reader.Value == "SEK"))
                        {
                            string currencyCode = reader.Value;

                            reader.MoveToNextAttribute();
                            decimal rate = decimal.Parse(reader.Value.Replace('.', ','));
                            fetchedQuotes.Add(new KeyValuePair<string, decimal>(currencyCode, rate));
                        }
                    }
                }
            }
            return fetchedQuotes;
        }
    }
}
