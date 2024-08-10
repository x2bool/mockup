using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mockup.Analyzers;

public class MockAttributeSyntaxReceiver : ISyntaxReceiver
{
    public List<AttributeListSyntax> Targets { get; } = new();
    
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not AttributeListSyntax attributeListSyntax)
            return;

        if (attributeListSyntax?.Target?.Identifier.Kind() != SyntaxKind.AssemblyKeyword)
            return;
        
        SyntaxFactory.AttributeTargetSpecifier(
            SyntaxFactory.Token(SyntaxKind.AssemblyKeyword));

        Targets.Add(attributeListSyntax);
    }
}
