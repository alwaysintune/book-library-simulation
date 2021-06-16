using BookLibrary.ConsoleApp.Entities;
using BookLibrary.ConsoleApp.Enums;
using System;
using System.Collections.Generic;

namespace BookLibrary.ConsoleApp.Services.Library
{
    public interface ILibraryService
    {
        void AddBook(Book book, int count);

        void BorrowBook(BookRecord bookRecord);

        void ReturnBook(string ISBN, Guid LibraryCardId);

        List<LibraryBook> ListAllBooks(
            BookFilter bookFilter = null,
            BookState stateFilter = BookState.Available
            );

        void DeleteBook(string ISBN);
    }
}
