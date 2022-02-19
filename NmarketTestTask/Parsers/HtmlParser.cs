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
                var houseNode = line.Elements("td").ToList();
                var houseFound = houses.FirstOrDefault(x => x.Name == houseNode.FirstOrDefault(n => n.Attributes.FirstOrDefault(y => y.Name == "class").Value == "house").InnerText);
                var number = houseNode.Find(x => x.Attributes.FirstOrDefault(y => y.Name == "class").Value == "number");
                var price = houseNode.Find(x => x.Attributes.FirstOrDefault(y => y.Name == "class").Value == "price");

                if (houseFound == null)  
                {
                    var name = houseNode.Find(x => x.Attributes.FirstOrDefault(y => y.Name == "class").Value == "house");
                    var newHouse = CreateNewHouse(name, number, price);
                    houses.Add(newHouse);
                }
                else //если такой дом уже есть в списке
                {
                    var flatFound = houseFound.Flats.FirstOrDefault(x => x.Number == houseNode.FirstOrDefault(h => h.Attributes.FirstOrDefault(y => y.Name == "class").Value == "number").InnerText);
                    if (flatFound == null)
                    {
                        if (number != null && price != null)
                        {
                            houseFound.Flats.Add(new Flat()
                            {
                                Number = number.InnerText,
                                Price = price.InnerText.Replace(" ", "").Replace(",",".")
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

        private static House CreateNewHouse(HtmlNode name, HtmlNode number, HtmlNode price)
        {
            House house;
            try
            {
                house = new House()
                {
                    Name = name.InnerText,
                    Flats = new List<Flat>
                    {
                        new Flat()
                        {
                            Number = number.InnerText,
                            Price = price.InnerText.Replace(" ", "").Replace(",",".")
                        }
                    }
                };
            }
            catch
            {
                throw new Exception("Не удалось считать со страницы данные о доме");
            }

            return house;
        }
    }
}
