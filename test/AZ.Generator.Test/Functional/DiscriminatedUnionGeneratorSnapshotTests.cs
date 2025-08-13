namespace AZ.Generator.Test.Functional;

public sealed class DiscriminatedUnionGeneratorSnapshotTests
{
	#region Generate simple

	[Fact]
	public async Task Generate_SimpleClassHierarchy_GenerateExtensionsClass_Correct()
	{
		var code =
			"""
			using AZ.Generator.Functional.Attributes;

			namespace Foo.Bar.Baz;

			[DiscriminatedUnion]
			public abstract class TestUnion;
			public sealed class TestImplementation1 : TestUnion;
			public sealed class TestImplementation2 : TestUnion;
			""";

		await TestHelper.RecompileGeneratedVerify<DiscriminatedUnionGenerator>(code);
	}

	[Fact]
	public async Task Generate_SimpleRecordHierarchy_GenerateExtensionsClass_Correct()
	{
		var code =
			"""
			using AZ.Generator.Functional.Attributes;

			namespace Foo.Bar.Baz;

			[DiscriminatedUnion]
			public abstract record TestUnion;
			public sealed record TestImplementation1 : TestUnion;
			public sealed record TestImplementation2 : TestUnion;
			""";

		await TestHelper.RecompileGeneratedVerify<DiscriminatedUnionGenerator>(code);
	}

	[Fact]
	public async Task Generate_InternalAccessibilityClass_GenerateExtensionsClass_Correct()
	{
		var code =
			"""
			using AZ.Generator.Functional.Attributes;

			namespace Foo.Bar.Baz;

			[DiscriminatedUnion]
			internal abstract class TestUnion;
			internal sealed class TestImplementation1 : TestUnion;
			internal sealed class TestImplementation2 : TestUnion;
			""";

		await TestHelper.RecompileGeneratedVerify<DiscriminatedUnionGenerator>(code);
	}

	[Fact]
	public async Task Generate_CustomArgumentName_GenerateExtensionsClass_Correct()
	{
		var code =
			"""
			using AZ.Generator.Functional.Attributes;

			namespace Foo.Bar.Baz;

			[DiscriminatedUnion]
			public abstract class TestUnion;

			[DiscriminatedUnionMember(ArgumentName = "myImplementationOne")]
			public sealed class TestImplementation1 : TestUnion;
			[DiscriminatedUnionMember(ArgumentName = "myImplementationTwo")]
			public sealed class TestImplementation2 : TestUnion;
			""";

		await TestHelper.RecompileGeneratedVerify<DiscriminatedUnionGenerator>(code);
	}

	[Fact]
	public async Task Generate_CustomIsMethodName_GenerateExtensionsClass_Correct()
	{
		var code =
			"""
			using AZ.Generator.Functional.Attributes;

			namespace Foo.Bar.Baz;

			[DiscriminatedUnion]
			public abstract class TestUnion;

			[DiscriminatedUnionMember(IsMethodName = "IsMyImplementationOne")]
			public sealed class TestImplementation1 : TestUnion;
			[DiscriminatedUnionMember(IsMethodName = "IsMyImplementationTwo")]
			public sealed class TestImplementation2 : TestUnion;
			""";

		await TestHelper.RecompileGeneratedVerify<DiscriminatedUnionGenerator>(code);
	}

	#endregion

	#region Generate nested

	[Fact]
	public async Task Generate_NestedPrivateClass_GeneratePartialImplementation_Correct()
	{
		var code =
			"""
			using AZ.Generator.Functional.Attributes;
			
			namespace Foo.Bar.Baz;
			
			public partial class ContainingType
			{
				[DiscriminatedUnion]
				private abstract partial class NestedUnion;
			
				private sealed class NestedImplementation : NestedUnion;
			}
			""";

		await TestHelper.RecompileGeneratedVerify<DiscriminatedUnionGenerator>(code);
	}

