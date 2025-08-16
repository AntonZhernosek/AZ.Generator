namespace AZ.Generator.EntityFrameworkCore.Specs;

internal sealed record DbContextConfigurationsSpec
{
	public required string Namespace { get; init; }
	public required string Name { get; init; }
	public bool OverridesOnModelCreating { get; init; }
}
