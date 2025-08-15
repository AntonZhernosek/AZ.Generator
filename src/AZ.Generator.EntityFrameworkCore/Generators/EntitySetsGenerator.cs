namespace AZ.Generator.EntityFrameworkCore.Generators;

[Generator]
public sealed class EntitySetsGenerator : IIncrementalGenerator
{
	#region Init

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(AddAttributes);

		var syntaxProvider = context.SyntaxProvider
			.ForAttributeWithMetadataName($"{Namespaces.Attributes}.{Attributes.EntitySets}", Filter, Transform)
			.WithTrackingName(TrackingNames.EntitySets);
	}

	private static bool Filter(SyntaxNode node, CancellationToken _) => node is ClassDeclarationSyntax;
	private static (TypeDeclarationSyntax Node, SemanticModel SemanticModel) Transform(GeneratorAttributeSyntaxContext context, CancellationToken _) =>
		((TypeDeclarationSyntax)context.TargetNode, context.SemanticModel);

	#endregion

	#region Post-init

	private void AddAttributes(IncrementalGeneratorPostInitializationContext context)
	{
		context.AddEmbeddedAttributeDefinition();

		var entitySetsText = SourceText.From(Attributes.EntitySetsDefinition, Encoding.UTF8);
		var entitySetConfigText = SourceText.From(Attributes.EntitySetConfigDefinition, Encoding.UTF8);

		context.AddSource($"{Attributes.EntitySets}.g.cs", entitySetsText);
		context.AddSource($"{Attributes.EntitySetConfig}.g.cs", entitySetConfigText);
	}

	#endregion
}
