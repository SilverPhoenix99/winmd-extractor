namespace Winmd.ClassExtensions;

static class CollectionExtensions
{
    public static bool IsEmpty<T>(this ICollection<T> array) => array.Count == 0;
}
