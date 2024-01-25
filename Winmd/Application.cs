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

var allTypes = assembly.Modules
    .SelectMany(m => m.GetTypes())
    .Where(t => t.IsPublic) // Also automatically excludes nested types
    .Where(t => t.Namespace != "Windows.Win32.Foundation.Metadata") // Metadata is not directly needed to generate output
    .GroupBy(t => t.Namespace!);

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,
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
    var types = groupedTypes
        .SelectMany(t => t.Accept(ModelGenerator.Instance))
        .ToImmutableList();

    if (types.IsEmpty)
    {
        continue;
    }

    var filePath = Path.Combine(generatedPath, $"{groupedTypes.Key}.json");
    using var output = new FileStream(filePath, FileMode.Create, FileAccess.Write);
    JsonSerializer.Serialize(output, types, jsonOptions);
}
