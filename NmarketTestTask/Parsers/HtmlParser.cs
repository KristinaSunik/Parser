using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using HtmlAgilityPack;
using NmarketTestTask.Models;

namespace NmarketTestTask.Parsers
{
    public class HtmlParser : IParser
    {
        public IList<House> GetHouses(string path)
        {
            List<House> houses = new List<House>();
            House currentHouse;
            var doc = new HtmlDocument();
            doc.Load(path);


            var deserialised = XDocument.Parse(doc.ParsedText)
                .Descendants("html")
                .Descendants("body")
                .Descendants("table")
                .Descendants("tbody")
                .Descendants("tr")
                .ToList();

            foreach (var line in deserialised)
            {
                var houseFound = houses.FirstOrDefault(x => x.Name == line.Elements()
                                                                          .FirstOrDefault(h => h.FirstAttribute.Value == ("house")).Value);
                if (houseFound == null)
                {
                    currentHouse = CreateNewHouse(line);
                    houses.Add(currentHouse);
                }
                else
                {
                    var flatFound = houseFound.Flats.FirstOrDefault(x => x.Number == line.Elements()
                                                                                         .FirstOrDefault(h => h.FirstAttribute.Value == ("number")).Value);
                    if (flatFound == null)
                    {
                        houseFound.Flats.Add(new Flat()
                        {
                            Number = line.Elements().First(x => x.FirstAttribute.Value == "number").Value,
                            Price = line.Elements().First(x => x.FirstAttribute.Value == "price").Value
                        });
                    }
                }

            }

            return houses;
        }

        private static House CreateNewHouse(XElement element)
        {
            var elements = element.Elements("td").ToList();
            var name = elements.FirstOrDefault(x => x.FirstAttribute.Value == "house");
            return new House()
            {
                Name = name.Value,
                Flats = new List<Flat>
                {
                    new Flat()
                    {
                        Number = elements.FirstOrDefault(x => x.FirstAttribute.Value =="number").Value,
                        Price = elements.FirstOrDefault(x => x.FirstAttribute.Value =="price").Value
                    }
                }
            };
        }
    }
}
