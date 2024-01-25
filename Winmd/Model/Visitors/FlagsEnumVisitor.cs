namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using ClassExtensions;

class FlagsEnumVisitor : IVisitor<object, IImmutableSet<Enum>>
{
    public static readonly FlagsEnumVisitor Instance = new();

    private static readonly Dictionary<Type, ImmutableHashSet<Enum>> Flags = new();

    private FlagsEnumVisitor() {}

    public IImmutableSet<Enum> Visit(object value)
    {
        var type = value.GetType();
        if (type.GetCustomAttribute(typeof(FlagsAttribute)) is null)
        {
            throw new NotSupportedException($"Type {type} isn't a Flags enum.");
        }

        var attributeSet = Flags.ComputeIfMissing(type, ComputeFlags);

        var enumValue = (Enum) value;

        return ImmutableSortedSet.CreateRange(
            from a in attributeSet
            where enumValue.HasFlag(a)
            select a
        );
    }

    private static ImmutableHashSet<Enum> ComputeFlags(Type type)
    {
        var underlyingType = Enum.GetUnderlyingType(type);

        return ImmutableHashSet.CreateRange(
            from e in type.GetEnumValues().Cast<Enum>()
            let value = Convert.ChangeType(e, underlyingType)
            where BitOperations.IsPow2((dynamic) value)
            select e
        );
    }
}
