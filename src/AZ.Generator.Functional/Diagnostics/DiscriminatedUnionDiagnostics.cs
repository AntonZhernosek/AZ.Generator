namespace AZ.Generator.Functional.Diagnostics;

internal static class DiscriminatedUnionDiagnostics
{
    private const string Category = "DiscriminatedUnion";

	#region Descriptors

	public static readonly DiagnosticDescriptor ShouldBeAbstractDescriptor = new(
        id: DiagnosticErrors.NotAbstract,
        title: "Type is not abstract",
        messageFormat: "Class {0} is marked as a discriminated union, but is not abstract",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor HasNoImplementationsDescriptor = new(
	    id: DiagnosticErrors.HasNoImplementations,
	    title: "Type has no implementations",
	    messageFormat: "Class {0} is marked as a discriminated union, but has no implementations",
	    category: Category,
	    defaultSeverity: DiagnosticSeverity.Error,
	    isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor ShouldBePartialDescriptor = new(
        id: DiagnosticErrors.ShouldBePartial,
        title: "Type is not partial",
        messageFormat: "Class {0} is marked as a discriminated union, and should be partial",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor ShouldBePartialContainingTypeDescriptor = new(
		id: DiagnosticErrors.ShouldBePartialContainingType,
		title: "Containing type is not partial",
		messageFormat: "Private class {0} is marked as a discriminated union inside of a containing, and all containing types should be partial",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor ShouldShareContainingTypeDescriptor = new(
		id: DiagnosticErrors.ShouldShareContainingType,
		title: "Implementation must have the same containing type",
		messageFormat: "Nested class {0} is a discriminated union implementation, and should be within the same containing type as its base type {1}",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor ShouldShareContainingNamespaceDescriptor = new(
		id: DiagnosticErrors.ShouldShareContainingNamespace,
		title: "Implementation must have the same containing namespace",
		messageFormat: "Class {0} is a discriminated union implementation, and should be within the same containing namespace as its base type {1}",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	#endregion

	#region Diagnostics

	public static Diagnostic ShouldBeAbstract(ITypeSymbol type) => Diagnostic.Create(
        descriptor: ShouldBeAbstractDescriptor,
        location: type.Locations.FirstOrDefault(),
        messageArgs: type.Name);

	public static Diagnostic HasNoImplementations(ITypeSymbol type) => Diagnostic.Create(
	    descriptor: HasNoImplementationsDescriptor,
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

	public static Diagnostic ShouldShareContainingType(ITypeSymbol implementation, ITypeSymbol baseType) => Diagnostic.Create(
		descriptor: ShouldShareContainingTypeDescriptor,
		location: implementation.Locations.FirstOrDefault(),
		messageArgs: [implementation.Name, baseType.Name]);

	public static Diagnostic ShouldShareContainingNamespace(ITypeSymbol implementation, ITypeSymbol baseType) => Diagnostic.Create(
		descriptor: ShouldShareContainingNamespaceDescriptor,
		location: implementation.Locations.FirstOrDefault(),
		messageArgs: [implementation.Name, baseType.Name]);

	#endregion
}

file static class DiagnosticErrors
{
    private const string Prefix = "DUN";

    public const string NotAbstract = $"{Prefix}0001";
    public const string ShouldBePartial = $"{Prefix}0002";
    public const string ShouldBePartialContainingType = $"{Prefix}0003";
    public const string HasNoImplementations = $"{Prefix}0004";
    public const string ShouldShareContainingType = $"{Prefix}0005";
    public const string ShouldShareContainingNamespace = $"{Prefix}0006";
}
