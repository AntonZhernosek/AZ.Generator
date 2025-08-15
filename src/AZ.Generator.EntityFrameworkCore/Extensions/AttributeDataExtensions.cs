namespace AZ.Generator.EntityFrameworkCore.Extensions;

internal static class AttributeDataExtensions
{
	public static T GetConstructorArgument<T>(this AttributeData attribute, int ordinal) => (T)attribute.ConstructorArguments.ElementAt(ordinal).Value!;
}
