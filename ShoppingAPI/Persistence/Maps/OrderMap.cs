using System.Data.Entity.ModelConfiguration;
using ShoppingAPI.Core.Models;

namespace ShoppingAPI.Persistence.Maps
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            HasKey(t => t.Id);

            ToTable("tblOrder");
            Property(t => t.Id)
                .HasColumnName("intOrderId");
            Property(t => t.IdentityUserId)
                .HasColumnName("strIdentityUserId")
                .IsRequired();

            HasMany(t => t.OrderItems)
                .WithRequired()
                .WillCascadeOnDelete(true);
        }
    }
}