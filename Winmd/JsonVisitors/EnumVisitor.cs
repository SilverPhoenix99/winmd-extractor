namespace Winmd.JsonVisitors;

using System.Text.Json.Nodes;
using Mono.Cecil;
using Mono.Cecil.Rocks;

class EnumVisitor : IVisitor<TypeDefinition, JsonObject>
{
    public JsonObject Visit(TypeDefinition type)
    {
        var json = new JsonObject();

        var baseType = type.GetEnumUnderlyingType().FullName;
        if (baseType != typeof(int).FullName)
        {
            json["Type"] = baseType;
        }

        // TODO: enum members

        return json;
    }
}
