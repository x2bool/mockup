using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mockup.Visitors;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mockup.Generators;

public class PropertyImplHandler : IPropertySymbolVisitor<MemberDeclarationSyntax[]>
{
    private ITypeSymbol _typeSymbol;
    private ITypeSymbol _returnType;
    private string _name;
    private bool _read;
    private bool _write;

    public void Begin()
    {
    }

    public void OwnerType(ITypeSymbol typeSymbol)
    {
        _typeSymbol = typeSymbol;
    }

    public void ReturnType(ITypeSymbol typeSymbol)
    {
        _returnType = typeSymbol;
    }

    public void Name(string name)
    {
        _name = name;
    }

    public void Read(bool read)
    {
        _read = read;
    }

    public void Write(bool write)
    {
        _write = write;
    }

    public MemberDeclarationSyntax[]? End()
    {
        if (_read && _write)
        {
            return new MemberDeclarationSyntax[]
            {
                GetterAndSetter()
            };
        }
        
        if (_read)
        {
            return new MemberDeclarationSyntax[]
            {
                Getter()
            };
        }
        
        if (_write)
        {
            return new MemberDeclarationSyntax[]
            {
                Setter()
            };
        }

        return Array.Empty<MemberDeclarationSyntax>();
    }

    private PropertyDeclarationSyntax Getter()
    {
        return PropertyDeclaration
        (
            Utils.GetTypeSyntax(_returnType),
            Identifier(_name)
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
                                IfStatement
                                (
                                    BinaryExpression
                                    (
                                        SyntaxKind.EqualsExpression,
                                        MemberAccessExpression
                                        (
                                            SyntaxKind.SimpleMemberAccessExpression,
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
                                            ),
                                            IdentifierName($"_getterOf{_name}")
                                        ),
                                        LiteralExpression
                                        (
                                            SyntaxKind.NullLiteralExpression
                                        )
                                    ),
                                    Block
                                    (
                                        SingletonList<StatementSyntax>
                                        (
                                            ThrowStatement
                                            (
                                                ObjectCreationExpression
                                                    (
                                                        IdentifierName("NotImplementedException")
                                                    )
                                                    .WithArgumentList
                                                    (
                                                        ArgumentList()
                                                    )
                                            )
                                        )
                                    )
                                ),
                                ReturnStatement
                                (
                                    InvocationExpression
                                    (
                                        MemberAccessExpression
                                        (
                                            SyntaxKind.SimpleMemberAccessExpression,
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
                                            ),
                                            IdentifierName($"_getterOf{_name}")
                                        )
                                    )
                                )
                            )
                        )
                )
            )
        );
    }
    
    private PropertyDeclarationSyntax Setter()
    {
        return PropertyDeclaration
        (
            Utils.GetTypeSyntax(_returnType),
            Identifier(_name)
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
                        SyntaxKind.SetAccessorDeclaration
                    )
                    .WithBody
                    (
                        Block
                        (
                            IfStatement
                            (
                                BinaryExpression
                                (
                                    SyntaxKind.EqualsExpression,
                                    MemberAccessExpression
                                    (
                                        SyntaxKind.SimpleMemberAccessExpression,
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
                                        ),
                                        IdentifierName($"_setterOf{_name}")
                                    ),
                                    LiteralExpression
                                    (
                                        SyntaxKind.NullLiteralExpression
                                    )
                                ),
                                Block
                                (
                                    SingletonList<StatementSyntax>
                                    (
                                        ThrowStatement
                                        (
                                            ObjectCreationExpression
                                            (
                                                IdentifierName("NotImplementedException")
                                            )
                                            .WithArgumentList
                                            (
                                                ArgumentList()
                                            )
                                        )
                                    )
                                )
                            ),
                            ExpressionStatement
                            (
                                InvocationExpression
                                (
                                    MemberAccessExpression
                                    (
                                        SyntaxKind.SimpleMemberAccessExpression,
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
                                        ),
                                        IdentifierName($"_setterOf{_name}")
                                    )
                                )
                                .WithArgumentList
                                (
                                    ArgumentList
                                    (
                                        SingletonSeparatedList
                                        (
                                            Argument
                                            (
                                                IdentifierName("value")
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
    
    private PropertyDeclarationSyntax GetterAndSetter()
    {
        return PropertyDeclaration
        (
            Utils.GetTypeSyntax(_returnType),
            Identifier(_name)
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
                List
                (
                    new AccessorDeclarationSyntax[]
                    {
                        AccessorDeclaration
                        (
                            SyntaxKind.GetAccessorDeclaration
                        )
                        .WithBody
                        (
                            Block
                            (
                                IfStatement
                                (
                                    BinaryExpression
                                    (
                                        SyntaxKind.EqualsExpression,
                                        MemberAccessExpression
                                        (
                                            SyntaxKind.SimpleMemberAccessExpression,
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
                                            ),
                                            IdentifierName($"_getterOf{_name}")
                                        ),
                                        LiteralExpression
                                        (
                                            SyntaxKind.NullLiteralExpression
                                        )
                                    ),
                                    Block
                                    (
                                        SingletonList<StatementSyntax>
                                        (
                                            ThrowStatement
                                            (
                                                ObjectCreationExpression
                                                    (
                                                        IdentifierName("NotImplementedException")
                                                    )
                                                    .WithArgumentList
                                                    (
                                                        ArgumentList()
                                                    )
                                            )
                                        )
                                    )
                                ),
                                ReturnStatement
                                (
                                    InvocationExpression
                                    (
                                        MemberAccessExpression
                                        (
                                            SyntaxKind.SimpleMemberAccessExpression,
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
                                            ),
                                            IdentifierName($"_getterOf{_name}")
                                        )
                                    )
                                )
                            )
                        ),
                        AccessorDeclaration
                        (
                            SyntaxKind.SetAccessorDeclaration
                        )
                        .WithBody
                        (
                            Block
                            (
                                IfStatement
                                (
                                    BinaryExpression
                                    (
                                        SyntaxKind.EqualsExpression,
                                        MemberAccessExpression
                                        (
                                            SyntaxKind.SimpleMemberAccessExpression,
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
                                            ),
                                            IdentifierName($"_setterOf{_name}")
                                        ),
                                        LiteralExpression
                                        (
                                            SyntaxKind.NullLiteralExpression
                                        )
                                    ),
                                    Block
                                    (
                                        SingletonList<StatementSyntax>
                                        (
                                            ThrowStatement
                                            (
                                                ObjectCreationExpression
                                                (
                                                    IdentifierName("NotImplementedException")
                                                )
                                                .WithArgumentList
                                                (
                                                    ArgumentList()
                                                )
                                            )
                                        )
                                    )
                                ),
                                ExpressionStatement
                                (
                                    InvocationExpression
                                    (
                                        MemberAccessExpression
                                        (
                                            SyntaxKind.SimpleMemberAccessExpression,
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
                                            ),
                                            IdentifierName($"_setterOf{_name}")
                                        )
                                    )
                                    .WithArgumentList
                                    (
                                        ArgumentList
                                        (
                                            SingletonSeparatedList
                                            (
                                                Argument
                                                (
                                                    IdentifierName("value")
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    }
                )
            )
        );
    }
}