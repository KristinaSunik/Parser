using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using NmarketTestTask.Models;

namespace NmarketTestTask.Parsers
{
    public class HtmlParser : IParser
    {
        public IList<House> GetHouses(string path)
        {
            List<House> houses = new List<House>();

            var doc = new HtmlDocument();
            doc.Load(path);

            var node = doc.DocumentNode.SelectSingleNode("//tbody");
            var nodes = node.SelectNodes(".//tr");

            foreach (var line in nodes)
            {
                var elements = line.Elements("td").ToList();
                var houseFound = houses.FirstOrDefault(x => x.Name == elements.FirstOrDefault(n => n.Attributes.FirstOrDefault(y => y.Name == "class").Value == "house").InnerText);
                if (houseFound == null)
                {
                    var currentHouse = CreateNewHouse(elements);
                    houses.Add(currentHouse);
                }
                else
                {
                    var flatFound = houseFound.Flats.FirstOrDefault(x => x.Number == elements.FirstOrDefault(h => h.Attributes.FirstOrDefault(y => y.Name == "class").Value == "number").InnerText);
                    if (flatFound == null)
                    {
                        string number = elements.Find(x => x.Attributes.FirstOrDefault(y => y.Name == "class").Value == "number").InnerText;
                        string price = elements.Find(x => x.Attributes.FirstOrDefault(y => y.Name == "class").Value == "price").InnerText;

                        if (number != "" && price != "")
                        {
                            houseFound.Flats.Add(new Flat()
                            {
                                Number = number,
                                Price = price
                            });
                        }
                        else
                        {
                            throw new Exception("The Html page doesn't have parameters for Flat");
                        }
                    }
                }
            }

            return houses;
        }

        private static House CreateNewHouse(List<HtmlNode> elements)
        {
            return new House()
            {
                Name = elements.Find(x => x.Attributes.FirstOrDefault(y => y.Name == "class").Value == "house").InnerText,
                Flats = new List<Flat>
                {
                    new Flat()
                    {
                        Number = elements.Find(x => x.Attributes.FirstOrDefault(y => y.Name == "class").Value == "number").InnerText,
                        Price = elements.Find(x => x.Attributes.FirstOrDefault(y => y.Name == "class").Value == "price").InnerText
                    }
                }
            };
        }
    }
}
