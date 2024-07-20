using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mockup;

public static class Utils
{
    public static string GetFullName(ISymbol? symbol) 
    {
        if (symbol == null || IsRootNamespace(symbol))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(symbol.MetadataName);
        var current = symbol;

        symbol = symbol.ContainingSymbol;

        while (!IsRootNamespace(symbol))
        {
            if (symbol is ITypeSymbol && current is ITypeSymbol)
            {
                builder.Insert(0, '.');
            }
            else
            {
                builder.Insert(0, '.');
            }

            builder.Insert(0, symbol.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            
            symbol = symbol.ContainingSymbol;
        }

        return builder.ToString();
    }

    public static bool IsRootNamespace(ISymbol symbol) 
    {
        INamespaceSymbol? s = null;
        return ((s = symbol as INamespaceSymbol) != null) && s.IsGlobalNamespace;
    }

    public static bool IsEqualSymbol(ISymbol? x, ISymbol? y)
    {
        return SymbolEqualityComparer.Default.Equals(x, y);
    }

    public static NameSyntax GetQualifiedNameSyntax(ITypeSymbol typeSymbol)
    {
        var parts = GetFullName(typeSymbol).Split('.');

        NameSyntax name = AliasQualifiedName(
            IdentifierName
            (
                Token(SyntaxKind.GlobalKeyword)
            ),
            IdentifierName(parts[0])
        );
        for (int i = 1; i < parts.Length; i++)
        {
            name = QualifiedName(name, IdentifierName(parts[i]));
        }

        return name;
    }

    public static TypeSyntax GetArrayTypeSyntax(IArrayTypeSymbol typeSymbol)
    {
        return ArrayType(
            GetTypeSyntax(typeSymbol.ElementType)
        )
        .WithRankSpecifiers(
            List<ArrayRankSpecifierSyntax>(
                Enumerable.Range(0, typeSymbol.Rank)
                    .Select(_ => ArrayRankSpecifier(
                        SingletonSeparatedList<ExpressionSyntax>(
                            OmittedArraySizeExpression()
                        )
                    ))
            )
        );
    }

    public static TypeSyntax GetComplexTypeSyntax(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            return GetArrayTypeSyntax(arrayTypeSymbol);

        return GetQualifiedNameSyntax(typeSymbol);
    }
    
    public static TypeSyntax GetTypeSyntax(ITypeSymbol typeSymbol)
    {
        return typeSymbol.SpecialType switch
        {
            // SpecialType.None => expr,
            SpecialType.System_Object => PredefinedType(Token(SyntaxKind.ObjectKeyword)),
            // SpecialType.System_Enum => expr,
            // SpecialType.System_MulticastDelegate => expr,
            // SpecialType.System_Delegate => expr,
            // SpecialType.System_ValueType => expr,
            SpecialType.System_Void => PredefinedType(Token(SyntaxKind.VoidKeyword)),
            SpecialType.System_Boolean => PredefinedType(Token(SyntaxKind.BoolKeyword)),
            SpecialType.System_Char => PredefinedType(Token(SyntaxKind.CharKeyword)),
            SpecialType.System_SByte => PredefinedType(Token(SyntaxKind.SByteKeyword)),
            SpecialType.System_Byte => PredefinedType(Token(SyntaxKind.ByteKeyword)),
            SpecialType.System_Int16 => PredefinedType(Token(SyntaxKind.ShortKeyword)),
            SpecialType.System_UInt16 => PredefinedType(Token(SyntaxKind.UShortKeyword)),
            SpecialType.System_Int32 => PredefinedType(Token(SyntaxKind.IntKeyword)),
            SpecialType.System_UInt32 => PredefinedType(Token(SyntaxKind.UIntKeyword)),
            SpecialType.System_Int64 => PredefinedType(Token(SyntaxKind.LongKeyword)),
            SpecialType.System_UInt64 => PredefinedType(Token(SyntaxKind.ULongKeyword)),
            SpecialType.System_Decimal => PredefinedType(Token(SyntaxKind.DecimalKeyword)),
            SpecialType.System_Single => PredefinedType(Token(SyntaxKind.FloatKeyword)),
            SpecialType.System_Double => PredefinedType(Token(SyntaxKind.DoubleKeyword)),
            SpecialType.System_String => PredefinedType(Token(SyntaxKind.StringKeyword)),
            // SpecialType.System_IntPtr => expr,
            // SpecialType.System_UIntPtr => expr,
            // SpecialType.System_Array => GetArrayTypeSyntax((IArrayTypeSymbol)typeSymbol),
            // SpecialType.System_Collections_IEnumerable => expr,
            // SpecialType.System_Collections_Generic_IEnumerable_T => expr,
            // SpecialType.System_Collections_Generic_IList_T => expr,
            // SpecialType.System_Collections_Generic_ICollection_T => expr,
            // SpecialType.System_Collections_IEnumerator => expr,
            // SpecialType.System_Collections_Generic_IEnumerator_T => expr,
            // SpecialType.System_Collections_Generic_IReadOnlyList_T => expr,
            // SpecialType.System_Collections_Generic_IReadOnlyCollection_T => expr,
            // SpecialType.System_Nullable_T => expr,
            // SpecialType.System_DateTime => expr,
            // SpecialType.System_Runtime_CompilerServices_IsVolatile => expr,
            // SpecialType.System_IDisposable => expr,
            // SpecialType.System_TypedReference => expr,
            // SpecialType.System_ArgIterator => expr,
            // SpecialType.System_RuntimeArgumentHandle => expr,
            // SpecialType.System_RuntimeFieldHandle => expr,
            // SpecialType.System_RuntimeMethodHandle => expr,
            // SpecialType.System_RuntimeTypeHandle => expr,
            // SpecialType.System_IAsyncResult => expr,
            // SpecialType.System_AsyncCallback => expr,
            // SpecialType.System_Runtime_CompilerServices_RuntimeFeature => expr,
            // SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute => expr,
            _ => GetComplexTypeSyntax(typeSymbol),
        };
    }
}