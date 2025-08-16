namespace AZ.Generator.Test.EntityFrameworkCore;

public sealed class EntityConfigurationsGeneratorSnapshotTests
{
	#region Happy paths

	[Fact]
	public async Task Generate_GenerateValidConfigurations_Correct()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;
			using Foo.Configurations;

			namespace Foo;

			[EntityConfigurations(typeof(EntityOneConfiguration))]
			public sealed partial class TestDbContext : DbContext;
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			public sealed class EntityTwo;
			""";

		var configurations =
			"""
			using Foo.Entities;
			using Microsoft.EntityFrameworkCore;
			using Microsoft.EntityFrameworkCore.Metadata.Builders;

			namespace Foo.Configurations;
			
			public sealed class EntityOneConfiguration : IEntityTypeConfiguration<EntityOne>
			{
				public void Configure(EntityTypeBuilder<EntityOne> builder) { }
			}

			public sealed class EntityTwoConfiguration : IEntityTypeConfiguration<EntityTwo>
			{
				public void Configure(EntityTypeBuilder<EntityTwo> builder) { }
			}
			""";

		await TestHelper.RecompileGeneratedVerify<EntityConfigurationsGenerator>(dbContext, entities, configurations);
	}

	[Fact]
	public async Task Generate_ContextOverridesOnModelCreating_Correct()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;
			using Foo.Configurations;

			namespace Foo;

			[EntityConfigurations(typeof(EntityOneConfiguration))]
			public sealed partial class TestDbContext : DbContext
			{
				protected override void OnModelCreating(ModelBuilder modelBuilder) => base.OnModelCreating(modelBuilder);
			}
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			public sealed class EntityTwo;
			""";

		var configurations =
			"""
			using Foo.Entities;
			using Microsoft.EntityFrameworkCore;
			using Microsoft.EntityFrameworkCore.Metadata.Builders;

			namespace Foo.Configurations;
			
			public sealed class EntityOneConfiguration : IEntityTypeConfiguration<EntityOne>
			{
				public void Configure(EntityTypeBuilder<EntityOne> builder) { }
			}

			public sealed class EntityTwoConfiguration : IEntityTypeConfiguration<EntityTwo>
			{
				public void Configure(EntityTypeBuilder<EntityTwo> builder) { }
			}
			""";

