using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Winmd;

var executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
var generatedPath = Path.Combine(executableDirectory, "generated");

Directory.CreateDirectory(generatedPath);

var winmdAssemblies = Directory.GetFiles(executableDirectory, "*.winmd");
var runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
var assemblies = runtimeAssemblies.Concat(winmdAssemblies).ToArray();
var pathAssemblyResolver = new PathAssemblyResolver(assemblies);
var metadataLoadContext = new MetadataLoadContext(pathAssemblyResolver);

var assembly = metadataLoadContext.LoadFromAssemblyPath(winmdAssemblies[0]);

var allTypes =
(
    from t in assembly.GetTypes()
    where !t.IsNested
    select t
).GroupBy(t => t.Namespace!);

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    TypeInfoResolver = new DefaultJsonTypeInfoResolver()
};
options.MakeReadOnly();

var jsonGenerator = new JsonGenerator();

foreach (var g in allTypes)
{
    var groupTypes =
        from t in g
        orderby t.Name
        select new KeyValuePair<string, JsonNode>(
            t.Name,
            t.Accept(jsonGenerator)
        );

    var j = new JsonObject(groupTypes.DistinctBy(t => t.Key));

    var filePath = Path.Combine(generatedPath, $"{g.Key}.json");

    File.WriteAllText(filePath, j.ToJsonString(options));
}