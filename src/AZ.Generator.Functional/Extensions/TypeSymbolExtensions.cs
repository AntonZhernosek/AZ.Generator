namespace AZ.Generator.Functional.Extensions;

public static class TypeSymbolExtensions
{
	private static readonly SymbolDisplayFormat FullyQualifiedFormat = SymbolDisplayFormat.FullyQualifiedFormat;

    public static string GetFullyQualifiedName(this ITypeSymbol type) => type.ToDisplayString(FullyQualifiedFormat);

    public static string GetFullNamespace(this ITypeSymbol type) => type.ContainingNamespace.IsGlobalNamespace ? string.Empty : type.GetNamespaces().Reverse().Join(".");

	public static IEnumerable<string> GetNamespaces(this ITypeSymbol type)
	{
		var current = type.ContainingNamespace;

		while (current != null)
		{
			if (current.IsGlobalNamespace)
			{
				break;
			}

			yield return current.Name;

			current = current.ContainingNamespace;
		}
	}

	public static string ClassOrRecordDeclaration(this ITypeSymbol type) => type.IsRecord ? "record" : "class";

	public static AttributeData? GetAttributeOrDefault(this ITypeSymbol type, string attribute) => type.GetAttributes().FirstOrDefault(att => att.AttributeClass?.Name == attribute);

	public static bool IsBaseType(this ITypeSymbol type, ITypeSymbol baseType) => 
		type.BaseType is INamedTypeSymbol symbol && SymbolEqualityComparer.Default.Equals(symbol, baseType);

	public static bool IsBaseTypeInHierarchy(this ITypeSymbol type, ITypeSymbol baseType)
	{
		var checkedType = type;
		while (checkedType is not null)
		{
			if (checkedType.IsBaseType(baseType))
			{
				return true;
			}

			checkedType = checkedType.BaseType;
		}

		return false;
	}

	public static List<INamedTypeSymbol> GetImplementationsInContainingType(this ITypeSymbol type)
	{
		return type.ContainingType.GetMembers()
			.OfType<INamedTypeSymbol>()
			.Where(x => !x.IsAbstract && x.IsBaseTypeInHierarchy(type))
			.ToList();
	}

	public static List<INamedTypeSymbol> GetImplementationsInContainingNamespace(this ITypeSymbol type)
	{
		return type.ContainingNamespace.GetMembers()
			.OfType<INamedTypeSymbol>()
			.Where(x => !x.IsAbstract && x.IsBaseTypeInHierarchy(type))
			.ToList();
	}

	public static bool IsNestedType(this ITypeSymbol type) => type.ContainingType is not null;
}
