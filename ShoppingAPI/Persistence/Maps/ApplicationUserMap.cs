using System.Data.Entity.ModelConfiguration;
using ShoppingAPI.Core.Models;

namespace ShoppingAPI.Persistence.Maps
{
    public class ApplicationUserMap : EntityTypeConfiguration<ApplicationUser>
    {
        public ApplicationUserMap()
        {
            HasRequired(a => a.ShoppingBasket)
                .WithRequiredPrincipal(s => s.ApplicationUser);
        }
    }
}