namespace AZ.Generator.Test.Verify;

internal static class GlobalSettings
{
	public static readonly VerifySettings VerifySettings = GetSettings();

	private static VerifySettings GetSettings()
	{
		var settings = new VerifySettings();
		settings.UseDirectory("Snapshot");

		return settings;
	}
}
