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
                        Name = value.Replace("Дом ",""),
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
                            currentHouse.Flats.Add(CreateNewFlat(sheet, value, row, column));
                        }
                        else
                        {
                            currentHouse.Flats = new List<Flat>
                            {
                               CreateNewFlat(sheet, value, row, column)
                            };
                        }
                    }
                    else
                    {
                        throw new Exception("На странице не было найдено имени дома");
                    }
                }
            }

            houses.Add(currentHouse); //добавляем последний считанный дом

            return houses;
        }

        private static Flat CreateNewFlat(IXLWorksheet sheet, string value, int row, int column)
        {
            string price;
            try
            {
                price = sheet.Cell(row + 1, column).GetValue<string>();
            }
            catch
            {
                throw new Exception("На странице не была найдена цена квартиры");
            }

            return new Flat()
            {
                Number = value.Replace("№", ""),
                Price = price
            };
        }
    }
}
