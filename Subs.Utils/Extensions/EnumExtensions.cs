namespace Subs.Utils.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Returns all names of the enum T as a list of strings.
    /// </summary>
    public static List<string> GetAllNames<T>() where T : Enum
        => [.. Enum.GetNames(typeof(T))];

    /// <summary>
    /// Returns all values of the enum T as a list of T.
    /// </summary>
    public static List<T> GetAllValues<T>() where T : Enum
        => [.. Enum.GetValues(typeof(T)).Cast<T>()];
}