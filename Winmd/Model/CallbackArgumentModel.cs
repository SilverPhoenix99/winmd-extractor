namespace Winmd.Model;

using System.Collections.Immutable;

class CallbackArgumentModel(string name, TypeModel type)
{
    public string Name => name;
    public IImmutableSet<string> Attributes { get; set; } = ImmutableSortedSet<string>.Empty;
    public TypeModel Type => type;
}
