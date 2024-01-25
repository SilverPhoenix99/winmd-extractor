namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class TypedefVisitor : BaseObjectVisitor<TypedefModel>
{
    public static readonly TypedefVisitor Instance = new();

    private TypedefVisitor() {}

    public override TypedefModel Visit(TypeDefinition type)
    {
        var fieldType = type.Fields
            .First(f => f.IsPublic && !f.IsStatic)
            .FieldType;

        return new TypedefModel(
            type.Name,
            GetAttributes(type),
            fieldType.Accept(TypeVisitor.Instance)
        );
    }

    protected override IImmutableList<AttributeModel>? GetAttributes(TypeDefinition type)
    {
        var attributes = base.GetAttributes(type);
        if (attributes is null)
        {
            return null;
        }

        attributes = ImmutableList.CreateRange(
            from a in attributes
            where a is not { Name: "NativeTypedef", Namespace: "Windows.Win32.Foundation.Metadata" }
            select a
        );

        return attributes.IsEmpty() ? null : attributes;
    }
}
