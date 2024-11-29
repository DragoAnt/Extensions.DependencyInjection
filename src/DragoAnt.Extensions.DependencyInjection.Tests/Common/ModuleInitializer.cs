using System.Runtime.CompilerServices;
using VerifyTests.DiffPlex;

namespace DragoAnt.Extensions.DependencyInjection.Tests;

public static class ModuleInitializer
{

    [ModuleInitializer]
    public static void Initialize() =>
        VerifyDiffPlex.Initialize(OutputType.Compact);


    [ModuleInitializer]
    public static void OtherInitialize()
    {
        VerifierSettings.InitializePlugins();
        VerifierSettings.ScrubLinesContaining("DiffEngineTray");
        VerifierSettings.IgnoreStackTrace();
    }
}