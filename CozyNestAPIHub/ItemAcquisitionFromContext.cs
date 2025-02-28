global using static CozyNestAPIHub.ItemAcquisitionFromContext;

namespace CozyNestAPIHub
{
    public class ItemAcquisitionFromContext
    {
        public static T GetItemFromContext<T>(HttpContext context, string itemName) where T : class
        {
            return context.Items[itemName] as T;
        }
    }
}
