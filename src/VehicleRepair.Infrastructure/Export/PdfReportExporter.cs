using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VehicleRepair.Application.Common.Interfaces;

namespace VehicleRepair.Infrastructure.Export;

public class PdfReportExporter : IReportExporter
{
    static PdfReportExporter()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Export<T>(string title, string[] headers, IReadOnlyList<T> rows, Func<T, object[]> rowMapper)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Text(title).SemiBold().FontSize(14).AlignCenter();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        foreach (var _ in headers)
                            cols.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        foreach (var h in headers)
                        {
                            header.Cell().Background(Colors.LightBlue.Lighten2)
                                .Padding(4).Text(h).SemiBold();
                        }
                    });

                    foreach (var item in rows)
                    {
                        var values = rowMapper(item);
                        foreach (var val in values)
                        {
                            table.Cell().BorderBottom(0.5f, Unit.Point).BorderColor(Colors.Grey.Lighten2)
                                .Padding(3).Text(val?.ToString() ?? string.Empty);
                        }
                    }
                });

                page.Footer().AlignRight()
                    .Text(t =>
                    {
                        t.Span($"Сформировано: {DateTime.UtcNow:dd.MM.yyyy HH:mm} UTC  Стр. ");
                        t.CurrentPageNumber();
                        t.Span(" из ");
                        t.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }
}
