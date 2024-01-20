namespace Winmd.JsonVisitors;

using System.Text.Json.Nodes;

class EnumVisitor : IVisitor<Type, JsonObject>
{
    public JsonObject Visit(Type type)
    {
        var json = new JsonObject();

        if (type.GetEnumUnderlyingType() != typeof(int))
        {
            json["Type"] = type.GetEnumUnderlyingType().FullName;
        }

        // TODO:

        return json;
    }
}
