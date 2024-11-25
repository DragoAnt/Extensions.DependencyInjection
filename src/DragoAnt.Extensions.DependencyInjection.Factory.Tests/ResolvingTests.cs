using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit.Abstractions;

namespace DragoAnt.Extensions.DependencyInjection.Factory.Tests;

public sealed class ResolvingTests(ITestOutputHelper output)
{
    [Fact]
    public void CheckInheritCase()
    {
        var code =
            //language=csharp
            """
                    [SuperAttr]
                    public abstract class SuperClass
                    {
                    }        
            
                    [BaseAttr]
                    public abstract class BaseClass : SuperClass
                    {
                    }
            
                    [ResultAttr]
                    public sealed class DerivedClass : BaseClass
                    {
                    }
            """;

        // Parse the code
        var tree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("Example")
            .AddSyntaxTrees(tree)
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

        var semanticModel = compilation.GetSemanticModel(tree);

        // Find the DerivedClass declaration
        var root = tree.GetRoot();
        var classDeclaration = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First(cls => cls.Identifier.Text == "DerivedClass");

        // Get attributes from base class
        var classAttributes = classDeclaration.GetClassAttributes(semanticModel).ToArray();
        var attrNames = classAttributes.Select(a => a.AttributeClass?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)).ToArray();

        output.WriteLine("Attributes: " + string.Join(", ", attrNames));

        attrNames.Should().BeEquivalentTo(["ResultAttr", "BaseAttr", "SuperAttr"]);
    }

    [Fact]
    public void GetFactoryCase()
    {
        var code =
            //language=csharp
            """
                    public interface IModel
                    {
                    }
                    
                    public class Model : IModel
                    {
                    }
            
                    [ResolveFactory(SharedFactoryInterfaceTypeDefinition = typeof(IFactory<>), OnlySharedFactory = true)]
                    public abstract class SuperClass<TModel>
                        where TModel : IModel
                    {
                        protected SuperClass(TModel model)
                        {
                            Model = model;
                        }
                        
                        public TModel Model { get; }
                    }        
            
                    public abstract class BaseClass<TModel> : SuperClass<TModel>
                        where TModel : IModel
                    {
                        protected SuperClass(TModel model)
                        {
                            Model = model;
                        }
                    }
            
                    public sealed class DerivedClass : BaseClass
                    {
                    }
            """;

        // Parse the code
        var tree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("Example")
            .AddSyntaxTrees(tree)
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

        var semanticModel = compilation.GetSemanticModel(tree);

        // Find the DerivedClass declaration
        var root = tree.GetRoot();
        var classDeclaration = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First(cls => cls.Identifier.Text == "DerivedClass");

        var factoryDeclaration = classDeclaration.GetFactoryDeclaration(semanticModel);
    }
    
    
    [Fact]
    public void GetSimpleFactoryCase()
    {
        var code =
            //language=csharp
            """
            [ResolveFactory]
            public abstract class AbstractModel
            {
            }
            
            [ResolveFactory]
            public sealed class ViewModel
            {
                public ViewModel(
                    string exportPath,
                    string? exportPath2,
                    int i,
                    int? i2,
                    Guid guid,
                    Guid? guid2,
                    DateTime dt,
                    DateTime? dt2,
                    DateTimeOffset dto,
                    DateTimeOffset? dto2,
                    DateTimeOffset? dto3,
                    ITestService serviceInterface,
                    TestService serviceClass,
                    IEnumerable<ITestService> serviceInterfaces,
                    IEnumerable<TestService> serviceClasses,
                    ISingletonViewModelFactory singletonViewModelFactory,
                    bool test = false,
                    int defaultLen = 100,
                    double defaultDblLen = 10.01,
                    [ResolveFactoryParameter] AppOptions? options = null)
                {
                    ExportPath = exportPath;
                    //...
                }
            
                public ViewModel(
                    string exportPath,
                    string customCodePath,
                    ITestService serviceInterface,
                    ISingletonViewModelFactory singletonViewModelFactory,
                    [ResolveFactoryParameter] AppOptions? options = null)
                {
                    ExportPath = exportPath;
                    //...
                }
            
                public string ExportPath { get; }
            }
            """;

        // Parse the code
        var tree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("Example")
            .AddSyntaxTrees(tree)
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

        var semanticModel = compilation.GetSemanticModel(tree);

        // Find the DerivedClass declaration
        var root = tree.GetRoot();
        var classDeclarations = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>().ToArray();
            
            
        var abstractViewModelFactoryDecl = classDeclarations.First(cls => cls.Identifier.Text == "AbstractModel").GetFactoryDeclaration(semanticModel);

        abstractViewModelFactoryDecl.Should().BeNull();
        
        var viewModelFactoryDecl = classDeclarations.First(cls => cls.Identifier.Text == "ViewModel").GetFactoryDeclaration(semanticModel);
        
        viewModelFactoryDecl.Should().NotBeNull();
    }
}