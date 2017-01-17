﻿using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor;
using WebApplication2.ViewModels.Include;

namespace WebApplication2.ViewModels
{
    public static class MyRazorExtensions
    {
        public static MvcHtmlString RazorEncode(this HtmlHelper helper, string template)
        {
            return RazorEncode(helper, template, (BaseViewModel)null);
        }

        public static MvcHtmlString RazorEncode(this HtmlHelper helper, string template, BaseViewModel model)
        {
            string output = Render(helper, template, model);
            return new MvcHtmlString(output);
        }

        private static string Render(HtmlHelper helper, string template, BaseViewModel model)
        {
            if (template == null)
            {
                return "";
            }

            // turn @{iframe_stock_1}asdfsdf@{iframe_stock_1}asdfsdf  into  constantasdfsdfconstantasdfsdf

            string input = template;

            string pattern = @"@{C:(?<key>\w+)}";
            string output = Regex.Replace(input, pattern, delegate (Match m) {
                var str = m.Value;
                str = str.Substring(4);
                str = str.Substring(0, str.Length - 1);

                if (model != null)
                {
                    var value = model.GetConstant(str);
                    if (value == null)
                    {
                        return "";
                    }
                    return value;
                }

                return "";
            });

            pattern = @"@{Q:(?<key>\w+)}";
            output = Regex.Replace(output, pattern, delegate (Match m) {
                var str = m.Value;
                str = str.Substring(4);
                str = str.Substring(0, str.Length - 1);

                if (model != null)
                {
                    var value = model.GetQuery(str);
                    if (value == null)
                    {
                        return "";
                    }
                    return value;
                }

                return "";
            });


            pattern = @"@{S:LOCALE}";
            output = Regex.Replace(output, pattern, delegate (Match m) {
                if (model != null
                    && model.lang != null
                    && model.lang.locale != null)
                {
                    return model.lang.locale;
                }
                return "";
            });

            pattern = @"@{S:LANG}";
            output = Regex.Replace(output, pattern, delegate (Match m) {
                if (model != null
                    && model.lang != null
                    && model.lang.lang != null)
                {
                    return model.lang.lang;
                }
                return "";
            });

            pattern = @"@{S:CULTURE}";
            output = Regex.Replace(output, pattern, delegate (Match m) {
                if (model != null
                    && model.lang != null
                    && model.lang.culture != null)
                {
                    return model.lang.culture;
                }
                return "";
            });


            pattern = @"@{S:CLIENTID}";
            output = Regex.Replace(output, pattern, delegate (Match m) {
                if (model != null
                    && model.current != null
                    && model.current.session != null
                    && model.current.session.clientID != null)
                {
                    return model.current.session.clientID;
                }
                return "";
            });


            pattern = @"@{S:SESSIONID}";
            output = Regex.Replace(output, pattern, delegate (Match m) {
                if (model != null
                    && model.current != null
                    && model.current.session != null
                    && model.current.session.sessionID != null)
                {
                    return model.current.session.sessionID;
                }
                return "";
            });


            pattern = @"@{S:ACCOUNTSEQ}";
            output = Regex.Replace(output, pattern, delegate (Match m) {
                try
                {
                    return model.current.session.accountSeq + "";
                }
                catch (Exception e)
                {
                    return "";
                }
            });


            pattern = @"@{S:ACCOUNTTYPE}";
            output = Regex.Replace(output, pattern, delegate (Match m) {
                try
                {
                    return model.current.session.accountType + "";
                }
                catch (Exception e)
                {
                    return "";
                }
            });


            pattern = @"@{S:TRADINGACCSEQ}";
            output = Regex.Replace(output, pattern, delegate (Match m) {
                if (model != null
                    && model.current != null
                    && model.current.session != null
                    && model.current.session.tradingAccSeq != null)
                {
                    return model.current.session.tradingAccSeq;
                }
                return "";
            });

            pattern = @"@{S:TRADINGACCSTATUS}";
            output = Regex.Replace(output, pattern, delegate (Match m) {
                if (model != null
                    && model.current != null
                    && model.current.session != null
                    && model.current.session.tradingAccStatus != null)
                {
                    return model.current.session.tradingAccStatus;
                }
                return "";
            });







            return output;
        }
    }
}