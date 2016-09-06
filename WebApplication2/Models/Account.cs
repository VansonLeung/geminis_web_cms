using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WebApplication2.Resources;
using WebApplication2.Attributes;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Account : BaseModel
    {
        public Account()
        {
            LoginFails = 0;
        }

        [Key]
        public int AccountID { get; set; }

        [Display(Name = "username", ResourceType = typeof(Resource))]
        [Required]
        public string Username { get; set; }

        [Display(Name = "email", ResourceType = typeof(Resource))]
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
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

        [Display(Name = "confirmpassword", ResourceType = typeof(Resource))]
        [Compare("Password")]
        [Required]
        [AccountPasswordLengthValidation]
        [AccountPasswordLetterValidation]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "role", ResourceType = typeof(Resource))]
        public string Role { get; set; }
        
        [Display(Name = "role", ResourceType = typeof(Resource))]
        public List<string> RoleList { get; set; }

        public bool isRoleSuperadmin()
        {
            return Role != null && Role.Contains("superadmin");
        }

        public bool isRoleEditor()
        {
            return Role != null && Role.Contains("editor");
        }

        public bool isRoleApprover()
        {
            return Role != null && Role.Contains("approver");
        }

        public bool isRolePublisher()
        {
            return Role != null && Role.Contains("publisher");
        }

        public int? GroupID { get; set; }
        [ForeignKey("GroupID")]
        public virtual AccountGroup Group { get; set; }

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

        public bool isEnabled { get; set; }

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
            var salted = password + "GCP_V1";
            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(salted);
            Byte[] encodedBytes = md5.ComputeHash(originalBytes);

            StringBuilder hex = new StringBuilder(encodedBytes.Length * 2);
            foreach (byte b in encodedBytes)
                hex.AppendFormat("{0:x2}", b);

            var str = hex.ToString();
            str = str.Substring(0, 20);
            return str;
        }
    }
    
}