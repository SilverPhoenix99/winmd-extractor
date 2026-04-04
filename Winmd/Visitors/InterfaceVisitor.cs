using System.Collections.Immutable;
using Mono.Cecil;
using Winmd.ClassExtensions;
using Winmd.Model;

namespace Winmd.Visitors;

internal class InterfaceVisitor : BaseObjectVisitor<InterfaceModel>
{
    public static readonly InterfaceVisitor Instance = new();

    private InterfaceVisitor() {}

    public override InterfaceModel Visit(TypeDefinition type)
    {
        var annotations = GetAnnotations(type);

        return new InterfaceModel(type.Name, annotations)
        {
            Parent = type.HasInterfaces ? type.Interfaces[0].InterfaceType.Accept(TypeVisitor.Instance) : null,
            Methods = ImmutableList.CreateRange(
                from m in type.Methods
                where !m.IsConstructor
                select m.Accept(FunctionVisitor.Instance)
            )
        };
    }
}
