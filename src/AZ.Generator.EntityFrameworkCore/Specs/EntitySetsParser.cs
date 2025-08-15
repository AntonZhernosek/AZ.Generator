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
		var entities = GetEntities(containingType);

		if (entities.Count == 0)
		{
			Diagnostics.Add(EntitySetsDiagnostics.ShouldHaveEntities(type, containingType.ContainingNamespace));
		}

		return Diagnostics.Count != 0 ? null : new EntitySetsSpec()
		{
			EntitiesNamespace = containingType.GetFullNamespace(),
			DbContextSpec = GetDbContextSpec(type),
			Entities = entities,
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

	private ImmutableEquatableArray<EntitySpec> GetEntities(INamedTypeSymbol containingType)
	{
		return containingType.ContainingNamespace.GetTypeMembers()
			.OfType<INamedTypeSymbol>()
			.Where(IsIncluded)
			.Select(type => new EntitySpec()
			{
				Accessibility = type.DeclaredAccessibility,
				FullyQualifiedName = type.GetFullyQualifiedName(),
				DbSetName = GetDbSetName(type),
			})
			.ToImmutableEquatableArray();

		static bool IsIncluded(INamedTypeSymbol type)
		{
			if (type.DeclaredAccessibility is not Accessibility.Public and not Accessibility.Internal)
			{
				return false;
			}

			var attribute = type.GetAttributeOrDefault(Attributes.EntitySetConfig);

			if (attribute is null)
			{
				return true;
			}

			var ignored = (bool)attribute.GetArgumentOrDefault(Attributes.EntitySetConfig_Ignore, false)!;
			return !ignored;
		}

		static string GetDbSetName(INamedTypeSymbol type)
		{
			var attribute = type.GetAttributeOrDefault(Attributes.EntitySetConfig);

			if (attribute is null)
			{
				return type.Name;
			}

			var attributeOverrideName = (string)attribute.GetArgumentOrDefault(Attributes.EntitySetConfig_Name, type.Name)!;
			return string.IsNullOrWhiteSpace(attributeOverrideName) ? type.Name : attributeOverrideName;
		}
	}
}
