namespace Marcet_Api_V2.Repository
{
    public static class HttpContextExtensions
    {
        private const string CurrentUserIdKey = "CurrentUserId";

        public static string GetCurrentUserId(this HttpContext context)
        {
            return context.Items[CurrentUserIdKey] as string;
        }
        public static void SetCurrentUserId(this HttpContext context, string currentUserId)
        {
            context.Items[CurrentUserIdKey] = currentUserId;
        }
    }
}
