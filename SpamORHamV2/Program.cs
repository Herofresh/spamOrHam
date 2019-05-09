using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpamORHamV2
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Loading...");
            Console.WriteLine("----------------------------------------");
            Tree tree = new Tree();
            FileImporter FI = new FileImporter();
            List<string> allLines = FI.importFile();
            List<Document> testData = new List<Document>();
            foreach(var line in allLines)
            {
                testData.Add(new Document(line));
            }
            int TP = 0, FP = 0, FN = 0, TN = 0;
            foreach(var data in testData)
            {
                bool outcome = tree.climbTree(tree.root, data);
                if (data.spam && outcome)
                    TP++;
                else if (data.spam && !outcome)
                {
                    Console.WriteLine(data.msg);
                    FN++;
                }
                else if (!data.spam && outcome)
                    FP++;
                else if (!data.spam && !outcome)
                    TN++;
            }
            Console.WriteLine("TP: " + TP + " FP: " + FP);
            Console.WriteLine("FN: " + FN + " TN: " + TN);
            Console.ReadLine();
        }
    }
}
