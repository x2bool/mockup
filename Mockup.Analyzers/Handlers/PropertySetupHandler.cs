using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mockup.Analyzers.Visitors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mockup.Analyzers.Handlers;

public class PropertySetupHandler : IPropertySymbolVisitor<MemberDeclarationSyntax[]>
{
    private readonly IPropertySetupStrategy _strategy;
    
    private ITypeSymbol _ownerTypeSymbol;
    private bool _isAbstract;
    private bool _isVirtual;
    private ITypeSymbol _returnTypeSymbol;
    
    private string? _name;
    private bool _read;
    private bool _write;

    public PropertySetupHandler(IPropertySetupStrategy strategy)
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
        var members = new List<MemberDeclarationSyntax>();

        if (_read)
        {
            members.Add(GetterBackingField());
            members.Add(Getter());
        }

        if (_write)
        {
            members.Add(SetterBackingField());
            members.Add(Setter());
        }

        if (_read && _write)
        {
            members.Add(GetterAndSetter());
        }

        return members.ToArray();
    }

    private FieldDeclarationSyntax GetterBackingField()
    {
        return FieldDeclaration
            (
                VariableDeclaration
                (
                    GenericName
                    (
                        Identifier("Func")
                    )
                    .WithTypeArgumentList
                    (
                        TypeArgumentList
                        (
                            SingletonSeparatedList
                            (
                                Utils.GetTypeSyntax(_returnTypeSymbol)
                            )
                        )
                    )
                )
                .WithVariables
                (
                    SingletonSeparatedList
                    (
                        VariableDeclarator
                        (
                            Identifier($"_getterOf{_name}")
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

    private MethodDeclarationSyntax Getter()
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
                                Identifier("getter")
                            )
                            .WithType
                            (
                                GenericName
                                    (
                                        Identifier("Func")
                                    )
                                    .WithTypeArgumentList
                                    (
                                        TypeArgumentList
                                        (
                                            SingletonSeparatedList
                                            (
                                                Utils.GetTypeSyntax(_returnTypeSymbol)
                                            )
                                        )
                                    )
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
                            IdentifierName($"_getterOf{_name}"),
                            IdentifierName("getter")
                        )
                    ),
                    ReturnStatement
                    (
                        ThisExpression()
                    )
                )
            );
    }

    private FieldDeclarationSyntax SetterBackingField()
    {
        return FieldDeclaration
            (
                VariableDeclaration
                (
                    GenericName
                    (
                        Identifier("Action")
                    )
                    .WithTypeArgumentList
                    (
                        TypeArgumentList
                        (
                            SingletonSeparatedList
                            (
                                Utils.GetTypeSyntax(_returnTypeSymbol)
                            )
                        )
                    )
                )
                .WithVariables
                (
                    SingletonSeparatedList<VariableDeclaratorSyntax>
                    (
                        VariableDeclarator
                        (
                            Identifier($"_setterOf{_name}")
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

    private MethodDeclarationSyntax Setter()
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
                                Identifier("setter")
                            )
                            .WithType
                            (
                                GenericName
                                    (
                                        Identifier("Action")
                                    )
                                    .WithTypeArgumentList
                                    (
                                        TypeArgumentList
                                        (
                                            SingletonSeparatedList
                                            (
                                                Utils.GetTypeSyntax(_returnTypeSymbol)
                                            )
                                        )
                                    )
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
                            IdentifierName($"_setterOf{_name}"),
                            IdentifierName("setter")
                        )
                    ),
                    ReturnStatement
                    (
                        ThisExpression()
                    )
                )
            );
    }

    private MethodDeclarationSyntax GetterAndSetter()
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
                    SeparatedList<ParameterSyntax>
                    (
                        new SyntaxNodeOrToken[]
                        {
                            Parameter
                            (
                                Identifier("getter")
                            )
                            .WithType
                            (
                                GenericName
                                (
                                    Identifier("Func")
                                )
                                .WithTypeArgumentList
                                (
                                    TypeArgumentList
                                    (
                                        SingletonSeparatedList
                                        (
                                            Utils.GetTypeSyntax(_returnTypeSymbol)
                                        )
                                    )
                                )
                            ),
                            Token(SyntaxKind.CommaToken),
                            Parameter
                            (
                                Identifier("setter")
                            )
                            .WithType
                            (
                                GenericName
                                (
                                    Identifier("Action")
                                )
                                .WithTypeArgumentList
                                (
                                    TypeArgumentList
                                    (
                                        SingletonSeparatedList
                                        (
                                            Utils.GetTypeSyntax(_returnTypeSymbol)
                                        )
                                    )
                                )
                            )
                        }
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
                            IdentifierName($"_getterOf{_name}"),
                            IdentifierName("getter")
                        )
                    ),
                    ExpressionStatement
                    (
                        AssignmentExpression
                        (
                            SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName($"_setterOf{_name}"),
                            IdentifierName("setter")
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

public interface IPropertySetupStrategy
{
    string GetTypeName(ITypeSymbol typeSymbol);
}

public class PropertySetupStrategy : IPropertySetupStrategy
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