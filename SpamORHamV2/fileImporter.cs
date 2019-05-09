using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpamORHamV2
{
    class FileImporter
    {
        public List<string> importFile()
        {
            List<string> allData = new List<string>();
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var path = ofd.FileName;

                    var fileStream = ofd.OpenFile();
                    string currendLine = "";
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        do
                        {
                            currendLine = reader.ReadLine();
                            if (!currendLine.Equals(""))
                            {
                                allData.Add(currendLine);
                            }
                        } while (!reader.EndOfStream);
                    }
                }
            }
            return allData;
        }
    }
}
