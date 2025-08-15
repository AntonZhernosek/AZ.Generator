namespace AZ.Generator.EntityFrameworkCore.Diagnostics;

internal static class EntitySetsDiagnostics
{
	private const string Category = "Entity sets";

	public static Diagnostic ShouldInheritDbContext(INamedTypeSymbol type) => Diagnostic.Create(
		descriptor: ShouldInheritDbContextDescriptor,
		location: type.Locations.FirstOrDefault(),
		messageArgs: type.Name);

	public static Diagnostic ShouldBePartial(INamedTypeSymbol type) => Diagnostic.Create(
		descriptor: ShouldBePartialDescriptor,
		location: type.Locations.FirstOrDefault(),
		messageArgs: type.Name);

	public static Diagnostic ShouldHaveEntities(INamedTypeSymbol type, INamespaceSymbol containingNamespace) => Diagnostic.Create(
		descriptor: ShouldHaveEntitiesDescriptor,
		location: type.Locations.FirstOrDefault(),
		messageArgs: containingNamespace.ToDisplayString());

	#region Descriptors

	private static readonly DiagnosticDescriptor ShouldInheritDbContextDescriptor = new(
		id: DiagnosticErrors.ShouldInheritDbContext,
		title: "Type should inherit from DbContext",
		messageFormat: "Class {0} is marked as implementing entity sets, but does not inherit from DbContext",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	private static readonly DiagnosticDescriptor ShouldBePartialDescriptor = new(
		id: DiagnosticErrors.ShouldBePartial,
		title: "Type should be partial",
		messageFormat: "Class {0} is marked as implementing entity sets, and should be partial",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	private static readonly DiagnosticDescriptor ShouldHaveEntitiesDescriptor = new(
		id: DiagnosticErrors.ShouldHaveEntities,
		title: "Containing namespace has no entities",
		messageFormat: "Containing namespace {0} has no entities to generate sets from",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	#endregion
}

file static class DiagnosticErrors
{
	private const string Prefix = "ES";

	public const string ShouldInheritDbContext = $"{Prefix}0001";
	public const string ShouldBePartial = $"{Prefix}0002";
	public const string ShouldHaveEntities = $"{Prefix}0003";
}
