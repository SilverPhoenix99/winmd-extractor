using System.Collections.Immutable;

namespace Winmd.Model;

internal record TypeModel(string Name, string? Namespace = null, IImmutableList<string>? Nesting = null, IImmutableList<TypeModifier>? Modifiers = null)
{
    public static readonly TypeModel GuidType = new(nameof(Guid));
    public static readonly TypeModel StringType = new(nameof(String));
}

internal record TypeModifier(int? Pointer, int? Array);
