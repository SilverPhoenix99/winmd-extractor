namespace Winmd.JsonVisitors;

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using Mono.Cecil;

class InterfaceTypeVisitor : IVisitor<InterfaceImplementation, JsonValue?>
{
    private static readonly ImmutableHashSet<string> EnumInterfaces = typeof(Enum).GetInterfaces()
        .Select(x => x.FullName!)
        .ToImmutableHashSet();

    public InterfaceTypeVisitor(TypeDefinition type)
    {
        Type = type;
    }

    private TypeDefinition Type { get; }

    public JsonValue? Visit(InterfaceImplementation interfaceType)
    {
        var name = interfaceType.InterfaceType.Resolve().FullName!;
        return Type.IsEnum && EnumInterfaces.Contains(name) ? null : JsonValue.Create(name);
    }
}
