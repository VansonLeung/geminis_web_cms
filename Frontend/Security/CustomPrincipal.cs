using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using WebApplication2.Models;

namespace Frontend.Security
{
    public class CustomPrincipal : IPrincipal
    {
        private User User;

        public CustomPrincipal(User account)
        {
            this.User = account;
            this.Identity = new GenericIdentity(account.username);
        }

        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            if (role == null || role.Trim() == "")
            {
                return true;
            }
            return false;
        }
    }
}