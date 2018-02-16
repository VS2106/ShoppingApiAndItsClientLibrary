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
            Ignore(t => t.Id);
            Property(t => t.ApplicationUserId)
                .HasColumnName("strApplicationUserId");


            HasMany(t => t.OrderItems)
                .WithRequired(i => i.ShoppingBasket)
                .HasForeignKey(i => i.ShoppingBasketId);
        }
    }
}