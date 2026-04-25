using MinimalInterceptorSourceGenerator.SharedLibrary;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace MinimalInterceptorSourceGenerator
{

    [Generator]
    public class InterceptorGenerator : IIncrementalGenerator
    {

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            
            // Log.LogMessage($"Initializing...");
            var invocationsLocations = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (SyntaxNode node, CancellationToken cancelToken) =>
                {
                    // Log.LogMessage($"{node.GetType().FullName}");
                    // Log.LogMessage($"{node.ToString()}");
                    bool isTarget = node is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax maes && maes.Name.Identifier.ValueText== "MyMethod";

                    return isTarget;
                },
                transform: (GeneratorSyntaxContext ctx, CancellationToken cancelToken) =>
                {
                    
                    var invocation = (InvocationExpressionSyntax)ctx.Node;
                    var location = ctx.SemanticModel.GetInterceptableLocation(invocation);
                    
                    // Log.LogMessage($"{ctx.Node.GetType().FullName} ((InvocationExpressionSyntax)ctx.Node).Expression.GetType()}} ==> {((InvocationExpressionSyntax)ctx.Node).Expression.GetType()} \r\n{ctx.Node.ToFullString()} location.Version => {location.Version} location.Data => {location.Data}");
                    
                    return location;
                }
            ).Collect();
            
            context.RegisterSourceOutput(invocationsLocations, (sourceProductionContext, locations)
              => {
                  // Log.LogMessage("Executing....");
                  var location = locations.First();
                  string interceptMethod = $"[global::System.Runtime.CompilerServices.InterceptsLocation({location!.Version},\"{location.Data}\")] public static void InterceptorMethod(this MyClass mc, global:: System.String s){{Console.WriteLine(\"intercepted!\");}}";

                  string classThatHoldsTheInterceptorMethod = $"namespace MinimalInterceptorSourceGenerator{{ static file class MyInterceptors{{ {interceptMethod} }} }}";

                  sourceProductionContext.AddSource("Calculator.Generated.cs",
                       $$"""
                        namespace System.Runtime.CompilerServices
                        {
                            // this type is needed by the compiler to implement interceptors - it doesn't need to
                            // come from the runtime itself, though
            
                            [global::System.Diagnostics.Conditional("DEBUG")] // not needed post-build, so: evaporate
                            [global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]
                            sealed file class InterceptsLocationAttribute : global::System.Attribute
                            {
                                public InterceptsLocationAttribute(int version, string data)
                                {
                                    _ = version;
                                    _ = data;
                                }
                            }
                        }
                       """ +
                       classThatHoldsTheInterceptorMethod
                      );
              });
        }
    }
}
