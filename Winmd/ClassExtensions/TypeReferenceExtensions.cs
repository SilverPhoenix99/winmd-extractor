namespace Winmd.ClassExtensions;

using Mono.Cecil;

static class TypeReferenceExtensions
{
    public static TO Accept<TO>(this TypeReference element, IVisitor<TypeReference, TO> visitor) =>
        visitor.Visit(element);

    public static (string Name, string? Namespace) GetQualifiedName(this TypeReference? type)
    {
        if (type is null)
        {
            return ("", null);
        }

        return (
            type.Name,
            type.Namespace != "System" ? type.Namespace : null
        );
    }
}
