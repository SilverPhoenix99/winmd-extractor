namespace Winmd.Model;

using System.Collections.Immutable;

class FunctionArgumentModel(string name, TypeModel type)
{
    public string Name => name;
    public IImmutableList<AttributeModel>? Attributes { get; set; }
    public TypeModel Type => type;
}
