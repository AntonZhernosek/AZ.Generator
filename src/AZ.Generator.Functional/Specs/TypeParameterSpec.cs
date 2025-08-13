namespace AZ.Generator.Functional.Specs;

public sealed record TypeParameterSpec
{
	public required string Name { get; init; }
	public required bool HasReferenceTypeConstraint { get; init; }
	public required bool HasValueTypeConstraint { get; init; }
	public required bool HasUnmanagedTypeConstraint { get; init; }
	public required bool HasNotNullConstraint { get; init; }
	public required bool AllowsRefLikeType { get; init; }
	public required bool HasConstructorConstraint { get; init; }
	public required ImmutableEquatableArray<string> ConstraintTypes { get; init; }
}
