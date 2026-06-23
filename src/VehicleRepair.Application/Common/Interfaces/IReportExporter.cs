namespace VehicleRepair.Application.Common.Interfaces;

public interface IReportExporter
{
    byte[] Export<T>(string title, string[] headers, IReadOnlyList<T> rows, Func<T, object[]> rowMapper);
}
