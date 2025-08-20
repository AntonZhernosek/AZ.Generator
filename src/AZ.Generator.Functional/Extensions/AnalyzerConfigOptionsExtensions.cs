namespace AZ.Generator.Functional.Extensions;

internal static class AnalyzerConfigOptionsExtensions
{
	public static bool TryGetBuildProperty(this AnalyzerConfigOptions options, string propertyName, [NotNullWhen(true)] out string? value) =>
		options.TryGetValue($"build_property.{propertyName}", out value);

	public static string? GetBuildPropertyOrDefault(this AnalyzerConfigOptions options, string propertyName, string? defaultValue = default) =>
		options.TryGetBuildProperty(propertyName, out var value) ? value : defaultValue;
}
