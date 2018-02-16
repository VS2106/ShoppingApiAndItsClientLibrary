using System.Data.Entity.ModelConfiguration;
using ShoppingAPI.Core.Models;

namespace ShoppingAPI.Persistence.Maps
{
    public class ShoppingBasketMap : EntityTypeConfiguration<ShoppingBasket>
    {
        public ShoppingBasketMap()
        {
            HasKey(t => t.ApplicationUserId);

            ToTable("tblShoppingBasket");
            Property(t => t.ApplicationUserId)
                .HasColumnName("intApplicationUserId");

            HasMany(t => t.OrderItems)
                .WithRequired(i => i.ShoppingBasket);
        }
    }
}