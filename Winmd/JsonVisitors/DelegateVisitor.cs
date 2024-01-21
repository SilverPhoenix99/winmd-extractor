namespace Winmd.JsonVisitors;

using System.Text.Json.Nodes;
using ClassExtensions;
using Mono.Cecil;

class DelegateVisitor : IVisitor<TypeDefinition, JsonObject>
{
    public static readonly DelegateVisitor Instance = new();

    public JsonObject Visit(TypeDefinition type)
    {
        var method = type.Methods.First(m => !m.IsConstructor && m.Name == "Invoke")!;

        var json = new JsonObject();

        if (method.CallingConvention != MethodCallingConvention.Default)
        {
            json["CallingConvention"] = method.CallingConvention.ToString();
        }

        json["ReturnType"] = method.ReturnType.Resolve().Accept(TypeDefinitionVisitor.Instance);
        json["Arguments"] = JsonGenerator.CreateArray(
            from p in method.Parameters
            select p.Accept(DelegateArgumentVisitor.Instance)
        );

        return json;
    }
}
