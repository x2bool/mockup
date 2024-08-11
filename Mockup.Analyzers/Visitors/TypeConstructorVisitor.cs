using Microsoft.CodeAnalysis;

namespace Mockup.Analyzers.Visitors;

public static class TypeConstructorVisitor
{
    public static T? Visit<T>(this ITypeSymbol typeSymbol, IMethodSymbolVisitor<T> visitor)
    {
        visitor.Begin();

        visitor.OwnerType(typeSymbol);
        
        visitor.Abstract(false);
        visitor.Virtual(false);
        visitor.ReturnType(typeSymbol);
        visitor.Name(typeSymbol.Name);
        
        visitor.BeginParams(0);
        // foreach (var parameterSymbol in methodSymbol.Parameters)
        // {
        //     visitor.Param(parameterSymbol);
        // }
        visitor.EndParams();
        
        return visitor.End();
    }
}
