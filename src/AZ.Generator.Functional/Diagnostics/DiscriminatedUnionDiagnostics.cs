namespace AZ.Generator.Functional.Diagnostics;

internal static class DiscriminatedUnionDiagnostics
{
    private const string Category = "Discriminated Union";

	#region Descriptors

	private static readonly DiagnosticDescriptor ShouldBeAbstractDescriptor = new(
        id: DiagnosticErrors.ShouldBeAbstract,
        title: "Type is not abstract",
        messageFormat: "Class {0} is marked as a discriminated union, but is not abstract",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

	private static readonly DiagnosticDescriptor ShouldHaveImplementationsDescriptor = new(
	    id: DiagnosticErrors.ShouldHaveImplementations,
	    title: "Type has no implementations",
	    messageFormat: "Class {0} is marked as a discriminated union, but has no implementations",
	    category: Category,
	    defaultSeverity: DiagnosticSeverity.Error,
	    isEnabledByDefault: true);

	private static readonly DiagnosticDescriptor ShouldBePartialDescriptor = new(
        id: DiagnosticErrors.ShouldBePartial,
        title: "Type is not partial",
        messageFormat: "Private class {0} is marked as a discriminated union, and should be partial",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

	private static readonly DiagnosticDescriptor ShouldBePartialContainingTypeDescriptor = new(
		id: DiagnosticErrors.ShouldBePartialContainingType,
		title: "Containing type is not partial",
		messageFormat: "Private class {0} is marked as a discriminated union inside of a containing type, and all containing types should be partial",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	#endregion

	#region Diagnostics

	public static Diagnostic ShouldBeAbstract(ITypeSymbol type) => Diagnostic.Create(
        descriptor: ShouldBeAbstractDescriptor,
        location: type.Locations.FirstOrDefault(),
        messageArgs: type.Name);

	public static Diagnostic ShouldHaveImplementations(ITypeSymbol type) => Diagnostic.Create(
	    descriptor: ShouldHaveImplementationsDescriptor,
	    location: type.Locations.FirstOrDefault(),
	    messageArgs: type.Name);

	public static Diagnostic ShouldBePartial(ITypeSymbol type) => Diagnostic.Create(
        descriptor: ShouldBePartialDescriptor,
        location: type.Locations.FirstOrDefault(),
        messageArgs: type.Name);

	public static Diagnostic ShouldBePartialContainingType(ITypeSymbol type) => Diagnostic.Create(
		descriptor: ShouldBePartialContainingTypeDescriptor,
		location: type.Locations.FirstOrDefault(),
		messageArgs: type.Name);

	#endregion
}

file static class DiagnosticErrors
{
    private const string Prefix = "DU";

    public const string ShouldBeAbstract = $"{Prefix}0001";
	public const string ShouldHaveImplementations = $"{Prefix}0002";
	public const string ShouldBePartial = $"{Prefix}0003";
    public const string ShouldBePartialContainingType = $"{Prefix}0004";
}
