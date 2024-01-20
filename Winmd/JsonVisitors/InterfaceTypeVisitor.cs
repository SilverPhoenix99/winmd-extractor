namespace Winmd.JsonVisitors;

using System.Collections.Immutable;
using System.Text.Json.Nodes;

class InterfaceTypeVisitor : IVisitor<Type, JsonValue?>
{
    private static readonly ImmutableHashSet<string> EnumInterfaces = typeof(Enum).GetInterfaces()
        .Select(x => x.FullName!)
        .ToImmutableHashSet();

    public InterfaceTypeVisitor(Type type)
    {
        Type = type;
    }

    private Type Type { get; }

    public JsonValue? Visit(Type interfaceType)
    {
        var name = interfaceType.FullName!;
        return Type.IsEnum && EnumInterfaces.Contains(name) ? null : JsonValue.Create(name);
    }
}