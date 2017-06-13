using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace WebApplication2.Helpers
{
    public static class LinkScannerParser
    {
        public static string getDocumentFromInnerUrl(string innerUrl)
        {
            FileInfo fileInfo = getFileInfoFromInnerUrl(innerUrl);
            if (fileInfo != null && fileInfo.Exists)
            {
                string fullname = fileInfo.FullName;
                string ext = fileInfo.Extension;

                //                    byte[] fileBytes = System.IO.File.ReadAllBytes(baseFileUrl);

                if (ext.ToLower() == ".pdf")
                {
                    string data = LuceneSearch.PdfFileReader(fileInfo);
                    return data;
                }

                if (ext.ToLower() == ".doc")
                {
                    string data = LuceneSearch.DocxFileReader(fileInfo);
                    return data;
                }

                if (ext.ToLower() == ".docx")
                {
                    string data = LuceneSearch.DocxFileReader(fileInfo);
                    return data;
                }
            }
            return "";
        }

        public static FileInfo getFileInfoFromInnerUrl(string innerUrl)
        {
            var constant = WebApplication2.Context.ConstantDbContext.getInstance().findActiveByKeyNoTracking("CMS_BASE_URL");
            var filebase = WebApplication2.Context.ConstantDbContext.getInstance().findActiveByKeyNoTracking("CMS_FILE_BASE_URL");

            if (constant == null)
            {
                return null;
            }

            if (filebase == null)
            {
                return null;
            }


            if (innerUrl.IndexOf("/ckfinder/userfiles/") != -1)
            {
                int startIndex = innerUrl.IndexOf("/ckfinder/userfiles/");
                string[] segments = innerUrl.Substring(startIndex).Split('/');

                if (segments.Length >= 5)
                {
                    string type = segments[3];
                    string path = segments[4];
                    //                    string[] exts = path.Split('.');
                    //                    string ext = exts[exts.Length - 1];

                    string filepath = filebase.Value + type + @"\" + path;
                    var baseFileUrl = filepath;

                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(baseFileUrl);
                    if (fileInfo != null && fileInfo.Exists)
                    {
                        return fileInfo;
                    }
                }
            }
            return null;
        }

        public static string getFilenameFromInnerUrl(string innerUrl)
        {
            FileInfo fileInfo = getFileInfoFromInnerUrl(innerUrl);
            if (fileInfo != null && fileInfo.Exists)
            {
                string fullname = fileInfo.FullName;
                string name = fileInfo.Name;
                return name;
            }
            return "";
        }

        public static string getFiletypeFromInnerUrl(string innerUrl)
        {
            FileInfo fileInfo = getFileInfoFromInnerUrl(innerUrl);
            if (fileInfo != null && fileInfo.Exists)
            {
                string fullname = fileInfo.FullName;
                string ext = fileInfo.Extension;
                return ext.ToLower();
            }
            return null;
        }
    }

    public class LinkScanner
    {
        private static string urlPattern = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
        private static string tagPattern = @"<a\b[^>]*(.*?)</a>";
        private static string emailPattern = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";


        /// <summary>
        /// gets all links that the url contains
        /// </summary>
        public static List<string> getInnerUrlsFromHTML(string htmlCode)
        {
            List<string> innerUrls = new List<string>();

            string pattern1 = @"/ckfinder/userfiles/files/(\w+).pdf";
            MatchCollection matches = Regex.Matches(htmlCode, pattern1);
            foreach (Match match in matches)
            {
                innerUrls.Add(match.Value);
            }
            string pattern2 = @"/ckfinder/userfiles/files/(\w+).docx";
            MatchCollection matches2 = Regex.Matches(htmlCode, pattern2);
            foreach (Match match in matches2)
            {
                innerUrls.Add(match.Value);
            }


            List<string> links = getMatches(htmlCode);
            foreach (string link in links)
            {
                if (!Regex.IsMatch(link, urlPattern) && !Regex.IsMatch(link, emailPattern))
                {
                    innerUrls.Add(link);
                }
                else
                {
                    innerUrls.Add(link);
                }
            }
            return innerUrls;
        }
        public static List<string> getInnerUrls(string url)
        {
            //create the WebRequest for url eg "http://www.codeproject.com"
            WebRequest request = WebRequest.Create(url);

            //get the stream from the web response
            StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());

            //get the htmlCode
            string htmlCode = reader.ReadToEnd();

            return getInnerUrlsFromHTML(htmlCode);
        }

        /// <summary>
        /// gets all pages links
        /// </summary>
        /// <param name="source">page html code</param>
        /// <returns> list of links </returns>
        private static List<string> getMatches(string source)
        {
            var matchesList = new List<string>();
            //reg expression for A Tag [html] 
            //get the collection that match the pattern
            MatchCollection matches = Regex.Matches(source, tagPattern);
            //add the text under the href attribute
            //to the list
            foreach (Match match in matches)
            {
                string val = match.Value.Trim();
                if (val.Contains("href=\""))
                {
                    string link = getSubstring(val, "href=\"", "\"");
                    matchesList.Add(link);
                }
            }

            return matchesList;
        }

        private static string getSubstring(string source, string start, string end)
        {
            int startIndex = source.IndexOf(start) + start.Length;
            int length = source.IndexOf(end, startIndex) - startIndex;
            return source.Substring(startIndex, length);
        }

        /// <summary>
        /// creates an absolute url for the source whitch the site contains
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="path">path of the source</param>
        /// <returns></returns>
        private static string getAblosuteUrl(string domainName, string path)
        {
            string absoluteUrl = "";
            if (domainName[domainName.Length - 1] == '/')
            {
                absoluteUrl += domainName;
            }
            else
            {
                absoluteUrl += domainName + "/";
            }

            if (path.Contains("../"))
            {
                string temp = domainName.Substring(0, domainName.LastIndexOf("/", 1));
                temp = temp.Substring(0, temp.LastIndexOf("/", 1));
                absoluteUrl = temp + path.Substring(3);
                return absoluteUrl;
            }
            if (path.Contains("./"))
            {
                string temp = domainName.Substring(0, domainName.LastIndexOf("/", 1));
                absoluteUrl = temp + path.Substring(2);
                return absoluteUrl;
            }
            if (path[0] == '/')
            {
                absoluteUrl += path.Substring(1);
                return absoluteUrl;
            }
            absoluteUrl += path;

            return absoluteUrl;
        }

        private static string getDomainName(string url)
        {
            int length = url.IndexOf("/", 8);
            string domainName = url.Substring(0, length);
            return domainName;
        }

    }

}