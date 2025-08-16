namespace AZ.Generator.EntityFrameworkCore.Specs;

internal sealed class EntityConfigurationsParser
{
	public List<Diagnostic> Diagnostics { get; } = [];

	public EntityConfigurationsSpec? Parse(TypeDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken ct)
	{
		var type = semanticModel.GetDeclaredSymbol(declaration, ct);

		if (type is null)
		{
			return null;
		}

		if (!type.InheritsFromDbContext())
		{
			Diagnostics.Add(EntityConfigurationsDiagnostics.ShouldInheritDbContext(type));
		}

		if (!type.IsPartial())
		{
			Diagnostics.Add(EntityConfigurationsDiagnostics.ShouldBePartial(type));
		}

		var containingTypes = type.GetAttribute(Attributes.EntityConfigurations).GetConstructorArgumentEnumerable<INamedTypeSymbol>(0);
		var configurations = GetEntityConfigurations(containingTypes);

		if (configurations.Count == 0)
		{
			Diagnostics.Add(EntityConfigurationsDiagnostics.ShouldHaveConfigurations(type));
		}

		return Diagnostics.Count != 0 ? null : new EntityConfigurationsSpec()
		{
			DbContextSpec = GetDbContextSpec(type),
			Configurations = configurations,
		};
	}

	private DbContextConfigurationsSpec GetDbContextSpec(INamedTypeSymbol type) => new()
	{
		Name = type.Name,
		Namespace = type.GetFullNamespace(),
		OverridesOnModelCreating = type.GetMembers()
			.OfType<IMethodSymbol>()
			.Any(x => x.IsOverride && x.Name.Equals("OnModelCreating")),
	};

	private ImmutableEquatableArray<EntityConfigurationSpec> GetEntityConfigurations(INamedTypeSymbol[] containingTypes)
	{
		return containingTypes.Select(x => x.ContainingNamespace)
			.Distinct(SymbolEqualityComparer.Default)
			.OfType<INamespaceSymbol>()
			.SelectMany(x => x.GetTypeMembers())
			.Distinct(SymbolEqualityComparer.Default)
			.OfType<INamedTypeSymbol>()
			.Where(IsIncluded)
			.Select(type => new EntityConfigurationSpec()
			{
				Accessibility = type.DeclaredAccessibility,
				FullyQualifiedName = type.GetFullyQualifiedName(),
				Namespace = type.GetFullNamespace(),
				Name = type.Name,
			})
			.ToImmutableEquatableArray();

		static bool IsIncluded(INamedTypeSymbol type)
		{
			if (!type.IsReferenceType)
			{
				return false;
			}

			if (type.IsFileLocal)
			{
				return false;
			}

			if (type.DeclaredAccessibility is not Accessibility.Public and not Accessibility.Internal)
			{
				return false;
			}

			if (!type.ImplementsEntityTypeConfiguration())
			{
				return false;
			}

			var attribute = type.GetAttributeOrDefault(Attributes.EntityConfiguration);

			if (attribute is null)
			{
				return true;
			}

			var ignored = (bool)attribute.GetArgumentOrDefault(Attributes.EntityConfiguration_Ignore, false)!;
			return !ignored;
		}
	}
}
