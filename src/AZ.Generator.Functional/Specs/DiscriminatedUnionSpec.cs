namespace AZ.Generator.Functional.Specs;

internal sealed record DiscriminatedUnionSpec
{
	public required ClassOrRecord DeclarationType { get; init; }
	public required string Name { get; init; }
	public required string NameWithContainingTypes { get; init; }
	public required string FullyQualifiedName { get; init; }
	public required string Namespace { get; init; }
	public required Accessibility Accessibility { get; init; }
	public required ImmutableEquatableArray<UnionImplementationSpec> Implementations { get; init; }
	public required ImmutableEquatableArray<ContainingTypeSpec> ContainingTypes { get; init; }
	public required ImmutableEquatableArray<TypeParameterSpec> TypeParameters { get; init; }
}
