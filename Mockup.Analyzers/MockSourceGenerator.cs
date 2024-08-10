using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mockup.Analyzers;

[Generator]
public class MockSourceGenerator : ISourceGenerator
{   
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new MockAttributeSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not MockAttributeSyntaxReceiver receiver) return;

        var mockAttribute = context.Compilation.GetTypeByMetadataName(
            "Mockup.MockAttribute");
        var genericMockAttribute = context.Compilation.GetTypeByMetadataName(
            "Mockup.MockAttribute`1");

        var mockTargets = new List<MockTarget>();
        
        foreach (var target in receiver.Targets)
        {
            foreach (var attribute in target.Attributes)
            {
                var attributeSemantics = context.Compilation.GetSemanticModel(attribute.SyntaxTree);

                if (attributeSemantics.GetSymbolInfo(attribute).Symbol is not IMethodSymbol methodSymbol)
                {
                    continue;
                }

                if (methodSymbol.ContainingType is not ITypeSymbol containerSymbol)
                {
                    continue;
                }

                var definition = containerSymbol.OriginalDefinition.ToDisplayString();
                if (definition == mockAttribute?.ToDisplayString())
                {
                    foreach (var argument in attribute.ArgumentList.Arguments)
                    {
                        if (argument.Expression is TypeOfExpressionSyntax typeExpressionSyntax)
                        {
                            var typeSyntax = typeExpressionSyntax.Type;
                            var typeSemantics = context.Compilation.GetSemanticModel(typeSyntax.SyntaxTree);

                            if (typeSemantics.GetSymbolInfo(typeSyntax).Symbol is not ITypeSymbol typeSymbol)
                            {
                                continue;
                            }
                        
                            var mockTarget = new MockTarget(typeSymbol);
                            mockTargets.Add(mockTarget);
                        }
                    }
                }
                else if (definition == genericMockAttribute?.ToDisplayString()
                         && attribute.Name is GenericNameSyntax genericNameSyntax)
                {
                    foreach (var typeSyntax in genericNameSyntax.TypeArgumentList.Arguments)
                    {
                        var typeSemantics = context.Compilation.GetSemanticModel(typeSyntax.SyntaxTree);
                        
                        if (typeSemantics.GetSymbolInfo(typeSyntax).Symbol is not ITypeSymbol typeSymbol)
                        {
                            continue;
                        }
                        
                        var mockTarget = new MockTarget(typeSymbol);
                        mockTargets.Add(mockTarget);
                    }
                }
            }
        }

        foreach (var mockTarget in mockTargets)
        {
            var (source, diagnostic) = mockTarget.Process();

            if (source != null)
            {
                context.AddSource(
                    source.HintName, source.CompilationUnit.NormalizeWhitespace().ToFullString());
            }

            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
