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

namespace Index
{
    class Indexing
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
                Console.WriteLine(i);
                i++;
                writera.AddDocument(documenta);
            }



            writera.Optimize();
            writera.Flush(true, true, true);

        }
    }
}
