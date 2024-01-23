namespace Winmd.ClassExtensions;

static class CollectionExtensions
{
    public static bool IsEmpty<T>(this List<T> list) => list.Count == 0;
}
