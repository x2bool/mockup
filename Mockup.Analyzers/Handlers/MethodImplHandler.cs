using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mockup.Analyzers.Visitors;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mockup.Analyzers.Handlers;

public class MethodImplHandler : IMethodSymbolVisitor<MemberDeclarationSyntax[]>
{
    private ITypeSymbol _ownerType;
    private bool _isAbstract;
    private bool _isVirtual;
    private ITypeSymbol _returnType;
    private string _name;
    private List<IParameterSymbol> _parameters;
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

    public void BeginParams(int count)
    {
        _parameters = new List<IParameterSymbol>(count);
    }

    public void Param(IParameterSymbol parameterSymbol)
    {
        _parameters.Add(parameterSymbol);
    }

    public void EndParams()
    {
    }

    public MemberDeclarationSyntax[]? End()
    {
        return new MemberDeclarationSyntax[]
        {
            _returnType.SpecialType == SpecialType.System_Void
                ? InvokeMethod()
                : ReturnInvokeMethod()
        };
    }

    private SyntaxNodeOrToken[] Parameters()
    {
        var nodes = new List<SyntaxNodeOrToken>();

        foreach (var parameter in _parameters)
        {
            nodes.Add(Parameter
                (
                    Identifier(parameter.Name)
                )
                .WithType
                (
                    Utils.GetTypeSyntax(parameter.Type)
                ));
            nodes.Add(Token(SyntaxKind.CommaToken));
        }

        if (nodes.Count > 0)
        {
            nodes.RemoveAt(nodes.Count - 1);
        }
        
        return nodes.ToArray();
    }

    private SyntaxNodeOrToken[] Arguments()
    {
        var nodes = new List<SyntaxNodeOrToken>();

        foreach (var parameter in _parameters)
        {
            nodes.Add(Argument
            (
                IdentifierName(parameter.Name)
            ));
            nodes.Add(Token(SyntaxKind.CommaToken));
        }

        if (nodes.Count > 0)
        {
            nodes.RemoveAt(nodes.Count - 1);
        }
        
        return nodes.ToArray();
    }

    private MethodDeclarationSyntax InvokeMethod()
    {
        var method = MethodDeclaration
        (
            PredefinedType
            (
                Token(SyntaxKind.VoidKeyword)
            ),
            Identifier(_name)
        );

        if (_ownerType.TypeKind == TypeKind.Interface)
        {
            method = method.WithExplicitInterfaceSpecifier
            (
                ExplicitInterfaceSpecifier
                (
                    IdentifierName(_ownerType.Name)
                )
            );
        }
        else
        {
            method = method.WithModifiers(Modifiers());
        }
            
        method = method.WithParameterList
        (
            ParameterList
            (
                SeparatedList<ParameterSyntax>
                (
                    Parameters()
                )
            )
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
                            IdentifierName($"_implOf{_name}")
                        ),
                        LiteralExpression
                        (
                            SyntaxKind.NullLiteralExpression
                        )
                    ),
                    FallbackBlock()
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
                                IdentifierName($"_implOf{_name}")
                            )
                        )
                        .WithArgumentList
                        (
                            ArgumentList
                            (
                                SeparatedList<ArgumentSyntax>
                                (
                                    Arguments()
                                )
                            )
                        )
                )
            )
        );

        return method;
    }

    private MethodDeclarationSyntax ReturnInvokeMethod()
    {
        var method = MethodDeclaration
        (
            Utils.GetTypeSyntax(_returnType),
            Identifier(_name)
        );

        if (_ownerType.TypeKind == TypeKind.Interface)
        {
            method = method.WithExplicitInterfaceSpecifier
            (
                ExplicitInterfaceSpecifier
                (
                    IdentifierName(_ownerType.Name)
                )
            );
        }
        else
        {
            method = method.WithModifiers(Modifiers());
        }

        method = method.WithParameterList
        (
            ParameterList
            (
                SeparatedList<ParameterSyntax>
                (
                    Parameters()
                )
            )
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
                            IdentifierName($"_implOf{_name}")
                        ),
                        LiteralExpression
                        (
                            SyntaxKind.NullLiteralExpression
                        )
                    ),
                    FallbackBlock()
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
                            IdentifierName($"_implOf{_name}")
                        )
                    )
                    .WithArgumentList
                    (
                        ArgumentList
                        (
                            SeparatedList<ArgumentSyntax>
                            (
                                Arguments()
                            )
                        )
                    )
                )
            )
        );

        return method;
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

    private BlockSyntax FallbackBlock()
    {
        if (_isVirtual)
        {
            if (_returnType.SpecialType == SpecialType.System_Void)
            {
                return FallbackCallBlock();
            }

            return FallbackReturnCallBlock();
        }
        
        // if (_ownerType.TypeKind == TypeKind.Interface)
        // {
        //     return FallbackThrowBlock();
        // }
        //
        // if (_isAbstract)
        // {
        //     return FallbackThrowBlock();
        // }

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

    private BlockSyntax FallbackCallBlock()
    {
        return Block
        (
            ExpressionStatement
            (
                InvocationExpression
                (
                    MemberAccessExpression
                    (
                        SyntaxKind.SimpleMemberAccessExpression,
                        BaseExpression(),
                        IdentifierName(_name)
                    )
                )
                .WithArgumentList
                (
                    ArgumentList
                    (
                        SeparatedList<ArgumentSyntax>
                        (
                            Arguments()
                        )
                    )
                )
            ),
            ReturnStatement()
        );
    }

    private BlockSyntax FallbackReturnCallBlock()
    {
        return Block
        (
            SingletonList<StatementSyntax>
            (
                ReturnStatement
                (
                    InvocationExpression
                    (
                        MemberAccessExpression
                        (
                            SyntaxKind.SimpleMemberAccessExpression,
                            BaseExpression(),
                            IdentifierName(_name)
                        )
                    )
                    .WithArgumentList
                    (
                        ArgumentList
                        (
                            SeparatedList<ArgumentSyntax>
                            (
                                Arguments()
                            )
                        )
                    )
                )
            )
        );
    }
}