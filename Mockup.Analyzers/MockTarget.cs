using Microsoft.CodeAnalysis;
using Mockup.Generators;
using Mockup.Visitors;

namespace Mockup;

internal class MockTarget
{
    public ITypeSymbol TypeSymbol { get; }

    public MockTarget(ITypeSymbol typeSymbol)
    {
        TypeSymbol = typeSymbol;
    }

    public string GetTypeName()
    {
        var typeName = TypeSymbol.Name;
        if (typeName.Length > 1)
        {
            var first = typeName[0];
            if (first == 'I')
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

    public string GetNamespace()
    {
        return Utils.GetFullName(TypeSymbol.ContainingNamespace);
    }

    public string GetHintName()
    {
        return $"{GetNamespace().Replace('.', '_')}_{GetTypeName()}";
    }

    public string GenerateSource()
    {
        var compilationUnit = TypeSymbol.Visit(new TargetInterfaceBuilderHandler(
            new TargetInterfaceBuilderStrategy()));
        
        if (compilationUnit != null)
        {
            return compilationUnit.NormalizeWhitespace().ToFullString();
        }

        return "";

//         return $@"
// using System;
//
// namespace {GetNamespace()} {{
//     public class {GetTypeName()} {{
//     }}
// }}
// ";
    }
}
