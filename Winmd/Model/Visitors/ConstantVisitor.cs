namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class ConstantVisitor : IVisitor<FieldDefinition, ConstantModel>
{
    public static readonly ConstantVisitor Instance = new();

    private ConstantVisitor() {}

    public ConstantModel Visit(FieldDefinition field) => new Context(field).Visit();

    private class Context(FieldDefinition field)
    {
        public ConstantModel Visit() => VisitGuid() ?? VisitConstant() ?? VisitDefault();

        private ConstantModel? VisitGuid()
        {
            if (field.FieldType.Name != TypeModel.GuidType.Name || field.HasConstant)
            {
                // If it has a constant, it'll be dealt with in VisitDefault
                return null;
            }

            var guidAttribute = attributes.Value.FirstOrDefault(a => a.Name == TypeModel.GuidType.Name);
            if (guidAttribute is null)
            {
                return null;
            }

            var attrs = this.attributes.Value
                .Where(a => a != guidAttribute)
                .ToImmutableList();

            return new ConstantModel(field.Name, TypeModel.GuidType)
            {
                Attributes = attrs.IsEmpty ? null : attrs,
                Value = guidAttribute.Arguments[0].Value!,
                ValueType = TypeModel.StringType
            };
        }

        private ConstantModel? VisitConstant()
        {
            if (field.HasConstant)
            {
                // If it has a constant, it'll be dealt with in VisitDefault
                return null;
            }

            var constAttribute = attributes.Value
                .FirstOrDefault(a =>
                    a is { Name: "Constant", Namespace: "Windows.Win32.Foundation.Metadata" }
                );

            if (constAttribute is null)
            {
                return null;
            }

            var attrs = attributes.Value
                .Where(a => a != constAttribute)
                .ToImmutableList();

            return new ConstantModel(field.Name, FieldType)
            {
                Attributes = attrs.IsEmpty ? null : attrs,
                Value = constAttribute.Arguments[0].Value!,
                ValueType = TypeModel.StringType
            };
        }

        private ConstantModel VisitDefault()
        {
            var model = new ConstantModel(field.Name, FieldType)
            {
                Attributes = attributes.Value.IsEmpty ? null : attributes.Value
            };

            if (!field.HasConstant)
            {
                return model;
            }

            var value = model.Value = field.Constant!;
            var constantName = value.GetType().GetQualifiedName();
            model.ValueType = new TypeModel(constantName.Name, constantName.Namespace);

            return model;
        }

        private readonly Lazy<ImmutableList<AttributeModel>> attributes = new(() =>
            field.CustomAttributes
                .Select(a => a.Accept(AttributeVisitor.Instance))
                .ToImmutableList()
        );

        private TypeModel FieldType
        {
            get
            {
                var fieldType = field.FieldType.Accept(TypeVisitor.Instance);
                if (fieldType.Namespace == field.DeclaringType.Namespace)
                {
                    // Ignore namespace when it's enclosed in the same namespace as Apis
                    return new TypeModel(fieldType.Name)
                    {
                        Modifiers = fieldType.Modifiers
                    };
                }

                return fieldType;
            }
        }
    }
}
