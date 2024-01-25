namespace Winmd.Model;

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using ClassExtensions;

record TypeModel(string Name, string? Namespace = null, IImmutableList<TypeModifier>? Modifiers = null)
{
    #region Constants

    public static readonly TypeModel GuidType = new(nameof(Guid));

    public static readonly (string Name, string Namespace) LayoutKindName = typeof(LayoutKind).GetQualifiedName()!;
    public static readonly TypeModel LayoutKindType = new(LayoutKindName.Name, LayoutKindName.Namespace);

    public static readonly TypeModel StringType = new(nameof(String));

    #endregion
}

class TypeModifier
{
    public int? Pointer { get; set; }
    public int? Array { get; set; }
}
