using System;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ShoppingAPI.Core.Models;
using ShoppingAPI.Migrations;
using ShoppingAPI.Persistence;

namespace ShoppingAPI.IntegrationTests
{
    [SetUpFixture]
    public class GlobalSetUp
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            InitializeDb();
            SeedUsers();
            SeedProducts();
        }

        private static void InitializeDb()
        {
            AppDomain.CurrentDomain.SetData(
                "DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""));
            new DbMigrator(new Configuration()).Update();
        }

        public void SeedUsers()
        {
            var context = new ShoppingApiDbContext();
            if (!context.Users.Any(u => u.UserName == "UserA"))
                context.Users.Add(new ApplicationUser { UserName = "UserA", Email = "-", PasswordHash = "-" });
            if (!context.Users.Any(u => u.UserName == "UserB"))
                context.Users.Add(new ApplicationUser { UserName = "UserB", Email = "-", PasswordHash = "-" });
            context.SaveChanges();
        }
        private void SeedProducts()
        {
            var context = new ShoppingApiDbContext();
            if (!context.Products.Any(u => u.Name == "ProductA"))
                context.Products.Add(new Product { Name = "ProductA", StockQuantity = 15 });
            if (!context.Products.Any(u => u.Name == "ProductB"))
                context.Products.Add(new Product { Name = "ProductB", StockQuantity = 20 });
            context.SaveChanges();
        }

    }
}