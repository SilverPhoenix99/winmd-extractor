namespace Winmd.Model;

using System.Collections.Immutable;

class StructModel(string name, IImmutableList<AttributeModel>? attributes) : BaseObjectModel(name, attributes)
{
    public override ModelKind Kind => ModelKind.Struct;
    public IImmutableList<FieldModel> Fields { get; init; } = ImmutableList<FieldModel>.Empty;
}

class UnionModel(string name, IImmutableList<AttributeModel>? attributes) : StructModel(name, attributes)
{
    public override ModelKind Kind => ModelKind.Union;
}

record FieldModel(string Name, TypeModel Type, IImmutableList<AttributeModel>? Attributes = null);
