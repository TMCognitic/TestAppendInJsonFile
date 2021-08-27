using System;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestJson
{
    class Program
    {
        static void Main(string[] args)
        {
            const string FILENAME = "data.json";

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", "C# Application");

            HttpResponseMessage httpResponseMessage = httpClient.GetAsync("https://api.openweathermap.org/data/2.5/weather?q=mons&units=metric&appid=[yourtoken]").Result;
            httpResponseMessage.EnsureSuccessStatusCode();

            if(!File.Exists(FILENAME))
            {
                using (TextWriter tw = File.CreateText(FILENAME))
                {
                    tw.WriteLine("{ values: [] }");
                }
            }

            string Json = httpResponseMessage.Content.ReadAsStringAsync().Result;
            JObject result = JObject.Parse(Json);

            JObject jObject = JObject.Parse(File.ReadAllText(FILENAME));
            JArray values = (JArray)jObject["values"];
            values.Add(result);

            using (StreamWriter streamWriter = File.CreateText(FILENAME))
            {
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented })
                {
                    jObject.WriteTo(jsonTextWriter);
                }
            }
        }
    }
}
