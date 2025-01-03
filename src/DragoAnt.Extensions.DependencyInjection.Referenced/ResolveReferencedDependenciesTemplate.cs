﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace DragoAnt.Extensions.DependencyInjection.Referenced
{
    using DragoAnt.Extensions.T4;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    internal partial class ResolveReferencedDependenciesTemplate : BaseTransformation<ResolveReferencedDependenciesData>
    {
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("// This file generated by DragoAnt.Extensions.DependencyInjection source generator.\r\n// More details: https://github.com/DragoAnt/Extensions.DependencyInjection\r\n\r\n");

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

            this.Write("\r\nnamespace ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.Namespace));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\ninternal static class ReferencedRegistrationExtensions\r\n{\r\n    /// <summary>\r\n    /// Register all referenced dependencies.\r\n    /// </summary>\r\n    /// <remarks>Generated by <a href=\"https://github.com/DragoAnt/Extensions.DependencyInjection\">DragoAnt.Extensions.DependencyInjection.Referenced</a> source generator.</remarks>\r\n    public static void AddReferencedDependencies(this IServiceCollection services)\r\n    {\r\n");

    foreach (var method in Data.GetReferencedDependenciesMethodsNames())
    {

            this.Write("        ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(method));
            
            #line default
            #line hidden
            this.Write("(services);\r\n");

    }

            this.Write("    }\r\n");

    //NOTE: We can guarantee that method is existed only if it will always generate
    if (Data.AlwaysGenerateOwnDependenciesMethod)
    {

            this.Write("\r\n    /// <summary>\r\n    /// Register all dependencies(referenced anf own).\r\n    /// </summary>\r\n    /// <remarks>Generated by <a href=\"https://github.com/DragoAnt/Extensions.DependencyInjection\">DragoAnt.Extensions.DependencyInjection.Referenced</a> source generator.</remarks>\r\n    public static void AddDependencies(this IServiceCollection services)\r\n    {\r\n        services.AddReferencedDependencies();\r\n");

        foreach (var method in Data.GetOwnDependenciesMethodsNames())
        {

            this.Write("        ");
            
            this.Write(this.ToStringHelper.ToStringWithCulture(method));
            
            #line default
            #line hidden
            this.Write("(services);\r\n");

        }

            this.Write("    }\r\n");

    }

            this.Write("}");
            return this.GenerationEnvironment.ToString();
        }
    }
}
