namespace AZ.Generator.Test.EntityFrameworkCore;

public sealed class EntitySetsGeneratorSnapshotTests
{
	#region Happy paths

	[Fact]
	public async Task Generate_GenerateValidSets_Correct()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;

			namespace Foo;

			[EntitySets(typeof(EntityOne))]
			public sealed partial class TestDbContext : DbContext;
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			public sealed class EntityTwo;
			""";

		await TestHelper.RecompileGeneratedVerify<EntitySetsGenerator>(dbContext, entities);
	}

	[Fact]
	public async Task Generate_EntitiesFromMultipleNamespaces_Correct()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;
			using Bar.Entities;

			namespace Foo;

			[EntitySets(typeof(EntityOne), typeof(EntityTwo))]
			public sealed partial class TestDbContext : DbContext;
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			""";

		var entities2 =
			"""
			namespace Bar.Entities;

			public sealed class EntityTwo;
			""";

		await TestHelper.RecompileGeneratedVerify<EntitySetsGenerator>(dbContext, entities, entities2);
	}

	[Fact]
	public async Task Generate_GenerateVariousTypes_PicksOnlyClasses()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;

			namespace Foo;

			[EntitySets(typeof(EntityOne))]
			public sealed partial class TestDbContext : DbContext;
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			public sealed record EntityTwo;
			public struct EntityThree;
			""";

		await TestHelper.RecompileGeneratedVerify<EntitySetsGenerator>(dbContext, entities);
	}

	[Fact]
	public async Task Generate_GenerateVariousAccessibilities_GeneratesPublicInternalCorrectly()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;

			namespace Foo;

			[EntitySets(typeof(EntityOne))]
			public sealed partial class TestDbContext : DbContext;
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			internal sealed class EntityTwo;
			file sealed class EntityThree;
			""";

		await TestHelper.RecompileGeneratedVerify<EntitySetsGenerator>(dbContext, entities);
	}

	[Fact]
	public async Task Generate_EntityIsIgnored_Correct()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;

			namespace Foo;

			[EntitySets(typeof(EntityOne))]
			public sealed partial class TestDbContext : DbContext;
			""";

		var entities =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;

			namespace Foo.Entities;

			[EntitySet(Ignore = true)]
			public sealed class EntityOne;
			public sealed class EntityTwo;
			""";

		await TestHelper.RecompileGeneratedVerify<EntitySetsGenerator>(dbContext, entities);
	}

	[Fact]
	public async Task Generate_CustomDbSetNames_Correct()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;

			namespace Foo;

			[EntitySets(typeof(EntityOne))]
			public sealed partial class TestDbContext : DbContext;
			""";

		var entities =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;

			namespace Foo.Entities;

			[EntitySet(Name = "MyEntityOneSet")]
			public sealed class EntityOne;
			[EntitySet(Name = "MyEntityTwoSet")]
			public sealed class EntityTwo;
			""";

		await TestHelper.RecompileGeneratedVerify<EntitySetsGenerator>(dbContext, entities);
	}

	#endregion

	#region Diagnostics

	[Fact]
	public async Task Generate_DoesNotInheritDbContext_ProducesDiagnostic()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;

			namespace Foo;

			[EntitySets(typeof(EntityOne))]
			public sealed partial class TestDbContext;
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			public sealed class EntityTwo;
			""";

		await TestHelper.RecompileGeneratedVerify<EntitySetsGenerator>(dbContext, entities);
	}

	[Fact]
	public async Task Generate_IsNotPartial_ProducesDiagnostic()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;

			namespace Foo;

			[EntitySets(typeof(EntityOne))]
			public sealed class TestDbContext : DbContext;
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			public sealed class EntityTwo;
			""";

		await TestHelper.RecompileGeneratedVerify<EntitySetsGenerator>(dbContext, entities);
	}

	[Fact]
	public async Task Generate_HasNoEntities_ProducesDiagnostic()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;

			namespace Foo;

			[EntitySets(typeof(EntityOne))]
			public sealed partial class TestDbContext : DbContext;
			""";

		var entities =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;

			namespace Foo.Entities;

			[EntitySet(Ignore = true)]
			public sealed class EntityOne;
			[EntitySet(Ignore = true)]
			public sealed class EntityTwo;
			""";

		await TestHelper.RecompileGeneratedVerify<EntitySetsGenerator>(dbContext, entities);
	}

	#endregion
}
