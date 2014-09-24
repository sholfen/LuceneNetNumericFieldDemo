using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace LuceneNetDemoForNumericField
{
    public class Product
    {
        public string ProductName { get; set; }
        public int Price { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {

        }

        static void IndexItems(List<Product> list)
        {
            Lucene.Net.Store.Directory directory = FSDirectory.Open(new DirectoryInfo(@"D:\Index"));
            IndexWriter indexWriter = new IndexWriter(
                directory,
                new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30),
                false,
                IndexWriter.MaxFieldLength.LIMITED);

            foreach (Product product in list)
            {
                Document doc = new Document();

                Field productName = new Field("ProductName", product.ProductName, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO);
                doc.Add(productName);
                NumericField price = new NumericField("Price", Field.Store.YES, true);
                doc.Add(price);

                indexWriter.AddDocument(doc);
            }

            indexWriter.Optimize();
            indexWriter.Commit();
        }

        static void SearchItems(int min, int max)
        {
            NumericRangeQuery<int> q = NumericRangeQuery.NewIntRange("Price", min, max, true, true);
            Lucene.Net.Store.Directory directory = FSDirectory.Open(new DirectoryInfo(@"D:\Index"));
            IndexSearcher indexSearcher = new IndexSearcher(directory, true);
            ScoreDoc[] scoreDocs = indexSearcher.Search(q, 10).ScoreDocs;

            foreach (ScoreDoc scoreDoc in scoreDocs)
            {
                Document doc = indexSearcher.Doc(scoreDoc.Doc);
                Console.WriteLine(doc.Get("ProductName"));
                Console.WriteLine(doc.Get("Price"));
                Console.WriteLine();


            }
        }
    }
}
