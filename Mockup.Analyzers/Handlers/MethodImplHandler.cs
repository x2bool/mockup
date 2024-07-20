using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mockup.Visitors;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mockup.Generators;

public class MethodImplHandler : IMethodSymbolVisitor<MemberDeclarationSyntax[]>
{
    private ITypeSymbol _typeSymbol;
    private ITypeSymbol _returnType;
    private string _name;
    private List<IParameterSymbol> _parameters;

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
        return MethodDeclaration
        (
            PredefinedType
            (
                Token(SyntaxKind.VoidKeyword)
            ),
            Identifier(_name)
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
    }

    private MethodDeclarationSyntax ReturnInvokeMethod()
    {
        return MethodDeclaration
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
        .WithParameterList
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
    }
}