namespace Winmd.Model;

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using ClassExtensions;

record TypeModel(string Name, string? Namespace = null, IImmutableList<string>? Nesting = null, IImmutableList<TypeModifier>? Modifiers = null)
{
    public static readonly TypeModel GuidType = new(nameof(Guid));
    public static readonly TypeModel StringType = new(nameof(String));
}

record TypeModifier(int? Pointer, int? Array);
