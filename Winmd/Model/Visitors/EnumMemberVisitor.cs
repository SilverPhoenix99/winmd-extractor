namespace Winmd.Model.Visitors;

using Mono.Cecil;

class EnumMemberVisitor : IVisitor<FieldDefinition, EnumMemberModel>
{
    public static readonly EnumMemberVisitor Instance = new();

    private EnumMemberVisitor() {}

    public EnumMemberModel Visit(FieldDefinition value) => new(value.Name, value.Constant);
}
