using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace DemoClient
{
    class Program
    {
        static string LastComm = "";

        public static void Main(string[] args)
        {
            string url = "http://localhost:8000/";
            string[] commands = new string[] { "SET", "GET", "HGET", "LGET", "DEL", "KEYS", "EXIT" };

            while (true)
            {
                string input = Console.ReadLine();

                string[] split = input.Split(' ');

                if (split[0] == "EXIT") { Environment.Exit(0); }
                if (!commands.Contains(split[0]))
                {
                    Console.WriteLine("Unknown command. Posible commands:");
                    foreach (string c in commands)
                    {
                        Console.Write(" " + c);
                    }
                    Console.WriteLine();
                }
                else { LastComm = split[0]; Post(input, url).GetAwaiter().GetResult(); }
            }
        }

        static async Task Post(string dat, string url)
        {
            var data = new StringContent(dat, Encoding.UTF8, "text/plain");
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;

            switch (LastComm)
            {
                case "SET":
                case "GET":
                case "DEL":
                    Console.WriteLine(result);
                    Console.WriteLine();
                    break;
                case "KEYS":
                    result.TrimEnd(' ');
                    string[] s = result.Split(' ');
                    foreach (string str in s)
                    {
                        Console.WriteLine(str);
                    }
                    break;
                case "LGET":
                    string[] Lsplit = result.Split(' ');
                    for (int i = 0; i < Lsplit.Length; i++)
                    {
                        Console.WriteLine(Lsplit[i]);
                    }
                    Console.WriteLine();
                    break;
                case "HGET":
                    string[] Hsplit = result.Split(' ');
                    for (int i = 0; i < Hsplit.Length - 1; i = i + 2)
                    {
                        Console.WriteLine(Hsplit[i] + " " + Hsplit[i + 1]);
                    }
                    Console.WriteLine();
                    break;
                default:
                    Console.WriteLine("???");
                    break;
            }
        }
    }
}
