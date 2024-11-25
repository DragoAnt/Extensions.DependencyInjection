using DragoAnt.Extensions.DependencyInjection.Factory.Templates;
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

        var factoryDeclaration = classDeclaration.GetFactory(semanticModel);
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
            
            
        var abstractViewModelFactoryDecl = classDeclarations.First(cls => cls.Identifier.Text == "AbstractModel").GetFactory(semanticModel);

        abstractViewModelFactoryDecl.Should().BeNull();
        
        var viewModelFactoryDecl = classDeclarations.First(cls => cls.Identifier.Text == "ViewModel").GetFactory(semanticModel);
        
        viewModelFactoryDecl.Should().NotBeNull();
    }
    
    [Fact]
    public void GetComplexFactoryCase()
    {
        var code =
            //language=csharp
            """
            // This class imported to the project by DragoAnt.Extensions.DependencyInjection.Factory package
            using System;
            using static System.AttributeTargets;
            
            internal enum ResolveFactoryServiceLifetime
            {
                Scoped = 0,
                Singleton = 1,
            }
            
            /// <summary>
            /// Mark class with this attribute to generate factory and factory registration for Dependency Injection ServiceCollection.
            /// </summary>
            /// <param name="lifetime">Factory service lifetime.</param>
            [AttributeUsage(Class)]
            internal sealed class ResolveFactoryAttribute(
                ResolveFactoryServiceLifetime lifetime = ResolveFactoryServiceLifetime.Scoped) : Attribute
            {
                public ResolveFactoryServiceLifetime Lifetime { get; } = lifetime;
            
                /// <summary>
                /// Skip generation and registration of specific factory interface.
                /// </summary>
                public bool SkipGenerateInterface { get; set; }
            }
            
            /// <summary>
            /// Mark class with this attribute to add implementation of factory contract to factory and registration for Dependency Injection ServiceCollection.
            /// </summary>
            /// <param name="interfaceTypeDefinition">Factory service contract.</param>
            [AttributeUsage(Class, AllowMultiple = true)]
            internal sealed class ResolveFactoryContractAttribute(Type interfaceTypeDefinition) : Attribute
            {
                /// <summary>
                /// Shared factory generic interface type definition.(e.g. IFactory{T}).
                /// The interface contains Create methods with the same factory parameters as constructors.
                /// </summary>
                public Type? InterfaceTypeDefinition { get; } = interfaceTypeDefinition;
            
                /// <summary>
                /// Cast parameters to shared factory method parameters. Mapped by name. Default is true.
                /// </summary>
                public bool CastParameters { get; set; } = true;
            
                /// <summary>
                ///If true and constructors can't be mapped by parameters to shared factory methods then generates throw <see cref="NotSupportedException"/> otherwise generates pragma error. 
                /// </summary>
                public bool AllowNotSupportedMethods { get; set; }
            }
            
            /// <summary>
            /// Attribute to mark constrictor to be ignored during Factory code generation.
            /// </summary>
            [AttributeUsage(Constructor)]
            internal sealed class ResolveFactoryIgnoreCtorAttribute : Attribute
            {
            }
            
            /// <summary>
            /// Attribute to mark parameter as factory parameter 
            /// </summary>
            [AttributeUsage(Parameter)]
            internal sealed class ResolveFactoryParameterAttribute : Attribute
            {
            }
            
            /// <summary>
            /// Attribute to mark parameter as factory service 
            /// </summary>
            [AttributeUsage(Parameter)]
            internal sealed class ResolveFactoryServiceAttribute : Attribute
            {
            }
            
            /// <summary>
            /// Attribute to mark referenced type to be an explicit factory parameter by default. 
            /// </summary>
            [AttributeUsage(Class | Interface)]
            internal sealed class AsResolveFactoryParameterAttribute : Attribute
            {
            }
            }
            
            [AsResolveFactoryParameter]
            public interface IModel
            {
            }
            
            public class SimpleModel : IModel
            {
            }
            
            public class ComplexModel : IModel
            {
                public int Value { get; }
            
                public ComplexModel(int value)
                {
                    Value = value;
                }
            }
            
            public interface IFactory<out T>
            {
                T Create();
                T Create(IModel model);
            }
            
            public interface IAdvFactory<out T>
            {
                T Create();
                T Create(IModel model);
            }
            
            //[ResolveFactory(SkipGenerateInterface = true)]
            [ResolveFactory]
            [ResolveFactoryContract(typeof(IFactory<>), AllowNotSupportedMethods = true)]
            [ResolveFactoryContract(typeof(IAdvFactory<>), AllowNotSupportedMethods = true)]
            public abstract class SuperViewModel<TModel>
                where TModel : IModel
            {
                protected SuperViewModel(TModel model)
                {
                    Model = model;
                }
            
                public TModel Model { get; }
            }
            
            public abstract class BaseViewModel<TModel> : SuperViewModel<TModel>
                where TModel : IModel
            {
                protected BaseViewModel(TModel model)
                    : base(model)
                {
                }
            }
            
            public sealed class SimpleViewModel : BaseViewModel<SimpleModel>
            {
                public SimpleViewModel()
                    : this(new SimpleModel())
                {
                }
            
                public SimpleViewModel(SimpleModel model)
                    : base(model)
                {
                }
            }
            
            public sealed class ComplexViewModel : BaseViewModel<ComplexModel>
            {
                public ComplexViewModel(ComplexModel model)
                    : base(model)
                {
                }
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

        var abstractViewModelFactoryDecl = classDeclarations.First(cls => cls.Identifier.Text == "SuperViewModel").GetFactory(semanticModel);

        abstractViewModelFactoryDecl.Should().BeNull();
        
        var viewModelFactoryDecl = classDeclarations.First(cls => cls.Identifier.Text == "SimpleViewModel").GetFactory(semanticModel);
        
        viewModelFactoryDecl.Should().NotBeNull();

        var generatedCode = new ResolveFactoriesTemplate
        {
            Data = new GenerationData("Test", "ns_ns", [viewModelFactoryDecl!.Value])
        }.TransformText();
    }
}