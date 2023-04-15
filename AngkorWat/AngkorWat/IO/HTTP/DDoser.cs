using AngkorWat.IO.JSON;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.IO.HTTP
{
    /// <summary>
    /// Результат одного запроса-ответа
    /// </summary>
    public class DDosRecord<TInput, TOutput>
        where TOutput : class
        where TInput : notnull, new()
    {
        public DateTime TimeStamp { get; set; }
        public int Iteration { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public TInput SendedData { get; set; }
        public TOutput? RespondedData { get; set; }
        public DDosRecord(TInput inputData, TOutput? outputData)
        {
            TimeStamp = DateTime.Now;
            SendedData = inputData;
            RespondedData = outputData;
        }
        public DDosRecord()
        {
            Iteration = 0;
            TimeStamp = DateTime.Now;
            SendedData = new();
            RespondedData = null;
        }
    }

    public class DDosRecordContainer<TInput, TOutput>
        where TOutput : class
        where TInput : notnull, new()
    {
        public List<DDosRecord<TInput, TOutput>> Records { get; set; }
        public DDosRecordContainer()
        {
            Records = new();
        }
    }

    public class DDoser<TInput, TOutput>
        where TOutput : class
        where TInput : notnull, new()
    {
        public string Url { get; }
        public int RetrySeconds { get; }
        public int Iteration { get; private set; }
        public List<DDosRecord<TInput, TOutput>> Records { get; }
        public string SessionFileName { get; }
        public DDoser(string url, int retrySeconds, string sessionFileName = "ddos.json")
        {
            Url = url;
            RetrySeconds = retrySeconds;

            Iteration = 0;

            Records = new();

            SessionFileName = sessionFileName;
        }

        public async Task RunStep(TInput input)
        {
            /// Шлем запрос - получаем ответ
            var task = await HttpHelper.PostWithStatus<TInput, TOutput>(Url, input);
            //var task = await HttpHelper.Post<TInput>(Url, input);

            /// Запоминаем запись
            var (response, statusCode) = task;
            //var tresponse = task;
            //var statusCode = HttpStatusCode.OK;
            //var response = tresponse as TOutput;

            var record = new DDosRecord<TInput, TOutput>(input, response)
            {
                Iteration = ++Iteration,
                StatusCode = statusCode,
            };

            Records.Add(record);
            /// Пишем ее в файл (потому что кто знает, что потом сломается)

            var container = new DDosRecordContainer<TInput, TOutput>
            {
                Records = Records
            };

            IOHelper.SerializeResult(container, SessionFileName);

            /// Ждем следующей итерации
            await Task.Delay(RetrySeconds * 1000);
        } 
    }
}
