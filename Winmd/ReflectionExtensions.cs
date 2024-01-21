namespace Winmd;

using Mono.Cecil;

static class ReflectionExtensions
{
    public static bool IsDelegate(this TypeDefinition element) => element.BaseType?.FullName == "System.MulticastDelegate";

    public static TO Accept<TO>(this TypeDefinition element, IVisitor<TypeDefinition, TO> visitor) =>
        visitor.Visit(element);

    public static TO Accept<TO>(this CustomAttribute element, IVisitor<CustomAttribute, TO> visitor) =>
        visitor.Visit(element);

    public static TO Accept<TO>(this CustomAttributeArgument element, IVisitor<CustomAttributeArgument, TO> visitor) =>
        visitor.Visit(element);

    public static O Accept<O>(
        this CustomAttributeNamedArgument element,
        IVisitor<CustomAttributeNamedArgument, O> visitor
    ) =>
        visitor.Visit(element);

    public static TO Accept<TO>(this TypeAttributes element, IVisitor<TypeAttributes, TO> visitor) =>
        visitor.Visit(element);

    public static TO Accept<TO>(this InterfaceImplementation element, IVisitor<InterfaceImplementation, TO> visitor) =>
        visitor.Visit(element);
}
