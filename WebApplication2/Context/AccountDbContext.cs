using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using WebApplication2.Security;

namespace WebApplication2.Context
{
    public class AccountDbContext : BaseDbContext
    {
        public DbSet<Account> accountDb { get; set; }
        

        public List<Account> findAccounts()
        {
            return accountDb.ToList();
        }

        public Account findAccountByID(int accountID)
        {
            return accountDb.Where(acc => acc.AccountID == accountID).FirstOrDefault();
        }

        public Account findAccountByAccount(Account account)
        {
            return accountDb.Where(acc => acc.Username == account.Username && acc.Password == account.Password).FirstOrDefault();
        }

        public Account tryLoginAccountByAccount(Account account)
        {
            Account _account = findAccountByAccount(account);
            if (_account != null)
            {
                Entry(_account).State = EntityState.Modified;
                _account.LastLogin = DateTime.UtcNow;
                SaveChanges();
                SessionPersister.createSessionForAccount(_account);
            }
            return _account;
        }

        public void tryLogout()
        {
            SessionPersister.removeSession();
        }

        public void tryRegisterAccount(Account account)
        {
            account.LastPasswordModifiedAt = DateTime.UtcNow;
            account.historyPasswords = account.Password;
            accountDb.Add(account);
            SaveChanges();
        }

        public string tryEdit(Account account)
        {
            string error = null;
            Account _account = findAccountByID(account.AccountID);
            if (_account.Password != account.Password)
            {
                error = tryChangePassword(account, account.Password);
                if (error != null)
                {
                    return error;
                }
            }

            error = tryChangeRole(account, account.Role);
            if (error != null)
            {
                return error;
            }


            return null;
        }

        public string tryChangePassword(Account account, String newPassword)
        {
            Account _account = findAccountByID(account.AccountID);
            if (_account != null)
            {
                var passwords = _account.historyPasswordList();

                // check if this password is already used in the list
                // if yes, then return error message
                for (var i = 0; i < passwords.Count; i++)
                {
                    var pass = passwords[i];
                    if (pass == newPassword)
                    {
                        return "New password must be different from your 9 previously used passwords";
                    }
                }




                Entry(_account).State = EntityState.Modified;
                _account.Password = newPassword;
                _account.ConfirmPassword = newPassword;
                _account.LastPasswordModifiedAt = DateTime.UtcNow;


                passwords.Add(newPassword);
                while (passwords.Count > 9)
                {
                    passwords.RemoveAt(0);
                }

                _account.historyPasswords = _account.historyPasswordsFromList(passwords);
                SessionPersister.updateSessionForAccount();
                SaveChanges();
                return null;
            }
            else
            {
                return "Change password failed: Account not found";
            }
        }

        public string tryChangeRole(Account account, String role)
        {
            Account _account = findAccountByID(account.AccountID);
            if (_account != null)
            {
                Entry(_account).State = EntityState.Modified;
                _account.Role = role;

                SessionPersister.updateSessionForAccount();
                SaveChanges();
                return null;
            }
            else
            {
                return "Change password failed: Account not found";
            }
        }
    }
}