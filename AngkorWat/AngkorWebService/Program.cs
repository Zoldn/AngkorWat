using AngkorWebService.Models;

namespace AngkorWebService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");
            app.MapPost("/run", RunHandler);

            app.Run();
        }

        public static async Task<IResult> RunHandler(TestInputObject inputObject)
        {
            var outputObject = new TestOutputObject();

            Console.WriteLine(inputObject.Id);
            Console.WriteLine(inputObject.Name);
            Console.WriteLine(inputObject.IsActive);
            Console.WriteLine(inputObject.Size);

            outputObject.ResultMap.Add(1, 3);
            outputObject.ResultMap.Add(2, 67);

            outputObject.ResultList.Add(5);
            outputObject.ResultList.Add(54);
            outputObject.ResultList.Add(5454);

            await Task.Delay(1000);

            Console.WriteLine("1");

            return TypedResults.Ok(outputObject);
        }
    }
}
