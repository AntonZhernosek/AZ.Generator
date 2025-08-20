namespace AZ.Generator.Functional.Specs;

internal sealed class Parser
{
	public List<Diagnostic> Diagnostics { get; } = [];

	public DiscriminatedUnionSpec? Parse(GenerateOptions options, TypeDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken ct)
	{
		var type = semanticModel.GetDeclaredSymbol(declaration, ct);

		if (type is null)
		{
			return null;
		}

		if (!type.IsAbstract)
		{
			Diagnostics.Add(DiscriminatedUnionDiagnostics.ShouldBeAbstract(type));
		}

		if (type.DeclaredAccessibility.IsPrivateAccessibility() && !type.IsPartial())
		{
			Diagnostics.Add(DiscriminatedUnionDiagnostics.ShouldBePartial(type));
		}

		var classOrRecord = type.ClassOrRecordDeclaration();

		var implementationSymbols = type.IsNestedType() ? type.GetImplementationsInContainingType() : type.GetImplementationsInContainingNamespace();
		var implementations = implementationSymbols.Count == 0 ? ImmutableEquatableArray<UnionImplementationSpec>.Empty : implementationSymbols.Select(ToImplementationSpec).ToImmutableEquatableArray();

		if (implementations.Count == 0)
		{
			Diagnostics.Add(DiscriminatedUnionDiagnostics.ShouldHaveImplementations(type));
		}

		var containingTypeSymbols = type.GetContainingTypes();
		var containingTypes = containingTypeSymbols.Count == 0 ? ImmutableEquatableArray<ContainingTypeSpec>.Empty : containingTypeSymbols.Select(ToContainingTypeSpec).ToImmutableEquatableArray();

		if (type.DeclaredAccessibility.IsPrivateAccessibility())
		{
			foreach (var containingType in containingTypeSymbols.Where(x => !x.IsPartial()))
			{
				Diagnostics.Add(DiscriminatedUnionDiagnostics.ShouldBePartialContainingType(containingType));
			}
		}

		var typeParameters = GetTypeParameters(type);

		return Diagnostics.Count != 0 ? null : new DiscriminatedUnionSpec()
		{
			GenerateOptions = options,
			DeclarationType = ClassOrRecord(type),
			Name = type.GetName(),
			NameWithContainingTypes = GetNameWithContainingTypes(type, containingTypeSymbols),
			FullyQualifiedName = type.GetFullyQualifiedName(),
			Namespace = type.GetFullNamespace(),
			Accessibility = type.DeclaredAccessibility,
			Implementations = implementations,
			ContainingTypes = containingTypes,
			TypeParameters = typeParameters,
		};
	}

	private static UnionImplementationSpec ToImplementationSpec(INamedTypeSymbol type)
	{
		return new()
		{
			FullyQualifiedName = type.GetFullyQualifiedName(),
			LocalVariableName = GetVariableName(type),
			ArgumentName = GetArgumentName(type),
			IsMethodName = GetIsMethodName(type),
		};
	}

	private static ContainingTypeSpec ToContainingTypeSpec(INamedTypeSymbol type)
	{
		return new()
		{
			DeclarationType = ClassOrRecord(type),
			Name = type.GetName(),
			Namespace = type.GetFullNamespace(),
			TypeParameters = GetTypeParameters(type),
		};
	}

	private static TypeParameterSpec ToTypeParameterSpec(ITypeParameterSymbol type)
	{
		var constraintTypeNames = type.ConstraintTypes.Select(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
		var constraintTypes = !constraintTypeNames.Any() ? ImmutableEquatableArray<string>.Empty : constraintTypeNames.ToImmutableEquatableArray(); 

		return new()
		{
			Name = type.Name,
			AllowsRefLikeType = type.AllowsRefLikeType,
			HasConstructorConstraint = type.HasConstructorConstraint,
			HasNotNullConstraint = type.HasNotNullConstraint,
			HasReferenceTypeConstraint = type.HasReferenceTypeConstraint,
			HasUnmanagedTypeConstraint = type.HasUnmanagedTypeConstraint,
			HasValueTypeConstraint = type.HasValueTypeConstraint,
			ConstraintTypes = constraintTypes,
		};
	}

	private static ImmutableEquatableArray<TypeParameterSpec> GetTypeParameters(INamedTypeSymbol type)
	{
		var typeParameterSymbols = type.TypeParameters;
		var typeParameters = typeParameterSymbols.Length == 0 ? ImmutableEquatableArray<TypeParameterSpec>.Empty : typeParameterSymbols.Select(ToTypeParameterSpec).ToImmutableEquatableArray();

		return typeParameters;
	}

	public static ClassOrRecord ClassOrRecord(ITypeSymbol type) => type.IsRecord ? Specs.ClassOrRecord.Record : Specs.ClassOrRecord.Class;

	static string GetNameWithContainingTypes(INamedTypeSymbol type, List<INamedTypeSymbol> containingTypes) => 
		containingTypes.Select(x => x.Name).Append(type.Name).Join(string.Empty);

	private static string GetVariableName(INamedTypeSymbol type) => $"__{type.Name}";

	private static string GetArgumentName(INamedTypeSymbol type) => GetAttributeStringArgumentOrDefault(
		type: type,
		attributeName: Attributes.DiscriminatedUnionMember,
		argumentName: Attributes.DiscriminatedUnionMember_ArgumentName,
		defaultHandler: type => $"on{type.Name}");

	private static string GetIsMethodName(INamedTypeSymbol type) => GetAttributeStringArgumentOrDefault(
		type: type,
		attributeName: Attributes.DiscriminatedUnionMember,
		argumentName: Attributes.DiscriminatedUnionMember_IsMethodName,
		defaultHandler: type => $"Is{type.Name}");

	private static string GetAttributeStringArgumentOrDefault(INamedTypeSymbol type, string attributeName, string argumentName, Func<INamedTypeSymbol, string> defaultHandler)
	{
		var attribute = type.GetAttributeOrDefault(attributeName);

		if (attribute is null)
		{
			return defaultHandler(type);
		}

		var argument = attribute.GetArgumentOrDefault(argumentName);

		return argument is string stringArgument && !string.IsNullOrWhiteSpace(stringArgument) ? stringArgument : defaultHandler(type);
	}
}
