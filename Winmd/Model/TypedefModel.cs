namespace Winmd.Model;

using System.Collections.Immutable;

class TypedefModel(string name, IImmutableList<AttributeModel>? attributes, TypeModel sourceType)
    : BaseObjectModel(name, attributes)
{
    public override ModelKind Kind => ModelKind.Typedef;
    public TypeModel SourceType => sourceType;
}
