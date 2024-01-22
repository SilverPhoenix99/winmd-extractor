namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class ModelGenerator : IVisitor<TypeDefinition, IImmutableList<BaseObjectModel>>
{
    public static readonly ModelGenerator Instance = new();

    private ModelGenerator() {}

    public IImmutableList<BaseObjectModel> Visit(TypeDefinition value)
    {
        Func<BaseObjectModel, ImmutableList<BaseObjectModel>> list = ImmutableList.Create;

        var modelType = GetModelType(value);

        return modelType switch
        {
            ModelType.Enum => list(value.Accept(EnumVisitor.Instance)),
            ModelType.Callback => list(value.Accept(CallbackVisitor.Instance)),
            ModelType.Typedef => list(value.Accept(TypedefVisitor.Instance)),
            _ => list(value.Accept(new ObjectVisitor(modelType)))
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