		await TestHelper.RecompileGeneratedVerify<EntityConfigurationsGenerator>(dbContext, entities, configurations);
	}

	[Fact]
	public async Task Generate_PicksOnlyValidEntityConfigurations_Correct()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;
			using Foo.Configurations;

			namespace Foo;

			[EntityConfigurations(typeof(EntityOneConfiguration))]
			public sealed partial class TestDbContext : DbContext;
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			public sealed class EntityTwo;
			""";

		var configurations =
			"""
			using Foo.Entities;
			using Microsoft.EntityFrameworkCore;
			using Microsoft.EntityFrameworkCore.Metadata.Builders;

			namespace Foo.Configurations;
			
			public sealed class EntityOneConfiguration : IEntityTypeConfiguration<EntityOne>
			{
				public void Configure(EntityTypeBuilder<EntityOne> builder) { }
			}

			file sealed class EntityTwoConfiguration : IEntityTypeConfiguration<EntityTwo>
			{
				public void Configure(EntityTypeBuilder<EntityTwo> builder) { }
			}

			internal struct SomeStruct;

			public sealed class NotEntityConfiguration;

			public abstract class AbstractConfiguration : IEntityTypeConfiguration<EntityOne>
			{
				public void Configure(EntityTypeBuilder<EntityOne> builder) { }
			}

			public sealed class HasNoParameterlessConstructor : IEntityTypeConfiguration<EntityOne>
			{
				private readonly string someValue;

				public HasNoParameterlessConstructor(string someValue)
				{
					this.someValue = someValue;
				}

				public void Configure(EntityTypeBuilder<EntityOne> builder) { }
			}
			""";

		await TestHelper.RecompileGeneratedVerify<EntityConfigurationsGenerator>(dbContext, entities, configurations);
	}

	[Fact]
	public async Task Generate_IgnoresConfigurationsFromAttribute_Correct()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;
			using Foo.Configurations;

			namespace Foo;

			[EntityConfigurations(typeof(EntityOneConfiguration))]
			public sealed partial class TestDbContext : DbContext;
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			public sealed class EntityTwo;
			""";

		var configurations =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Foo.Entities;
			using Microsoft.EntityFrameworkCore;
			using Microsoft.EntityFrameworkCore.Metadata.Builders;

			namespace Foo.Configurations;
			
			public sealed class EntityOneConfiguration : IEntityTypeConfiguration<EntityOne>
			{
				public void Configure(EntityTypeBuilder<EntityOne> builder) { }
			}

			[EntityConfiguration(Ignore = true)]
			public sealed class EntityTwoConfiguration : IEntityTypeConfiguration<EntityTwo>
			{
				public void Configure(EntityTypeBuilder<EntityTwo> builder) { }
			}
			""";

		await TestHelper.RecompileGeneratedVerify<EntityConfigurationsGenerator>(dbContext, entities, configurations);
	}

	[Fact]
	public async Task Generate_ImportsFromMultipleNamespaces_Correct()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;
			using Foo.Configurations;
			using Foo.Configurations2;

			namespace Foo;

			[EntityConfigurations(typeof(EntityOneConfiguration), typeof(EntityTwoConfiguration))]
			public sealed partial class TestDbContext : DbContext;
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			public sealed class EntityTwo;
			""";

		var configurations =
			"""
			using Foo.Entities;
			using Microsoft.EntityFrameworkCore;
			using Microsoft.EntityFrameworkCore.Metadata.Builders;

			namespace Foo.Configurations;
			
			public sealed class EntityOneConfiguration : IEntityTypeConfiguration<EntityOne>
			{
				public void Configure(EntityTypeBuilder<EntityOne> builder) { }
			}
			""";

		var configurations2 =
			"""
			using Foo.Entities;
			using Microsoft.EntityFrameworkCore;
			using Microsoft.EntityFrameworkCore.Metadata.Builders;

			namespace Foo.Configurations2;
			
			public sealed class EntityTwoConfiguration : IEntityTypeConfiguration<EntityTwo>
			{
				public void Configure(EntityTypeBuilder<EntityTwo> builder) { }
			}
			""";

		await TestHelper.RecompileGeneratedVerify<EntityConfigurationsGenerator>(dbContext, entities, configurations, configurations2);
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
			using Foo.Configurations;

			namespace Foo;

			[EntityConfigurations(typeof(EntityOneConfiguration))]
			public sealed partial class TestDbContext;
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			public sealed class EntityTwo;
			""";

		var configurations =
			"""
			using Foo.Entities;
			using Microsoft.EntityFrameworkCore;
			using Microsoft.EntityFrameworkCore.Metadata.Builders;

			namespace Foo.Configurations;
			
			public sealed class EntityOneConfiguration : IEntityTypeConfiguration<EntityOne>
			{
				public void Configure(EntityTypeBuilder<EntityOne> builder) { }
			}

			public sealed class EntityTwoConfiguration : IEntityTypeConfiguration<EntityTwo>
			{
				public void Configure(EntityTypeBuilder<EntityTwo> builder) { }
			}
			""";

		await TestHelper.RecompileGeneratedVerify<EntityConfigurationsGenerator>(dbContext, entities, configurations);
	}

	[Fact]
	public async Task Generate_DbContextNotPartial_ProducesDiagnostic()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;
			using Foo.Configurations;

			namespace Foo;

			[EntityConfigurations(typeof(EntityOneConfiguration))]
			public sealed class TestDbContext : DbContext;
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			public sealed class EntityTwo;
			""";

		var configurations =
			"""
			using Foo.Entities;
			using Microsoft.EntityFrameworkCore;
			using Microsoft.EntityFrameworkCore.Metadata.Builders;

			namespace Foo.Configurations;
			
			public sealed class EntityOneConfiguration : IEntityTypeConfiguration<EntityOne>
			{
				public void Configure(EntityTypeBuilder<EntityOne> builder) { }
			}

			public sealed class EntityTwoConfiguration : IEntityTypeConfiguration<EntityTwo>
			{
				public void Configure(EntityTypeBuilder<EntityTwo> builder) { }
			}
			""";

		await TestHelper.RecompileGeneratedVerify<EntityConfigurationsGenerator>(dbContext, entities, configurations);
	}

	[Fact]
	public async Task Generate_NoValidConfigurations_ProducesDiagnostic()
	{
		var dbContext =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Microsoft.EntityFrameworkCore;
			using Foo.Entities;
			using Foo.Configurations;

			namespace Foo;

			[EntityConfigurations(typeof(EntityOneConfiguration))]
			public sealed partial class TestDbContext : DbContext;
			""";

		var entities =
			"""
			namespace Foo.Entities;

			public sealed class EntityOne;
			public sealed class EntityTwo;
			""";

		var configurations =
			"""
			using AZ.Generator.EntityFrameworkCore.Attributes;
			using Foo.Entities;
			using Microsoft.EntityFrameworkCore;
			using Microsoft.EntityFrameworkCore.Metadata.Builders;

			namespace Foo.Configurations;
			
			[EntityConfiguration(Ignore = true)]
			public sealed class EntityOneConfiguration : IEntityTypeConfiguration<EntityOne>
			{
				public void Configure(EntityTypeBuilder<EntityOne> builder) { }
			}

			[EntityConfiguration(Ignore = true)]
			public sealed class EntityTwoConfiguration : IEntityTypeConfiguration<EntityTwo>
			{
				public void Configure(EntityTypeBuilder<EntityTwo> builder) { }
			}
			""";

		await TestHelper.RecompileGeneratedVerify<EntityConfigurationsGenerator>(dbContext, entities, configurations);
	}

	#endregion
}
