namespace Winmd.ClassExtensions;

internal static class CollectionExtensions
{
    extension<T>(IReadOnlyCollection<T> list)
    {
        public bool IsEmpty => list.Count == 0;
    }
}
