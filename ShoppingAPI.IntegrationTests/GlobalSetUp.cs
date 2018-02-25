using System;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ShoppingAPI.App_Start;
using ShoppingAPI.Core.Models;
using ShoppingAPI.Migrations;
using ShoppingAPI.Persistence;

namespace ShoppingAPI.IntegrationTests
{
    [SetUpFixture]
    public class GlobalSetUp
    {
        public static string _currentUserId { get; private set; }
        public static string _currentUserName { get; private set; }
        private ShoppingApiDbContext _context;

        [OneTimeSetUp]
        public void SetUp()
        {
            InstantiateContext();
            InitializeDb();
            SeedTestData();
            SetCurrentUserUserA();
            InitializeAutoMap();
            DisposeContext();
        }

        private void DisposeContext()
        {
            _context.Dispose();
        }

        private static void InitializeAutoMap()
        {
            AutoMapperConfig.Initialize();
        }

        private void SeedTestData()
        {
            SeedUsers();
            SeedProducts();
        }

        private void InstantiateContext()
        {
            _context = new ShoppingApiDbContext();
        }

        private static void InitializeDb()
        {
            AppDomain.CurrentDomain.SetData(
                "DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""));
            new DbMigrator(new Configuration()).Update();
        }

        public void SeedUsers()
        {
            if (!_context.Users.Any(u => u.UserName == "UserA"))
                _context.Users.Add(new ApplicationUser { UserName = "UserA", Email = "-", PasswordHash = "-" });
            if (!_context.Users.Any(u => u.UserName == "UserB"))
                _context.Users.Add(new ApplicationUser { UserName = "UserB", Email = "-", PasswordHash = "-" });
            _context.SaveChanges();

        }
        private void SeedProducts()
        {
            if (!_context.Products.Any(u => u.Name == "ProductA"))
                _context.Products.Add(new Product { Name = "ProductA", StockQuantity = 15 });
            if (!_context.Products.Any(u => u.Name == "ProductB"))
                _context.Products.Add(new Product { Name = "ProductB", StockQuantity = 20 });
            _context.SaveChanges();
        }

        private void SetCurrentUserUserA()
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.UserName == "UserA");
            _currentUserId = currentUser.Id;
            _currentUserName = currentUser.UserName;
        }
    }
}