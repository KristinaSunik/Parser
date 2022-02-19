using System.Collections.Generic;
using HtmlAgilityPack;
using NmarketTestTask.Models;

namespace NmarketTestTask.Parsers
{
    public class HtmlParser : IParser
    {
        public IList<House> GetHouses(string path)
        {
            var doc = new HtmlDocument();
            doc.Load(path);

            //Примеры использования библиотек
            var node = doc.DocumentNode.SelectSingleNode("//thead");
            var text = node.InnerText;
            var nodes = doc.DocumentNode.SelectNodes(".//th");

            throw new System.NotImplementedException();
        }
    }
}
