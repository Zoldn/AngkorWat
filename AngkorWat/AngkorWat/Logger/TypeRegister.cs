using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Logger
{
    public static class TypeRegister
    {
        public static object? GetParsedObject(string data, int typeId)
        {
            return typeId switch
            {
                0 => JsonConvert.DeserializeObject<int>(data),
                _ => default,
            };
        }

        public static int GetTypeId<T>(T t)
        {
            return t switch
            {
                int => 0,
                _ => -1,
            };
        }
    }
}
