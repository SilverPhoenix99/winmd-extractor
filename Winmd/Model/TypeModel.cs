namespace Winmd.Model;

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using ClassExtensions;

class TypeModel(string name, string? @namespace = null)
{
    public string Name => name;
    public string? Namespace => @namespace;
    public IImmutableList<TypeModifier>? Modifiers { get; set; }
    public IImmutableList<AttributeModel>? Attributes { get; set; }

    #region Constants

    public static readonly TypeModel GuidType = new("Guid");

    public static readonly (string Name, string Namespace) LayoutKindName = typeof(LayoutKind).GetQualifiedName()!;
    public static readonly TypeModel LayoutKindType = new(LayoutKindName.Name, LayoutKindName.Namespace);

    public static readonly TypeModel StringType = new("String");

    #endregion
}

class TypeModifier
{
    public int? Pointer { get; set; }
    public int? Array { get; set; }
}
