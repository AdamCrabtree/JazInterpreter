using System;
using System.Collections.Generic;
using System.IO;

namespace TokenInterpreter
{
    public class JazParser
    {
        private Dictionary<int, string[]> lines = new Dictionary<int, string[]>();
        private Dictionary<string, int> functions = new Dictionary<string, int>();

        public JazParser(string fileName)
        {
            StreamReader jazFile = null;
            try
            {
                jazFile = new StreamReader(fileName);
            } 
            catch (System.Exception ex)
            {
                Console.WriteLine("Jaz file could not be opened. " + ex.Message);
            }

            int lineNumber = 1;
            while(!jazFile.EndOfStream)
            {
                string lineString = jazFile.ReadLine();
                if (lineString != string.Empty)
                {
                    string[] splitLine = lineString.Trim(' ').Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if(splitLine[0].Equals("label"))
                    {
                        functions.Add(splitLine[1], lineNumber);
                    }
                    lines.Add(lineNumber, splitLine);
                    lineNumber++;
                }
            }
        }

        public Dictionary<int, string[]> getLines()
        {
            return lines;
        } 

        public Dictionary<string, int> getFunctions()
        {
            return functions;
        }
    }
}
