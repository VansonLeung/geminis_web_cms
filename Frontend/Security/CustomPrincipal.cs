using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using Frontend.Models;

namespace Frontend.Security
{
    public class CustomPrincipal : IPrincipal
    {
        private User User;

        public CustomPrincipal(User account)
        {
            this.User = account;
            this.Identity = new GenericIdentity(account.Username);
        }

        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            if (role == null || role.Trim() == "")
            {
                return true;
            }
            var roles = role.Split(new char[] { ',' });
            return roles.Any(r => this.User.Role == r);
        }
    }
}