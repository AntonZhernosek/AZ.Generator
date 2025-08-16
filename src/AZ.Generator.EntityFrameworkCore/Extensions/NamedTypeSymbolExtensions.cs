namespace AZ.Generator.EntityFrameworkCore.Extensions;

internal static class NamedTypeSymbolExtensions
{
	public static bool InheritsFromDbContext(this INamedTypeSymbol type)
	{
		var current = type.BaseType;

		while (current is not null)
		{
			if (current.ToDisplayString().Equals("Microsoft.EntityFrameworkCore.DbContext"))
			{
				return true;
			}

			current = current.BaseType;
		}

		return false;
	}

	public static bool ImplementsEntityTypeConfiguration(this INamedTypeSymbol type) => type.AllInterfaces
		.Any(x => x.OriginalDefinition.ToDisplayString().Equals("Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<TEntity>"));
}
