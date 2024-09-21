using Microsoft.CodeAnalysis;

namespace Mockup.Analyzers.Visitors;

public interface IMethodSymbolVisitor<out T>
{
    void Begin();
    
    void OwnerType(ITypeSymbol typeSymbol);

    void Access(Accessibility access);
    void Abstract(bool isAbstract);
    void Virtual(bool isVirtual);
    void ReturnType(ITypeSymbol typeSymbol);
    void Name(string name);

    void BeginParams(int count);
    void Param(IParameterSymbol parameterSymbol);
    void EndParams();
    
    T? End();
}

public static class MethodSymbolVisitor
{
    public static T? Visit<T>(this IMethodSymbol methodSymbol, IMethodSymbolVisitor<T> visitor)
    {
        visitor.Begin();

        visitor.OwnerType(methodSymbol.ContainingType);
        
        visitor.Access(methodSymbol.DeclaredAccessibility);
        visitor.Abstract(methodSymbol.IsAbstract);
        visitor.Virtual(methodSymbol.IsVirtual);
        visitor.ReturnType(methodSymbol.ReturnType);
        visitor.Name(methodSymbol.Name);
        
        visitor.BeginParams(methodSymbol.Parameters.Length);
        foreach (var parameterSymbol in methodSymbol.Parameters)
        {
            visitor.Param(parameterSymbol);
        }
        visitor.EndParams();
        
        return visitor.End();
    }
}
