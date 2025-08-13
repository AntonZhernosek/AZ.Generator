namespace AZ.Generator.Functional.Extensions;

public static class AccessibilityExtensions
{
	public static bool IsPublicAccessibility(this Accessibility accessibility) => accessibility is Accessibility.Public or Accessibility.Internal;
	public static bool IsPrivateAccessibility(this Accessibility accessibility) => !accessibility.IsPublicAccessibility();
}
