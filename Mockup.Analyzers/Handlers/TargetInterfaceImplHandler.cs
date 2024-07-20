using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mockup.Visitors;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mockup.Generators;

public class TargetInterfaceImplHandler : ITypeSymbolVisitor<MemberDeclarationSyntax[]>
{
    private readonly ITargetInterfaceImplStrategy _strategy;
    
    private INamespaceSymbol _namespaceSymbol;
    private ITypeSymbol _typeSymbol;
    
    private List<MemberDeclarationSyntax>? _members;

    public TargetInterfaceImplHandler(ITargetInterfaceImplStrategy strategy)
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

        var members = methodSymbol.Visit(new MethodImplHandler());
        _members.AddRange(members);
    }

    public void Member(IPropertySymbol propertySymbol)
    {
        // var members = propertySymbol.Visit(new PropertySetupHandler(
        //     new PropertySetupStrategy()));
        var members = propertySymbol.Visit(new PropertyImplHandler());
        _members.AddRange(members);
    }

    public void MembersEnd()
    {
    }

    public MemberDeclarationSyntax[]? End()
    {
        var accessor = Accessor();
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
            accessor,
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

    private PropertyDeclarationSyntax Accessor()
    {
        return PropertyDeclaration
        (
            IdentifierName(_typeSymbol.Name),
            Identifier("Object")
        )
        .WithModifiers
        (
            TokenList
            (
                Token(SyntaxKind.PublicKeyword)
            )
        )
        .WithAccessorList
        (
            AccessorList
            (
                SingletonList
                (
                    AccessorDeclaration
                    (
                        SyntaxKind.GetAccessorDeclaration
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
                    )
                )
            )
        );
    }
}

public interface ITargetInterfaceImplStrategy
{
    string GetTypeName(ITypeSymbol typeSymbol);
}

public class TargetInterfaceImplStrategy : ITargetInterfaceImplStrategy
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
