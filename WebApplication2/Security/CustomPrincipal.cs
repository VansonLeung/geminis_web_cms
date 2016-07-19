using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using WebApplication2.Models;

namespace WebApplication2.Security
{
    public class CustomPrincipal : IPrincipal
    {
        private Account Account;

        public CustomPrincipal(Account account)
        {
            this.Account = account;
            this.Identity = new GenericIdentity(account.Username);
        }

        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            var roles = role.Split(new char[] { ',' });
            return roles.Any(r => this.Account.Role == r);
        }
    }
}