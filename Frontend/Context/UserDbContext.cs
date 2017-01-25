﻿using Frontend.Models;
using Frontend.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Frontend.Context
{
    public class UserDbContext : BaseDbContext
    {
        public DbSet<User> accountDb { get; set; }
    }
}