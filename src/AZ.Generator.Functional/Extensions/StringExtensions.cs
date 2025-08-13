namespace AZ.Generator.Functional.Extensions;

public static class StringExtensions
{
	public static string ApplyNewlineTab(this string value) => value.Replace("\r\n", "\r\n\t");
	public static string Join(this IEnumerable<string> source, string separator) => string.Join(separator, source);
}