	[Fact]
	public async Task Generate_NestedPrivateRecord_GeneratePartialImplementation_Correct()
	{
		var code =
			"""
			using AZ.Generator.Functional.Attributes;
			
			namespace Foo.Bar.Baz;
			
			public partial record ContainingType
			{
				[DiscriminatedUnion]
				private abstract partial record NestedUnion;
			
				private sealed record NestedImplementation : NestedUnion;
			}
			""";

		await TestHelper.RecompileGeneratedVerify<DiscriminatedUnionGenerator>(code);
	}

	[Fact]
	public async Task Generate_NestedPublicClass_GenerateExtensionsClass_Correct()
	{
		var code =
			"""
			using AZ.Generator.Functional.Attributes;
			
			namespace Foo.Bar.Baz;
			
			public sealed class ContainingType
			{
				[DiscriminatedUnion]
				public abstract class NestedUnion;
			
				public sealed class NestedImplementation : NestedUnion;
			}
			""";

		await TestHelper.RecompileGeneratedVerify<DiscriminatedUnionGenerator>(code);
	}

	[Fact]
	public async Task Generate_NestedInternalClass_GenerateExtensionsClass_Correct()
	{
		var code =
			"""
			using AZ.Generator.Functional.Attributes;
			
			namespace Foo.Bar.Baz;
			
			public sealed class ContainingType
			{
				[DiscriminatedUnion]
				internal abstract class NestedUnion;
			
				internal sealed class NestedImplementation : NestedUnion;
			}
			""";

		await TestHelper.RecompileGeneratedVerify<DiscriminatedUnionGenerator>(code);
	}

	[Fact]
	public async Task Generate_MultipleNestedDeclarations_GeneratePartialImplementation_Correct()
	{
		var code =
			"""
			using AZ.Generator.Functional.Attributes;
			
			namespace Foo.Bar.Baz;
			
			public partial class ContainingType
			{
				public partial class NestedContainingType
				{
					[DiscriminatedUnion]
					private abstract partial class NestedUnion;
			
					private sealed class NestedImplementation : NestedUnion;
				}
			}
			""";

		await TestHelper.RecompileGeneratedVerify<DiscriminatedUnionGenerator>(code);
	}

	[Fact]
	public async Task Generate_HasGenericNestedDeclaration_GeneratePartialImplementation_Correct()
	{
		var code =
			"""
			using AZ.Generator.Functional.Attributes;
			
			namespace Foo.Bar.Baz;
			
			public partial class ContainingType
			{
				public partial class NestedContainingType<T>
					where T : System.IDisposable, new()
				{
					[DiscriminatedUnion]
					private abstract partial class NestedUnion;
			
					private sealed class NestedImplementation : NestedUnion;
				}
			}
			""";

		await TestHelper.RecompileGeneratedVerify<DiscriminatedUnionGenerator>(code);
	}

	// TODO uncomment test. This scenario should also be supported
	//[Fact]
	//public async Task Generate_HasGenericNestedDeclaration_GenerateExtensionsClass_Correct()
	//{
	//	var code =
	//		"""
	//		using AZ.Generator.Functional.Attributes;
			
	//		namespace Foo.Bar.Baz;
			
	//		public sealed class ContainingType
	//		{
	//			public sealed class NestedContainingType<T>
	//				where T : System.IDisposable, new()
	//			{
	//				[DiscriminatedUnion]
	//				public abstract partial class NestedUnion;
			
	//				public sealed class NestedImplementation : NestedUnion;
	//			}
	//		}
	//		""";

	//	await TestHelper.RecompileGeneratedVerify<DiscriminatedUnionGenerator>(code);
	//}

	[Fact]
	public async Task Generate_HasGenericNameCollision_GeneratePartialImplementation_Correct()
	{
		var code =
			"""
			using AZ.Generator.Functional.Attributes;
			
			namespace Foo.Bar.Baz;
			
			public partial class ContainingType
			{
				public partial class NestedContainingType<TResult>
					where TResult : System.IDisposable, new()
				{
					[DiscriminatedUnion]
					private abstract partial class NestedUnion;
			
					private sealed class NestedImplementation : NestedUnion;
				}
			}
			""";

		await TestHelper.RecompileGeneratedVerify<DiscriminatedUnionGenerator>(code);
	}

	#endregion
}
