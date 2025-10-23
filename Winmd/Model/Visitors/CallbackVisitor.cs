using System.Collections.Immutable;
using Mono.Cecil;
using Winmd.ClassExtensions;

namespace Winmd.Model.Visitors;

internal class CallbackVisitor : BaseObjectVisitor<CallbackModel?>
{
    public static readonly CallbackVisitor Instance = new();

    private CallbackVisitor() {}

    public override CallbackModel? Visit(TypeDefinition type)
    {
        var method = type.Methods.First(m => !m.IsConstructor && m.Name == "Invoke")!;

        var isCom = method.Parameters
            .Select(p => p.ParameterType)
            .Concat([method.MethodReturnType.ReturnType])
            .Any(TypeVisitor.IsCom);
        if (isCom)
        {
            // TODO: Callbacks with COM parameters
            return null;
        }

        var @return = method.MethodReturnType.Accept(MethodReturnVisitor.Instance);

        return new CallbackModel(type.Name, GetAnnotations(type), @return)
        {
            Arguments = ImmutableList.CreateRange(
                from p in method.Parameters
                select p.Accept(FunctionArgumentVisitor.Instance)
            )
        };
    }
}
