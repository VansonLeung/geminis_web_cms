using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using WebApplication2.Security;

namespace WebApplication2.Context
{
    public class AccountDbContext
    {
        // singleton

        private static AccountDbContext accountDbContext;

        public static AccountDbContext getInstance()
        {
            if (accountDbContext == null)
            {
                accountDbContext = new AccountDbContext();
            }
            return accountDbContext;
        }



        // initializations

        private BaseDbContext db = new BaseDbContext();

        protected DbSet<Account> getAccountDb()
        {
            return db.accountDb;
        }



        // methods

        public bool isSuperadminExists()
        {
            return getAccountDb().Where(acc => acc.Role.Contains("superadmin")).Count() > 0;
        }

        public bool isEditorExists()
        {
            return getAccountDb().Where(acc => acc.Role.Contains("editor")).Count() > 0;
        }

        public bool isApproverExists()
        {
            return getAccountDb().Where(acc => acc.Role.Contains("approver")).Count() > 0;
        }

        public bool isPublisherExists()
        {
            return getAccountDb().Where(acc => acc.Role.Contains("publisher")).Count() > 0;
        }


        public List<Account> findAccounts()
        {
            return getAccountDb()
                .Include(acc => acc.Group)
                .ToList();
        }

        public Account findAccountByID(int accountID)
        {
            return getAccountDb().Where(acc => acc.AccountID == accountID)
                .Include(acc => acc.Group)
                .FirstOrDefault();
        }

        public Account findAccountByAccountUsername(Account account)
        {
            return getAccountDb().Where(acc => acc.Username == account.Username)
                .Include(acc => acc.Group)
                .FirstOrDefault();
        }

        public Account tryLoginAccountByAccount(Account account)
        {
            var encPassword = account.MakeEncryptedPassword(account.Password);
            Account _account = findAccountByAccountUsername(account);
            if (_account != null)
            {
                if (_account.Password == encPassword)
                {
                    if (_account.LoginFails < 3)
                    {
                        db.Entry(_account).State = EntityState.Modified;
                        _account.LastLogin = DateTime.UtcNow;
                        _account.LoginFails = 0;
                        db.SaveChanges();
                        SessionPersister.createSessionForAccount(_account);
                    }
                }
                else
                {
                    db.Entry(_account).State = EntityState.Modified;
                    _account.LoginFails = _account.LoginFails + 1;
                    db.SaveChanges();
                }
            }
            return _account;
        }

        public void tryLogout()
        {
            SessionPersister.removeSession();
        }

        public void tryRegisterAccount(Account account)
        {
            var encPassword = account.MakeEncryptedPassword(account.Password);
            account.Password = encPassword;
            account.ConfirmPassword = encPassword;
            account.LastPasswordModifiedAt = DateTime.UtcNow;
            account.historyPasswords = account.Password;
            getAccountDb().Add(account);
            db.SaveChanges();
        }

        public string tryEdit(Account account)
        {
            string error = null;
            Account _account = findAccountByID(account.AccountID);
            if (_account.Password != account.Password)
            {
                var encPassword = account.MakeEncryptedPassword(account.Password);
                error = tryChangePassword(account, encPassword);
                if (error != null)
                {
                    return error;
                }
                else
                {
                    db.Entry(_account).State = EntityState.Modified;
                    _account.LoginFails = 0;

                    if (!_account.Role.Contains("superadmin"))
                    {
                        _account.NeedChangePassword = true;
                    }

                    db.SaveChanges();
                }
            }

            error = tryChangeRole(account);
            if (error != null)
            {
                return error;
            }


            return null;
        }

        public string tryChangePassword(Account account, String newPassword, bool shouldInvalidateResetPasswordNeeds = false)
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




                db.Entry(_account).State = EntityState.Modified;
                _account.Password = newPassword;
                _account.ConfirmPassword = newPassword;
                _account.LastPasswordModifiedAt = DateTime.UtcNow;

                if (shouldInvalidateResetPasswordNeeds)
                {
                    _account.NeedChangePassword = false;
                }

                passwords.Add(newPassword);
                while (passwords.Count > 9)
                {
                    passwords.RemoveAt(0);
                }

                _account.historyPasswords = _account.historyPasswordsFromList(passwords);
                SessionPersister.updateSessionForAccount();
                db.SaveChanges();
                return null;
            }
            else
            {
                return "Change password failed: Account not found";
            }
        }

        public string tryChangeRole(Account account)
        {
            Account _account = findAccountByID(account.AccountID);
            if (_account != null)
            {
                db.Entry(_account).State = EntityState.Modified;

                if (account.RoleList != null)
                {
                    account.Role = String.Join(",", account.RoleList);
                }

                _account.Role = account.Role;

                SessionPersister.updateSessionForAccount();
                db.SaveChanges();
                return null;
            }
            else
            {
                return "Change password failed: Account not found";
            }
        }



        public string tryDeleteAccount(Account account)
        {
            getAccountDb().Remove(account);
            db.SaveChanges();
            return null;
        }

    }
}