namespace Winmd.JsonVisitors;

using System.Text.Json.Nodes;
using Mono.Cecil;

class TypeDefinitionVisitor : IVisitor<TypeDefinition, JsonObject>
{
    public static readonly TypeDefinitionVisitor Instance = new();

    public JsonObject Visit(TypeDefinition type)
    {
        var json = new JsonObject
        {
            ["Name"] = type.FullName,
        };

        // TODO: pointers
        // var pointer = 0;
        // while (type.IsPointer)
        // {
        //     pointer++;
        //     type = type.
        // }

        return json;
    }
}
