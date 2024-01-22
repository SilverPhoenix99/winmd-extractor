namespace Winmd.Model.Visitors;

using Mono.Cecil;

class TypeVisitor : IVisitor<TypeReference, TypeModel>
{
    public static readonly TypeVisitor Instance = new();

    private TypeVisitor() {}

    public TypeModel Visit(TypeReference type)
    {
        return new TypeModel(type.Name)
        {
            Namespace = type.Namespace != "System" ? type.Namespace : null,
            // TODO: Modifiers = [{ Pointer = 2 }, { Array = [] }]
            // void**[]
        };
    }
}
