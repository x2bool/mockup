using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mockup.Analyzers.Visitors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mockup.Analyzers.Handlers;

public class MethodSetupHandler : IMethodSymbolVisitor<MemberDeclarationSyntax[]>
{
    private readonly IMethodSetupStrategy _strategy;
    
    private ITypeSymbol _ownerTypeSymbol;
    private bool _isAbstract;
    private bool _isVirtual;
    private ITypeSymbol _returnTypeSymbol;
    private string _name;
    private List<IParameterSymbol> _parameters;

    public MethodSetupHandler(IMethodSetupStrategy strategy)
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
        _isAbstract = isAbstract;
    }

    public void Virtual(bool isVirtual)
    {
        _isVirtual = isVirtual;
    }

    public void ReturnType(ITypeSymbol typeSymbol)
    {
        _returnTypeSymbol = typeSymbol;
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
        var members = new List<MemberDeclarationSyntax>();

        members.Add(BackingField());
        members.Add(Delegate());
        
        return members.ToArray();
    }

    private SyntaxNodeOrToken[] FuncParams()
    {
        var nodes = new List<SyntaxNodeOrToken>(_parameters.Count * 2);
        
        foreach (var parameter in _parameters)
        {
            nodes.Add(Utils.GetTypeSyntax(parameter.Type));
            nodes.Add(Token(SyntaxKind.CommaToken));
        }
        
        nodes.Add(Utils.GetTypeSyntax(_returnTypeSymbol));

        return nodes.ToArray();
    }

    private SyntaxNodeOrToken[] ActionParams()
    {
        var nodes = new List<SyntaxNodeOrToken>(_parameters.Count * 2);
        
        foreach (var parameter in _parameters)
        {
            nodes.Add(Utils.GetTypeSyntax(parameter.Type));
            nodes.Add(Token(SyntaxKind.CommaToken));
        }

        if (nodes.Count > 0)
        {
            nodes.RemoveAt(nodes.Count - 1);
        }
        
        return nodes.ToArray();
    }

    private NameSyntax FuncSyntax()
    {
        return GenericName
            (
                Identifier("Func")
            )
            .WithTypeArgumentList
            (
                TypeArgumentList
                (
                    SeparatedList<TypeSyntax>
                    (
                        FuncParams()
                    )
                )
            );
    }

    private NameSyntax ActionSyntax()
    {
        if (_parameters.Count == 0)
        {
            return IdentifierName("Action");
        }
        
        return GenericName
            (
                Identifier("Action")
            )
            .WithTypeArgumentList
            (
                TypeArgumentList
                (
                    SeparatedList<TypeSyntax>
                    (
                        ActionParams()
                    )
                )
            );
    }
    
    private FieldDeclarationSyntax BackingField()
    {
        return FieldDeclaration
        (
            VariableDeclaration
            (
                _returnTypeSymbol.SpecialType == SpecialType.System_Void
                    ? ActionSyntax()
                    : FuncSyntax()
            )
            .WithVariables
            (
                SingletonSeparatedList
                (
                    VariableDeclarator
                    (
                        Identifier($"_implOf{_name}")
                    )
                )
            )
        )
        .WithModifiers
        (
            TokenList
            (
                Token(SyntaxKind.PrivateKeyword)
            )
        );
    }
    
    public MethodDeclarationSyntax Delegate()
    {
        return MethodDeclaration
        (
            IdentifierName(_strategy.GetTypeName(_ownerTypeSymbol)),
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
                SingletonSeparatedList
                (
                    Parameter
                    (
                        Identifier("impl")
                    )
                    .WithType
                    (
                        _returnTypeSymbol.SpecialType == SpecialType.System_Void
                            ? ActionSyntax()
                            : FuncSyntax()
                    )
                )
            )
        )
        .WithBody
        (
            Block
            (
                ExpressionStatement
                (
                    AssignmentExpression
                    (
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName($"_implOf{_name}"),
                        IdentifierName("impl")
                    )
                ),
                ReturnStatement
                (
                    ThisExpression()
                )
            )
        );
    }
}

public interface IMethodSetupStrategy
{
    string GetTypeName(ITypeSymbol typeSymbol);
}

public class MethodSetupStrategy : IMethodSetupStrategy
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
