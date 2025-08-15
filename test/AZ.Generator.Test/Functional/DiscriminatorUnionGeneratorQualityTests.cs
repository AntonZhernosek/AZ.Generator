namespace AZ.Generator.Test.Functional;

public sealed class DiscriminatorUnionGeneratorQualityTests
{
	[Fact]
	public async Task Generate_ValidInput_GenerateExtensionsClass_RunsEqualAndCache()
	{
		var code =
			"""
			using AZ.Generator.Functional.Attributes;

			namespace Foo.Bar.Baz;

			[DiscriminatedUnion]
			public abstract class TestUnion;

			[DiscriminatedUnionMember(ArgumentName = "myImplementationOne", IsMethodName = "IsMyImplementationOne")]
			public sealed class TestImplementation1 : TestUnion;

			public sealed class TestImplementation2 : TestUnion;
			""";

		await TestHelper.VerifyConsecutiveRuns<DiscriminatedUnionGenerator>([code], TrackingNames.DiscriminatedUnion);
	}
}
