using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BulkUpload
{
    class DataReader
    {
       /* static void Main(string[] args)
        {
            Console.WriteLine("Starting data reader.....");
            using (var fs = File.OpenRead(@"C:\log\data.csv"))
            using (var reader = new StreamReader(fs))
            {
                List<List<string>> outerList = new List<List<string>>();
               // List<> listB = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    List<string> innnerList = new List<string>();

                    foreach(String s in values) 
                    {
                        innnerList.Add(s);
                    }
                    outerList.Add(innnerList);

                    //     listA.Add(values[0]);
                    //     listB.Add(values[1]);

                }

                foreach (List<string> item in outerList)
                {
                    foreach(string str in item)
                    {
                        Console.Write(str + " ");
                    }
                    Console.WriteLine();
                }
            }
        } */
    }
}
