namespace Winmd.ClassExtensions;

static class CollectionExtensions
{
    public static bool IsEmpty<T>(this IReadOnlyCollection<T> list) => list.Count == 0;
}
