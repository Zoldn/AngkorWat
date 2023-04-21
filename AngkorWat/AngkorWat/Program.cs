using AngkorWat.Components;
using Newtonsoft.Json;
using System.Net;
using AngkorWat.Phases;
using System;
using System.Text;
using AngkorWat.IO.HTTP;

internal class Program
{
    public class User
    {
        public string Name { get; set; }
        public int Occupation { get; set; }
        public User()
        {
            Name = string.Empty;
        }
    }

    private static async Task Main(string[] _)
    {
        HttpHelper.SetApiKey("643ec1df4dabc643ec1df4dac0");

        //Phase1.Phase1Start();
    }

    /// <summary>
    /// 
    /// </summary>
    private static async void Test()
    {
        var ddoser = new DDoser<User, string>("https://httpbin.org/post", 3);

        for (int i = 0; i < 3; i++)
        {
            var user = new User()
            {
                Name = "John Doe",
                Occupation = 100 * i,
            };

            await ddoser.RunStep(user);
        }
    }
}