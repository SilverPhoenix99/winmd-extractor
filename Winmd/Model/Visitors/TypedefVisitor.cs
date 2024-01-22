namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using Mono.Cecil;

class TypedefVisitor : BaseObjectVisitor<TypedefModel>
{
    public static readonly TypedefVisitor Instance = new();

    private TypedefVisitor() {}

    protected override TypedefModel CreateModel(string name) => new(name);

    public override TypedefModel Visit(TypeDefinition value)
    {
        var model = base.Visit(value);

        model.Attributes = model.Attributes
            .Where(a =>
                a.Name != "NativeTypedef"
                && a.Namespace != "Windows.Win32.Foundation.Metadata"
            )
            .ToImmutableList();

        return model;
    }
}
