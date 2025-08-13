namespace AZ.Generator.Functional.Specs;

internal sealed record ContainingTypeSpec
{
	public required ClassOrRecord DeclarationType { get; init; }
	public required string Name { get; init; }
	public required string Namespace { get; init; }
	public required ImmutableEquatableArray<TypeParameterSpec> TypeParameters { get; init; }
}
