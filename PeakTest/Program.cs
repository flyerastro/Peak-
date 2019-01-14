using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Peak;
using PeakWebBase;

namespace PeakTest
{
    class Program
    {
        public static void Main()
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
}
