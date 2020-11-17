using System.Security.Claims;
using MoneyManagerService.Constants;

namespace MoneyManagerService.Core
{
    public static class Extensions
    {
        public static bool TryGetUserId(this ClaimsPrincipal principal, out int userId)
        {
            userId = -1;

            var nameIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

            if (nameIdClaim == null)
            {
                return false;
            }

            if (int.TryParse(nameIdClaim.Value, out int value))
            {
                userId = value;
                return true;
            }

            return false;
        }

        public static bool IsAdmin(this ClaimsPrincipal principal)
        {
            return principal.IsInRole(UserRoleName.Admin);
        }

        public static bool HasProperty(this object obj, string property)
        {
            if (obj == null)
            {
                return false;
            }

            return obj.GetType().GetProperty(property) != null;
        }
    }
}