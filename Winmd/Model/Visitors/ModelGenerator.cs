namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;
using static ModelType;

class ModelGenerator : IVisitor<TypeDefinition, IImmutableList<BaseObjectModel>>
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
            _ => List(type.Accept(new ObjectVisitor(modelType)))
        };
    }

    private static ModelType GetModelType(TypeDefinition type) =>
        type.IsInterface ? Interface
        : type.IsEnum ? Enum
        : type.IsDelegate() ? Callback
        : type.IsValueType ? GetStructType(type)
        : GetClassType(type);

    private static ModelType GetStructType(TypeDefinition type) =>
        type.IsTypedef() ? Typedef
        : type.IsExplicitLayout ? Union
        : Struct;

    private static ModelType GetClassType(IMemberDefinition type) => type.Name == "Apis" ? Apis : Object;
}
