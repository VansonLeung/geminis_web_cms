using Frontend.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Frontend.Attributes;
using Frontend.Models;

namespace Frontend.Models
{
    public class User : BaseModel
    {
        public User()
        {

        }

        [Key]
        public int UserID { get; set; }

        [Display(Name = "username", ResourceType = typeof(Resource))]
        [Required]
        public string Username { get; set; }

        [Display(Name = "email", ResourceType = typeof(Resource))]
        [Required]
        public string Email { get; set; }

        [Display(Name = "firstname", ResourceType = typeof(Resource))]
        [Required]
        public string Firstname { get; set; }

        [Display(Name = "lastname", ResourceType = typeof(Resource))]
        [Required]
        public string Lastname { get; set; }

        [Display(Name = "password", ResourceType = typeof(Resource))]
        [Required]
        [AccountPasswordLengthValidation]
        [AccountPasswordLetterValidation]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "role", ResourceType = typeof(Resource))]
        [Required]
        public string Role { get; set; }

        [Display(Name = "lastlogin", ResourceType = typeof(Resource))]
        [DataType(DataType.DateTime)]
        public DateTime? LastLogin { get; set; }

        [Display(Name = "lastPasswordModifiedAt", ResourceType = typeof(Resource))]
        [DataType(DataType.DateTime)]
        public DateTime? LastPasswordModifiedAt { get; set; }

        [Display(Name = "needschangepassword", ResourceType = typeof(Resource))]
        public bool NeedChangePassword { get; set; }

        [Display(Name = "loginfails", ResourceType = typeof(Resource))]
        public int LoginFails { get; set; }

        public string historyPasswords { get; set; }

        public List<string> historyPasswordList()
        {
            var passwordString = historyPasswords != null ? historyPasswords : "";
            var passwords = passwordString.Split(new char[] { ',' });
            var passwordList = new List<string>(passwords);
            return passwordList;
        }

        public string historyPasswordsFromList(List<string> passwordList)
        {
            var passwords = passwordList.ToArray();
            var passwordString = String.Join(",", passwords);
            return passwordString;
        }


        public string MakeEncryptedPassword(string password)
        {
            var salted = "GCP_V1_" + password;
            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(salted);
            Byte[] encodedBytes = md5.ComputeHash(originalBytes);
            return encodedBytes.ToString();
        }
    }
}