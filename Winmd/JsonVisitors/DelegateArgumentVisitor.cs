namespace Winmd.JsonVisitors;

using System.Text.Json.Nodes;
using Mono.Cecil;

class DelegateArgumentVisitor : IVisitor<ParameterDefinition, JsonObject>
{
    public static readonly DelegateArgumentVisitor Instance = new();

    public JsonObject Visit(ParameterDefinition parameter)
    {
        var json = new JsonObject
        {
            ["Name"] = parameter.Name
        };

        // TODO: json["Type"]

        return json;
    }
}
