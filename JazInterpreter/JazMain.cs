
using System;
using System.Collections.Generic;

namespace TokenInterpreter
{
    class JazMain
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please give the path of the file");
            String filePath = Console.ReadLine();
            JazParser parser = new JazParser(filePath);
            Dictionary<int, string[]> lines = parser.getLines();
            Dictionary<string, int> labels = parser.getFunctions();
            TokenInterpreter myInterpreter = new TokenInterpreter();
            myInterpreter.interpret(lines, labels);
            System.Threading.Thread.Sleep(200000);
        }
    }
}
