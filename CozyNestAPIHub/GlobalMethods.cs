global using static CozyNestAPIHub.GlobalMethods;
using System.Collections.Concurrent;

namespace CozyNestAPIHub
{
    public class GlobalMethods
    {
        public static async Task<T> GetItemFromContext<T>(HttpContext context, string itemName) where T : class
        {
            if (!context.Items.TryGetValue(itemName, out var item))
            {
                throw new InvalidOperationException($"Item '{itemName}' not found in the context of this request.");
            }
            return await Task.FromResult(item as T);
        }
    }
}
