using TrackingNames = AZ.Generator.EntityFrameworkCore.Constants.TrackingNames;

namespace AZ.Generator.Test.EntityFrameworkCore;

public sealed class EntitySetsGeneratorQualityTests
{
	[Fact]
	public async Task Generate_ValidInput_RunsEqualAndCache()
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

		await TestHelper.VerifyConsecutiveRuns<EntitySetsGenerator>([dbContext, entities], TrackingNames.EntitySets);
	}
}
