using BookLibrary.ConsoleApp.Entities;

namespace BookLibrary.ConsoleApp.Services.Library
{
    public interface ILibraryService
    {
        void AddBook(Book book, int count);

        void BorrowBook(BookRecord bookRecord);
    }
}
