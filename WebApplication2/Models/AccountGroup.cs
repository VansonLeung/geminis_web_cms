using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class AccountGroup
    {
        [Key]
        public int AccountGroupID { get; set; }
        public string Name { get; set; }

        public string AccessibleArticleGroups { get; set; }
        public List<string> AccessibleArticleGroupList { get; set; }
        public List<string> getAccessibleArticleGroupList()
        {
            if (AccessibleArticleGroups != null)
            {
                return AccessibleArticleGroups.Split(',').ToList();
            }
            else if (AccessibleArticleGroupList != null)
            {
                return AccessibleArticleGroupList;
            }
            return new List<string>();
        }
        public List<int> getAccessibleArticleGroupListInt()
        {
            List<string> list = getAccessibleArticleGroupList();
            List<int> listInt = new List<int>();
            foreach (string str in list)
            {
                if (!str.Equals(""))
                {
                    int id = Convert.ToInt32(str);
                    listInt.Add(id);
                }
            }
            return listInt;
        }

        public string AccessibleContentPages { get; set; }
        public List<string> AccessibleContentPageList { get; set; }
        public List<string> getAccessibleContentPageList()
        {
            if (AccessibleContentPages != null)
            {
                return AccessibleContentPages.Split(',').ToList();
            }
            else if (AccessibleContentPageList != null)
            {
                return AccessibleContentPageList;
            }
            return new List<string>();
        }
        public List<int> getAccessibleContentPageListInt()
        {
            List<string> list = getAccessibleContentPageList();
            List<int> listInt = new List<int>();
            foreach (string str in list)
            {
                if (!str.Equals(""))
                {
                    int id = Convert.ToInt32(str);
                    listInt.Add(id);
                }
            }
            return listInt;
        }
    }
}