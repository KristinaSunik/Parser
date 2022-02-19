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
            var workbook = new XLWorkbook(path);
            var sheet = workbook.Worksheets.First();

            //Примеры использования библиотек
            var cell = sheet.Cell(1, 1);
            var row = cell.WorksheetRow().RowNumber();
            var column = cell.WorksheetColumn().ColumnNumber();
            var value = cell.GetValue<string>();
            var cells = sheet.Cells().Where(c => c.GetValue<string>().Contains("1")).ToList();
            
            throw new NotImplementedException();
        }
    }
}
