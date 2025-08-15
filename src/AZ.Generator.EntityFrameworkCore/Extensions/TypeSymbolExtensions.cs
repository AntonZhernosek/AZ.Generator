namespace AZ.Generator.EntityFrameworkCore.Extensions;

internal static class TypeSymbolExtensions
{
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

	public static AttributeData GetAttribute(this ITypeSymbol type, string attribute) => type.GetAttributes().First(att => att.AttributeClass?.Name == attribute);

	public static bool IsPartial(this ITypeSymbol type)
	{
		return type.DeclaringSyntaxReferences.Any(syntax => syntax.GetSyntax() is BaseTypeDeclarationSyntax declaration
			&& declaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword)));
	}
}
