namespace Winmd.ClassExtensions;

using Mono.Cecil;

static class ReflectionExtensions
{
    public static (string Name, string? Namespace) GetQualifiedName(this Type type)
    {
        var name = type.Name;
        if (type.BaseType == typeof(Attribute))
        {
            name = name.StripEnd("Attribute");
        }

        return (
            name,
            type.Namespace != "System" ? type.Namespace : null
        );
    }

    public static TO Accept<TO>(this CustomAttribute element, IVisitor<CustomAttribute, TO> visitor) =>
        visitor.Visit(element);

    public static TO Accept<TO>(this CustomAttributeArgument element, IVisitor<CustomAttributeArgument, TO> visitor) =>
        visitor.Visit(element);

    public static TO Accept<TO>(
        this CustomAttributeNamedArgument element,
        IVisitor<CustomAttributeNamedArgument, TO> visitor
    ) =>
        visitor.Visit(element);

    public static TO Accept<TO>(this Enum element, IVisitor<Enum, TO> visitor) =>
        visitor.Visit(element);

    public static TO Accept<TO>(this InterfaceImplementation element, IVisitor<InterfaceImplementation, TO> visitor) =>
        visitor.Visit(element);

    public static TO Accept<TO>(this FieldDefinition element, IVisitor<FieldDefinition, TO> visitor) =>
        visitor.Visit(element);

    public static TO Accept<TO>(this ParameterDefinition element, IVisitor<ParameterDefinition, TO> visitor) =>
        visitor.Visit(element);

    public static TO Accept<TO>(this MethodDefinition element, IVisitor<MethodDefinition, TO> visitor) =>
        visitor.Visit(element);

    public static TO Accept<TO>(this MethodReturnType element, IVisitor<MethodReturnType, TO> visitor) =>
        visitor.Visit(element);

    public static TO Accept<TO>(this PInvokeInfo element, IVisitor<PInvokeInfo, TO> visitor) => visitor.Visit(element);
}
