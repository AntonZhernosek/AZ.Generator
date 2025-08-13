namespace AZ.Generator.Test.Functional;

public sealed class DiscriminatorUnionGeneratorQualityTests
{
	[Fact]
	public void Generate_ValidInput_GenerateExtensionsClass_RunsEqualAndCache()
	{
		var code =
			"""
			using AZSoft.Generator.Functional.Attributes;

			namespace Foo.Bar.Baz;

			[DiscriminatedUnion]
			public abstract class TestUnion;

			[DiscriminatedUnionMember(ArgumentName = "myImplementationOne", IsMethodName = "IsMyImplementationOne")]
			public sealed class TestImplementation1 : TestUnion;

			public sealed class TestImplementation2 : TestUnion;
			""";

		TestHelper.VerifyConsecutiveRuns<DiscriminatedUnionGenerator>(code, TrackingNames.DiscriminatedUnion);
	}
}
