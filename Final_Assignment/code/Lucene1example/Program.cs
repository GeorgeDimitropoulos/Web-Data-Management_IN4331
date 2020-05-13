using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;
using Analyzer = Lucene.Net.Analysis.Analyzer;
using Version = Lucene.Net.Util.Version;
using System.Data;
using System.Xml.Linq;

namespace Lucene1example
{


    class Program
    {
        static void Main(string[] args)
        {


            DataSet tb = new DataSet();
            string filePath = "C:\\Users\\pc1\\Desktop\\WebDataManagement\\keyword.xml";
            tb.ReadXml(filePath);
            DataTable table = tb.Tables[0];

            Lucene.Net.Store.Directory directorya = FSDirectory.Open(Environment.CurrentDirectory + "\\LuceneIndex");

            Analyzer analyzera = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Version.LUCENE_29);

            var writera = new Lucene.Net.Index.IndexWriter(directorya, analyzera, true, Lucene.Net.Index.IndexWriter.MaxFieldLength.LIMITED);
            int i = 1;
            foreach (DataRow row in table.Rows)
            {
                var documenta = new Document();
                documenta.Add(new Field("Id", row["id"].ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                documenta.Add(new Field("keywordlist", row["keywordlist"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                documenta.Add(new Field("phonetic_code", row["phonetic_code"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                //documenta.Add(new Field("all",row["keywordlist"].ToString()+" "+ row["phonetic_code"].ToString(),Field.Store.YES,Field.Index.ANALYZED));
               
                Console.WriteLine(i);
                i++;
                writera.AddDocument(documenta);
            }



            writera.Optimize();
            writera.Flush(true, true, true);
            /////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////searcher starts/////////////////////////////////////////////

            var watch = System.Diagnostics.Stopwatch.StartNew();
            Lucene.Net.Index.IndexReader indexReader = Lucene.Net.Index.IndexReader.Open(directorya, true);
            Lucene.Net.Search.Searcher indexSearcher = new Lucene.Net.Search.IndexSearcher(indexReader);

            // var queryParser = new Lucene.Net.QueryParsers.QueryParser(Version.LUCENE_29, "keywordlist", analyzera);
            string[] cols ={ "keywordlist", "phonetic_code"};
            var queryParser = new Lucene.Net.QueryParsers.MultiFieldQueryParser(Version.LUCENE_29, cols, analyzera);
            queryParser.AllowLeadingWildcard = true;
            var query = queryParser.Parse("Kill");

            Console.WriteLine("Searching for query: " + query);

            Lucene.Net.Search.TopDocs resultDocs = indexSearcher.Search(query, indexReader.MaxDoc);

            var hits = resultDocs.ScoreDocs;
            int count = 0;
            List<String> retrievedDocs = new List<string>();
            foreach (var hit in hits)
            {
                count++;
                var id = hit.Doc;
                var retrievedDoc = indexSearcher.Doc(id);
                string docToAdd = "ID: " + retrievedDoc.GetValues("Id")[0] + ",keyword: " + retrievedDoc.GetValues("keywordlist")[0] + ",phonetic_code: " + retrievedDoc.GetValues("phonetic_code")[0];
                //retrievedDocs.Add(docToAdd);
                Console.WriteLine(docToAdd);
                Console.WriteLine(" ");
                

            }

            //Console.WriteLine("Retrieved Documents:");

            //for (i = 0; i < retrievedDocs.Count; i++)
            //{
            //  Console.WriteLine(retrievedDocs[i].ToString());
            //}
            Console.WriteLine("Searching for query: " + query);
            Console.WriteLine(" ");
            Console.WriteLine("Number of Occurence:" + count);

            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Excecution time:" + elapsedMs);
            Console.ReadLine();
        }
    }
    }




    

