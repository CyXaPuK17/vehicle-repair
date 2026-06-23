using ClosedXML.Excel;
using VehicleRepair.Application.Common.Interfaces;

namespace VehicleRepair.Infrastructure.Export;

public class ExcelReportExporter : IReportExporter
{
    public byte[] Export<T>(string title, string[] headers, IReadOnlyList<T> rows, Func<T, object[]> rowMapper)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Отчёт");

        ws.Cell(1, 1).Value = title;
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;
        ws.Range(1, 1, 1, headers.Length).Merge();

        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(2, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        int row = 3;
        foreach (var item in rows)
        {
            var values = rowMapper(item);
            for (int col = 0; col < values.Length; col++)
            {
                ws.Cell(row, col + 1).Value = values[col]?.ToString() ?? string.Empty;
            }
            row++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}
