using Microsoft.AspNet.Identity.EntityFramework;
using ShoppingAPI.Core.Models;

namespace ShoppingAPI.Persistence
{
    public class ShoppingApiDbContext : IdentityDbContext<ApplicationUser>
    {
        public ShoppingApiDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ShoppingApiDbContext Create()
        {
            return new ShoppingApiDbContext();
        }
    }
}