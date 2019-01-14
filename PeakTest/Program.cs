using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Peak;

namespace PeakTest
{
    class Program
    {
        public static void Main()
        {
            XmlNodeReader reader = null;

            try
            {
                //Create and load the XML document.
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<book xmlns:bk='urn:samples'> " +
                            "<title>Pride And Prejudice</title>" +
                            "<bk:genre>novel</bk:genre>" +
                            "</book>");

                //Load the XmlNodeReader 
                reader = new XmlNodeReader(doc);

                //Parse the XML.  If they exist, display the prefix and  
                //namespace URI of each node.
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Prefix == String.Empty)
                            Console.WriteLine("LocalName:<{0}>", reader.LocalName);
                        else
                        {
                            Console.Write("Prifix:<{0} LocalName:{1}>", reader.Prefix, reader.LocalName);
                            Console.WriteLine(" The namespace URI is " + reader.NamespaceURI);
                        }
                    }
                }

            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            Console.Read();
        }


    }

    public class Test
    {
        public string Name { get; set; }
        public Order Order { get; set; }
    }

    public class Order
    {
       public string OrderId { get; set; }
       public DateTime Pdate { get; set; }
    }
}
