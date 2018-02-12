using System;
using System.Data.Entity;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using ShoppingAPI;
using ShoppingAPI.Migrations;
using ShoppingAPI.Persistence;

[assembly: OwinStartup(typeof(Startup))]

namespace ShoppingAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            ConfigureAuth(app);
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using (var context = new ShoppingApiDbContext())
                {
                    Database.SetInitializer(
                        new MigrateDatabaseToLatestVersion<ShoppingApiDbContext, Configuration>());

                    context.Database.Initialize(true);
                }
            }
            catch (Exception)
            {
                //TODO later: do something here. 
            }
        }
    }
}