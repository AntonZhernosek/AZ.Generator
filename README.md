# AZ.Generator.Functional

A source-generated version for supporting discriminated unions in C#. An alternative to using OneOf by creating hierarchies for classes and records.
Generated code uses file-scoped namespaces and code analysis attributes, so it should be compatible with any .NET version >= 6

## Usage

The generator will initialize and add the required attributes as part of your project. You can declare discriminated unions in the following way by adding DiscriminatedUnionAttribute on top of an abstract class in a hierarchy:

```csharp
using AZ.Generator.Functional.Attributes;

namespace Foo.Bar.Baz;

[DiscriminatedUnion]
public abstract class TestUnion;
public sealed class TestImplementation1 : TestUnion;
public sealed class TestImplementation2 : TestUnion;
```

The generator will create a Switch, Match and IsImplementation (for every implementation) methods:

```csharp
TestUnion myUnion = new TestImplementation1();

myUnion.Switch(
	impl1 => Console.WriteLine("First"),
	impl2 => Console.WriteLine("Second"));

var stringDescription = myUnion.Match(
	impl1 => "My first implementation",
	impl2 => "My second implementation");

Console.WriteLine(stringDescription);

if (myUnion.IsTestImplementation1(out var impl))
{
	Console.WriteLine("First implementation");
}
```

2 modes of generation are supported:
1. For public and internal classes, an extensions class will be generated
2. For nested private classes, a partial implementation will be generated (every containing type will also have to be partial)

The latter is useful for cases where you want to have a private discriminated union (for example, as a private component state in Blazor apps). The following declaration will also generate an implementation for the union methods.

```csharp
public partial class ContainingType
{
	[DiscriminatedUnion]
	private abstract partial class NestedUnion;
			
	private sealed class NestedImplementation : NestedUnion;
}
```

Union members can be customized by using the generated DiscriminatedUnionMemberAttribute. Currently, the attribute supports custom naming for Switch and Match parameter names, and a custom name for the Is method

```csharp
[DiscriminatedUnion]
public abstract class TestUnion;

[DiscriminatedUnionMember(ArgumentName = "myImplementationOne", IsMethodName = "IsMyFirstImpl")]
public sealed class TestImplementation1 : TestUnion;

[DiscriminatedUnionMember(ArgumentName = "myImplementationTwo", IsMethodName = "IsMySecondImpl")]
public sealed class TestImplementation2 : TestUnion;
```

# AZ.Generator.EntityFrameworkCore

Provides small utility source generators that help manage DbContexts with large data models.

## EntitySetsGenerator

Creates a partial implementation for the DbContext that implements all entities from a particular namespace as DbSets. Configured via EntitySetsAttribute on the DbContext. The attribute accepts multiple types as "containing types", meaning that the generator will import all valid entities from that type's namespace. 

Entities must:
1. Be reference types
2. Have public or internal accessibility

Classes can be ignored and DbSet names can additionally be customized via EntitySetAttribute

```csharp
[EntitySets(typeof(EntityOne))]
public sealed partial class TestDbContext : DbContext;

[EntitySet(Name = "EntityOneDbSet")]
public sealed class EntityOne;

[EntitySet(Ignore = true)]
public sealed class EntityTwo;
```

The above code will generate a partial implementation for the DbContext:

```csharp
partial class TestDbContext : DbContext
{
	public Microsoft.EntityFrameworkCore.DbSet<global::Foo.Entities.EntityOne> EntityOneDbSet { get; private set; }
}
```

## EntityConfigurationsGenerator

Creates a partial implementation for the DbContext that creates a model using all of the EntityTypeConfigurations from particular namespaces. Configured via EntityConfigurationsAttribute on the DbContext. The attribute accepts multiple types as "containing types", meaning that the generator will import all valid EntityTypeConfigurations from that type's namespace. 

Imported types must:
1. Be references type
2. Implement IEntityTypeConfiguration\<TEntity\>, 
3. Have public or internal accessibility
4. Not be abstract 
5. Have a public or internal parameterless constructor.

Configurations can be ignored by using EntityConfigurationAttribute

```csharp
[EntityConfigurations(typeof(EntityOneConfiguration))]
public sealed partial class TestDbContext : DbContext;

public sealed class EntityOne;
public sealed class EntityTwo;

public sealed class EntityOneConfiguration : IEntityTypeConfiguration<EntityOne>
{
	public void Configure(EntityTypeBuilder<EntityOne> builder) { }
}

[EntityConfiguration(Ignore = true)]
public sealed class EntityTwoConfiguration : IEntityTypeConfiguration<EntityTwo>
{
	public void Configure(EntityTypeBuilder<EntityTwo> builder) { }
}
```

As an optimisation, the generator will first create a static class with singleton configurations

```csharp
internal static class TestDbContextConfigurations
{
	internal static readonly global::Foo.Configurations.EntityOneConfiguration Your_Namespace_EntityOneConfiguration = new();
}
```

If there's no override for OnModelCreating on the DbContext, we will produce an override for OnModelCreating. Additional configurations can be done after by implementing a partial declaration for OnModelCreatingPartial

```csharp
partial class TestDbContext
{
	protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfiguration(global::AZ.Generator.EntityFrameworkCore.Configurations.TestDbContextConfigurations.Your_Namespace_EntityOneConfiguration);

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder);
}
```

If there's already an override, the generator will produce an additional method on the context instead, that can be used to configure the model

```csharp
partial class TestDbContext
{
	private void ApplyEntityConfigurations(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(global::AZ.Generator.EntityFrameworkCore.Configurations.TestDbContextConfigurations.Your_Namespace_EntityOneConfiguration);
	}
}
```