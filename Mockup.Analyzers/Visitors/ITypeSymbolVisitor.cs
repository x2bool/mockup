using Microsoft.CodeAnalysis;

namespace Mockup.Analyzers.Visitors;

public interface ITypeSymbolVisitor<out T>
{
    void Begin();
    
    void Namespace(INamespaceSymbol namespaceSymbol);
    void Type(ITypeSymbol typeSymbol);
    
    void MembersBegin(int count);
    void Member(ISymbol symbol);
    void MembersEnd();
    
    T? End();
}

public static class TypeSymbolVisitorExtensions
{
    public static T? Visit<T>(this ITypeSymbol typeSymbol, ITypeSymbolVisitor<T> visitor)
    {   
        visitor.Begin();

        visitor.Namespace(typeSymbol.ContainingNamespace);
        visitor.Type(typeSymbol);

        var members = typeSymbol.GetMembers();
        visitor.MembersBegin(members.Length);
        foreach (var member in members)
        {
            visitor.Member(member);
        }
        visitor.MembersEnd();
        
        return visitor.End();
    }
}
