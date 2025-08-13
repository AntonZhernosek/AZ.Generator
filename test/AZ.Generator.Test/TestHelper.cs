namespace AZ.Generator.Test;

public static class TestHelper
{
	public static Task RecompileGeneratedVerify<T>(string text)
		where T : IIncrementalGenerator, new()
	{
		var (compilation, driver) = VerifyInternal<T>(text);
		var secondCompilation = compilation.AddSyntaxTrees(driver.GetRunResult().GeneratedTrees);

		AssertCompiles(secondCompilation);

		return Verifier.Verify(driver, GlobalSettings.VerifySettings);
	}

	public static Task Verify<T>(string text)
		where T : IIncrementalGenerator, new()
	{
		var (_, driver) = VerifyInternal<T>(text);
		return Verifier.Verify(driver, GlobalSettings.VerifySettings);
	}

	private static (CSharpCompilation Compilation, GeneratorDriver Driver) VerifyInternal<T>(string text)
		where T : IIncrementalGenerator, new()
	{
		var compilation = GenerateCompilation(in text);

		var generator = new T();

		GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

		driver = driver.RunGenerators(compilation);

		return (compilation, driver);
	}

	public static void VerifyConsecutiveRuns<T>(in string text, params string[] trackingNames)
		where T : IIncrementalGenerator, new()
	{
		var compilation = GenerateCompilation(in text);
		var compilation2 = compilation.Clone();

		var generator = new T();

		var driverOptions = new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, true);

		GeneratorDriver driver = CSharpGeneratorDriver.Create([ generator.AsSourceGenerator() ], driverOptions: driverOptions);
		driver = driver.RunGenerators(compilation);

		var firstRunResult = driver.GetRunResult();
		var secondRunResult = driver.RunGenerators(compilation2).GetRunResult();

		AssertRunsEqual(firstRunResult, secondRunResult, trackingNames);
	}

	private static void AssertRunsEqual(GeneratorDriverRunResult runResult1, GeneratorDriverRunResult runResult2, string[] trackingNames)
	{
		var trackedSteps1 = GetTrackedSteps(runResult1, trackingNames);
		var trackedSteps2 = GetTrackedSteps(runResult2, trackingNames);
		
		Assert.Equal(trackedSteps1.Count, trackedSteps2.Count);
		trackedSteps1.AssetContainsKeys(trackedSteps2);

		foreach (var (trackingName, runSteps1) in trackedSteps1)
		{
			var runSteps2 = trackedSteps2[trackingName];
			AssertEqual(runSteps1, runSteps2, trackingName);
		}

		static Dictionary<string, ImmutableArray<IncrementalGeneratorRunStep>> GetTrackedSteps(GeneratorDriverRunResult runResult, string[] trackingNames)
		{
			return runResult.Results.SelectMany(x => x.TrackedSteps)
				.Where(step => trackingNames.Contains(step.Key))
				.ToDictionary(x => x.Key, x => x.Value);
		}
	}

	private static void AssertEqual(ImmutableArray<IncrementalGeneratorRunStep> runSteps1, ImmutableArray<IncrementalGeneratorRunStep> runSteps2, string stepName)
	{
		Assert.Equal(runSteps1.Length, runSteps2.Length);

		foreach (var (runStep1, runStep2) in runSteps1.Zip(runSteps2))
		{
			var outputs1 = runStep1.Outputs.Select(x => x.Value);
			var outputs2 = runStep2.Outputs.Select(x => x.Value);

			outputs1.AssertEqual(outputs2, $"{stepName} should produce cacheable outputs");
			Assert.All(runStep2.Outputs, output => Assert.True(output.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged, $"{stepName} expected to produce cached results"));

			AssertObjectGraph(runStep1, stepName);
		}
	}

	static void AssertObjectGraph(IncrementalGeneratorRunStep runStep, string stepName)
	{
		// Including the stepName in error messages to make it easy to isolate issues
		var visited = new HashSet<object>();

		foreach (var (obj, _) in runStep.Outputs)
		{
			Visit(obj);
		}

		void Visit(object? node)
		{
			if (node is null || !visited.Add(node))
			{
				return;
			}

#pragma warning disable xUnit2032
			Assert.IsNotAssignableFrom<Compilation>(node);
			Assert.IsNotAssignableFrom<ISymbol>(node);
			Assert.IsNotAssignableFrom<SyntaxNode>(node);
#pragma warning restore xUnit2032

			var type = node.GetType();
			if (type.IsPrimitive || type.IsEnum || type == typeof(string))
			{
				return;
			}

			// If the object is a collection, check each of the values
			if (node is IEnumerable collection and not string)
			{
				foreach (var element in collection)
				{
					Visit(element);
				}

				return;
			}

			// Recursively check each field in the object
			foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				var fieldValue = field.GetValue(node);
				Visit(fieldValue);
			}
		}
	}

	private static CSharpCompilation GenerateCompilation(in string text)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(text);

		var references = AppDomain.CurrentDomain.GetAssemblies()
			.Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
			.Select(assembly => MetadataReference.CreateFromFile(assembly.Location));

		var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
			.WithOptimizationLevel(OptimizationLevel.Debug)
			.WithPlatform(Platform.AnyCpu);

		var compilation = CSharpCompilation.Create(
			assemblyName: "Tests",
			syntaxTrees: [syntaxTree],
			references: references,
			options: options);

		return compilation;
	}

	private static void AssertCompiles(CSharpCompilation compilation)
	{
		var symbolsName = Path.ChangeExtension(compilation.AssemblyName, "pdb");

		using var assemblyStream = new MemoryStream();
		using var symbolsStream = new MemoryStream();

		var emitOptions = new EmitOptions(
			debugInformationFormat: DebugInformationFormat.PortablePdb,
			pdbFilePath: symbolsName);

		var result = compilation.Emit(
			peStream: assemblyStream,
			pdbStream: symbolsStream,
			options: emitOptions);

		Assert.True(result.Success, "Code should compile");
		Assert.Empty(result.Diagnostics.Where(x => x.IsWarningAsError || x.Severity == DiagnosticSeverity.Error));
	}
}
