using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mono.Cecil;
using Winmd.ClassExtensions;
using Winmd.Json;
using Winmd.Model.Visitors;

var executableDirectory = AppDomain.CurrentDomain.BaseDirectory;

var generatedPath = Path.Combine(executableDirectory, "generated");
Directory.CreateDirectory(generatedPath);
foreach (var file in Directory.EnumerateFiles(generatedPath))
{
    File.Delete(file);
}

var winmdAssemblies = Directory.GetFiles(executableDirectory, "*.winmd");

var assembly = AssemblyDefinition.ReadAssembly(winmdAssemblies[0]);

var allTypes =
    from m in assembly.Modules
    from t in m.GetTypes()
    where t.IsPublic || t.IsNestedPublic
    where t.GetNamespace() != "Windows.Win32.Foundation.Metadata" // Metadata is not directly needed to generate output
    group t by t.GetNamespace()!;

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented =
#if DEBUG
        true,
#else
        false,
#endif
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Converters =
    {
        new ObjectModelConverter(),
        new JsonStringEnumConverter(),
    }
};
jsonOptions.MakeReadOnly();

foreach (var groupedTypes in allTypes)
{
    var types = ImmutableList.CreateRange(
        from type in groupedTypes
        from model in type.Accept(ModelGenerator.Instance)
        select model
    );

    if (types.IsEmpty)
    {
        continue;
    }

    var filePath = Path.Combine(generatedPath, $"{groupedTypes.Key}.json");
    using var output = new FileStream(filePath, FileMode.Create, FileAccess.Write);
    JsonSerializer.Serialize(output, types, jsonOptions);
}

Console.WriteLine("Done");
