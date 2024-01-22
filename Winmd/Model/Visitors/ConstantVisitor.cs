namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class ConstantVisitor : IVisitor<FieldDefinition, ConstantModel>
{
    public static readonly ConstantVisitor Instance = new();

    private ConstantVisitor() {}

    public ConstantModel Visit(FieldDefinition field)
    {
        var constantName = field.Constant.GetType().GetQualifiedName();

        var fieldType = field.FieldType.Accept(TypeVisitor.Instance);
        if (fieldType.Namespace == field.DeclaringType.Namespace)
        {
            // Ignore namespace when it's enclosed in the same namespace as Apis
            fieldType.Namespace = null;
        }

        var model = new ConstantModel(field.Name)
        {
            Attributes = field.CustomAttributes
                .Select(a => a.Accept(AttributeVisitor.Instance))
                .ToImmutableList(),
            ConstantType = fieldType,
            Value = field.Constant,
            ValueType = new TypeModel(constantName.Name) { Namespace = constantName.Namespace }
        };

        return model;
    }
}
