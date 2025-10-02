namespace Subs.Utils.Extensions;

public static class DateTimeExtension
{
    public static DateTime EnsureUtc(this DateTime dateTime)
    {
        return dateTime.Kind == DateTimeKind.Utc
            ? dateTime
            : dateTime.ToUniversalTime();
    }
}