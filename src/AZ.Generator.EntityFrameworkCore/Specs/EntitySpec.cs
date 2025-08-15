namespace AZ.Generator.EntityFrameworkCore.Specs;

internal sealed record EntitySpec
{
	public required Accessibility Accessibility { get; init; }
	public required string FullyQualifiedName { get; init; }
	public required string DbSetName { get; init; }
}
