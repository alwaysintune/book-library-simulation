using System.Threading.Tasks;

namespace BookLibrary.ConsoleApp.Services.JsonBuilder
{
    public interface IJsonService
    {
        string SerializeObject<T>(T t);

        T DeserializeObject<T>(string json);

        Task WriteToFileAsync<T>(T t, string filePath);

        Task<T> ReadFromFileAsync<T>(string filePath);

        void WriteToFile<T>(T t, string filePath);

        T ReadFromFile<T>(string filePath);
    }
}
