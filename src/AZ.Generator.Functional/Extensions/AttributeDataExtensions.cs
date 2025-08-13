namespace AZ.Generator.Functional.Extensions;

public static class AttributeDataExtensions
{
    public static object? GetArgumentOrDefault(this AttributeData attribute, string name, object? defaultValue = default)
    {
        var argument = attribute.NamedArguments.Where(kvp => kvp.Key == name)
            .Cast<KeyValuePair<string, TypedConstant>?>()
            .FirstOrDefault();

        return argument?.Value.Value ?? defaultValue;
    }
}
