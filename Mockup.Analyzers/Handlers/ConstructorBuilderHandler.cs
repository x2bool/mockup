using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mockup.Analyzers.Visitors;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mockup.Analyzers.Handlers;

public class ConstructorBuilderHandler : IMethodSymbolVisitor<MemberDeclarationSyntax[]>
{
    private readonly IConstructorBuilderStrategy _strategy;
    
    private ITypeSymbol _ownerTypeSymbol;
    private List<IParameterSymbol> _parameters;

    public ConstructorBuilderHandler(
        IConstructorBuilderStrategy strategy)
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
            BuilderMethod(),
        };
    }
    
    private MethodDeclarationSyntax BuilderMethod()
    {
        return MethodDeclaration
        (
            IdentifierName(_ownerTypeSymbol.Name),
            Identifier("Build") // TODO: make configurable
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
                                SeparatedList<ArgumentSyntax>
                                (
                                    Arguments()
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

        nodes.Add(
            Argument(ThisExpression())
        );
        
        foreach (var parameter in _parameters)
        {
            nodes.Add(Token(SyntaxKind.CommaToken));
            nodes.Add(Argument
            (
                IdentifierName(parameter.Name)
            ));
        }
        
        return nodes.ToArray();
    }
}

public interface IConstructorBuilderStrategy
{
    string GetTypeName(ITypeSymbol typeSymbol);
}

public class ConstructorBuilderStrategy : IConstructorBuilderStrategy
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
