using System.Collections.Generic;

namespace BookLibrary.ConsoleApp.Entities
{
    public class LibraryMember
    {
        public LibraryMember(LibraryCard libraryCard)
        {
            Card = libraryCard;
        }

        public List<BookRecord> Records { get; set; } = new List<BookRecord>();

        public LibraryCard Card { get; set; }
    }
}
