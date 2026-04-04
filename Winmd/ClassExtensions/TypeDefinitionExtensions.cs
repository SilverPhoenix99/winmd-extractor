using Mono.Cecil;

namespace Winmd.ClassExtensions;

internal static class TypeDefinitionExtensions
{
    extension(TypeDefinition element)
    {
        public TO Accept<TO>(IVisitor<TypeDefinition, TO> visitor) => visitor.Visit(element);

        public bool IsDelegate => element.BaseType?.FullName == "System.MulticastDelegate";

        public bool IsTypedef =>
            element.CustomAttributes
                .Any(a =>
                    a.AttributeType.Name == "NativeTypedefAttribute"
                    && a.AttributeType.GetNamespace()!.StartsWith("Windows.Win32")
                );
    }
}
