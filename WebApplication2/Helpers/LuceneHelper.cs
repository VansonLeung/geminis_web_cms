using ICSharpCode.SharpZipLib.Zip;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using org.pdfbox.pdmodel;
using org.pdfbox.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace WebApplication2.Helpers
{
    public class LuceneSearchData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class LuceneSearchDataRepository
    {
        public static LuceneSearchData Get(int id)
        {
            return GetAll().SingleOrDefault(x => x.Id.Equals(id));
        }
        public static List<LuceneSearchData> GetAll()
        {
            return new List<LuceneSearchData> {
                new LuceneSearchData {Id = 1, Name = "Belgrad", Description = "City in Serbia"},
                new LuceneSearchData {Id = 2, Name = "Moscow", Description = "City in Russia"},
                new LuceneSearchData {Id = 3, Name = "Chicago", Description = "City in USA"},
                new LuceneSearchData {Id = 4, Name = "Mumbai", Description = "City in India"},
                new LuceneSearchData {Id = 5, Name = "Hong-Kong", Description = "City in Hong-Kong"},
            };
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
            doc.Add(new Field("Name", sampleData.Name, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Description", sampleData.Description, Field.Store.YES, Field.Index.ANALYZED));

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
                Name = doc.Get("Name"),
                Description = doc.Get("Description")
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
            (string searchQuery, string searchField = "")
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
                        (Lucene.Net.Util.Version.LUCENE_30, new[] { "Id", "Name", "Description" }, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search
                    (query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
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