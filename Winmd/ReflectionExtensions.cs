namespace Winmd;

using System.Reflection;
using System.Runtime.InteropServices;

public static class ReflectionExtensions
{
    public static O Accept<O>(this Type type, IVisitor<Type, O> visitor) => visitor.Visit(type);

    public static O Accept<O>(this TypeAttributes attributes, IVisitor<TypeAttributes, O> visitor) =>
        visitor.Visit(attributes);

    public static O Accept<O>(this CustomAttributeData attribute, IVisitor<CustomAttributeData, O> visitor) =>
        visitor.Visit(attribute);

    public static O Accept<O>(this StructLayoutAttribute attribute, IVisitor<StructLayoutAttribute, O> visitor) =>
        visitor.Visit(attribute);

    public static O Accept<O>(
        this CustomAttributeTypedArgument arg,
        IVisitor<CustomAttributeTypedArgument, O> visitor
    ) =>
        visitor.Visit(arg);

    public static O Accept<O>(
        this CustomAttributeNamedArgument arg,
        IVisitor<CustomAttributeNamedArgument, O> visitor
    ) =>
        visitor.Visit(arg);
}
