namespace Winmd.Model;

using System.Collections.Immutable;

record AnnotationModel(string Name, string? Namespace = null)
{
    public IImmutableList<AnnotationArgumentModel> Arguments { get; init; } = ImmutableArray<AnnotationArgumentModel>.Empty;
    public IDictionary<string, object> Properties { get; init; } = ImmutableDictionary<string, object>.Empty;
}

record AnnotationArgumentModel(object? Value, TypeModel Type);
