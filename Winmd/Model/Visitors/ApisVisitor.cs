using System.Collections.Immutable;
using Mono.Cecil;
using Winmd.ClassExtensions;

namespace Winmd.Model.Visitors;

internal class ApisVisitor : IVisitor<TypeDefinition, IImmutableList<BaseObjectModel>>
{
    public static readonly ApisVisitor Instance = new();

    private ApisVisitor() {}

    public IImmutableList<BaseObjectModel> Visit(TypeDefinition type)
    {
        var fieldModels =
            from f in type.Fields
            where f is { IsPublic: true, IsStatic: true, IsSpecialName: false }
            select f.Accept<BaseObjectModel>(ConstantVisitor.Instance);

        var functionModels =
            from m in type.Methods
            where m is { IsPublic: true, IsStatic: true, IsPInvokeImpl: true, IsSpecialName: false }
            let model = m.Accept(FunctionVisitor.Instance)
            where model is not null
            select model;

        var models = fieldModels.Concat(functionModels);

        return ImmutableList.CreateRange(models);
    }
}
