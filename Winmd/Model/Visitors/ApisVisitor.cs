namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class ApisVisitor : IVisitor<TypeDefinition, IImmutableList<BaseObjectModel>>
{
    public static readonly ApisVisitor Instance = new();

    private ApisVisitor() {}

    public IImmutableList<BaseObjectModel> Visit(TypeDefinition type)
    {
        var fieldModels =
            from f in type.Fields
            where f.IsPublic && f.IsStatic && !f.IsSpecialName
            select f.Accept<BaseObjectModel>(ConstantVisitor.Instance);

        var functionModels =
            from m in type.Methods
            where m.IsPublic && m.IsStatic && m.IsPInvokeImpl && !m.IsSpecialName
            select m.Accept(FunctionVisitor.Instance);

        var models = fieldModels.Concat(functionModels);

        return ImmutableList.CreateRange(models);
    }
}
