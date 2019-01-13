using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Peak;
using PeakWebBase;

namespace PeakTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("王永群".BinaryDecoding(Encoding.UTF8));


            Console.WriteLine(new {Name="王永群",Age="28",Sex="男",Like=new[]{@"运动",@"电脑"},Order=new{OrderId="20181231",OrderDateTime=DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}}.ObjectToJson());

            Console.ReadLine();





            //while (true)
            //{
            //  var s=  Console.ReadLine();
            //    List<string> jpgs = FileUtil.Search(s);
            //    Console.WriteLine(jpgs.Count);
            //    foreach (var variable in jpgs)
            //    {

            //        Console.WriteLine(variable);
            //    }

            //    Console.ReadLine();
            //}
         


        }
      
    }
}
