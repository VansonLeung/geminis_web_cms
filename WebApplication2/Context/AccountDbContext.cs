using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Helpers;
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

        private BaseDbContext db = BaseDbContext.getInstance();

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

        public List<Account> findAccountsByEmail(string email)
        {
            return getAccountDb().Where(acc => acc.Email == email)
                .ToList();
        }

        public List<Account> findAccountsByRole(string role)
        {
            return getAccountDb().Where(acc => acc.Role.Contains(role))
                .ToList();
        }

        public Account tryLoginAccountByAccount(Account account)
        {
            var encPassword = account.MakeEncryptedPassword(account.Password);
            Account _account = findAccountByAccountUsername(account);
            if (_account != null)
            {
                if (_account.Password == encPassword)
                {
                    if (!_account.isEnabled)
                    {
                        return _account;
                    }
                    else if (_account.LoginFails < 3)
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
                    return null;
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

            if (account.RoleList != null)
            {
                account.Role = String.Join(",", account.RoleList);
            }

            getAccountDb().Add(account);
            db.SaveChanges();
        }

        public string tryEdit(Account account)
        {
            string error = null;
            Account _account = findAccountByID(account.AccountID);
            if (_account.Password != account.Password)
            {
                var rawPassword = account.Password;
                error = tryChangePassword(account, rawPassword);
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

                        EmailHelper.SendEmailToAccountOnPasswordReset(_account, rawPassword);
                    }

                    db.SaveChanges();
                }
            }

            error = tryChangeProfile(account);
            if (error != null)
            {
                return error;
            }


            return null;
        }

        public string tryChangePassword(Account account, String newPassword, bool shouldInvalidateResetPasswordNeeds = false)
        {
            var encPassword = account.MakeEncryptedPassword(newPassword);
            Account _account = findAccountByID(account.AccountID);
            if (_account != null)
            {
                var passwords = _account.historyPasswordList();

                // check if this password is already used in the list
                // if yes, then return error message
                for (var i = 0; i < passwords.Count; i++)
                {
                    var pass = passwords[i];
                    if (pass == encPassword)
                    {
                        return "New password must be different from your 9 previously used passwords";
                    }
                }




                db.Entry(_account).State = EntityState.Modified;
                _account.Password = encPassword;
                _account.ConfirmPassword = encPassword;
                _account.LastPasswordModifiedAt = DateTime.UtcNow;

                if (shouldInvalidateResetPasswordNeeds)
                {
                    _account.NeedChangePassword = false;
                }

                passwords.Add(encPassword);
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

        public string tryChangeProfile(Account account)
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
                _account.Username = account.Username;
                _account.Email = account.Email;
                _account.Firstname = account.Firstname;
                _account.Lastname = account.Lastname;
                _account.GroupID = account.GroupID;
                _account.isEnabled = account.isEnabled;

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