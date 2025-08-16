namespace AZ.Generator.EntityFrameworkCore.Extensions;

internal static class AttributeDataExtensions
{
	public static object? GetArgumentOrDefault(this AttributeData attribute, string name, object? defaultValue = default)
	{
		var argument = attribute.NamedArguments.Where(kvp => kvp.Key == name)
			.Cast<KeyValuePair<string, TypedConstant>?>()
			.FirstOrDefault();

		return argument?.Value.Value ?? defaultValue;
	}

	public static T[] GetConstructorArgumentEnumerable<T>(this AttributeData attribute, int ordinal)
	{
		return attribute.ConstructorArguments
			.ElementAt(ordinal)
			.Values
			.Select(x => (T)x.Value!)
			.ToArray();
	}
}
