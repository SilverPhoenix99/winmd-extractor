namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using System.Diagnostics;
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

            var guidAnnotation = annotations.Value.FirstOrDefault(a => a.Name == TypeModel.GuidType.Name);

            if (guidAnnotation is null)
            {
                return null;
            }

            var filteredAnnotations = ImmutableList.CreateRange(
                from a in annotations.Value
                where a != guidAnnotation
                select a
            );

            return new ConstantModel(
                field.Name,
                TypeModel.GuidType,
                filteredAnnotations.IsEmpty ? null : filteredAnnotations,
                guidAnnotation.Properties["Value"],
                TypeModel.StringType
            );
        }

        private ConstantModel? VisitConstant()
        {
            if (field.HasConstant)
            {
                // If it has a constant, it'll be dealt with in VisitDefault
                return null;
            }

            var constAnnotation = annotations.Value
                .FirstOrDefault(a =>
                    a is { Name: "Constant", Namespace: "Windows.Win32.Foundation.Metadata" }
                );

            if (constAnnotation is null)
            {
                return null;
            }

            var filteredAnnotations = ImmutableList.CreateRange(
                from a in annotations.Value
                where a != constAnnotation
                select a
            );

            return new ConstantModel(
                field.Name,
                FieldType,
                filteredAnnotations.IsEmpty ? null : filteredAnnotations,
                constAnnotation.Properties["Value"],
                TypeModel.StringType
            );
        }

        private ConstantModel VisitDefault()
        {
            if (!field.HasConstant)
            {
                throw new UnreachableException($"Constants should always have a value. Name = {field.FullName}");
            }

            var value = field.Constant!;
            var valueType = value.GetType().GetQualifiedName();

            return new ConstantModel(
                field.Name,
                FieldType,
                annotations.Value.IsEmpty ? null : annotations.Value,
                value,
                new TypeModel(valueType.Name, valueType.Namespace)
            );
        }

        private readonly Lazy<ImmutableList<AnnotationModel>> annotations = new(() =>
            ImmutableList.CreateRange(
                from a in field.CustomAttributes
                select a.Accept(AnnotationVisitor.Instance)
            )
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
