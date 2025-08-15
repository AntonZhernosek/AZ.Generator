namespace AZ.Generator.EntityFrameworkCore.Specs;

internal sealed class EntitySetsParser
{
	public List<Diagnostic> Diagnostics { get; } = [];

	public EntitySetsSpec? Parse(TypeDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken ct)
	{
		var type = semanticModel.GetDeclaredSymbol(declaration, ct);

		if (type is null)
		{
			return null;
		}

		if (!type.InheritsFromDbContext())
		{
			Diagnostics.Add(EntitySetsDiagnostics.ShouldInheritDbContext(type));
		}

		if (!type.IsPartial())
		{
			Diagnostics.Add(EntitySetsDiagnostics.ShouldBePartial(type));
		}

		var containingType = type.GetAttribute(Attributes.EntitySets).GetConstructorArgument<INamedTypeSymbol>(0);

		return Diagnostics.Count != 0 ? null : new EntitySetsSpec()
		{
			EntitiesNamespace = containingType.GetFullNamespace(),
			DbContextSpec = GetDbContextSpec(type),
		};
	}

	private DbContextSpec GetDbContextSpec(INamedTypeSymbol type)
	{
		return new DbContextSpec()
		{
			Name = type.Name,
			Namespace = type.GetFullNamespace(),
		};
	}
}
