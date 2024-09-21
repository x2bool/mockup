using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mockup.Analyzers.Visitors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mockup.Analyzers.Handlers;

public class PropertyImplHandler : IPropertySymbolVisitor<MemberDeclarationSyntax[]>
{
    private ITypeSymbol _ownerType;
    private bool _isAbstract;
    private bool _isVirtual;
    private ITypeSymbol _returnType;
    private string _name;
    private bool _read;
    private bool _write;
    private Accessibility _access;

    public void Begin()
    {
    }

    public void OwnerType(ITypeSymbol typeSymbol)
    {
        _ownerType = typeSymbol;
    }

    public void Access(Accessibility access)
    {
        _access = access;
    }

    public void Abstract(bool isAbstract)
    {
        _isAbstract = isAbstract;
    }

    public void Virtual(bool isVirtual)
    {
        _isVirtual = isVirtual;
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
        var prop = PropertyDeclaration
        (
            Utils.GetTypeSyntax(_returnType),
            Identifier(_name)
        );

        if (_ownerType.TypeKind == TypeKind.Interface)
        {
            prop = prop.WithExplicitInterfaceSpecifier
            (
                ExplicitInterfaceSpecifier
                (
                    IdentifierName(_ownerType.Name)
                )
            );
        }
        else
        {
            prop = prop.WithModifiers(Modifiers());
        }
        
        prop = prop.WithAccessorList
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
                                    FallbackGetBlock()
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

        return prop;
    }
    
    private PropertyDeclarationSyntax Setter()
    {
        var prop = PropertyDeclaration
        (
            Utils.GetTypeSyntax(_returnType),
            Identifier(_name)
        );

        if (_ownerType.TypeKind == TypeKind.Interface)
        {
            prop = prop.WithExplicitInterfaceSpecifier
            (
                ExplicitInterfaceSpecifier
                (
                    IdentifierName(_ownerType.Name)
                )
            );
        }
        else
        {
            prop = prop.WithModifiers(Modifiers());
        }
            
        prop = prop.WithAccessorList
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
                                FallbackSetBlock()
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

        return prop;
    }
    
    private PropertyDeclarationSyntax GetterAndSetter()
    {
        var prop = PropertyDeclaration
        (
            Utils.GetTypeSyntax(_returnType),
            Identifier(_name)
        );

        if (_ownerType.TypeKind == TypeKind.Interface)
        {
            prop = prop.WithExplicitInterfaceSpecifier
            (
                ExplicitInterfaceSpecifier
                (
                    IdentifierName(_ownerType.Name)
                )
            );
        }
        else
        {
            prop = prop.WithModifiers(Modifiers());
        }
            
        prop = prop.WithAccessorList
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
                                    FallbackGetBlock()
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
                                    FallbackSetBlock()
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

        return prop;
    }

    private SyntaxTokenList Modifiers()
    {
        return _access switch // never private
        {
            Accessibility.Public => TokenList(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.OverrideKeyword)),
            Accessibility.Protected => TokenList(
                Token(SyntaxKind.ProtectedKeyword),
                Token(SyntaxKind.OverrideKeyword)),
            Accessibility.Internal => TokenList(
                Token(SyntaxKind.InternalKeyword),
                Token(SyntaxKind.OverrideKeyword)),
            Accessibility.ProtectedOrInternal => TokenList(
                Token(SyntaxKind.ProtectedKeyword),
                Token(SyntaxKind.InternalKeyword),
                Token(SyntaxKind.OverrideKeyword)),
            _ => throw new ArgumentException("Unsupported method accessibility")
        };
    }
    
    private BlockSyntax FallbackGetBlock()
    {
        if (_isVirtual)
        {
            return Block
            (
                SingletonList<StatementSyntax>
                (
                    ReturnStatement
                    (
                        MemberAccessExpression
                        (
                            SyntaxKind.SimpleMemberAccessExpression,
                            BaseExpression(),
                            IdentifierName(_name)
                        )
                    )
                )
            );
        }

        return FallbackThrowBlock();
    }

    private BlockSyntax FallbackSetBlock()
    {
        if (_isVirtual)
        {
            return Block
            (
                ExpressionStatement
                (
                    AssignmentExpression
                    (
                        SyntaxKind.SimpleAssignmentExpression,
                        MemberAccessExpression
                        (
                            SyntaxKind.SimpleMemberAccessExpression,
                            BaseExpression(),
                            IdentifierName(_name)
                        ),
                        IdentifierName("value")
                    )
                ),
                ReturnStatement()
            );
        }
        
        return FallbackThrowBlock();
    }
    
    private BlockSyntax FallbackThrowBlock()
    {
        return Block
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
        );
    }
}