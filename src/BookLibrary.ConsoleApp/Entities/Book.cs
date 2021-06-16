using System;

namespace BookLibrary.ConsoleApp.Entities
{
    public record Book
    {
        public string Name { get; init; }

        public string Author { get; init; }

        public string Category { get; init; }

        public string Language { get; init; }

        public DateTime PublicationDate { get; init; }

        public string ISBN { get; init; }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Author)}: {Author}, " +
                $"{nameof(Category)}: {Category}, {nameof(Language)}: {Language}, " +
                $"{nameof(PublicationDate)}: {PublicationDate:yyyy-MM-dd}, {nameof(ISBN)}: {ISBN}";
        }
    }
}
