namespace Winmd.JsonVisitors;

using System.Runtime.InteropServices;
using System.Text.Json.Nodes;

class StructLayoutAttributeVisitor : IVisitor<StructLayoutAttribute, JsonObject?>
{
    public JsonObject? Visit(StructLayoutAttribute attribute)
    {
        var args = new List<JsonNode>();

        if (attribute.Value != 0)
        {
            args.Add(CustomAttributeArgumentVisitor.Create(
                null,
                false,
                typeof(LayoutKind),
                attribute.Value
            ));
        }

        if (attribute.Pack != IntPtr.Size)
        {
            args.Add(CustomAttributeArgumentVisitor.Create(
                nameof(StructLayoutAttribute.Pack),
                true,
                attribute.Pack.GetType(),
                attribute.Pack
            ));
        }

        if (attribute.Size != 0)
        {
            args.Add(CustomAttributeArgumentVisitor.Create(
                nameof(StructLayoutAttribute.Size),
                true,
                attribute.Size.GetType(),
                attribute.Size
            ));
        }

        if (attribute.CharSet != CharSet.Ansi)
        {
            args.Add(CustomAttributeArgumentVisitor.Create(
                nameof(StructLayoutAttribute.CharSet),
                true,
                attribute.CharSet.GetType(),
                attribute.CharSet
            ));
        }

        return args.Count == 0
            ? null
            : new JsonObject
            {
                ["Name"] = typeof(StructLayoutAttribute).FullName,
                ["Arguments"] = JsonGenerator.CreateArray(args)
            };
    }
}