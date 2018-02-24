using System;
using System.Data.Entity.Migrations;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using ShoppingAPI;
using ShoppingAPI.Migrations;

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
                new DbMigrator(new Configuration()).Update();
            }
            catch (Exception)
            {
                //TODO later: do something here. 
            }
        }
    }
}