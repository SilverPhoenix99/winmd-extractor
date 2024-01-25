namespace Winmd.ClassExtensions;

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

        return (
            name,
            type.Namespace != "System" ? type.Namespace : null
        );
    }
}
