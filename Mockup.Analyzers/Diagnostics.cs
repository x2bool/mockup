using Microsoft.CodeAnalysis;

namespace Mockup.Analyzers;

internal static class Diagnostics
{
    public static readonly DiagnosticDescriptor InvalidTarget = new(
        "MOCK0001",
        "Not a valid target for mocking",
        "Could not generate mock for '{0}', target type must be an interface or a non-sealed class",
        "Mockup.Analyzers",
        DiagnosticSeverity.Error,
        true
    );
}
