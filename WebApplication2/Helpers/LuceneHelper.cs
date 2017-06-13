using HtmlAgilityPack;
using ICSharpCode.SharpZipLib.Zip;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using WebApplication2.Context;
using WebApplication2.Models;
using WebApplication2.ViewModels;

namespace WebApplication2.Helpers
{
    public class LuceneSearchData
    {
        public int Id { get; set; }

        public int ArticleId { get; set; }
        public int BaseArticleId { get; set; }
        public int CategoryId { get; set; }

        public string Url { get; set; }

        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string GetName(string locale = null)
        {
            string val = "";
            if (locale != null)
            {
                if (locale.Equals("en") || locale.Equals("en-US"))
                {
                    val = name_en;
                }
                if (locale.Equals("zh") || locale.Equals("zh-HK"))
                {
                    val = name_zh;
                }
                if (locale.Equals("cn") || locale.Equals("zh-CN"))
                {
                    val = name_cn;
                }
            }

            if (val != "")
            {
                return val;
            }
            return Name;
        }

        public string GetDesc(string locale = null)
        {
            string val = "";
            if (locale != null)
            {
                if (locale.Equals("en") || locale.Equals("en-US"))
                {
                    val = desc_en;
                }
                if (locale.Equals("zh") || locale.Equals("zh-HK"))
                {
                    val = desc_zh;
                }
                if (locale.Equals("cn") || locale.Equals("zh-CN"))
                {
                    val = desc_cn;
                }
            }

            if (val != "")
            {
                return val;
            }
            return Description;
        }

        public string GetURL(string locale = "en-US")
        {
            if (is_page == 1)
            {
                return "/" + locale + "/Page/" + Url;
            }
            else
            {
                return Url;
            }
        }

        public string name_en { get; set; }
        public string name_zh { get; set; }
        public string name_cn { get; set; }
        public string desc_en { get; set; }
        public string desc_zh { get; set; }
        public string desc_cn { get; set; }

        public int is_page { get; set; }
        public int is_pdf { get; set; }
        public int is_doc { get; set; }
        public int is_docx { get; set; }

        public int is_visitor { get; set; }
        public int is_member { get; set; }
        public int is_trading { get; set; }
    }

