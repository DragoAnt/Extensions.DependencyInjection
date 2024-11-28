namespace DragoAnt.Extensions.DependencyInjection;

// ReSharper disable InconsistentNaming
internal static partial class BuildProperties
{
    /// <summary>
    /// Suffix for Add_Suffix_Dependencies name
    /// </summary>
    public const string MethodSuffix = "DragoAnt_MethodSuffix";

    /// <summary>
    /// For suffix convention. Count of removing name parts at RootNamespace. Usually used for cut the first part - company name.  
    /// </summary>
    public const string MethodSuffix_SkippedNamePartsCount = "DragoAnt_MethodSuffix_SkippedNamePartsCount";

    /// <summary>
    /// Always generate AddDependencies method
    /// </summary>
    public const string AlwaysGenerateMethod = "DragoAnt_AlwaysGenerateMethod";

    /// <summary>
    /// Always generate AddDependencies method
    /// </summary>
    public const string CustomDependencies = "DragoAnt_CustomDependencies";
}