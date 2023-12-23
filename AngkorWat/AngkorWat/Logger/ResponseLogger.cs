using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Logger
{
    internal class ResponseLogger
    {
        public LoggerData Data { get; set; } = new LoggerData();
        public string LogFilePath { get; set; } = string.Empty;
        public ResponseLogger(string logPath) 
        {
            LogFilePath = logPath;
        }

        public void ReadFromFile()
        {
            try
            {
                string json = File.ReadAllText(LogFilePath);
                var container = JsonConvert.DeserializeObject<LoggerData>(json);
                if (container == null)
                {
                    throw new FileLoadException();
                }
            }
            catch (Exception)
            { 
                Data = new LoggerData();
            }
        }

        public void WriteToFile()
        {
            var json = JsonConvert.SerializeObject(Data);

            File.WriteAllText(LogFilePath, json);
        }

        public void AddItem<TRequest, TResponse>(TRequest? request, TResponse? response, int code = 200)
        {
            Data.Items.Add(new LogItem()
            {
                RequestClassId = TypeRegister.GetTypeId(typeof(TRequest)),
                ResponseClassId = TypeRegister.GetTypeId(typeof(TResponse)),
                RequestString = JsonConvert.SerializeObject(request),
                ResponseString = JsonConvert.SerializeObject(response),
                ReturnCode = code,
                TimeStamp = DateTime.Now,
            });
        }
    }
}