    public class LuceneSearchDataRepository
    {
        public static LuceneSearchData Get(int id)
        {
            return GetAll().SingleOrDefault(x => x.Id.Equals(id));
        }
        public static List<LuceneSearchData> GetAll()
        {
            List<LuceneSearchData> searchData = new List<LuceneSearchData>();
            List<ArticlePublished> items = ArticlePublishedDbContext.getInstance().findPublishedArticlesGroupByBaseVersion("en", "trading");

            List<string> innerHTMLLinks = new List<string>();

            for (int i = 0; i < items.Count; i++)
            {
                try
                {
                    ArticlePublished article = items[i];
                    LuceneSearchData data = new LuceneSearchData();

                    data.is_page = 0;
                    data.is_pdf = 0;
                    data.is_doc = 0;
                    data.is_docx = 0;

                    data.is_visitor = 0;
                    data.is_member = 0;
                    data.is_trading = 0;

                    if (article != null
                        && article.categoryID != null
                        && article.categoryID.HasValue)
                    {
                        data.Id = i;
                        data.ArticleId = article.ArticleID;
                        data.BaseArticleId = article.BaseArticleID;
                        data.CategoryId = article.categoryID.Value;
                        data.Url = article.category.url;
                        data.Type = "page";
                        data.Name = article.Name;
                        data.Description = article.Desc;

                        if (data.Name == null)
                        {
                            continue;
                        }

                        if (data.Description == null)
                        {
                            data.Description = "";
                        }

                        data.name_en = article.Name;
                        data.desc_en = article.Desc;

                        ArticlePublished a_zh = ArticlePublishedDbContext.getInstance().getArticlePublishedByBaseArticleID(article.BaseArticleID, "zh");
                        if (a_zh != null)
                        {
                            data.name_zh = a_zh.Name;
                            data.desc_zh = a_zh.Desc;
                        }

                        ArticlePublished a_cn = ArticlePublishedDbContext.getInstance().getArticlePublishedByBaseArticleID(article.BaseArticleID, "cn");
                        if (a_cn != null)
                        {
                            data.name_cn = a_cn.Name;
                            data.desc_cn = a_cn.Desc;
                        }

                        if (data.name_en == null)
                        {
                            data.name_en = "";
                        }

                        if (data.name_zh == null)
                        {
                            data.name_zh = "";
                        }

                        if (data.name_cn == null)
                        {
                            data.name_cn = "";
                        }

                        if (data.desc_en == null)
                        {
                            data.desc_en = "";
                        }

                        if (data.desc_zh == null)
                        {
                            data.desc_zh = "";
                        }

                        if (data.desc_cn == null)
                        {
                            data.desc_cn = "";
                        }

                        data.Description = MyRazorExtensions.Render(null, data.Description, null, true);
                        data.desc_en = MyRazorExtensions.Render(null, data.desc_en, null, true);
                        data.desc_zh = MyRazorExtensions.Render(null, data.desc_zh, null, true);
                        data.desc_cn = MyRazorExtensions.Render(null, data.desc_cn, null, true);


                        data.Description = HtmlToPlainText(data.Description);
                        data.desc_en = HtmlToPlainText(data.desc_en);
                        data.desc_zh = HtmlToPlainText(data.desc_zh);
                        data.desc_cn = HtmlToPlainText(data.desc_cn);


                        data.is_page = 1;

                        data.is_trading = article.category.isVisibleToTradingOnly ? 1 : 0;
                        data.is_member = article.category.isVisibleToMembersOnly ? 1 : 0;
                        data.is_visitor = article.category.isVisibleToVisitorOnly ? 1 : 0;

                        searchData.Add(data);

                        List<string> innerLinks_en = LinkScanner.getInnerUrlsFromHTML(data.desc_en);
                        List<string> innerLinks_zh = LinkScanner.getInnerUrlsFromHTML(data.desc_zh);
                        List<string> innerLinks_cn = LinkScanner.getInnerUrlsFromHTML(data.desc_cn);

                        innerLinks_en.AddRange(innerLinks_zh.Except(innerLinks_en));
                        innerLinks_zh.AddRange(innerLinks_cn.Except(innerLinks_zh));

                        for (var m = innerLinks_en.Count - 1; m >= 0; m--)
                        {
                            string link = innerLinks_en[m];
                            if (innerHTMLLinks.Contains(link))
                            {
                                innerLinks_en.RemoveAt(m);
                            }
                        }

                        for (var m = innerLinks_en.Count - 1; m >= 0; m--)
                        {
                            string link = innerLinks_en[m];
                            string name = LinkScannerParser.getFilenameFromInnerUrl(link);
                            string ext = LinkScannerParser.getFiletypeFromInnerUrl(link);

                            LuceneSearchData _data = new LuceneSearchData();

                            _data.ArticleId = 0;
                            _data.BaseArticleId = 0;
                            _data.CategoryId = 0;

                            _data.is_page = 0;
                            _data.is_pdf = 0;
                            _data.is_doc = 0;
                            _data.is_docx = 0;

                            if (ext == ".pdf")
                            {
                                _data.is_pdf = 1;
                            }
                            else if (ext == ".doc")
                            {
                                _data.is_doc = 1;
                            }
                            else if (ext == ".docx")
                            {
                                _data.is_docx = 1;
                            }
                            else
                            {
                                continue;
                            }

                            string desc = LinkScannerParser.getDocumentFromInnerUrl(link);

                            _data.is_visitor = data.is_visitor;
                            _data.is_member = data.is_member;
                            _data.is_trading = data.is_trading;
                            _data.Name = name;

                            if (name == "")
                            {
                                continue;
                            }

                            _data.name_en = "";
                            _data.name_zh = "";
                            _data.name_cn = "";

                            _data.Url = "/ckfinder/userfiles/files/" + _data.Name;
                            _data.Type = ext.ToUpper();
                            _data.Description = desc;

                            _data.desc_en = "";
                            _data.desc_zh = "";
                            _data.desc_cn = "";

                            searchData.Add(_data);
                        }
                    }
                }
                catch (Exception e)
                {
                    AuditLogDbContext.getInstance().createAuditLog(new AuditLog
                    {
                        action = "Search Indexing",
                        remarks = "Exception GetAll: " + e.Message,
                    });
                }

            }

            return searchData;
        }

