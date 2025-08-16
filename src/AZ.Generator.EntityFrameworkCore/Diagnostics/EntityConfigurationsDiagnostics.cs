namespace AZ.Generator.EntityFrameworkCore.Diagnostics;

internal static class EntityConfigurationsDiagnostics
{
	private const string Category = "Entity configurations";

	public static Diagnostic ShouldInheritDbContext(INamedTypeSymbol type) => Diagnostic.Create(
		descriptor: ShouldInheritDbContextDescriptor,
		location: type.Locations.FirstOrDefault(),
		messageArgs: type.Name);

	public static Diagnostic ShouldBePartial(INamedTypeSymbol type) => Diagnostic.Create(
		descriptor: ShouldBePartialDescriptor,
		location: type.Locations.FirstOrDefault(),
		messageArgs: type.Name);

	public static Diagnostic ShouldHaveConfigurations(INamedTypeSymbol type) => Diagnostic.Create(
		descriptor: ShouldHaveConfigurationsDescriptor,
		location: type.Locations.FirstOrDefault(),
		messageArgs: type.Name);

	#region Descriptors

	private static readonly DiagnosticDescriptor ShouldInheritDbContextDescriptor = new(
		id: DiagnosticErrors.ShouldInheritDbContext,
		title: "Type should inherit from DbContext",
		messageFormat: "Class {0} is marked as implementing entity configurations, but does not inherit from DbContext",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	private static readonly DiagnosticDescriptor ShouldBePartialDescriptor = new(
		id: DiagnosticErrors.ShouldBePartial,
		title: "Type should be partial",
		messageFormat: "Class {0} is marked as implementing entity configurations, and should be partial",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	private static readonly DiagnosticDescriptor ShouldHaveConfigurationsDescriptor = new(
		id: DiagnosticErrors.ShouldHaveConfigurations,
		title: "No valid entity configurations found",
		messageFormat: "Failed to find any valid entity configurations for {0}",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	#endregion
}

file static class DiagnosticErrors
{
	private const string Prefix = "EC";

	public const string ShouldInheritDbContext = $"{Prefix}0001";
	public const string ShouldBePartial = $"{Prefix}0002";
	public const string ShouldHaveConfigurations = $"{Prefix}0003";
}
