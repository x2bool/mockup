using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mockup.Analyzers.Visitors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mockup.Analyzers.Handlers;

public class TargetClassImplHandler : ITypeSymbolVisitor<MemberDeclarationSyntax[]>
{
    private readonly ITargetClassImplStrategy _strategy;
    
    private INamespaceSymbol _namespaceSymbol;
    private ITypeSymbol _typeSymbol;
    
    private List<MemberDeclarationSyntax>? _members;

    public TargetClassImplHandler(ITargetClassImplStrategy strategy)
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
        _members.Add(BackingField());
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
                   && methodSymbol.MethodKind != MethodKind.Destructor;
        }

        return false;
    }

    private void MemberConstructor(IMethodSymbol methodSymbol)
    {
        var members = methodSymbol.Visit(new ConstructorImplHandler(
            new ConstructorImplStrategy()));
        _members.AddRange(members);
    }
    
    private void MemberMethod(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.MethodKind == MethodKind.PropertyGet)
            return;
        
        if (methodSymbol.MethodKind == MethodKind.PropertySet)
            return;

        var members = methodSymbol.Visit(new MethodImplHandler());
        _members.AddRange(members);
    }

    private void MemberProp(IPropertySymbol propertySymbol)
    {
        var members = propertySymbol.Visit(new PropertyImplHandler());
        _members.AddRange(members);
    }

    public void MembersEnd()
    {
    }

    public MemberDeclarationSyntax[]? End()
    {
        var cls = ClassDeclaration("@class")
            .WithModifiers
            (
                TokenList
                (
                    Token(SyntaxKind.PrivateKeyword)
                )
            )
            .WithBaseList
            (
                BaseList
                (
                    SingletonSeparatedList<BaseTypeSyntax>
                    (
                        SimpleBaseType
                        (
                            IdentifierName(_typeSymbol.Name)
                        )
                    )
                )
            )
            .WithMembers
            (
                List(_members)
            );

        return new MemberDeclarationSyntax[]
        {
            cls
        };
    }
    
    private FieldDeclarationSyntax BackingField()
    {
        return FieldDeclaration
        (
            VariableDeclaration
            (
                IdentifierName(_strategy.GetTypeName(_typeSymbol))
            )
            .WithVariables
            (
                SingletonSeparatedList
                (
                    VariableDeclarator
                    (
                        Identifier
                        (
                            TriviaList(),
                            SyntaxKind.VarKeyword,
                            "@var",
                            "var",
                            TriviaList()
                        )
                    )
                )
            )
        )
        .WithModifiers
        (
            TokenList
            (
                new[]
                {
                    Token(SyntaxKind.PrivateKeyword),
                    Token(SyntaxKind.ReadOnlyKeyword)
                }
            )
        );
    }
}

public interface ITargetClassImplStrategy
{
    string GetTypeName(ITypeSymbol typeSymbol);
}

public class TargetClassImplStrategy : ITargetClassImplStrategy
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
