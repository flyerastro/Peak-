using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Peak;

namespace PeakTest
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
              var s=  Console.ReadLine();
                List<string> jpgs = FileUtil.Search(s);
                Console.WriteLine(jpgs.Count);
                foreach (var variable in jpgs)
                {

                    Console.WriteLine(variable);
                }

                Console.ReadLine();
            }
         


        }
    }
}
