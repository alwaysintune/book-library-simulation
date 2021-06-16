namespace BookLibrary.ConsoleApp.Entities
{
    public class BookFilter
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public string Category { get; set; }

        public string Language { get; set; }

        /// <summary>
        /// By keeping inventory separate, filtering by ISBN is not required
        /// </summary>
        public string ISBN { get; set; }
    }
}
