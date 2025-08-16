namespace AZ.Generator.EntityFrameworkCore.Generators;

[Generator]
public sealed class EntityConfigurationsGenerator : IIncrementalGenerator
{
	#region Init

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(AddAttributes);

		var syntaxProvider = context.SyntaxProvider
			.ForAttributeWithMetadataName($"{Namespaces.Attributes}.{Attributes.EntityConfigurations}", Filter, Transform)
			.Select((tuple, ct) =>
			{
				var parser = new EntityConfigurationsParser();
				var spec = parser.Parse(tuple.Node, tuple.SemanticModel, ct);
				var diagnostics = parser.Diagnostics.ToImmutableEquatableArray();
				return (spec, diagnostics);
			})
			.WithTrackingName(TrackingNames.EntityConfigurations);

		context.RegisterSourceOutput(syntaxProvider, ReportDiagnosticsAndEmit);
	}

	private static bool Filter(SyntaxNode node, CancellationToken _) => node is ClassDeclarationSyntax;
	private static (TypeDeclarationSyntax Node, SemanticModel SemanticModel) Transform(GeneratorAttributeSyntaxContext context, CancellationToken _) =>
		((TypeDeclarationSyntax)context.TargetNode, context.SemanticModel);

	#endregion

	#region Post-init

	private void AddAttributes(IncrementalGeneratorPostInitializationContext context)
	{
		context.AddEmbeddedAttributeDefinition();

		var entityConfigurationsText = SourceText.From(Attributes.EntityConfigurationsDefinition, Encoding.UTF8);
		var entityConfigurationText = SourceText.From(Attributes.EntityConfigurationDefinition, Encoding.UTF8);

		context.AddSource($"{Attributes.EntityConfigurations}.g.cs", entityConfigurationsText);
		context.AddSource($"{Attributes.EntityConfiguration}.g.cs", entityConfigurationText);
	}

	#endregion

	#region Generate

	private void ReportDiagnosticsAndEmit(SourceProductionContext context, (EntityConfigurationsSpec? Spec, ImmutableEquatableArray<Diagnostic> Diagnostics) input)
	{
		var spec = input.Spec;
		var diagnostics = input.Diagnostics;

		foreach (var diagnostic in diagnostics)
		{
			context.ReportDiagnostic(diagnostic);
		}

		if (spec is null)
		{
			return;
		}

		context.CancellationToken.ThrowIfCancellationRequested();

		GenerateConfigurations(in context, spec);
	}

	private void GenerateConfigurations(in SourceProductionContext context, EntityConfigurationsSpec spec)
	{

	}

	#endregion
}
