namespace AZ.Generator.EntityFrameworkCore.Specs;

internal sealed record EntitySetsSpec
{
	public required DbContextSpec DbContextSpec { get; init; }
	public required ImmutableEquatableArray<EntitySpec> Entities { get; init; }
}
