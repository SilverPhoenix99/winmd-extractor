namespace Winmd.ClassExtensions;

using Mono.Cecil;

static class ReflectionExtensions
{
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

    public static TO Accept<TO>(this FieldDefinition element, IVisitor<FieldDefinition, TO> visitor) =>
        visitor.Visit(element);
}
