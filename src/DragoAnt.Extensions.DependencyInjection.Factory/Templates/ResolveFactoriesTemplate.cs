﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace DragoAnt.Extensions.DependencyInjection.Factory.Templates
{
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    internal partial class ResolveFactoriesTemplate : BaseTransformation<GenerationData>
    {
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("// This file generated by DragoAnt.Extensions.DependencyInjection.Factory source generator\r\n\r\n#nullable enable\r\n");

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

    if (Data.Factories.Any(f => f.IsInvalid))
    {
        foreach (var invalidFactory in Data.Factories.Where(f => f.IsInvalid))
        {

            
            this.Write(this.ToStringHelper.ToStringWithCulture(invalidFactory.GetError()));
            
            #line default
            #line hidden
            this.Write(">\r\n");
            
        }
    }

            this.Write("\r\n\r\n\r\nnamespace ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.Namespace));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\n/// <summary>\r\n/// Factory dependency injection registration extensions.\r\n/// </summary>\r\npublic static class ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.MethodCodeName));
            
            #line default
            #line hidden
            this.Write("RegistrationExtensions\r\n{\r\n    /// <summary>\r\n    /// ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.MethodCodeName));
            
            #line default
            #line hidden
            this.Write(" factories registration extensions.\r\n    /// </summary>\r\n    public static void Add");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.MethodCodeName));
            
            #line default
            #line hidden
            this.Write("Factories(this IServiceCollection services)\r\n    {\r\n");

    foreach (var factory in Data.Factories.OrderBy(v => v.FactoryClassName))
    {
        var addService = $"services.Add{factory.Lifetime}";
        var interfaces = factory.GetInterfaces().ToArray();
        if (interfaces.Length > 1)
        {

            this.Write("        ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(addService));
            
            #line default
            #line hidden
            this.Write("<");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(factory.FactoryClassName));
            
            #line default
            #line hidden
            this.Write(">();\r\n");

            foreach (var iface in interfaces)
            {

            this.Write("        ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(addService));
            
            #line default
            #line hidden
            this.Write("<");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(iface.Name));
            
            #line default
            #line hidden
            this.Write(">(p => p.GetRequiredService<");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(factory.FactoryClassName));
            
            #line default
            #line hidden
            this.Write(">());\r\n");

            }
        }
        else
        {

            this.Write("        ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(addService));
            
            #line default
            #line hidden
            this.Write("<");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(interfaces[0].Name));
            
            #line default
            #line hidden
            this.Write(", ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(factory.FactoryClassName));
            
            #line default
            #line hidden
            this.Write(">();\r\n");

        }
    }

            this.Write("    }\r\n}\r\n");

    foreach (var factory in Data.Factories.OrderBy(v => v.FactoryClassName))
    {
        if (factory.GeneratingInterface is {} generatingInterface)
        {

            this.Write("\r\n/// <summary>\r\n/// Factory contract for <see cref=\"");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(factory.InstanceClassName));
            
            #line default
            #line hidden
            this.Write("\"/>.\r\n/// </summary>\r\npublic interface ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(generatingInterface.Name));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n");

            foreach (var method in generatingInterface.Methods)
            {

            this.Write("    ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(factory.InstanceClassName));
            
            #line default
            #line hidden
            this.Write(" Create(");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(method.GetParametersForSignature(true)));
            
            #line default
            #line hidden
            this.Write(");\r\n");

            }

            this.Write("}\r\n");

        }

            this.Write("\r\n");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(factory.GetError()));
            
            #line default
            #line hidden
            this.Write("\r\n/// <summary>\r\n/// Factory implementation for <see cref=\"");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(factory.InstanceClassName));
            
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
            this.Write(") =>\r\n");

                var ctor = method.GetEquivalentConstructorMethod(factory.Constructors, iface.CastParameters);
                if (ctor is null)
                {
                    if (iface.AllowNotSupportedMethods)
                    {

            this.Write("        throw new System.NotSupportedException();\r\n");

                    }
                    else
                    {

            this.Write("#error ResolveFactory: Can't find equivalent construtor for interface's method.\r\n");

                    }
                    continue;
                }

            this.Write("        new ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(factory.InstanceClassName));
            
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

            return this.GenerationEnvironment.ToString();
        }
    }
}
