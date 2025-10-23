using System.Collections.Immutable;
using Mono.Cecil;
using Winmd.ClassExtensions;

namespace Winmd.Model.Visitors;

internal class TypedefVisitor : BaseObjectVisitor<TypedefModel>
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
            GetAnnotations(type),
            fieldType.Accept(TypeVisitor.Instance)
        );
    }

    protected override IImmutableList<AnnotationModel>? GetAnnotations(TypeDefinition type)
    {
        var annotations = base.GetAnnotations(type);
        if (annotations is null)
        {
            return null;
        }

        annotations = ImmutableList.CreateRange(
            from a in annotations
            where a is not { Name: "NativeTypedef", Namespace: "Windows.Win32.Foundation.Metadata" }
            select a
        );

        return annotations.IsEmpty() ? null : annotations;
    }
}
