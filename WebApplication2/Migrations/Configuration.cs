namespace WebApplication2.Migrations
{
    using Context;
    using Security;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WebApplication2.Context.BaseDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(WebApplication2.Context.BaseDbContext context)
        {
            Models.AccountGroup accountGroup = null;
            if (!AccountGroupDbContext.getInstance().isDefaultGroupExists())
            {
                accountGroup = AccountGroupDbContext.getInstance().create(
                    new Models.AccountGroup
                    {
                        Name = "Default Group",
                    }
                );
            }
            else
            {
                accountGroup = AccountGroupDbContext.getInstance().getDefaultGroup();
            }

            if (!AccountDbContext.getInstance().isSuperadminExists())
            {
                AccountDbContext.getInstance().tryRegisterAccount(
                    new Models.Account
                    {
                        Username = "superadmin",
                        Email = "superadmin@superadmin.com",
                        Role = "superadmin",
                        GroupID = accountGroup.AccountGroupID,
                        Firstname = "super",
                        Lastname = "admin",
                        Password = "123123asd",
                        ConfirmPassword = "123123asd",
                        isEnabled = true,
                    }
                );
            }
            if (!AccountDbContext.getInstance().isEditorExists())
            {
                AccountDbContext.getInstance().tryRegisterAccount(
                    new Models.Account
                    {
                        Username = "editor",
                        Email = "editor@editor.com",
                        Role = "editor",
                        GroupID = accountGroup.AccountGroupID,
                        Firstname = "editor",
                        Lastname = "editor",
                        Password = "123123asd",
                        ConfirmPassword = "123123asd",
                        isEnabled = true,
                    }
                );
            }
            if (!AccountDbContext.getInstance().isApproverExists())
            {
                AccountDbContext.getInstance().tryRegisterAccount(
                    new Models.Account
                    {
                        Username = "approver",
                        Email = "approver@approver.com",
                        Role = "approver",
                        GroupID = accountGroup.AccountGroupID,
                        Firstname = "approver",
                        Lastname = "approver",
                        Password = "123123asd",
                        ConfirmPassword = "123123asd",
                        isEnabled = true,
                    }
                );
            }
            if (!AccountDbContext.getInstance().isPublisherExists())
            {
                AccountDbContext.getInstance().tryRegisterAccount(
                    new Models.Account
                    {
                        Username = "publisher",
                        Email = "publisher@publisher.com",
                        Role = "publisher",
                        GroupID = accountGroup.AccountGroupID,
                        Firstname = "publisher",
                        Lastname = "publisher",
                        Password = "123123asd",
                        ConfirmPassword = "123123asd",
                        isEnabled = true,
                    }
                );
            }

            ContentPageDbContext.getInstance().tryCreateNewArticle(
                new Models.ContentPage
                {
                    Name = "About Us",
                    Url = "about_us",
                    Slug = "about-us",
                    Desc = "About Us Description",
                    Excerpt = "About Us Summary",
                    Keywords = "about,us",
                },
                "123123asd"
            );

            ContentPageDbContext.getInstance().tryCreateNewArticle(
                new Models.ContentPage
                {
                    Name = "Contact Us",
                    Url = "contact",
                    Slug = "contact-us",
                    Desc = "Contact Us Description",
                    Excerpt = "Contact Us Summary",
                    Keywords = "contact,us",
                },
                "123123asd"
            );

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
