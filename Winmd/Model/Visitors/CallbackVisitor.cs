namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class CallbackVisitor : BaseObjectVisitor<CallbackModel>
{
    public static readonly CallbackVisitor Instance = new();

    private CallbackVisitor() {}

    public override CallbackModel Visit(TypeDefinition type)
    {
        var method = type.Methods.First(m => !m.IsConstructor && m.Name == "Invoke")!;
        var @return = method.MethodReturnType.Accept(MethodReturnVisitor.Instance);

        return new CallbackModel(type.Name, GetAttributes(type), @return)
        {
            Arguments = ImmutableList.CreateRange(
                from p in method.Parameters
                select p.Accept(FunctionArgumentVisitor.Instance)
            )
        };
    }
}
