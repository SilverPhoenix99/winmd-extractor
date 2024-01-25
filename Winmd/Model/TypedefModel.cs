namespace Winmd.Model;

using System.Collections.Immutable;

class TypedefModel(string name, IImmutableList<AttributeModel>? attributes, TypeModel sourceType)
    : BaseObjectModel(name, attributes)
{
    public override ModelType Type => ModelType.Typedef;
    public TypeModel SourceType => sourceType;
}
