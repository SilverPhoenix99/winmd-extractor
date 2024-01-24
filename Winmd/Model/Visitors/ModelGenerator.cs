namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

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
            ModelType.Enum => List(type.Accept(EnumVisitor.Instance)),
            ModelType.Callback => List(type.Accept(CallbackVisitor.Instance)),
            ModelType.Typedef => List(type.Accept(TypedefVisitor.Instance)),
            ModelType.Apis => type.Accept(ApisVisitor.Instance),
            ModelType.Struct => List(type.Accept(StructVisitor.Instance)),
            _ => List(type.Accept(new ObjectVisitor(modelType)))
        };
    }

    private static ModelType GetModelType(TypeDefinition type)
    {
        return type.IsInterface ? ModelType.Interface
            : type.IsEnum ? ModelType.Enum
            : type.IsDelegate() ? ModelType.Callback
            : type.IsValueType ? GetStructType(type)
            : GetClassType(type);
    }

    private static ModelType GetStructType(TypeDefinition type) =>
        type.IsTypedef() ? ModelType.Typedef : ModelType.Struct;

    private static ModelType GetClassType(IMemberDefinition type) =>
        type.Name == "Apis" ? ModelType.Apis : ModelType.Object;
}
