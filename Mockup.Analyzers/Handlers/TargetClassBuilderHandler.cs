using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mockup.Analyzers.Visitors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mockup.Analyzers.Handlers;

public class TargetClassBuilderHandler : ITypeSymbolVisitor<CompilationUnitSyntax>
{
    private readonly ITargetClassBuilderStrategy _strategy;
    
    private INamespaceSymbol _namespaceSymbol;
    private ITypeSymbol _typeSymbol;
    
    private List<MemberDeclarationSyntax>? _members;

    public TargetClassBuilderHandler(ITargetClassBuilderStrategy strategy)
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
                if (IsConstructor(methodSymbol))
                {
                    MemberConstructor(methodSymbol);
                }
                else if (IsOverrideable(methodSymbol))
                {
                    MemberMethod(methodSymbol);
                }
                break;
            
            case IPropertySymbol propertySymbol:
                if (propertySymbol.IsAbstract || propertySymbol.IsVirtual)
                {
                    MemberProp(propertySymbol);
                }
                break;
        }
    }

    private bool IsConstructor(IMethodSymbol methodSymbol)
    {
        return methodSymbol.MethodKind == MethodKind.Constructor;
    }

    private bool IsOverrideable(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.MethodKind == MethodKind.Ordinary
            || methodSymbol.MethodKind == MethodKind.DeclareMethod)
        {
            return methodSymbol.MethodKind != MethodKind.Constructor
                   && methodSymbol.MethodKind != MethodKind.Destructor
                   && (methodSymbol.IsAbstract || methodSymbol.IsVirtual);
        }

        return false;
    }

    private void MemberConstructor(IMethodSymbol methodSymbol)
    {
        var members = methodSymbol.Visit(new ConstructorBuilderHandler(
            new ConstructorBuilderStrategy()));
        _members.AddRange(members);
    }

    private void MemberMethod(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.MethodKind == MethodKind.PropertyGet)
            return;
        
        if (methodSymbol.MethodKind == MethodKind.PropertySet)
            return;
        
        var members = methodSymbol.Visit(new MethodSetupHandler(
            new MethodSetupStrategy()));
        
        _members.AddRange(members);
    }

    private void MemberProp(IPropertySymbol propertySymbol)
    {
        var members = propertySymbol.Visit(new PropertySetupHandler(
            new PropertySetupStrategy()));
        
        _members.AddRange(members);
    }

    public void MembersEnd()
    {
        var members = _typeSymbol.Visit(new TargetClassImplHandler(
            new TargetClassImplStrategy()));
        
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

public interface ITargetClassBuilderStrategy
{
    string GetTypeName(ITypeSymbol typeSymbol);
}

public class TargetClassBuilderStrategy : ITargetClassBuilderStrategy
{
    public string GetTypeName(ITypeSymbol typeSymbol)
    {
        var typeName = typeSymbol.Name;
        
        if (typeName.Length > 1)
        {
            if (typeSymbol.TypeKind == TypeKind.Interface && typeName[0] == 'I')
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
