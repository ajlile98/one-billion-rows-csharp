using System;
using System.Text;

namespace one_billion_rows_csharp.Extensions;

public static class CollectionExtensions
{
    public static string Dump<T>(this IEnumerable<T> collection)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Collection Type: {typeof(T).Name}[]");
        int index = 0;
        foreach (T item in collection)
        {
            sb.AppendLine($"  [{index++}]: {item?.ToString() ?? "null"}");
            // For more complex objects, you could use reflection here to dump properties
        }
        return sb.ToString();
    }
}