using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace DemoServer
{
    class HttpServer
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";

        public static Dictionary<string, string> data = new Dictionary<string, string>();

        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            while (runServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();

                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.Write(req.UserAgent);

                string ret = "";

                if (req.HttpMethod == "POST")
                {
                    StreamReader reader = new StreamReader(req.InputStream, req.ContentEncoding);
                    string input = reader.ReadToEnd();
                    char[] sep = { ' ' };
                    string[] s = input.Split(sep, 2, StringSplitOptions.None);
                    string comm = s[0];
                    string dat = s[1];

                    switch (comm)
                    {
                        case "SET":
                            Set(dat.Split(sep, 2, StringSplitOptions.None)[0], dat.Split(sep, 2, StringSplitOptions.None)[1]);
                            ret = "Stored";

                            if (dat.IndexOf(" EX ") != -1 && Int32.TryParse(dat.Substring(dat.IndexOf(" EX ") + 4), out int numb))
                            {
                                TTLAsync(dat.Split(sep, 2, StringSplitOptions.None)[0], Int32.Parse(dat.Substring(dat.IndexOf(" EX ") + 4)));
                            }
                            break;
                        case "HGET":
                        case "LGET":
                        case "GET":
                            ret = Get(dat);
                            break;
                        case "DEL":
                            Del(dat);
                            ret = "Deleted";
                            break;
                        case "KEYS":
                            ret = Keys(dat);
                            break;
                    }
                    Console.WriteLine("Command: " + input);
                    Console.WriteLine();
                }

                string disableSubmit = !runServer ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes(ret);
                resp.ContentType = "text/plain";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }


        public static void Main(string[] args)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            listener.Close();
        }

        private static void Set(string key, string value)
        {
            if (data.ContainsKey(key))
            {
                data.Remove(key);
            }
            data.Add(key, value);
        }

        private static string Get(string key)
        {
            if (data.ContainsKey(key))
            {
                return data[key];
            }
            else
            {
                return "The key does not exist";
            }
        }

        private static void Del(string key)
        {
            if (data.ContainsKey(key))
            {
                data.Remove(key);
            }
        }

        private static string Keys(string pattern)
        {
            Dictionary<string, string>.KeyCollection keyColl = data.Keys;
            var ks = data.Keys.Where(k => k.Contains(pattern)).OrderBy(k => k);
            string result = "";
            foreach (string s in ks)
            {
                result += s + " ";
            }
            return result;
        }

        private static void TTL(string key, int time)
        {
            Thread.Sleep(time);
            Del(key);
            Console.WriteLine("Key " + key + " deleted");
            Console.WriteLine();
        }

        private static async void TTLAsync(string key, int time)
        {
            await Task.Run(() => TTL(key, time));
        }
    }
}
