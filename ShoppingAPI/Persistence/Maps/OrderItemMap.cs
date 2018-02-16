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
            /*ShoppingBasketId and ProductId of OrderItem is multi column unique index
             *For a shopping basket, there won't be multi OrderItems with the same product. */
            HasIndex(t => new { t.ProductId, t.ShoppingBasketId })
                .IsUnique();
        }
    }
}