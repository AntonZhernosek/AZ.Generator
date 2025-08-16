using TrackingNames = AZ.Generator.EntityFrameworkCore.Constants.TrackingNames;

namespace AZ.Generator.Test.EntityFrameworkCore;

public sealed class EntityConfigurationsGeneratorQualityTests
{
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

		await TestHelper.VerifyConsecutiveRuns<EntityConfigurationsGenerator>([dbContext, entities, configurations], TrackingNames.EntityConfigurations);
	}
}
