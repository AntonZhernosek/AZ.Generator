namespace AZ.Generator.Functional.Extensions;

public static class NamedTypeSymbolExtensions
{
	public static string GetName(this INamedTypeSymbol type)
    {
        if (type.TypeParameters.Length == 0)
        {
            return type.Name;
        }

        var genericParameters = type.TypeParameters.Select(x => x.Name);
        var genericParametersDeclaration = $"<{genericParameters.Join(", ")}>";

		return $"{type.Name}{genericParametersDeclaration}";
	}

	public static List<INamedTypeSymbol> GetContainingTypes(this INamedTypeSymbol type)
	{
		var containingTypes = (List<INamedTypeSymbol>)[];
		var containingType = type;

		while (containingType.ContainingType is not null)
		{
			containingTypes.Add(containingType.ContainingType);
			containingType = containingType.ContainingType;
		}

		containingTypes.Reverse();

		return containingTypes;
	}
}
