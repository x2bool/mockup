using Microsoft.CodeAnalysis;

namespace Mockup.Visitors;

public interface IPropertySymbolVisitor<out T>
{
    void Begin();

    void OwnerType(ITypeSymbol typeSymbol);
    
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
        
        visitor.ReturnType(propertySymbol.Type);
        visitor.Name(propertySymbol.Name);

        visitor.Read(!propertySymbol.IsWriteOnly);
        visitor.Write(!propertySymbol.IsReadOnly);

        return visitor.End();
    }
}
