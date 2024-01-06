namespace Uploadify.Server.Core.Infrastructure.Types.Extensions;

public static class AsyncEnumerableExtensions
{
    public static Task<List<TItem>> ToListAsync<TItem>(this IAsyncEnumerable<TItem> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source), $"[{nameof(AsyncEnumerableExtensions)}] Null reference exception. Parameter: '{nameof(source)}' Value: '{null}'.");
        }

        return ExecuteAsync();

        async Task<List<TItem>> ExecuteAsync()
        {
            var list = new List<TItem>();

            await foreach (TItem element in source)
            {
                list.Add(element);
            }

            return list;
        }
    }
}
