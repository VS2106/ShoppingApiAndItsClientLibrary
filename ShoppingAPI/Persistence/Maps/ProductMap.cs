using System.Data.Entity.ModelConfiguration;
using ShoppingAPI.Core.Models;

namespace ShoppingAPI.Persistence.Maps
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            HasKey(t => t.Id);

            ToTable("tblProduct");
            Property(t => t.Id)
                .HasColumnName("intProductId");
            Property(t => t.Name)
                .HasColumnName("strName")
                .HasMaxLength(200)
                .IsRequired();
            Property(t => t.StockQuantity)
                .HasColumnName("intStockQuantity")
                .IsRequired();
        }
    }
}