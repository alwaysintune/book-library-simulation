using System;

namespace BookLibrary.ConsoleApp.Entities
{
    public class BookRecord
    {
        public BookRecord(
            string isbn,
            Guid libraryCardId,
            DateTime returnBy)
        {
            ISBN = isbn;
            LibraryCardId = libraryCardId;
            ReturnBy = returnBy;
        }

        public string ISBN { get; set; }

        public Guid LibraryCardId { get; set; }

        public DateTime TakenOn { get; set; } = DateTime.Now;

        public DateTime ReturnBy { get; set; }

        public bool IsReturned { get; set; }
    }
}
