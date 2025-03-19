using Core.Extentions;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Core.Services
{


    public static class ExcelService
    {
        public static async Task<List<string>> ReadExcelFile(IFormFile file)
        {
            if (!file.IsCorrectFile("xls") && !file.IsCorrectFile("xlsx"))
                throw new ArgumentException("Invalid file format. Only .xls and .xlsx files are supported");
            if (file.IsFileSizeSafe(10 * 1024 * 1024 ))
                throw new ArgumentException("File size exceeds 10MB limit");

            var extension = Path.GetExtension(file.FileName).ToLower();
            var data = new List<List<string>>();
            var results = new List<string>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                IWorkbook workbook;
                if (extension == ".xlsx")
                    workbook = new XSSFWorkbook(stream);
                else
                    workbook = new HSSFWorkbook(stream);

                ISheet sheet = workbook.GetSheetAt(0);
                if (sheet == null || sheet.LastRowNum < 0)
                    return results;

                // Get header row (first row)
                IRow headerRow = sheet.GetRow(0);
                if (headerRow == null)
                    throw new InvalidDataException("Excel file is missing header row");

                List<string> headers = new List<string>();
                for (int cellIndex = 0; cellIndex < headerRow.LastCellNum; cellIndex++)
                {
                    ICell cell = headerRow.GetCell(cellIndex);
                    headers.Add(GetCellValue(cell).Trim());
                }

                // Process data rows starting from row 1
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    if (row == null) continue;

                    List<string> rowValues = new List<string>();
                    for (int cellIndex = 0; cellIndex < headers.Count; cellIndex++)
                    {
                        ICell cell = row.GetCell(cellIndex);
                        rowValues.Add(GetCellValue(cell));
                    }

                    // Create formatted string
                    var formattedRow = new List<string>();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        formattedRow.Add($"{headers[i]}: {rowValues[i]}");
                    }

                    string resultRow = string.Join(", ", formattedRow);
                    results.Add(resultRow);
                    Console.WriteLine(resultRow);
                }
            }

            return results;
        }

        private static string GetCellValue(ICell cell)
        {
            if (cell == null) return string.Empty;

            switch (cell.CellType)
            {
                case CellType.Numeric:
                    return cell.NumericCellValue.ToString();
                case CellType.String:
                    return cell.StringCellValue.Trim();
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Formula:
                    return cell.ToString();
                default:
                    return string.Empty;
            }
        }
    }

}
