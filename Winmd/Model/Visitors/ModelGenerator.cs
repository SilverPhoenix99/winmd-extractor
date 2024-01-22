namespace Winmd.Model.Visitors;

using ClassExtensions;
using Mono.Cecil;

class ModelGenerator : IVisitor<TypeDefinition, BaseObjectModel>
{
    public static readonly ModelGenerator Instance = new();

    private ModelGenerator() {}

    public BaseObjectModel Visit(TypeDefinition value)
    {
        var modelType = GetModelType(value);
        return modelType switch
        {
            ModelType.Enum => value.Accept(EnumVisitor.Instance),
            ModelType.Callback => value.Accept(CallbackVisitor.Instance),
            ModelType.Typedef => value.Accept(TypedefVisitor.Instance),
            _ => value.Accept(new ObjectVisitor(modelType))
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
