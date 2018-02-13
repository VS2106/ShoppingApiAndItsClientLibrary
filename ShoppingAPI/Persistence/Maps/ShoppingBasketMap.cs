using System.Data.Entity.ModelConfiguration;
using ShoppingAPI.Core.Models;

namespace ShoppingAPI.Persistence.Maps
{
    public class ShoppingBasketMap : EntityTypeConfiguration<ShoppingBasket>
    {
        public ShoppingBasketMap()
        {
            HasKey(t => t.Id);

            ToTable("tblShoppingBasket");
            Property(t => t.Id)
                .HasColumnName("intShoppingBasketId");
            Property(t => t.IdentityUserId)
                .HasColumnName("strIdentityUserId")
                .IsRequired();

            HasMany(t => t.OrderItems)
                .WithRequired()
                .WillCascadeOnDelete(true);
        }
    }
}