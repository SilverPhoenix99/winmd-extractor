namespace Winmd.Model;

using System.Collections.Immutable;

record AttributeModel(string Name, string? Namespace = null)
{
    public IImmutableList<AttributeArgumentModel> Arguments { get; init; } = ImmutableArray<AttributeArgumentModel>.Empty;
    public IDictionary<string, object> Properties { get; init; } = ImmutableDictionary<string, object>.Empty;
}

record AttributeArgumentModel(object? Value, TypeModel Type);
