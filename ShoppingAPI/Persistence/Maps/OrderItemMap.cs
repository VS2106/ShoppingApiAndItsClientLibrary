using System.Data.Entity.ModelConfiguration;
using ShoppingAPI.Core.Models;

namespace ShoppingAPI.Persistence.Maps
{
    public class OrderItemMap : EntityTypeConfiguration<OrderItem>
    {
        public OrderItemMap()
        {
            HasKey(t => t.Id);

            ToTable("tblOrderItem");
            Property(t => t.Id)
                .HasColumnName("intOrderItemId");
            Property(t => t.ProductId)
                .HasColumnName("intProductId")
                .IsRequired();
            Property(t => t.Quantity)
                .HasColumnName("intQuantity")
                .IsRequired()
                .IsConcurrencyToken();
            Property(t => t.ShoppingBasketId)
                .HasColumnName("strApplicationUserId")
                .IsRequired();
            /*Apply an unique index on multiple columns ProductId, ShoppingBasketId
             *In one shopping basket, there won't be multiple OrderItems with the same product. */
            HasIndex(t => new { t.ProductId, t.ShoppingBasketId })
                .IsUnique();
        }
    }
}