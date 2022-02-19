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
                var housesNodes = line.Elements("td").ToList();
                var houseFound = houses.FirstOrDefault(x => x.Name == housesNodes.FirstOrDefault(n => n.Attributes.FirstOrDefault(y => y.Name == "class").Value == "house").InnerText);
                if (houseFound == null)  
                {
                    var newHouse = CreateNewHouse(housesNodes);
                    houses.Add(newHouse);
                }
                else //если такой дом уже есть в списке
                {
                    var flatFound = houseFound.Flats.FirstOrDefault(x => x.Number == housesNodes.FirstOrDefault(h => h.Attributes.FirstOrDefault(y => y.Name == "class").Value == "number").InnerText);
                    if (flatFound == null)
                    {
                        string number = housesNodes.Find(x => x.Attributes.FirstOrDefault(y => y.Name == "class").Value == "number").InnerText;
                        string price = housesNodes.Find(x => x.Attributes.FirstOrDefault(y => y.Name == "class").Value == "price").InnerText;

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

        private static House CreateNewHouse(List<HtmlNode> housesNodes)
        {
            return new House()
            {
                Name = housesNodes.Find(x => x.Attributes.FirstOrDefault(y => y.Name == "class").Value == "house").InnerText,
                Flats = new List<Flat>
                {
                    new Flat()
                    {
                        Number = housesNodes.Find(x => x.Attributes.FirstOrDefault(y => y.Name == "class").Value == "number").InnerText,
                        Price = housesNodes.Find(x => x.Attributes.FirstOrDefault(y => y.Name == "class").Value == "price").InnerText
                    }
                }
            };
        }
    }
}
