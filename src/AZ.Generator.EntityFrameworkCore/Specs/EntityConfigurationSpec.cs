namespace AZ.Generator.EntityFrameworkCore.Specs;

internal sealed record EntityConfigurationSpec
{
	public required Accessibility Accessibility { get; init; }
	public required string FullyQualifiedName { get; init; }
	public required string Namespace { get; init; }
	public required string Name { get; init; }
}
