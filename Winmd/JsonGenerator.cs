namespace Winmd;

using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;

public class JsonGenerator : IVisitor<Type, JsonObject>
{
    private static readonly ISet<TypeAttributes> TypeAttributeSet = Enum.GetValues(typeof(TypeAttributes))
        .Cast<TypeAttributes>()
        .Where(a => (a & (a - 1)) == 0)
        .ToImmutableHashSet();

    private static readonly ISet<string> EnumInterfaces = typeof(Enum).GetInterfaces()
        .Select(x => x.FullName!)
        .ToImmutableHashSet();

    // ReSharper disable once InconsistentNaming
    private static readonly TypeAttributesVisitor typeAttributesVisitor = new();
    // ReSharper disable once InconsistentNaming
    private static readonly CustomAttributeVisitor customAttributeVisitor = new();
    // ReSharper disable once InconsistentNaming
    private static readonly StructLayoutAttributeVisitor structLayoutAttributeVisitor = new();
    // ReSharper disable once InconsistentNaming
    private static readonly EnumVisitor enumVisitor = new();

    public JsonObject Visit(Type type)
    {
        var json = new JsonObject
        {
            ["BaseType"] = type.IsInterface ? "interface" : type.BaseType?.FullName,
            ["Interfaces"] = VisitInterfaces(type),
            ["Attributes"] = type.Attributes.Accept(typeAttributesVisitor),
            ["CustomAttributes"] = Visit(type.StructLayoutAttribute, type.GetCustomAttributesData()),
        };

        /*
        Types that can show up:
            * System.Enum
            * System.MulticastDelegate - callbacks
            * System.Object - class Apis, the list of available functions
            * System.ValueType - structs, unions, or typedefs (when it has a single field)
        */

        if (type.IsEnum)
        {
            json["Enum"] = type.Accept(enumVisitor);
        }
        else if (type.IsAssignableTo(typeof(Delegate)))
        {
            // TODO json["Delegate"] = type.Accept(delegateVisitor);
        }
        else if (type.IsValueType)
        {
            // TODO json["Struct"] = type.Accept(structVisitor);
        }
        else
        {
            // TODO json["Class"] = type.Accept(classVisitor);
        }

        return json;
    }

    private static JsonArray CreateArray<T>(IEnumerable<T> source) where T : JsonNode?
    {
        return new JsonArray(
            (
                from s in source
                where s is not null
                select (JsonNode) s
            ).ToArray()
        );
    }

    private static JsonArray VisitInterfaces(Type type)
    {
        var interfaceTypeVisitor = new InterfaceTypeVisitor(type);

        return CreateArray(
            from i in type.GetInterfaces()
            let v = i.Accept(interfaceTypeVisitor)
            where v is not null
            select v!
        );
    }

    private static JsonArray Visit(
        StructLayoutAttribute? structLayoutAttribute,
        IEnumerable<CustomAttributeData> attributes
    ) {
        var customAttributes = from a in attributes
            where a.GetType().FullName != "System.Reflection.TypeLoading.Ecma.EcmaCustomAttributeData"
            select a.Accept(customAttributeVisitor);

        var structLayout = structLayoutAttribute?.Accept(structLayoutAttributeVisitor);

        if (structLayout is not null)
        {
            customAttributes = customAttributes.Prepend(structLayout);
        }

        return CreateArray(customAttributes);
    }

    private class InterfaceTypeVisitor : IVisitor<Type, JsonValue?>
    {
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

    private class TypeAttributesVisitor : IVisitor<TypeAttributes, JsonArray>
    {
        public JsonArray Visit(TypeAttributes typeAttributes) => CreateArray(
            from a in TypeAttributeSet
            where (typeAttributes & a) != 0
            select JsonValue.Create(a.ToString())
        );
    }

    private class CustomAttributeVisitor : IVisitor<CustomAttributeData, JsonObject>
    {
        private static readonly CustomAttributeArgumentVisitor ArgumentVisitor = new();

        public JsonObject Visit(CustomAttributeData attribute)
        {
            var args =
                from a in attribute.ConstructorArguments
                select a.Accept<JsonObject>(ArgumentVisitor);

            var namedArgs =
                from a in attribute.NamedArguments
                select a.Accept<JsonObject>(ArgumentVisitor);

            return new JsonObject
            {
                ["Name"] = JsonValue.Create(attribute.AttributeType.FullName),
                ["Arguments"] = CreateArray(args.Concat(namedArgs))
            };
        }
    }

    private class StructLayoutAttributeVisitor : IVisitor<StructLayoutAttribute, JsonObject?>
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
                    ["Arguments"] = CreateArray(args)
                };
        }
    }

    private class CustomAttributeArgumentVisitor :
        IVisitor<CustomAttributeTypedArgument, JsonObject>,
        IVisitor<CustomAttributeNamedArgument, JsonObject>
    {
        public JsonObject Visit(CustomAttributeTypedArgument arg) => Create(null, false, arg);

        public JsonObject Visit(CustomAttributeNamedArgument arg) =>
            Create(arg.MemberName, arg.IsField, arg.TypedValue);

        private static JsonObject Create(string? name, bool isField, CustomAttributeTypedArgument arg) =>
            Create(name, isField, arg.ArgumentType, arg.Value);

        public static JsonObject Create(string? name, bool isField, Type argumentType, object? value)
        {
            if (argumentType.IsEnum && value is not null)
            {
                value = argumentType.GetEnumName(value) ?? value;
            }

            return new JsonObject
            {
                ["Name"] = name,
                ["IsField"] = isField,
                ["Type"] = argumentType.FullName,
                ["Value"] = JsonValue.Create(value),
            };
        }
    }

    private class EnumVisitor : IVisitor<Type, JsonObject>
    {
        public JsonObject Visit(Type type)
        {
            var json = new JsonObject();

            if (type.GetEnumUnderlyingType() != typeof(int))
            {
                json["Type"] = type.GetEnumUnderlyingType().FullName;
            }

            return json;
        }
    }
}
