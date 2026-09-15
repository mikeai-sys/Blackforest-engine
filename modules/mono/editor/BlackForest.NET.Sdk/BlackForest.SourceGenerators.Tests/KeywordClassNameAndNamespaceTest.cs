using System.Threading.Tasks;
using Xunit;

namespace BlackForest.SourceGenerators.Tests;

public class KeywordClassAndNamespaceTest
{
    [Fact]
    public async Task GenerateScriptMethodsTest()
    {
        await CSharpSourceGeneratorVerifier<ScriptMethodsGenerator>.Verify(
            "KeywordClassNameAndNamespace.cs",
            "namespace.class_ScriptMethods.generated.cs"
        );
    }
}
