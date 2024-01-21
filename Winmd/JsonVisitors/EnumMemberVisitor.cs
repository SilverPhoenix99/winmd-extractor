namespace Winmd.JsonVisitors;

using System.Text.Json.Nodes;
using Mono.Cecil;

class EnumMemberVisitor : IVisitor<FieldDefinition, JsonObject>
{
    public JsonObject Visit(FieldDefinition field)
    {
        var element = new JsonObject { ["Name"] = field.Name };

        if (field.HasConstant)
        {
            element["Constant"] = JsonValue.Create(field.Constant);
        }

        return element;
    }
}
