namespace Winmd.Model.Visitors;

using Mono.Cecil;

class TypeModelVisitor : IVisitor<TypeDefinition, TypeModel>
{
    public static readonly TypeModelVisitor Instance = new();

    private TypeModelVisitor() {}

    public TypeModel Visit(TypeDefinition value)
    {
        return new TypeModel(value.Name)
        {
            Namespace = value.Namespace != "System" ? value.Namespace : null,
            // TODO: Pointer =
            // TODO: Array =
        };
    }
}
