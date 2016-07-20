using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Context
{
    public class BaseDbContext : DbContext
    {
        public override int SaveChanges()
        {
            AddTimeStamps();
            return base.SaveChanges();
        }
        
        protected void AddTimeStamps()
        {

            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseModel && (x.State == EntityState.Added || x.State == EntityState.Modified));

            var currentUsername = !string.IsNullOrEmpty(System.Web.HttpContext.Current?.User?.Identity?.Name)
                ? HttpContext.Current.User.Identity.Name
                : "Anonymous";

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseModel)entity.Entity).created_at = DateTime.UtcNow;
                }

                ((BaseModel)entity.Entity).modified_at = DateTime.UtcNow;
            }

        }
    }
}