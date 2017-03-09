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



        // methods

        public bool isSuperadminExists()
        {
            using (var db = new BaseDbContext())
            {
                return db.accountDb.Where(acc => acc.Role.Contains("superadmin")).Count() > 0;
            }
        }

        public bool isEditorExists()
        {
            using (var db = new BaseDbContext())
            {
                return db.accountDb.Where(acc => acc.Role.Contains("editor")).Count() > 0;
            }
        }

        public bool isApproverExists()
        {
            using (var db = new BaseDbContext())
            {
                return db.accountDb.Where(acc => acc.Role.Contains("approver")).Count() > 0;
            }
        }

        public bool isPublisherExists()
        {
            using (var db = new BaseDbContext())
            {
                return db.accountDb.Where(acc => acc.Role.Contains("publisher")).Count() > 0;
            }
        }






        #region "query"


        public List<Account> findAccounts()
        {
            using (var db = new BaseDbContext())
            {
                return db.accountDb
                .Include(acc => acc.Group)
                .ToList();
            }
        }

        public Account findAccountByID(int accountID)
        {
            using (var db = new BaseDbContext())
            {
                return db.accountDb.Where(acc => acc.AccountID == accountID)
                .Include(acc => acc.Group)
                .FirstOrDefault();
            }
        }

        public Account findAccountByAccountUsername(Account account)
        {
            using (var db = new BaseDbContext())
            {
                return db.accountDb.Where(acc => acc.Username == account.Username)
                .Include(acc => acc.Group)
                .FirstOrDefault();
            }
        }

        public List<Account> findAccountsByEmail(string email)
        {
            using (var db = new BaseDbContext())
            {
                return db.accountDb.Where(acc => acc.Email == email)
                .ToList();
            }
        }

        public Account findAccountByAccountUsernameNoTracking(Account account)
        {
            using (var db = new BaseDbContext())
            {
                return db.accountDb.AsNoTracking().Where(acc => acc.Username == account.Username)
                .Include(acc => acc.Group)
                .FirstOrDefault();
            }
        }

        public List<Account> findAccountsByEmailNoTracking(string email)
        {
            using (var db = new BaseDbContext())
            {
                return db.accountDb.AsNoTracking().Where(acc => acc.Email == email)
                .ToList();
            }
        }

        public List<Account> findAccountsByRole(string role)
        {
            using (var db = new BaseDbContext())
            {
                return db.accountDb.Where(acc => acc.Role.Contains(role))
                .ToList();
            }
        }

        public List<Account> findAccountsByAccountGroup(AccountGroup accountGroup)
        {
            using (var db = new BaseDbContext())
            {
                var accounts = db.accountDb.Where(acc => acc.GroupID == accountGroup.AccountGroupID)
                    .ToList();
                return accounts;
            }
        }


        public List<Account> findAccountsByAccountGroupsToEmailNotify(List<AccountGroup> accountGroups, BaseArticle baseArticle)
        {
            List<int> accountGroupIDs = new List<int>();
            foreach (var group in accountGroups)
            {
                accountGroupIDs.Add(group.AccountGroupID);
            }

            if (accountGroupIDs.Count <= 0)
            {
                accountGroupIDs.Add(1);
            }

            int ownerAccountID = -1;

            if (SessionPersister.account != null)
            {
                ownerAccountID = SessionPersister.account.AccountID;
            }
            using (var db = new BaseDbContext())
            {
                return db.accountDb.AsNoTracking().Where(acc =>
                (
                    (
                        (
                            acc.EmailNotifications == 0 && accountGroupIDs.Contains(acc.GroupID ?? 1)
                        )
                        ||
                        (
                            acc.EmailNotifications == 1 && acc.AccountID == (baseArticle.createdBy ?? -2)
                        )
                        ||
                        (
                            acc.EmailNotifications == 1 && acc.AccountID == (baseArticle.approvedBy ?? -2)
                        )
                        ||
                        (
                            acc.EmailNotifications == 1 && acc.AccountID == (baseArticle.publishedBy ?? -2)
                        )
                    )
                    &&
                    (
                        (
                            acc.AccountID != ownerAccountID
                        )
                        ||
                        (
                            acc.AccountID == ownerAccountID && !acc.EmailNotificationsNotNotifyOwnChangesToMySelf
                        )
                    )
                )).ToList();
            }
        }


        #endregion "query"



        public Account tryLoginAccountByAccount(Account account)
        {
            using (var db = new BaseDbContext())
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
        }

        public void tryLogout()
        {
            SessionPersister.removeSession();
        }

        public string tryCheckUsername(Account account)
        {
            var acc = findAccountByAccountUsernameNoTracking(account);
            if (acc != null && account.AccountID == 0)
            {
                return "This Username already exists. Please enter another Username.";
            }
            if (acc == null)
            {
                return null;
            }
            if (account.AccountID != 0 && acc.AccountID != account.AccountID)
            {
                return "This Username already exists. Please enter another Username.";
            }

            return null;
        }

        public string tryCheckEmail(Account account)
        {
            return null;

            var acc = findAccountsByEmailNoTracking(account.Email);
            if (acc != null && acc.Count > 0 && account.AccountID == 0)
            {
                return "This Username already exists. Please enter another Username.";
            }
            if (account.AccountID != 0 && acc.Count > 0)
            {
                foreach (var a in acc)
                {
                    if (a.AccountID != account.AccountID)
                    {
                        return "This Username already exists. Please enter another Username.";
                    }
                }
            }

            return null;
        }

        public string tryRegisterAccount(Account account, bool isSeeding = false)
        {
            var error = tryCheckUsername(account);
            if (error != null)
            {
                return error;
            }

            error = tryCheckEmail(account);
            if (error != null)
            {
                return error;
            }

            var rawPassword = account.Password;
            var encPassword = account.MakeEncryptedPassword(account.Password);
            account.Password = encPassword;
            account.ConfirmPassword = encPassword;
            account.LastPasswordModifiedAt = DateTime.UtcNow;
            account.historyPasswords = account.Password;

            if (account.RoleList != null)
            {
                account.Role = String.Join(",", account.RoleList);
            }
            else if (account.Role == null)
            {
                account.Role = "";
            }

            account.NeedChangePassword = true;

            if (!isSeeding)
            {
                EmailHelper.SendEmailToAccountOnPasswordCreate(account, rawPassword);
            }

            using (var db = new BaseDbContext())
            {
                db.accountDb.Add(account);
                db.SaveChanges();
            }
            return null;
        }

        public string tryEdit(Account account)
        {
            string error = null;
            Account _account = findAccountByID(account.AccountID);

            _account.Username = account.Username;
            _account.Email = account.Email;

            error = tryCheckUsername(_account);
            if (error != null)
            {
                return error;
            }

            error = tryCheckEmail(_account);
            if (error != null)
            {
                return error;
            }


            if (_account.Password != account.Password)
            {
                var rawPassword = account.Password;
                error = tryChangePassword(account, rawPassword);
                _account.Password = account.Password;
                _account.ConfirmPassword = account.ConfirmPassword;
                if (error != null)
                {
                    return error;
                }
                else
                {
                    using (var db = new BaseDbContext())
                    {
                        db.Entry(_account).State = EntityState.Modified;
                        _account.LoginFails = 0;

                        if (SessionPersister.account != null && _account.AccountID != SessionPersister.account.AccountID)
                        {
                            _account.NeedChangePassword = true;

                            EmailHelper.SendEmailToAccountOnPasswordReset(_account, rawPassword);
                        }

                        db.SaveChanges();
                    }
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




                using (var db = new BaseDbContext())
                {
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
                    db.SaveChanges();

                    SessionPersister.updateSessionForAccount();

                    account.Password = _account.Password;
                    account.ConfirmPassword = _account.ConfirmPassword;

                    return null;
                }
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
                using (var db = new BaseDbContext())
                {
                    db.Entry(_account).State = EntityState.Modified;

                    if (account.RoleList != null)
                    {
                        account.Role = String.Join(",", account.RoleList);
                    }
                    else if (account.Role == null)
                    {
                        account.Role = "";
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
            }
            else
            {
                return "Change password failed: Account not found";
            }
        }



        public string tryDeleteAccount(Account account)
        {
            using (var db = new BaseDbContext())
            {
                db.accountDb.Remove(account);
                db.SaveChanges();
                return null;
            }
        }

    }
}