        private static string HtmlToPlainText(string html)
        {
            html = Regex.Replace(html, @"<style (.|\n)*?</style>", string.Empty);
            html = Regex.Replace(html, @"<style>(.|\n)*?</style>", string.Empty);
            html = Regex.Replace(html, @"<script (.|\n)*?</script>", string.Empty);
            html = Regex.Replace(html, @"<script>(.|\n)*?</script>", string.Empty);
            html = Regex.Replace(html, @"<!--(.|\n)*?-->", string.Empty);

            HtmlDocument mainDoc = new HtmlDocument();
            string htmlString = html;
            mainDoc.LoadHtml(htmlString);
            string cleanText = mainDoc.DocumentNode.InnerText;
            return cleanText;
        }

    }

    


    public static class LuceneSearch
    {
        private static string _luceneDir = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "lucene_index");
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }



        private static void _addToLuceneIndex(LuceneSearchData sampleData, IndexWriter writer)
        {
            // remove older index entry
            var searchQuery = new TermQuery(new Term("Id", sampleData.Id.ToString()));
            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to db fields
            doc.Add(new Field("Id", sampleData.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("ArticleId", sampleData.ArticleId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("BaseArticleId", sampleData.BaseArticleId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("CategoryId", sampleData.CategoryId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Type", sampleData.Type.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Name", sampleData.Name.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Description", sampleData.Description.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("name_en", sampleData.name_en.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("name_zh", sampleData.name_zh.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("name_cn", sampleData.name_cn.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("desc_en", sampleData.desc_en.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("desc_zh", sampleData.desc_zh.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("desc_cn", sampleData.desc_cn.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Url", sampleData.Url.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("is_page", sampleData.is_page.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("is_pdf", sampleData.is_pdf.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("is_doc", sampleData.is_doc.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("is_docx", sampleData.is_docx.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("is_trading", sampleData.is_trading.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("is_member", sampleData.is_member.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("is_visitor", sampleData.is_visitor.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            // add entry to index
            writer.AddDocument(doc);
        }





        public static void AddUpdateLuceneIndex(IEnumerable<LuceneSearchData> sampleDatas)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // add data to lucene search index (replaces older entry if any)
                foreach (var sampleData in sampleDatas) _addToLuceneIndex(sampleData, writer);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }




        public static void AddUpdateLuceneIndex(LuceneSearchData sampleData)
        {
            AddUpdateLuceneIndex(new List<LuceneSearchData> { sampleData });
        }




        public static void ClearLuceneIndexRecord(int record_id)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // remove older index entry
                var searchQuery = new TermQuery(new Term("Id", record_id.ToString()));
                writer.DeleteDocuments(searchQuery);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }






        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Close();
                    writer.Dispose();
                }
            }
            catch (java.lang.Exception e)
            {
                return false;
            }
            return true;
        }





        public static void Optimize()
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }




        private static LuceneSearchData _mapLuceneDocumentToData(Document doc)
        {
            return new LuceneSearchData
            {
                Id = Convert.ToInt32(doc.Get("Id")),
                ArticleId = Convert.ToInt32(doc.Get("ArticleId")),
                BaseArticleId = Convert.ToInt32(doc.Get("BaseArticleId")),
                CategoryId = Convert.ToInt32(doc.Get("CategoryId")),
                Type = doc.Get("Type"),
                Name = doc.Get("Name"),
                Description = doc.Get("Description"),
                name_en = doc.Get("name_en"),
                name_zh = doc.Get("name_zh"),
                name_cn = doc.Get("name_cn"),
                desc_en = doc.Get("desc_en"),
                desc_zh = doc.Get("desc_zh"),
                desc_cn = doc.Get("desc_cn"),
                Url = doc.Get("Url"),
                is_page = Convert.ToInt32(doc.Get("is_page")),
                is_pdf = Convert.ToInt32(doc.Get("is_pdf")),
                is_doc = Convert.ToInt32(doc.Get("is_doc")),
                is_docx = Convert.ToInt32(doc.Get("is_docx")),
                is_trading = Convert.ToInt32(doc.Get("is_trading")),
                is_member = Convert.ToInt32(doc.Get("is_member")),
                is_visitor = Convert.ToInt32(doc.Get("is_visitor")),
            };
        }



        private static LuceneSearchData _mapLuceneDocumentToData(Document doc, Highlighter highlighter, StandardAnalyzer analyzer)
        {
            return new LuceneSearchData
            {
                Id = Convert.ToInt32(doc.Get("Id")),
                ArticleId = Convert.ToInt32(doc.Get("ArticleId")),
                BaseArticleId = Convert.ToInt32(doc.Get("BaseArticleId")),
                CategoryId = Convert.ToInt32(doc.Get("CategoryId")),
                Type = doc.Get("Type"),
                Name = doc.Get("Name"),
                Description = getHighlight(highlighter, analyzer, doc.Get("Description")),
                name_en = doc.Get("name_en"),
                name_zh = doc.Get("name_zh"),
                name_cn = doc.Get("name_cn"),
                Url = doc.Get("Url"),
                desc_en = getHighlight(highlighter, analyzer, doc.Get("desc_en")),
                desc_zh = getHighlight(highlighter, analyzer, doc.Get("desc_zh")),
                desc_cn = getHighlight(highlighter, analyzer, doc.Get("desc_cn")),
                is_page = Convert.ToInt32(doc.Get("is_page")),
                is_pdf = Convert.ToInt32(doc.Get("is_pdf")),
                is_doc = Convert.ToInt32(doc.Get("is_doc")),
                is_docx = Convert.ToInt32(doc.Get("is_docx")),
                is_trading = Convert.ToInt32(doc.Get("is_trading")),
                is_member = Convert.ToInt32(doc.Get("is_member")),
                is_visitor = Convert.ToInt32(doc.Get("is_visitor")),
            };
        }



        private static IEnumerable<LuceneSearchData> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }
        private static IEnumerable<LuceneSearchData> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits,
            IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }


        private static IEnumerable<LuceneSearchData> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits,
            IndexSearcher searcher, Highlighter highlighter, StandardAnalyzer analyzer)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc), highlighter, analyzer)).ToList();
        }



        private static string getHighlight(Highlighter highlighter, StandardAnalyzer analyzer, string fieldContent)
        {
            Lucene.Net.Analysis.TokenStream stream = analyzer.TokenStream("", new StringReader(fieldContent));
            return highlighter.GetBestFragments(stream, fieldContent, 1, ".");
        }





        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }


        private static IEnumerable<LuceneSearchData> _search
            (string searchQuery, string searchField = "", string role = null)
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<LuceneSearchData>();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
                // search by multiple fields (ordered by RELEVANCE)
                else
                {
                    var parser = new MultiFieldQueryParser
                        (Lucene.Net.Util.Version.LUCENE_30, new[] {
                            "Name",
                            "name_en",
                            "name_zh",
                            "name_cn",
                            "Description",
                            "desc_en",
                            "desc_zh",
                            "desc_cn",
                        }, analyzer);
                    var query = parseQuery(searchQuery, parser);

                    BooleanQuery bq = new BooleanQuery();
                    bq.Add(query, Occur.MUST);

                    if (role == "trading")
                    {
                        var role_parser = new MultiFieldQueryParser
                            (Lucene.Net.Util.Version.LUCENE_30, new[] {
                            "is_trading",
                            "is_member",
                            "is_visitor",
                            }, analyzer);

                        role_parser.DefaultOperator = QueryParser.AND_OPERATOR;

                        var role_query = parseQuery("1", role_parser);
                        bq.Add(role_query, Occur.MUST);
                    }
                    else if (role == "member")
                    {
                        var role_parser = new MultiFieldQueryParser
                            (Lucene.Net.Util.Version.LUCENE_30, new[] {
                            "is_member",
                            "is_visitor",
                            }, analyzer);

                        role_parser.DefaultOperator = QueryParser.AND_OPERATOR;

                        var role_query = parseQuery("1", role_parser);
                        bq.Add(role_query, Occur.MUST);
                    }
                    else
                    {
                        var role_parser = new MultiFieldQueryParser
                            (Lucene.Net.Util.Version.LUCENE_30, new[] {
                            "is_visitor",
                            }, analyzer);

                        role_parser.DefaultOperator = QueryParser.AND_OPERATOR;

                        var role_query = parseQuery("1", role_parser);
                        bq.Add(role_query, Occur.MUST);
                    }


                    var scorer = new QueryScorer(bq);
                    var hits = searcher.Search(bq, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                    IFormatter formatter = new SimpleHTMLFormatter("<span style=\"font-weight:bold; background-color:yellow;\">", "</span>");

                    SimpleFragmenter fragmenter = new SimpleFragmenter(1000);

                    Highlighter highlighter = new Highlighter(formatter, scorer);
                    highlighter.TextFragmenter = fragmenter;

                    var results = _mapLuceneToDataList(hits, searcher, highlighter, analyzer);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
            }
        }






        public static IEnumerable<LuceneSearchData> Search(string input, string fieldName = "")
        {
            if (string.IsNullOrEmpty(input)) return new List<LuceneSearchData>();

            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return _search(input, fieldName);
        }


        public static IEnumerable<LuceneSearchData> SearchDefault(string input, string fieldName = "")
        {
            return string.IsNullOrEmpty(input) ? new List<LuceneSearchData>() : _search(input, fieldName);
        }


        public static IEnumerable<LuceneSearchData> GetAllIndexRecords()
        {
            // validate search index
            if (!System.IO.Directory.EnumerateFiles(_luceneDir).Any()) return new List<LuceneSearchData>();

            // set up lucene searcher
            var searcher = new IndexSearcher(_directory, false);
            var reader = IndexReader.Open(_directory, false);
            var docs = new List<Document>();
            var term = reader.TermDocs();
            while (term.Next()) docs.Add(searcher.Doc(term.Doc));
            reader.Dispose();
            searcher.Dispose();
            return _mapLuceneToDataList(docs);
        }





        public static string PdfFileReader(FileInfo fileName)
        {
            PDDocument doc = PDDocument.load(fileName.FullName);
            PDFTextStripper pdfStripper = new PDFTextStripper();
            string text = pdfStripper.getText(doc);
            return text;
        }


        public static string DocxFileReader(FileInfo fileName)
        {
            DocxToText dtt = new DocxToText(fileName.FullName);
            string text = dtt.ExtractText();
            return text;
        }

    }

    public class LuceneHelper
    {

    }





    public class DocxToText
    {
        private const string ContentTypeNamespace =
            @"http://schemas.openxmlformats.org/package/2006/content-types";

        private const string WordprocessingMlNamespace =
            @"http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        private const string DocumentXmlXPath =
            "/t:Types/t:Override[@ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml\"]";

        private const string BodyXPath = "/w:document/w:body";

        private string docxFile = "";
        private string docxFileLocation = "";


        public DocxToText(string fileName)
        {
            docxFile = fileName;
        }

        #region ExtractText()
        /// <summary>
        /// Extracts text from the Docx file.
        /// </summary>
        /// <returns>Extracted text.</returns>
        public string ExtractText()
        {
            if (string.IsNullOrEmpty(docxFile))
                throw new java.lang.Exception("Input file not specified.");

            // Usually it is "/word/document.xml"

            docxFileLocation = FindDocumentXmlLocation();

            if (string.IsNullOrEmpty(docxFileLocation))
                throw new java.lang.Exception("It is not a valid Docx file.");

            return ReadDocumentXml();
        }
        #endregion

        #region FindDocumentXmlLocation()
        /// <summary>
        /// Gets location of the "document.xml" zip entry.
        /// </summary>
        /// <returns>Location of the "document.xml".</returns>
        private string FindDocumentXmlLocation()
        {
            ZipFile zip = new ZipFile(docxFile);
            foreach (ZipEntry entry in zip)
            {
                // Find "[Content_Types].xml" zip entry

                if (string.Compare(entry.Name, "[Content_Types].xml", true) == 0)
                {
                    Stream contentTypes = zip.GetInputStream(entry);

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(contentTypes);
                    contentTypes.Close();

                    //Create an XmlNamespaceManager for resolving namespaces

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsmgr.AddNamespace("t", ContentTypeNamespace);

                    // Find location of "document.xml"

                    XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(DocumentXmlXPath, nsmgr);

                    if (node != null)
                    {
                        string location = ((XmlElement)node).GetAttribute("PartName");
                        return location.TrimStart(new char[] { '/' });
                    }
                    break;
                }
            }
            zip.Close();
            return null;
        }
        #endregion

        #region ReadDocumentXml()
        /// <summary>
        /// Reads "document.xml" zip entry.
        /// </summary>
        /// <returns>Text containing in the document.</returns>
        private string ReadDocumentXml()
        {
            StringBuilder sb = new StringBuilder();

            ZipFile zip = new ZipFile(docxFile);
            foreach (ZipEntry entry in zip)
            {
                if (string.Compare(entry.Name, docxFileLocation, true) == 0)
                {
                    Stream documentXml = zip.GetInputStream(entry);

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(documentXml);
                    documentXml.Close();

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsmgr.AddNamespace("w", WordprocessingMlNamespace);

                    XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(BodyXPath, nsmgr);

                    if (node == null)
                        return string.Empty;

                    sb.Append(ReadNode(node));

                    break;
                }
            }
            zip.Close();
            return sb.ToString();
        }
        #endregion

        #region ReadNode()
        /// <summary>
        /// Reads content of the node and its nested childs.
        /// </summary>
        /// <param name="node">XmlNode.</param>
        /// <returns>Text containing in the node.</returns>
        private string ReadNode(XmlNode node)
        {
            if (node == null || node.NodeType != XmlNodeType.Element)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element) continue;

                switch (child.LocalName)
                {
                    case "t":                           // Text
                        sb.Append(child.InnerText.TrimEnd());

                        string space = ((XmlElement)child).GetAttribute("xml:space");
                        if (!string.IsNullOrEmpty(space) && space == "preserve")
                            sb.Append(' ');

                        break;

                    case "cr":                          // Carriage return
                    case "br":                          // Page break
                        sb.Append(Environment.NewLine);
                        break;

                    case "tab":                         // Tab
                        sb.Append("\t");
                        break;

                    case "p":                           // Paragraph
                        sb.Append(ReadNode(child));
                        sb.Append(Environment.NewLine);
                        sb.Append(Environment.NewLine);
                        break;

                    default:
                        sb.Append(ReadNode(child));
                        break;
                }
            }
            return sb.ToString();
        }
        #endregion
    }
}