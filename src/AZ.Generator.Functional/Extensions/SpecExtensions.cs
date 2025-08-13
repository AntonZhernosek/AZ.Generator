namespace AZ.Generator.Functional.Extensions;

internal static class SpecExtensions
{
	public static string GetPartialDeclaration(this DiscriminatedUnionSpec spec)
	{
		var name = spec.Name;
		var classOrRecord = spec.DeclarationType;

		return GetPartialDeclaration(
			name: in name,
			classOrRecord: in classOrRecord,
			parameters: spec.TypeParameters);
	}

	public static string GetPartialDeclaration(this ContainingTypeSpec spec)
	{
		var name = spec.Name;
		var classOrRecord = spec.DeclarationType;

		return GetPartialDeclaration(
			name: in name,
			classOrRecord: in classOrRecord,
			parameters: spec.TypeParameters);
	}

	private static string GetPartialDeclaration(in string name, in ClassOrRecord classOrRecord, ImmutableEquatableArray<TypeParameterSpec> parameters)
	{
		var constraints = parameters.GetGenericConstraints();

		var declaration =
			$$"""
			partial {{classOrRecord.ToDeclaration()}} {{name}}
			""";

		if (!string.IsNullOrWhiteSpace(constraints))
		{
			declaration +=
				$$"""

					{{constraints.ApplyNewlineTab()}}
				""";
		}

		return declaration;
	}

	private static string GetGenericConstraints(this ImmutableEquatableArray<TypeParameterSpec> parameters)
	{
		if (parameters.Count == 0)
		{
			return string.Empty;
		}

		var constraints = new List<string>(parameters.Count);

		foreach (var parameter in parameters)
		{
			var parts = new List<string>(2);

			if (parameter.HasReferenceTypeConstraint)
			{
				parts.Add("class");
			}
			else if (parameter.HasValueTypeConstraint)
			{
				parts.Add("struct");
			}

			if (parameter.HasUnmanagedTypeConstraint)
			{
				parts.Add("unmanaged");
			}

			if (parameter.HasNotNullConstraint)
			{
				parts.Add("notnull");
			}

			if (parameter.AllowsRefLikeType)
			{
				parts.Add("allows ref struct");
			}

			parts.AddRange(parameter.ConstraintTypes);

			// CS0401 "new()" must be the last constraint specified
			if (parameter.HasConstructorConstraint)
			{
				parts.Add("new()");
			}

			if (parts.Count > 0)
			{
				constraints.Add($"where {parameter.Name} : {parts.Join(", ")}");
			}
		}

		return constraints.Join("\r\n");
	}

#pragma warning disable CS8524
	private static string ToDeclaration(this ClassOrRecord classOrRecord) => classOrRecord switch
	{
		ClassOrRecord.Class => "class",
		ClassOrRecord.Record => "record",
	};
#pragma warning restore CS8524
}
