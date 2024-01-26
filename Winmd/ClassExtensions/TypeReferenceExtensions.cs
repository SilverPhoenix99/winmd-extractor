namespace Winmd.ClassExtensions;

using System.Collections.Immutable;
using Mono.Cecil;

static class TypeReferenceExtensions
{
    public static TO Accept<TO>(this TypeReference element, IVisitor<TypeReference, TO> visitor) =>
        visitor.Visit(element);

    public static (string Name, string? Namespace) GetQualifiedName(this TypeReference? type, bool stripAttributes = true)
    {
        if (type is null)
        {
            return ("", null);
        }

        var name = type.Name;
        if (stripAttributes)
        {
            name = name.StripEnd("Attribute");
        }

        return (name, type.GetNamespace());
    }

    public static string? GetNamespace(this TypeReference type)
    {
        var @namespace = type.IsNested ? type.GetNesting()![0].Namespace : type.Namespace;
        return @namespace != "System" ? @namespace : null;
    }

    public static IImmutableList<TypeReference>? GetNesting(this TypeReference type)
    {
        var nesting = GetNestingEnumerable(type).Reverse().ToImmutableList();
        return nesting.IsEmpty ? null : nesting;
    }

    private static IEnumerable<TypeReference> GetNestingEnumerable(TypeReference type)
    {
        for (; type.IsNested ; type = type.DeclaringType)
        {
            yield return type.DeclaringType;
        }
    }
}
