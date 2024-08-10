using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mockup.Analyzers.Handlers;
using Mockup.Analyzers.Visitors;

namespace Mockup.Analyzers;

internal class MockTarget
{
    public ITypeSymbol TypeSymbol { get; }

    public MockTarget(ITypeSymbol typeSymbol)
    {
        TypeSymbol = typeSymbol;
    }

    public (SourceCode?, Diagnostic?) Process()
    {
        var visitor = GetVisitor();
        if (visitor == null)
        {
            return (
                null,
                Diagnostic.Create(
                    Diagnostics.InvalidTarget,
                    TypeSymbol.Locations.FirstOrDefault(),
                    Utils.GetFullName(TypeSymbol)
                )
            );
        }

        var compilationUnit = TypeSymbol.Visit(visitor);
        if (compilationUnit != null)
        {
            return (new SourceCode($"{GetHintName()}.g.cs", compilationUnit), null);
        }

        return (null, null);
    }

    private string GetTypeName()
    {
        var typeName = TypeSymbol.Name;
        if (typeName.Length > 1)
        {
            if (TypeSymbol.TypeKind == TypeKind.Interface && typeName[0] == 'I')
            {
                var second = typeName[1];
                if (char.IsUpper(second))
                {
                    typeName = typeName.Substring(1);
                }
            }
        }

        return $"{typeName}Mock";
    }

    private string GetNamespace()
    {
        return Utils.GetFullName(TypeSymbol.ContainingNamespace);
    }

    private string GetHintName()
    {
        return $"{GetNamespace().Replace('.', '_')}_{GetTypeName()}";
    }

    private ITypeSymbolVisitor<CompilationUnitSyntax>? GetVisitor()
    {
        return TypeSymbol.TypeKind switch
        {
            TypeKind.Interface => new TargetInterfaceBuilderHandler(new TargetInterfaceBuilderStrategy()),
            TypeKind.Class => TypeSymbol.IsSealed || TypeSymbol.IsStatic
                ? null
                : new TargetClassBuilderHandler(new TargetClassBuilderStrategy()),
            _ => null,
        };
    }
}
