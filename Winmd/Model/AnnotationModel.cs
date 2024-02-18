namespace Winmd.Model;

using System.Collections.Immutable;

record AnnotationModel(string Name, string? Namespace = null)
{
    public IImmutableDictionary<string, object> Properties { get; init; } = ImmutableDictionary<string, object>.Empty;
}
