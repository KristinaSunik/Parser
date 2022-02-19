using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using NmarketTestTask.Models;

namespace NmarketTestTask.Parsers
{
    public class ExcelParser : IParser
    {
        public IList<House> GetHouses(string path)
        {
            List<House> houses = new List<House>();
            House currentHouse = null;

            var workbook = new XLWorkbook(path);
            var sheet = workbook.Worksheets.First();

            var cellsVal = sheet.Cells().Where(c => c.GetValue<string>() != "").ToList();

            var rows = sheet.Rows();
            foreach (var cell in cellsVal)
            {
                var value = cell.GetValue<string>();
                if (value.Contains("Дом"))
                {
                    if (currentHouse != null)
                    {
                        houses.Add(currentHouse);
                        currentHouse = null;
                    }

                    currentHouse = new House()
                    {
                        Name = value,
                        Flats = new List<Flat>()
                    };
                }

                if (value.Contains("№"))
                {
                    if (currentHouse != null)
                    {
                        var row = cell.WorksheetRow().RowNumber();
                        var column = cell.WorksheetColumn().ColumnNumber();
                        if (currentHouse.Flats.Count != 0)
                        {
                            currentHouse.Flats.Add(new Flat()
                            {
                                Number = value,
                                Price = sheet.Cell(row + 1, column).GetValue<string>()
                            });
                        }
                        else
                        {
                            currentHouse.Flats = new List<Flat>
                            {
                                new Flat()
                                {
                                    Number = value,
                                    Price = sheet.Cell(row + 1, column).GetValue<string>()
                                }
                            };
                        }
                    }
                    else
                    {
                        throw new Exception("На странице не нашлось имени дома");
                    }
                }
            }
            houses.Add(currentHouse);

            return houses;
        }
    }
}
