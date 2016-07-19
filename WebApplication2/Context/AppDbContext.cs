using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using WebApplication2.Security;

namespace WebApplication2.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Account> accountDb { get; set; }

        public Account findAccountByID(int accountID)
        {
            return accountDb.Where(acc => acc.AccountID == accountID).FirstOrDefault();
        }

        public Account findAccountByAccount(Account account)
        {
            return accountDb.Where(acc => acc.Username == account.Username && acc.Password == acc.Password).FirstOrDefault();
        }

        public Account tryLoginAccountByAccount(Account account)
        {
            Account _account = findAccountByAccount(account);
            if (_account != null)
            {
                SessionPersister.account = _account;
            }
            return _account;
        }

        public void tryLogout()
        {
            SessionPersister.account = null;
        }
    }
}