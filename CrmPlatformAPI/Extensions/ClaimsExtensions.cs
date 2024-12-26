using System.Security.Claims;

namespace CrmPlatformAPI.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            var username = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if(username == null)
            {
                throw new Exception("Cannot find username in claims");
            }

            return username;
        }

    }
}
