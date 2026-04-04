using System.Collections.Immutable;
using Mono.Cecil;
using Winmd.ClassExtensions;
using Winmd.Model;

namespace Winmd.Visitors;

using static ModelKind;

internal class ModelGenerator : IVisitor<TypeDefinition, IImmutableList<BaseObjectModel>>
{
    public static readonly ModelGenerator Instance = new();

    private ModelGenerator() {}

    public IImmutableList<BaseObjectModel> Visit(TypeDefinition type)
    {
        var modelType = GetModelType(type);

        return modelType switch
        {
            Apis      => type.Accept(ApisVisitor.Instance),
            Callback  => ToSingletonList(type.Accept(CallbackVisitor.Instance)),
            Enum      => ToSingletonList(type.Accept(EnumVisitor.Instance)),
            Struct    => ToSingletonList(type.Accept(StructVisitor.Instance)),
            Typedef   => ToSingletonList(type.Accept(TypedefVisitor.Instance)),
            Union     => ToSingletonList(type.Accept(UnionVisitor.Instance)),
            Interface => ToSingletonList(type.Accept(InterfaceVisitor.Instance)),
            _         => ToSingletonList(type.Accept(new ObjectVisitor(modelType)))
        };
    }

    private static ModelKind GetModelType(TypeDefinition type) =>
        type.IsInterface ? Interface
        : type.IsEnum ? Enum
        : type.IsDelegate ? Callback
        : type.IsValueType ? GetStructType(type)
        : GetClassType(type);

    private static ModelKind GetStructType(TypeDefinition type) =>
        type.IsTypedef ? Typedef
        : type.IsExplicitLayout ? Union
        : Struct;

    private static ModelKind GetClassType(TypeDefinition type) => type.Name == "Apis" ? Apis : Object;

    private static ImmutableList<BaseObjectModel> ToSingletonList(BaseObjectModel item) => ImmutableList.Create(item);
}
