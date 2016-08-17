using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class AccountAccessRightsContentPage
    {
        [Key]
        public int ID { get; set; }


        public int? ContentPageBaseID { get; set; }

        public int? AccountID { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }

        public bool canEdit { get; set; }
        public bool canEditTitle { get; set; }
        public bool canEditContent { get; set; }
        public bool canEditUrl { get; set; }
        public bool canEditBanner { get; set; }
    }
}