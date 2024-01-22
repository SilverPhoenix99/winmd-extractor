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

var winmdAssemblies = Directory.GetFiles(executableDirectory, "*.winmd");

var assembly = AssemblyDefinition.ReadAssembly(winmdAssemblies[0]);

var allTypes = assembly.Modules
    .SelectMany(m => m.Types)
    .Where(t => t.IsPublic && !t.IsNested) // Nested types are dealt recursively, so they need to be excluded here
    .Where(t => t.BaseType?.FullName != "System.Attribute") // No need to generate attributes - their usage is enough
    .GroupBy(t => t.Namespace!);

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
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
        .OrderBy(t => t.Name)
        .SelectMany(t => t.Accept(ModelGenerator.Instance));

    var filePath = Path.Combine(generatedPath, $"{groupedTypes.Key}.json");

    var output = new FileStream(filePath, FileMode.Create, FileAccess.Write);
    JsonSerializer.Serialize(output, types, jsonOptions);
}
