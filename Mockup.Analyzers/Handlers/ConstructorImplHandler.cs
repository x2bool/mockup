using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mockup.Analyzers.Visitors;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mockup.Analyzers.Handlers;

public class ConstructorImplHandler : IMethodSymbolVisitor<MemberDeclarationSyntax[]>
{
    private readonly IConstructorImplStrategy _strategy;
    
    private ITypeSymbol _ownerTypeSymbol;
    private List<IParameterSymbol> _parameters;

    public ConstructorImplHandler(
        IConstructorImplStrategy strategy)
    {
        _strategy = strategy;
    }
    
    public void Begin()
    {
    }

    public void OwnerType(ITypeSymbol typeSymbol)
    {
        _ownerTypeSymbol = typeSymbol;
    }

    public void Access(Accessibility access)
    {
    }

    public void Abstract(bool isAbstract)
    {
    }

    public void Virtual(bool isVirtual)
    {
    }

    public void ReturnType(ITypeSymbol typeSymbol)
    {
    }

    public void Name(string name)
    {
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
            Constructor(),
        };
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
                SeparatedList<ParameterSyntax>
                (
                    Parameters()
                )
            )
        )
        .WithInitializer
        (
            ConstructorInitializer
            (
                SyntaxKind.BaseConstructorInitializer,
                ArgumentList
                (
                    SeparatedList<ArgumentSyntax>
                    (
                        Arguments()
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

    private SyntaxNodeOrToken[] Parameters()
    {
        var nodes = new List<SyntaxNodeOrToken>();

        nodes.Add(
            Parameter(
                Identifier("@var")
            )
            .WithType(
                IdentifierName(_strategy.GetTypeName(_ownerTypeSymbol))
            )
        );
        
        foreach (var parameter in _parameters)
        {
            nodes.Add(Token(SyntaxKind.CommaToken));
            nodes.Add(Parameter
                (
                    Identifier(parameter.Name)
                )
                .WithType
                (
                    Utils.GetTypeSyntax(parameter.Type)
                ));
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
}

public interface IConstructorImplStrategy
{
    string GetTypeName(ITypeSymbol typeSymbol);
}

public class ConstructorImplStrategy : IConstructorImplStrategy
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
