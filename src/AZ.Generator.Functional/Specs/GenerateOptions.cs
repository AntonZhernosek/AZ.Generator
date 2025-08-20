namespace AZ.Generator.Functional.Specs;

[Flags]
internal enum GenerateOptions
{
	None = 0,
	GenerateJson = 1 << 0,
}
