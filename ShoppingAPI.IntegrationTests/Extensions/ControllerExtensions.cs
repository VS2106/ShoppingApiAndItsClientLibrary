using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;

namespace ShoppingAPI.IntegrationTests.Extensions
{
    public static class ControllerExtensions
    {
        public static void MockCurrentUser(this ApiController controller, string userId, string userName)
        {
            var identity = new GenericIdentity(userName);

            identity.AddClaim(
                new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(
                new Claim(ClaimTypes.NameIdentifier, userId));

            controller.User = new GenericPrincipal(identity, null);
        }

        public static void MockCurrentUser(this ApiController controller)
        {
            controller.MockCurrentUser(GlobalSetUp._currentUserId, GlobalSetUp._currentUserName);
        }
    }
}