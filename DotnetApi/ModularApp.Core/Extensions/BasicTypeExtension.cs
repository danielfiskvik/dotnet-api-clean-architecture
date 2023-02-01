namespace ModularApp.Core.Extensions;

public static class BasicTypeExtension
{
    public static bool ToBoolOrDefault(this string value)
    {
        return bool.TryParse(value, out var result) ? result : default;
    }

    public static int ToIntOrDefault(this string value)
    {
        return int.TryParse(value, out var result) ? result : default;
    }


    public static int ToIntOrDefault(this double value)
    {
        try
        {
            return Convert.ToInt32(value);
        }
        catch (Exception)
        {
            return default;
        }
    }

    public static decimal? ToNullDecimalOrDefault(this string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? default
            : decimal.TryParse(value, out var result)
                ? result
                : Convert.ToDecimal(value);
    }

    public static double ToDoubleOrDefault(this uint value)
    {
        return Convert.ToDouble(value);
    }

    public static double ToDoubleOrDefault(this string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? default
            : double.TryParse(value, out var result)
                ? result
                : Convert.ToDouble(value);
    }

    public static decimal ToDecimalOrDefault(this string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? default
            : decimal.TryParse(value, out var result)
                ? result
                : Convert.ToDecimal(value);
    }

    public static IEnumerable<(T item, int index)>? WithIndex<T>(this IEnumerable<T>? source)
    {
        return source?.Select((item, index) => (item, index));
    }

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }

    public static List<int> PipedStringToInt(this string? pipedString)
    {
        var value = pipedString ?? "";

        return value
            .Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToIntOrDefault())
            .ToList();
    }

    public static string IntToPipedString(this IEnumerable<int>? values)
    {
        var types = values?.ToList() ?? new List<int>();

        var pipeSeparatedString = string.Join("|", types);

        return pipeSeparatedString.Length > 0 ? $"|{pipeSeparatedString}|" : "";
    }
}