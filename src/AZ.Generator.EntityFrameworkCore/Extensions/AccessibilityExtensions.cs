namespace AZ.Generator.EntityFrameworkCore.Extensions;

internal static class AccessibilityExtensions
{
	public static string ToKeyword(this Accessibility accessibility) => accessibility switch
	{
		Accessibility.Public => "public",
		Accessibility.Internal => "internal",
		_ => throw new NotSupportedException($"Invalid entity accessibility specified {accessibility}"),
	};
}
