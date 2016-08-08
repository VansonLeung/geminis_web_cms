using Frontend.Models;
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
        

        public List<User> findUsers()
        {
            return accountDb.ToList();
        }

        public User findUserByID(int userID)
        {
            return accountDb.Where(acc => acc.UserID == userID).FirstOrDefault();
        }

        public User findUserByUserUsername(User account)
        {
            return accountDb.Where(acc => acc.Username == account.Username).FirstOrDefault();
        }

        public User tryLoginUserByUser(User account)
        {
            var encPassword = account.MakeEncryptedPassword(account.Password);
            User _account = findUserByUserUsername(account);
            if (_account != null)
            {
                if (_account.Password == encPassword)
                {
                    if (_account.LoginFails < 3)
                    {
                        Entry(_account).State = EntityState.Modified;
                        _account.LastLogin = DateTime.UtcNow;
                        _account.LoginFails = 0;
                        SaveChanges();
                        SessionPersister.createSessionForUser(_account);
                    }
                }
                else
                {
                    Entry(_account).State = EntityState.Modified;
                    _account.LoginFails = _account.LoginFails + 1;
                    SaveChanges();
                }
            }
            return _account;
        }

        public void tryLogout()
        {
            SessionPersister.removeSession();
        }

        public void tryRegisterUser(User account)
        {
            var encPassword = account.MakeEncryptedPassword(account.Password);
            account.Password = encPassword;
            account.LastPasswordModifiedAt = DateTime.UtcNow;
            account.historyPasswords = account.Password;
            accountDb.Add(account);
            SaveChanges();
        }

        public string tryEdit(User account)
        {
            var encPassword = account.MakeEncryptedPassword(account.Password);
            string error = null;
            User _account = findUserByID(account.UserID);
            if (_account.Password != encPassword)
            {
                error = tryChangePassword(account, account.Password);
                if (error != null)
                {
                    return error;
                }
                else
                {
                    Entry(_account).State = EntityState.Modified;
                    _account.LoginFails = 0;
                    _account.NeedChangePassword = true;
                    SaveChanges();
                }
            }

            error = tryChangeRole(account, account.Role);
            if (error != null)
            {
                return error;
            }


            return null;
        }

        public string tryChangePassword(User account, String newPassword)
        {
            User _account = findUserByID(account.UserID);
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
                _account.LastPasswordModifiedAt = DateTime.UtcNow;


                passwords.Add(newPassword);
                while (passwords.Count > 9)
                {
                    passwords.RemoveAt(0);
                }

                _account.historyPasswords = _account.historyPasswordsFromList(passwords);
                SessionPersister.updateSessionForUser();
                SaveChanges();
                return null;
            }
            else
            {
                return "Change password failed: User not found";
            }
        }

        public string tryChangeRole(User account, String role)
        {
            User _account = findUserByID(account.UserID);
            if (_account != null)
            {
                Entry(_account).State = EntityState.Modified;
                _account.Role = role;

                SessionPersister.updateSessionForUser();
                SaveChanges();
                return null;
            }
            else
            {
                return "Change password failed: User not found";
            }
        }
    }
}