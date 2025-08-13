namespace AZ.Generator.Functional.Specs;

internal sealed record UnionImplementationSpec
{
	public required string FullyQualifiedName { get; init; }
	public required string LocalVariableName { get; init; }
	public required string ArgumentName { get; init; }
	public required string IsMethodName { get; init; }
}
