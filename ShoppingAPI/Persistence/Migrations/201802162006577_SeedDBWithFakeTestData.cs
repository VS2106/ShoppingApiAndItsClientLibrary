namespace ShoppingAPI.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class SeedDBWithFakeTestData : DbMigration
    {
        /// <summary>
        /// seed product, user and users shopping basket
        /// user test1@api.com passwrod !TestWebApi1
        /// User test2@api.com passwrod !TestWebApi2
        /// </summary>
        public override void Up()
        {
            Sql(
              @"INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'9bf37d84-8669-4130-bce2-f3c4bb7ed52c', N'test1@api.com', 0, N'ANza7RGbTJR/0qaE3KSJxAn972PkcR9c3h2vMtqNRbkQJHMuv4P5xeOLHgvCADVZYQ==', N'63f19c08-933c-4aae-acb5-098fed4763bd', NULL, 0, 0, NULL, 0, 0, N'test1@api.com')
INSERT INTO [dbo].[AspNetUsers]([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES(N'f72fb25c-0674-4f2b-b6e7-4576b9a6aa16', N'test2@api.com', 0, N'AJwGUPhvZNZB6khZGSkvyxIGkQLbs5xknJnt467vpgY64RVKG5grfx8Jn2rZMxLXZQ==', N'ef41c7aa-ae5d-4f12-bac1-d02f0c6ac786', NULL, 0, 0, NULL, 0, 0, N'test2@api.com')

INSERT INTO [dbo].[tblShoppingBasket] ([strApplicationUserId]) VALUES (N'9bf37d84-8669-4130-bce2-f3c4bb7ed52c')
INSERT INTO [dbo].[tblShoppingBasket] ([strApplicationUserId]) VALUES (N'f72fb25c-0674-4f2b-b6e7-4576b9a6aa16')

SET IDENTITY_INSERT [dbo].[tblProduct] ON
INSERT INTO [dbo].[tblProduct] ([intProductId], [strName], [intStockQuantity]) VALUES (1, N'Chocolate Cadbury assd. 200 ', 10)
INSERT INTO [dbo].[tblProduct] ([intProductId], [strName], [intStockQuantity]) VALUES (2, N'Nuts Cashew 150 g', 20)
INSERT INTO [dbo].[tblProduct] ([intProductId], [strName], [intStockQuantity]) VALUES (3, N'Potato Chips Pringles 190', 23)
INSERT INTO [dbo].[tblProduct] ([intProductId], [strName], [intStockQuantity]) VALUES (4, N'Red Wine Cabernet Merlot 750 ml', 14)
INSERT INTO [dbo].[tblProduct] ([intProductId], [strName], [intStockQuantity]) VALUES (5, N'Red Wine Stony Cape 3000 ml', 8)
INSERT INTO [dbo].[tblProduct] ([intProductId], [strName], [intStockQuantity]) VALUES (6, N'Pepsi Cola 330 ml', 10)
INSERT INTO [dbo].[tblProduct] ([intProductId], [strName], [intStockQuantity]) VALUES (7, N'Absolut 1000 ml', 3)
INSERT INTO [dbo].[tblProduct] ([intProductId], [strName], [intStockQuantity]) VALUES (8, N'NIVEA Body Lotion 250 ml', 8)
SET IDENTITY_INSERT [dbo].[tblProduct] OFF");
        }

        public override void Down()
        {
            Sql(
                @"DELETE  [dbo].[AspNetUsers] WHERE [Id] IN ('9bf37d84-8669-4130-bce2-f3c4bb7ed52c','f72fb25c-0674-4f2b-b6e7-4576b9a6aa16')
DELETE  [dbo].[tblShoppingBasket] WHERE [intApplicationUserId] IN ('9bf37d84-8669-4130-bce2-f3c4bb7ed52c','f72fb25c-0674-4f2b-b6e7-4576b9a6aa16')
DELETE  [dbo].[tblProduct] WHERE [intProductId] IN (1, 2, 3, 4, 5, 6, 7, 8)");
        }
    }
}
