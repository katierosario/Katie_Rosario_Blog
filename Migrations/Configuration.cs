namespace Katie_Rosario_Blog.Migrations
{
    using Katie_Rosario_Blog.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Katie_Rosario_Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Katie_Rosario_Blog.Models.ApplicationDbContext context)
        {
        var roleManager = new RoleManager<IdentityRole>(
                            new RoleStore<IdentityRole>(context));

        if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

        if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            #region Add User creation here
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (!context.Users.Any(u => u.Email == "katieliz@gmail.com"))
            {
                var user = new ApplicationUser
                { 
                UserName = "katieliz@gmail.com",
                Email = "katieliz@gmail.com",
                FirstName = "Katie",
                LastName = "Rosario",
                DisplayName = "Owner"
                };
            

            userManager.Create(user, "Abc&123!");

                userManager.AddToRoles(user.Id, "Admin");

            }

            if (!context.Users.Any(u => u.Email == "JasonTwichell@coderfoundry.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "JasonTwichell@coderfoundry.com",
                    Email = "JasonTwichell@coderfoundry.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "Prof1"
                };


                userManager.Create(user, "Abc&123!");

                userManager.AddToRoles(user.Id, "Moderator");

            }

            if (!context.Users.Any(u => u.Email == "arussell@coderfoundry.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "arussell@coderfoundry.com",
                    Email = "arussell@coderfoundry.com",
                    FirstName = "Andrew",
                    LastName = "Russell",
                    DisplayName = "Prof2"
                };


                userManager.Create(user, "Abc&123!");

                userManager.AddToRoles(user.Id, "Moderator");

            }
            #endregion

        }
    }
}
