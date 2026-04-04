using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Winmd.Model;

internal record TypeModel(
    string Name,
    string? Namespace = null,
    [UsedImplicitly] IImmutableList<string>? Nesting = null,
    IImmutableList<TypeModifier>? Modifiers = null
)
{
    public static readonly TypeModel GuidType = new(nameof(Guid));

    public static readonly TypeModel StringType = new(nameof(String));
}

internal record TypeModifier(
    int? Pointer,
    [UsedImplicitly] int? Array
);
