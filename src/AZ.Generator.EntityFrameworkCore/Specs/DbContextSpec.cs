namespace AZ.Generator.EntityFrameworkCore.Specs;

internal sealed record DbContextSpec
{
	public required string Namespace { get; init; }
	public required string Name { get; init; }
}
