﻿global using static CozyNestAPIHub.ItemAcquisitionFromContext;
using System.Collections.Concurrent;

namespace CozyNestAPIHub
{
    public class ItemAcquisitionFromContext
    {
        public static async Task<T> GetItemFromContext<T>(HttpContext context, string itemName) where T : class
        {
            if (!context.Items.TryGetValue(itemName, out var item))
            {
                throw new InvalidOperationException($"Item '{itemName}' not found in HttpContext.Items.");
            }
            return await Task.FromResult(item as T);
        }
    }
}
