namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class TypedefVisitor : BaseObjectVisitor<TypedefModel>
{
    public static readonly TypedefVisitor Instance = new();

    private TypedefVisitor() {}

    protected override TypedefModel CreateModel(string name) => new(name);

    public override TypedefModel Visit(TypeDefinition type)
    {
        var model = base.Visit(type);

        var fieldType = type.Fields
            .First(f => f.IsPublic && !f.IsStatic)
            .FieldType;

        model.SourceType = fieldType.Accept(TypeVisitor.Instance);

        var attributes = model.Attributes
            ?.Where(a => a.Name != "NativeTypedef" && a.Namespace != "Windows.Win32.Foundation.Metadata")
            .ToImmutableList();

        if (!attributes?.IsEmpty ?? true)
        {
            model.Attributes = attributes;
        }

        return model;
    }
}
