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
        _members.Add(Constructor());
    }

    public void Member(ISymbol symbol)
    {
        switch (symbol)
        {
            case IMethodSymbol methodSymbol:
                if (IsOverrideable(methodSymbol))
                {
                    Member(methodSymbol);
                }
                break;
            
            case IPropertySymbol propertySymbol:
                if (propertySymbol.IsAbstract || propertySymbol.IsVirtual)
                {
                    Member(propertySymbol);
                }
                break;
        }
    }

    private bool IsOverrideable(IMethodSymbol methodSymbol)
    {
        //if (methodSymbol.Name == ".ctor") return false; // TODO: ???
        //if (methodSymbol.Name == _typeSymbol.Name) return false;
        
        if (methodSymbol.MethodKind == MethodKind.Ordinary
            || methodSymbol.MethodKind == MethodKind.DeclareMethod)
        {
            return methodSymbol.MethodKind != MethodKind.Constructor
                   && methodSymbol.MethodKind != MethodKind.Destructor;
        }

        return false;
    }

    private void Member(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.MethodKind == MethodKind.PropertyGet)
            return;
        
        if (methodSymbol.MethodKind == MethodKind.PropertySet)
            return;

        var members = methodSymbol.Visit(new MethodImplHandler());
        _members.AddRange(members);
    }

    private void Member(IPropertySymbol propertySymbol)
    {
        var members = propertySymbol.Visit(new PropertyImplHandler());
        _members.AddRange(members);
    }

    public void MembersEnd()
    {
    }

    public MemberDeclarationSyntax[]? End()
    {
        var builderMethod = BuilderMethod();
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
            builderMethod,
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

    private ConstructorDeclarationSyntax Constructor()
    {
        return ConstructorDeclaration
        (
            Identifier("@class")
        )
        .WithModifiers
        (
            TokenList
            (
                Token(SyntaxKind.PublicKeyword)
            )
        )
        .WithParameterList
        (
            ParameterList
            (
                SingletonSeparatedList<ParameterSyntax>
                (
                    Parameter
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
                        .WithType
                        (
                            IdentifierName(_strategy.GetTypeName(_typeSymbol))
                        )
                )
            )
        )
        .WithBody
        (
            Block
            (
                SingletonList<StatementSyntax>
                (
                    ExpressionStatement
                    (
                        AssignmentExpression
                        (
                            SyntaxKind.SimpleAssignmentExpression,
                            MemberAccessExpression
                            (
                                SyntaxKind.SimpleMemberAccessExpression,
                                ThisExpression(),
                                IdentifierName
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
                            ),
                            IdentifierName
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
            )
        );
    }

    private MethodDeclarationSyntax BuilderMethod()
    {
        return MethodDeclaration
        (
            IdentifierName(_typeSymbol.Name),
            Identifier("Build") // TODO: make configurable
        )
        .WithModifiers
        (
            TokenList
            (
                Token(SyntaxKind.PublicKeyword)
            )
        )
        .WithBody
        (
            Block
            (
                SingletonList<StatementSyntax>
                (
                    ReturnStatement
                    (
                        ObjectCreationExpression
                        (
                            IdentifierName("@class")
                        )
                        .WithArgumentList
                        (
                            ArgumentList
                            (
                                SingletonSeparatedList<ArgumentSyntax>
                                (
                                    Argument
                                    (
                                        ThisExpression()
                                    )
                                )
                            )
                        )
                    )
                )
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
