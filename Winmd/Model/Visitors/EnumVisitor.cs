namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;
using Mono.Cecil.Rocks;

class EnumVisitor : BaseObjectModelVisitor<EnumModel>
{
    public static readonly EnumVisitor Instance = new();

    private EnumVisitor() {}

    protected override EnumModel CreateModel(string name) => new(name);

    public override EnumModel Visit(TypeDefinition value)
    {
        var model = base.Visit(value);

        // Get the primitive base type.
        // Ignore the default, to keep the output short.
        var baseType = value.GetEnumUnderlyingType().Name;
        if (baseType != nameof(Int32))
        {
            model.EnumType = baseType;
        }

        model.Members = value.Fields
            .Where(field => !field.IsSpecialName)
            .Select(field => field.Accept(EnumMemberVisitor.Instance))
            .ToImmutableList();

        return model;
    }
}
