using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.ViewModels.Include
{
    public class GlobalData
    {
        public string lbl_fontSize { get; set; }
        public string lbl_login_register { get; set; }
        public string lbl_logout { get; set; }
        public string lbl_free_open_acc { get; set; }
        public string lbl_welcome { get; set; }
        public string lbl_copyright { get; set; }
        public string lbl_trading { get; set; }
        public string lbl_search_placeholder { get; set; }
        public string lbl_search_results { get; set; }
        public string lbl_search_results_your { get; set; }

        public void implement_lbls(string lang)
        {
            if (lang == "zh")
            {
                lbl_fontSize = "字體大小";
                lbl_login_register = "登入／註冊";
                lbl_logout = "登出";
                lbl_free_open_acc = "免费開户";
                lbl_welcome = "你好，";
                lbl_copyright = " Geminis Securities Limited，版權所有，不得轉載。";
                lbl_trading = "交易帳號";
                lbl_search_placeholder = "搜尋";
                lbl_search_results = "搜尋結果";
                lbl_search_results_your = "你的搜尋結果";
            }
            else if (lang == "cn")
            {
                lbl_fontSize = "字体大小";
                lbl_login_register = "登入/注册";
                lbl_logout = "登出";
                lbl_free_open_acc = "免费开户";
                lbl_welcome = "你好，";
                lbl_copyright = " Geminis Securities Limited，版权所有，不得转载。";
                lbl_trading = "交易帐号";
                lbl_search_placeholder = "搜寻";
                lbl_search_results = "搜寻结果";
                lbl_search_results_your = "你的搜寻结果";
            }
            else
            {
                lbl_fontSize = "Font Size";
                lbl_login_register = "Login / Register";
                lbl_logout = "Logout";
                lbl_free_open_acc = "Open Free Account";
                lbl_welcome = "Welcome, ";
                lbl_copyright = " Geminis Securities Limited. All Rights Reserved.";
                lbl_trading = "Trading Account";
                lbl_search_placeholder = "Search";
                lbl_search_results = "Search Results";
                lbl_search_results_your = "Your Search Results";
            }
        }
    }
}