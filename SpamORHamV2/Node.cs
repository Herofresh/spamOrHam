using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpamORHamV2
{
    class Node
    {
        public Document.keys parameter;
        public Node valueTrue;
        public Node valueFalse;
        public bool value;

        public Node(List<Document> dataset, List<Document.keys> keysLeft)
        {
            if (dataset.Count == 0 || keysLeft.Count == 0)
                value = getDominantValue(dataset);
            else if (dataset[0].spam && dataset.FindIndex(x => x.spam == false) == -1 || !dataset[0].spam && dataset.FindIndex(x => x.spam == true) == -1)
                value = dataset[0].spam;
            else
            {
                double max = double.MinValue;
                foreach (var key in keysLeft)
                {
                    double gain = Gain(dataset, key);
                    if (gain > max)
                    {
                        max = gain;
                        parameter = key;
                    }
                }

                List<Document>[] splittDataSet = SplittDataSet(dataset, parameter);
                keysLeft.Remove(parameter);
                valueTrue = new Node(splittDataSet[0], keysLeft);
                valueFalse = new Node(splittDataSet[1], keysLeft);
            }

            
        }

        public List<Document>[] SplittDataSet(List<Document> dataset, Document.keys key)
        {
            List<Document>[] returnValue = new List<Document>[]
            {
                new List<Document>(),
                new List<Document>()
            };
            foreach (var d in dataset)
            {
                if (d.vector[key] == true)
                    returnValue[0].Add(d);
                else
                    returnValue[1].Add(d);
            }
            return returnValue;
        }

        private bool getDominantValue(List<Document> dataset)
        {
            double pos = 0, neg = 0;
            foreach (var d in dataset)
            {
                if (d.spam)
                    pos++;
                else
                    neg++;
            }
            return pos > neg;
        }

        private double[] CountPosNeg(List<Document> dataset)
        {
            double pos = 0, neg = 0;
            foreach (var d in dataset)
            {
                if (d.spam)
                    pos++;
                else
                    neg++;
            }
            double[] returnVal = new double[] { pos / dataset.Count, neg / dataset.Count };
            return returnVal;
        }

        private double Entropy(double p, double n)
        {
            double a = p + n;
            if (a == 0)
                return 0;
            p = p / a;
            n = n / a;
            if (p == 0 || n == 0)
                return 0;
            double e = -(p * Math.Log(p, 2)) - (n * Math.Log(n, 2));
            return e;
        }

        private double Percentage(double val, double all)
        {
            if (all == 0)
                return 0;
            return val / all;
        }

        private double TotalEntropy(List<Document> dataset)
        {
            double[] posNeg = CountPosNeg(dataset);
            double e = -(posNeg[0] * Math.Log(posNeg[0],2)) - (posNeg[1] * Math.Log(posNeg[1],2));
            return e;
        }

        public double Gain(List<Document> dataset, Document.keys key)
        {
            double spamAttrTrue = 0, spamAttrFalse = 0, notSpamAttrTrue = 0, notSpamAttrFalse = 0;
            foreach (var d in dataset)
            {
                if (d.spam)
                {
                    if (d.vector[key])
                        spamAttrTrue++;
                    else
                        spamAttrFalse++;
                }
                else
                {
                    if (d.vector[key])
                        notSpamAttrTrue++;
                    else
                        notSpamAttrFalse++;
                }
            }
            double sumAttrTrue = spamAttrTrue + notSpamAttrTrue;
            double sumAttrFalse = spamAttrFalse + notSpamAttrFalse;
            double attrEntropy = Percentage(sumAttrTrue, dataset.Count) * Entropy(spamAttrTrue, notSpamAttrTrue) + Percentage(sumAttrFalse, dataset.Count) * Entropy(spamAttrFalse, notSpamAttrFalse);
            return TotalEntropy(dataset) - attrEntropy;
        }
    }
}
