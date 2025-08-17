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
