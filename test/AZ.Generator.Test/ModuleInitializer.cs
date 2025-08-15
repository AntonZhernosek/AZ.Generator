namespace AZ.Generator.Test;

public static class ModuleInitializer
{
	[ModuleInitializer]
	public static void Init()
	{
		VerifyDiffPlex.Initialize();
		VerifySourceGenerators.Initialize();

		LoadRequiredAssemblies();
	}

	// TestHelper loads all assemblies from the current appdomain to generate a compilation. Use to load some additional assemblies we may need as references
	private static void LoadRequiredAssemblies()
	{
		_ = typeof(Microsoft.EntityFrameworkCore.DbContext).Assembly;
	}
}