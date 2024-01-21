namespace Winmd.ClassExtensions;

using Mono.Cecil;

static class TypeDefinitionExtensions
{
    public static TO Accept<TO>(this TypeDefinition element, IVisitor<TypeDefinition, TO> visitor) =>
        visitor.Visit(element);

    public static bool IsDelegate(this TypeDefinition element) =>
        element.BaseType?.FullName == "System.MulticastDelegate";

    public static bool IsTypedef(this TypeDefinition type)
    {
        return type.CustomAttributes
            .Any(a =>
                a.AttributeType.Name == "NativeTypedefAttribute"
                && a.AttributeType.Namespace.StartsWith("Windows.Win32")
            );
    }
}
