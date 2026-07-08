namespace VehicleRepair.Application.Common;

public static class DateTimeUtc
{
    /// <summary>
    /// Normalizes a client-supplied DateTime to Kind=Utc, as required by Npgsql for 'timestamptz' columns.
    /// Local-kind values (e.g. from clients that serialize with a UTC offset) are actually converted;
    /// Unspecified-kind values are assumed to already represent UTC and are only re-flagged, not shifted.
    /// </summary>
    public static DateTime EnsureUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
