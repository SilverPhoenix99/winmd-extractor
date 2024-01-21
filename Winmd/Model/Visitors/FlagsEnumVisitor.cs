namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using ClassExtensions;

class FlagsEnumVisitor : IVisitor<object, IImmutableSet<string>>
{
    public static readonly FlagsEnumVisitor Instance = new();

    private static readonly Dictionary<Type, ImmutableHashSet<Enum>> Flags = new();

    private FlagsEnumVisitor() {}

    public IImmutableSet<string> Visit(object value)
    {
        var type = value.GetType();
        if (type.GetCustomAttribute(typeof(FlagsAttribute)) is null)
        {
            throw new NotSupportedException($"Type {type} isn't a Flags enum.");
        }

        var attributeSet = Flags.ComputeIfMissing(type, ComputeFlags);

        var enumValue = (Enum) value;
        var attributes =
            from a in attributeSet
            where enumValue.HasFlag(a)
            select a.ToString();

        return attributes.ToImmutableSortedSet();
    }

    private static ImmutableHashSet<Enum> ComputeFlags(Type type)
    {
        var underlyingType = Enum.GetUnderlyingType(type);

        return type.GetEnumValues()
            .Cast<Enum>()
            .Where(e =>
            {
                dynamic value = Convert.ChangeType(e, underlyingType);
                return BitOperations.IsPow2(value);
            })
            .ToImmutableHashSet();
    }

}
