using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.IO.JSON
{
    internal class BaseOutputContainer
    {
    }

    internal static class IOHelper
    {
        public static string FilesRoute => Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..");
        public static void SerializeResult<T>(T output, string filename = "result.json")
        {
            var json = JsonConvert.SerializeObject(output);

            string path = Path.Combine(FilesRoute, filename);

            File.WriteAllText(path, json);
        }

        public static T ReadInputData<T>(string fileName)
            where T : new()
        {
            string path = Path.Combine(FilesRoute, fileName);

            string json = File.ReadAllText(path);

            var container = JsonConvert.DeserializeObject<T>(json);

            if (container == null)
            {
                throw new FileLoadException();
            }

            return container;
        }
    }
}
