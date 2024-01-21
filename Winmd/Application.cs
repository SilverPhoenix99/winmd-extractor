using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mono.Cecil;
using Winmd;
using Winmd.ClassExtensions;

var executableDirectory = AppDomain.CurrentDomain.BaseDirectory;

var generatedPath = Path.Combine(executableDirectory, "generated");
Directory.CreateDirectory(generatedPath);

var winmdAssemblies = Directory.GetFiles(executableDirectory, "*.winmd");

var assembly = AssemblyDefinition.ReadAssembly(winmdAssemblies[0]);

var allTypes = assembly.Modules
    .SelectMany(m => m.Types)
    .Where(t => t.IsPublic && !t.IsNested) // Nested types are dealt recursively, so they need to be excluded here
    .GroupBy(t => t.Namespace!);

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    TypeInfoResolver = new DefaultJsonTypeInfoResolver()
};
options.MakeReadOnly();

foreach (var groupedTypes in allTypes)
{
    var types =
        from t in groupedTypes
        orderby t.Name
        select new KeyValuePair<string, JsonNode>(
            t.Name,
            t.Accept(JsonGenerator.Instance)
        );

    var rootJson = new JsonObject(types.DistinctBy(t => t.Key));

    var filePath = Path.Combine(generatedPath, $"{groupedTypes.Key}.json");

    File.WriteAllText(filePath, rootJson.ToJsonString(options));
}
