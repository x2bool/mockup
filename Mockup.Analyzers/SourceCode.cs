using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mockup.Analyzers;

internal class SourceCode
{
    public string HintName { get; }
    
    public CompilationUnitSyntax CompilationUnit { get; }
    
    public SourceCode(
        string hintName,
        CompilationUnitSyntax compilationUnit)
    {
        HintName = hintName;
        CompilationUnit = compilationUnit;
    }
}
