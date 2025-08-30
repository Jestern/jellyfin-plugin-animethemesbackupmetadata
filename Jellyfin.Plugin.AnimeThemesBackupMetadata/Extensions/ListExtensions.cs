using System.Collections.Generic;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Extensions;

public static class ListExtensions
{
    public static void AddIfNotNull<T>(this List<T> list, T value)
    {
        if (value is not null)
        {
            list.Add(value);
        }
    }

    public static void AddRangeIfNotNull<T>(this List<T> list, IEnumerable<T> value)
    {
        if (value is not null)
        {
            list.AddRange(value);
        }
    }
}
