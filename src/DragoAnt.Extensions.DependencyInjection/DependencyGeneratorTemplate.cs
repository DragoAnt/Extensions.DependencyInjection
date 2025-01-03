﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace DragoAnt.Extensions.DependencyInjection
{
    using DragoAnt.Extensions.DependencyInjection.Templates;
    using DragoAnt.Extensions.T4;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    internal partial class DependencyGeneratorTemplate : BaseTransformation<ResolveDependenciesData>
    {
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("// This file generated by DragoAnt.Extensions.DependencyInjection source generator.\r\n// More details: https://github.com/DragoAnt/Extensions.DependencyInjection\r\n\r\n\r\n");

    if (Data is { CustomDependenciesEnabled: false, AlwaysGenerateAddDependenciesMethod: false, Errors.Length: 0, Dependencies.Length: 0, Factories.Length: 0 })
    {

            this.Write("// No Resolve or/and ResolveFactory marked class/interface found. \r\n");

    }
    else if (Data.Errors.Length != 0)
    {
        foreach (var error in Data.Errors)
        {

            this.Write("#error ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(error));
            
            #line default
            #line hidden
            this.Write("\r\n");

        }
    }
    else
    {

            this.Write("\r\n#nullable enable\r\n\r\n");

        foreach (var usingNs in Data
            .GetUsings("Microsoft.Extensions.DependencyInjection")
            .Except([Data.Namespace]))
        {

            this.Write("using ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(usingNs));
            
            #line default
            #line hidden
            this.Write(";\r\n");

        }

            this.Write("using static Microsoft.Extensions.DependencyInjection.ServiceLifetime;\r\n\r\n[assembly:ResolveAssembly(\"");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.FullMethodName));
            
            #line default
            #line hidden
            this.Write("\")]\r\n\r\nnamespace ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.Namespace));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\n/// <summary>\r\n/// Dependency injection registration extensions.\r\n/// </summary>\r\npublic static partial class ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.ExtensionsClassName));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    /// <summary>\r\n    /// ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.MethodCodeName));
            
            #line default
            #line hidden
            this.Write(" dependencies and factories registration extension.\r\n    /// </summary>\r\n    /// <remarks>Generated by <a href=\"https://github.com/DragoAnt/Extensions.DependencyInjection\">DragoAnt.Extensions.DependencyInjection</a> source generator.</remarks>\r\n    public static void ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.MethodCodeName));
            
            #line default
            #line hidden
            this.Write("(this IServiceCollection services)\r\n    {\r\n");

        if (Data.Dependencies.Any(d => d.Interfaces.Length > 1))
        {

            this.Write("        Func<IServiceProvider, object> factory;\r\n");

        }

        foreach (var dependency in Data.Dependencies.OrderBy(v => v.InstanceClassName))
        {

            this.Write("\r\n");

            var lifetime = dependency.Lifetime.ToServiceLifetime();
            if (dependency.Interfaces.Length == 1 && !dependency.ItselfRegistration)
            {

            this.Write("        services.Add(new(typeof(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(dependency.Interfaces[0]));
            
            #line default
            #line hidden
            this.Write("), typeof(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(dependency.InstanceClassName));
            
            #line default
            #line hidden
            this.Write("), ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(lifetime));
            
            #line default
            #line hidden
            this.Write("));\r\n");

            }
            else
            {
                if (dependency.CustomFactoryMethod is not null && !string.IsNullOrEmpty(dependency.CustomFactoryMethod))
                {

            this.Write("        services.Add(new(typeof(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(dependency.InstanceClassName));
            
            #line default
            #line hidden
            this.Write("), ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(dependency.CustomFactoryMethod));
            
            #line default
            #line hidden
            this.Write(", ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(lifetime));
            
            #line default
            #line hidden
            this.Write("));\r\n");

                }
                else
                {

            this.Write("        services.Add(new(typeof(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(dependency.InstanceClassName));
            
            #line default
            #line hidden
            this.Write("), typeof(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(dependency.InstanceClassName));
            
            #line default
            #line hidden
            this.Write("), ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(lifetime));
            
            #line default
            #line hidden
            this.Write("));\r\n");

                }

                if (dependency.Interfaces.Length == 1)
                {

            this.Write("        services.Add(new(typeof(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(dependency.Interfaces[0]));
            
            #line default
            #line hidden
            this.Write("), static p => p.GetRequiredService(typeof(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(dependency.InstanceClassName));
            
            #line default
            #line hidden
            this.Write(")), ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(lifetime));
            
            #line default
            #line hidden
            this.Write("));\r\n");

                }
                else if (dependency.Interfaces.Length > 1)
                {

            this.Write("        factory = static p => p.GetRequiredService(typeof(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(dependency.InstanceClassName));
            
            #line default
            #line hidden
            this.Write("));\r\n");

                    foreach (var iface in dependency.Interfaces)
                    {

            this.Write("        services.Add(new(typeof(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(iface));
            
            #line default
            #line hidden
            this.Write("), factory, ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(lifetime));
            
            #line default
            #line hidden
            this.Write("));\r\n");

                    }
                }
            }
        }

        if (Data.CustomDependenciesEnabled)
        {

            this.Write("\r\n        AddCustomDependencies(services);\r\n");

        }

            this.Write("    }\r\n");

        if (Data.CustomDependenciesEnabled)
        {

            this.Write("    /// <summary>\r\n    /// Custom dependencies registration.\r\n    /// </summary>\r\n    private static partial void AddCustomDependencies(IServiceCollection services);\r\n");

        }

        foreach (var dependencyWithFactoryMethod in Data.Dependencies.Where(d => !string.IsNullOrEmpty(d.CustomFactoryMethod)))
        {

            this.Write("    /// <summary>\r\n    /// Custom factory method for <see cref=\"");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(dependencyWithFactoryMethod.InstanceClassName));
            
            #line default
            #line hidden
            this.Write("\"/>. \r\n    /// </summary>\r\n    private static partial ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(dependencyWithFactoryMethod.InstanceClassName));
            
            #line default
            #line hidden
            this.Write(" ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(dependencyWithFactoryMethod.CustomFactoryMethod!));
            
            #line default
            #line hidden
            this.Write("(IServiceProvider provider);\r\n");

        }

            this.Write("\r\n}\r\n");

        foreach (var factory in Data.Factories.OrderBy(v => v.FactoryClassName))
        {
            var crefClassName = factory.InstanceClassDefinition.Replace("<", "{").Replace(">", "}");
            if (factory.GeneratingInterface is {} generatingInterface)
            {

            this.Write("\r\n/// <summary>\r\n/// Factory contract for <see cref=\"");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(crefClassName));
            
            #line default
            #line hidden
            this.Write("\"/>.\r\n/// </summary>\r\n");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(factory.IsPublic ? "public" : "internal"));
            
            #line default
            #line hidden
            this.Write(" interface ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(generatingInterface.Name));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n");

                foreach (var method in generatingInterface.Methods)
                {
                    if (!method.HasTypeParameterConstraints(false))
                    {

            this.Write("    ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(method.ReturnTypeName));
            
            #line default
            #line hidden
            this.Write(" ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(method.Name));
            
            #line default
            #line hidden
            this.Write("(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(method.GetParametersForSignature(true)));
            
            #line default
            #line hidden
            this.Write(");\r\n");

                    }
                    else
                    {

            this.Write("    ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(method.ReturnTypeName));
            
            #line default
            #line hidden
            this.Write(" ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(method.Name));
            
            #line default
            #line hidden
            this.Write("(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(method.GetParametersForSignature(true)));
            
            #line default
            #line hidden
            this.Write(")\r\n");

                        foreach (var clause in method.GetTypeParameterClauses(false))
                        {

            this.Write("        ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(clause));
            
            #line default
            #line hidden
            this.Write("\r\n");

                        }

            this.Write("    ;\r\n");

                    }
                }

            this.Write("}\r\n");

            }

            this.Write("\r\n/// <summary>\r\n/// Factory implementation for <see cref=\"");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(crefClassName));
            
            #line default
            #line hidden
            this.Write("\"/>.\r\n/// </summary>\r\ninternal sealed class ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(factory.FactoryClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(string.Join(", ", factory.GetInterfaces().Select(i => i.Name))));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    private readonly IServiceProvider _provider;\r\n\r\n    public ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(factory.FactoryClassName));
            
            #line default
            #line hidden
            this.Write("(IServiceProvider provider)\r\n    {\r\n        _provider = provider;        \r\n    }\r\n\r\n");

            foreach (var iface in factory.GetInterfaces())
            {
                foreach (var method in iface.Methods)
                {

            this.Write("    ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(method.ReturnTypeName));
            
            #line default
            #line hidden
            this.Write(" ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(iface.Name));
            
            #line default
            #line hidden
            this.Write(".");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(method.Name));
            
            #line default
            #line hidden
            this.Write("(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(method.GetParametersForSignature(false)));
            
            #line default
            #line hidden
            this.Write(") \r\n ");

                    foreach (var clause in method.GetTypeParameterClauses(true))
                    {

            this.Write("        ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(clause));
            
            #line default
            #line hidden
            this.Write("\r\n");

                    }
                    var ctor = method.GetEquivalentConstructorMethod(factory.Constructors, iface.CastParameters);
                    if (ctor is null || ctor.Value.IsEmpty)
                    {
                        if (iface.AllowNotSupportedMethods)
                        {

            this.Write("        => throw new System.NotSupportedException();\r\n");

                        }
                        else
                        {

            this.Write("#error ResolveFactory: Can't find equivalent construtor for interface's method.\r\n");

                        }
                        continue;
                    }

            this.Write("        => new ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(factory.InstanceClassDefinition));
            
            #line default
            #line hidden
            this.Write("(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(ctor.Value.GetCallCtorParameters("_provider", method)));
            
            #line default
            #line hidden
            this.Write(");\r\n");

                }
            }

            this.Write("}\r\n");

        }
    }

            return this.GenerationEnvironment.ToString();
        }
    }
}
