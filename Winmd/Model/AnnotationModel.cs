using System.Collections.Immutable;

namespace Winmd.Model;

internal record AnnotationModel(string Name, string? Namespace = null)
{
    public IImmutableDictionary<string, object> Properties { get; init; } = ImmutableDictionary<string, object>.Empty;
}
