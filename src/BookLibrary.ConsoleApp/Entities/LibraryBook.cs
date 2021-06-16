namespace BookLibrary.ConsoleApp.Entities
{
    public class LibraryBook
    {
        public LibraryBook(Book book, BookInventory bookInventory)
        {
            Book = book;
            Inventory = bookInventory;
        }

        public Book Book { get; init; }

        public BookInventory Inventory { get; init; }
    }
}
