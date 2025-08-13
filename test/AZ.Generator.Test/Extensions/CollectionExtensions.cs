namespace AZ.Generator.Test.Extensions;

internal static class CollectionExtensions
{
	public static void AssetContainsKeys<TKey, TItem>(this Dictionary<TKey, TItem> first, Dictionary<TKey, TItem> second)
		where TKey : notnull
	{
		foreach (var key in second.Keys)
		{
			Assert.True(first.ContainsKey(key), "Asserted dictionary should contain all the required keys!");
		}
	}

	public static void AssertEqual<T>(this IEnumerable<T> source, IEnumerable<T> other, string? message = default) => Assert.True(source.SequenceEqual(other), message);
}
