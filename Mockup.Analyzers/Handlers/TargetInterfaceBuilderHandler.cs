using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mockup.Visitors;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mockup.Generators;

public class TargetInterfaceBuilderHandler : ITypeSymbolVisitor<CompilationUnitSyntax>
{
    private readonly ITargetInterfaceBuilderStrategy _strategy;
    
    private INamespaceSymbol _namespaceSymbol;
    private ITypeSymbol _typeSymbol;
    
    private List<MemberDeclarationSyntax>? _members;

    public TargetInterfaceBuilderHandler(ITargetInterfaceBuilderStrategy strategy)
    {
        _strategy = strategy;
    }
    
    public void Begin()
    {
    }

    public void Namespace(INamespaceSymbol namespaceSymbol)
    {
        _namespaceSymbol = namespaceSymbol;
    }

    public void Type(ITypeSymbol typeSymbol)
    {
        _typeSymbol = typeSymbol;
    }

    public void MembersBegin(int count)
    {
        _members = new List<MemberDeclarationSyntax>(count);
    }

    public void Member(ISymbol symbol)
    {
        switch (symbol)
        {
            case IMethodSymbol methodSymbol:
                Member(methodSymbol);
                break;
            
            case IPropertySymbol propertySymbol:
                Member(propertySymbol);
                break;
        }
    }

    public void Member(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.MethodKind == MethodKind.PropertyGet)
            return;
        
        if (methodSymbol.MethodKind == MethodKind.PropertySet)
            return;
        
        var members = methodSymbol.Visit(new MethodSetupHandler(
            new MethodSetupStrategy()));
        
        _members.AddRange(members);
    }

    public void Member(IPropertySymbol propertySymbol)
    {
        var members = propertySymbol.Visit(new PropertySetupHandler(
            new PropertySetupStrategy()));
        
        _members.AddRange(members);
    }

    public void MembersEnd()
    {
        var members = _typeSymbol.Visit(new TargetInterfaceImplHandler(
            new TargetInterfaceImplStrategy()));
        
        _members.AddRange(members);
    }

    public CompilationUnitSyntax? End()
    {
        return CompilationUnit()
            .WithUsings
            (
                SingletonList
                (
                    UsingDirective
                    (
                        IdentifierName("System")
                    )
                )
            )
            .WithMembers
            (
                SingletonList<MemberDeclarationSyntax>
                (
                    NamespaceDeclaration
                    (
                        IdentifierName(Utils.GetFullName(_namespaceSymbol))
                    )
                    .WithMembers
                    (
                        SingletonList<MemberDeclarationSyntax>
                        (
                            ClassDeclaration(_strategy.GetTypeName(_typeSymbol))
                                .WithModifiers
                                (
                                    TokenList
                                    (
                                        Token(SyntaxKind.PublicKeyword)
                                    )
                                )
                                .WithMembers
                                (
                                    List(_members)
                                )
                        )
                    )
                )
            );
    }
}

public interface ITargetInterfaceBuilderStrategy
{
    string GetTypeName(ITypeSymbol typeSymbol);
}

public class TargetInterfaceBuilderStrategy : ITargetInterfaceBuilderStrategy
{
    public string GetTypeName(ITypeSymbol typeSymbol)
    {
        var typeName = typeSymbol.Name;
        
        if (typeName.Length > 1)
        {
            var first = typeName[0];
            if (first == 'I')
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
}
