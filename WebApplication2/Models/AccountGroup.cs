using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication2.Context;
using WebApplication2.Helpers;
using WebApplication2.Models.Infrastructure;

namespace WebApplication2.Models
{
    public class AccountGroup
    {
        [Key]
        public int AccountGroupID { get; set; }

        [Required]
        public string Name { get; set; }

        public bool isDefaultGroup { get; set; }

        public string AccessibleCategories { get; set; }
        public List<string> AccessibleCategoryList { get; set; }
        public List<string> getAccessibleCategoryList()
        {
            if (AccessibleCategories != null)
            {
                return AccessibleCategories.Split(',').ToList();
            }
            else if (AccessibleCategoryList != null)
            {
                return AccessibleCategoryList;
            }
            return new List<string>();
        }
        public List<int> getAccessibleCategoryListInt()
        {
            List<string> list = getAccessibleCategoryList();
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
        public List<Category> getAccessibleCategoryListObject()
        {
            List<Category> items = new List<Category>();
            var categoryList = getAccessibleCategoryList();
            foreach (var id in categoryList)
            {
                try
                {
                    int _id = Convert.ToInt32(id);
                    var item = InfrastructureCategoryDbContext.getInstance().findCategoryByID(_id);
                    items.Add(item);
                }
                catch (Exception e)
                {
                    LogHelper.Error(null, e);
                }
            }
            return items;
        }
        public string getAccessibleCategoryListRepresentation()
        {
            List<string> names = new List<string>();
            var categoryList = getAccessibleCategoryListObject();
            foreach (var item in categoryList)
            {
                names.Add(item.name_en);
            }
            return String.Join(",", names.ToArray());
        }
    }
}