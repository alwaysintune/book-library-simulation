using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.ConsoleApp.Services.JsonBuilder
{
    public class JsonService : IJsonService
    {
        public string SerializeObject<T>(T t)
        {
            string json = JsonConvert.SerializeObject(t, Formatting.Indented);

            return json;
        }

        public string SerializeObject<T>(List<T> t)
        {
            string json = JsonConvert.SerializeObject(t, Formatting.Indented);

            return json;
        }

        public T DeserializeObject<T>(string json)
        {
            T t = JsonConvert.DeserializeObject<T>(json);

            return t;
        }

        public async Task WriteToFileAsync<T>(T t, string filePath)
        {
            string json = SerializeObject(t);

            await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
        }

        public async Task<T> ReadFromFileAsync<T>(string filePath)
        {
            using StreamReader file = File.OpenText(filePath);
            string json = await file.ReadToEndAsync();

            T t = DeserializeObject<T>(json);

            return t;
        }

        public void WriteToFile<T>(T t, string filePath)
        {
            string json = SerializeObject(t);

            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        public T ReadFromFile<T>(string filePath)
        {
            using StreamReader file = File.OpenText(filePath);
            string json = file.ReadToEnd();

            T t = DeserializeObject<T>(json);

            return t;
        }
    }
}
