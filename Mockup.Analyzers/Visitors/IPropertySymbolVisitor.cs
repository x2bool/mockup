using Microsoft.CodeAnalysis;

namespace Mockup.Analyzers.Visitors;

public interface IPropertySymbolVisitor<out T>
{
    void Begin();

    void OwnerType(ITypeSymbol typeSymbol);
    
    void Abstract(bool isAbstract);
    void Virtual(bool isVirtual);
    void ReturnType(ITypeSymbol typeSymbol);
    void Name(string name);
    
    void Read(bool read);
    void Write(bool write);
    
    T? End();
}

public static class PropertySymbolVisitorExtensions
{
    public static T? Visit<T>(this IPropertySymbol propertySymbol, IPropertySymbolVisitor<T> visitor)
    {
        visitor.Begin();

        visitor.OwnerType(propertySymbol.ContainingType);
        
        visitor.Abstract(propertySymbol.IsAbstract);
        visitor.Virtual(propertySymbol.IsVirtual);
        visitor.ReturnType(propertySymbol.Type);
        visitor.Name(propertySymbol.Name);

        visitor.Read(!propertySymbol.IsWriteOnly);
        visitor.Write(!propertySymbol.IsReadOnly);

        return visitor.End();
    }
}
