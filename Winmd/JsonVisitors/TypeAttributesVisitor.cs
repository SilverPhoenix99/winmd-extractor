namespace Winmd.JsonVisitors;

using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json.Nodes;

class TypeAttributesVisitor : IVisitor<TypeAttributes, JsonArray>
{
    private static readonly ImmutableHashSet<TypeAttributes> TypeAttributeSet = Enum.GetValues(typeof(TypeAttributes))
        .Cast<TypeAttributes>()
        .Where(a => (a & (a - 1)) == 0)
        .ToImmutableHashSet();

    public JsonArray Visit(TypeAttributes typeAttributes) => JsonGenerator.CreateArray(
        from a in TypeAttributeSet
        where (typeAttributes & a) != 0
        select JsonValue.Create(a.ToString())
    );
}
