using BookLibrary.ConsoleApp.Entities;
using System;

namespace BookLibrary.ConsoleApp.Services.Library
{
    public interface ILibraryService
    {
        void AddBook(Book book, int count);

        void BorrowBook(BookRecord bookRecord);

        void ReturnBook(string ISBN, Guid LibraryCardId);
    }
}
