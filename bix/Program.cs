using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using Cognex.VisionPro;
using Cognex.VisionPro.QuickBuild;

namespace bix
{
    class Program
    {
        static int Main(string[] args)
        {
            //     1          2             3          4
            // bix source.vpp [--bin|--xml] target.vpp [option]
            string help = "usage: bix source.vpp [--bin|--xml] target.vpp [option]\n\t option: min, results, all";
            if (args.Length < 3 || args.Length > 4)
            {
                foreach (var s in args)
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine(help);
                return -1;
            }
            var source = args[0];
            var type = args[1].Replace("-", "").ToLower();
            var target = args[2];
            var option = args.Length == 4 ? args[3].ToLower() : "";
            CogJobManager vpro = null;
            try
            {
                if (!System.IO.File.Exists(source))
                {
                    Console.WriteLine("source does not exits.");
                    return -1;
                }
                vpro = (CogJobManager)CogSerializer.LoadObjectFromFile(source);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
            if (string.IsNullOrEmpty(option))
            {
                try
                {
                    CogSerializer.SaveObjectToFile(vpro, target, type == "xml" ? typeof(SoapFormatter) : typeof(BinaryFormatter));
                }
                catch (System.Runtime.Serialization.SerializationException ex)
                {
                    Console.WriteLine(ex.Message);
                    return -1;
                }
                Console.WriteLine("converted");
            }
            else
            {
                Dictionary<string, CogSerializationOptionsConstants> options = new Dictionary<string, CogSerializationOptionsConstants> 
                { 
                    {"min", CogSerializationOptionsConstants.Minimum},
                    {"results", CogSerializationOptionsConstants.Results},
                    {"all", CogSerializationOptionsConstants.All}
                };
                try
                {
                    CogSerializer.SaveObjectToFile(vpro, target,
                        type == "xml" ? typeof(SoapFormatter) : typeof(BinaryFormatter),
                        options[option]
                        );
                }
                catch (System.Runtime.Serialization.SerializationException ex)
                {
                    Console.WriteLine(ex.Message);
                    return -1;
                }
                Console.WriteLine("converted (" + option + ")");
            }
            vpro.Shutdown();
            return 0;
            //Console.ReadKey();
        }
    }
}
