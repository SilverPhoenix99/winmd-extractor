namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using Mono.Cecil;
using Mono.Cecil.Rocks;

class EnumVisitor : BaseObjectVisitor<EnumModel>
{
    public static readonly EnumVisitor Instance = new();

    private EnumVisitor() {}

    public override EnumModel Visit(TypeDefinition type)
    {
        // Get the primitive base type.
        // Ignore the default, to keep the output short.
        var baseType = type.GetEnumUnderlyingType().Name;
        if (baseType == nameof(Int32))
        {
            baseType = null; // It's the default
        }

        return new EnumModel(type.Name, GetAttributes(type))
        {
            EnumType = baseType,
            Members = ImmutableList.CreateRange(
                from f in type.Fields
                where !f.IsSpecialName
                select new EnumMemberModel(f.Name, f.Constant)
            )
        };
    }
}
