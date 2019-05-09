using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpamORHamV2
{
    class Tree
    {
        public Node root;
        List<Document> dataset;

        public Tree()
        {
            dataset = new List<Document>();
            FileImporter FI = new FileImporter();
            List<string> rawText = FI.importFile();
            foreach(var line in rawText)
            {
                dataset.Add(new Document(line));
            }
            List<Document.keys> keyList = new List<Document.keys>();
            foreach(Document.keys val in (Document.keys[]) Enum.GetValues(typeof(Document.keys)))
            {
                keyList.Add(val);
            }
            root = new Node(dataset, keyList);
        }

        public bool climbTree(Node currentNode, Document document)
        {
            if (currentNode.valueTrue == null)
                return currentNode.value;
            else
            {
                if (document.vector[currentNode.parameter] == true)
                    return climbTree(currentNode.valueTrue, document);
                else
                    return climbTree(currentNode.valueFalse, document);
            }
        }
    }
}
