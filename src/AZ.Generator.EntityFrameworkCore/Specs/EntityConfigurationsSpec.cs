namespace AZ.Generator.EntityFrameworkCore.Specs;

internal sealed record EntityConfigurationsSpec
{
	public required DbContextConfigurationsSpec DbContextSpec { get; init; }
	public required ImmutableEquatableArray<EntityConfigurationSpec> Configurations { get; init; }
}
