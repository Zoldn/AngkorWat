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
        HttpHelper.SetApiKey("05d74cb4-7f21-424f-afc6-9dc573c82974");

        await Phase1.Phase1Start();

        //var ret = await HttpHelper.Get("http://webcode.me");

        //Console.WriteLine(ret);

        //var user = new User("John Doe", "gardener");

        //var ret2 = await HttpHelper.Post("https://httpbin.org/post", user);

        //Console.WriteLine(ret2);

        //var ddoser = new DDoser<User, string>("https://httpbin.org/post", 3);

        //for (int i = 0; i < 3; i++)
        //{
        //    var user = new User()
        //    {
        //        Name = "John Doe",
        //        Occupation = 100 * i,
        //    };

        //    await ddoser.RunStep(user);
        //}

        //Phase1.Phase1Start();
    }
}