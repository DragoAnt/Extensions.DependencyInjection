namespace DragoAnt.Extensions.DependencyInjection.Tests;

using System;
using Xunit;

public class GetDefaultMethodSuffixTests
{
    [Theory]
    [InlineData("My.Company.Project", 0, "MyCompanyProject")]
    [InlineData("My.Company.Project", 1, "CompanyProject")]
    [InlineData("My.Company.Project", 2, "Project")]
    [InlineData("My.Company.Project", 3, "Project")]
    [InlineData("My.Company.Project", 4, "Project")] // Count exceeds the number of parts
    [InlineData("SinglePart", 1, "SinglePart")] // Single-part namespace
    [InlineData("SinglePart", 0, "SinglePart")] // Zero count, no parts skipped
    public void GetDefaultMethodSuffix_ValidInput_ReturnsExpected(string rootNamespace, int countOfParts, string expected)
    {
        // Act
        var result = DependencyGenerator.GetDefaultMethodSuffix(rootNamespace, countOfParts);

        // Assert
        Assert.Equal(expected, result);
    }
}
