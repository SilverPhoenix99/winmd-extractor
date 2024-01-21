namespace Winmd.JsonVisitors;

using System.Collections.Immutable;
using System.Text.Json.Nodes;
using ClassExtensions;
using Mono.Cecil;

class InterfaceTypeVisitor(TypeDefinition type) : IVisitor<InterfaceImplementation, JsonObject?>
{
    private static readonly ImmutableHashSet<string> EnumInterfaces = typeof(Enum).GetInterfaces()
        .Select(x => x.FullName!)
        .ToImmutableHashSet();

    public JsonObject? Visit(InterfaceImplementation interfaceType)
    {
        if (type.IsEnum && EnumInterfaces.Contains(interfaceType.InterfaceType.FullName))
        {
            return null;
        }

        var (name, ns) = interfaceType.InterfaceType.GetQualifiedName();

        return new JsonObject
        {
            ["Name"] = name,
            ["Namespace"] = ns
        };
    }
}
