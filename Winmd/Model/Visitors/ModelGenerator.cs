using System.Collections.Immutable;
using Mono.Cecil;
using Winmd.ClassExtensions;

namespace Winmd.Model.Visitors;

using static ModelKind;

internal class ModelGenerator : IVisitor<TypeDefinition, IImmutableList<BaseObjectModel>>
{
    public static readonly ModelGenerator Instance = new();

    private ModelGenerator() {}

    private static readonly Func<BaseObjectModel, ImmutableList<BaseObjectModel>> List = ImmutableList.Create;

    public IImmutableList<BaseObjectModel> Visit(TypeDefinition type)
    {
        var modelType = GetModelType(type);

        return modelType switch
        {
            Apis => type.Accept(ApisVisitor.Instance),
            Callback => List(type.Accept(CallbackVisitor.Instance)),
            Enum => List(type.Accept(EnumVisitor.Instance)),
            Struct => List(type.Accept(StructVisitor.Instance)),
            Typedef => List(type.Accept(TypedefVisitor.Instance)),
            Union => List(type.Accept(UnionVisitor.Instance)),
            Interface => ImmutableList<BaseObjectModel>.Empty,
            _ => List(type.Accept(new ObjectVisitor(modelType)))
        };
    }

    private static ModelKind GetModelType(TypeDefinition type) =>
        type.IsInterface ? Interface
        : type.IsEnum ? Enum
        : type.IsDelegate() ? Callback
        : type.IsValueType ? GetStructType(type)
        : GetClassType(type);

    private static ModelKind GetStructType(TypeDefinition type) =>
        type.IsTypedef() ? Typedef
        : type.IsExplicitLayout ? Union
        : Struct;

    private static ModelKind GetClassType(IMemberDefinition type) => type.Name == "Apis" ? Apis : Object;
}
