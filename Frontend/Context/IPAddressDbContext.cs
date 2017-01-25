using Frontend.Models;
using Frontend.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Frontend.Context
{
    public class IPAddressDbContext : BaseDbContext
    {
        public DbSet<IPAddress> ipaddressDb { get; set; }
    }
}