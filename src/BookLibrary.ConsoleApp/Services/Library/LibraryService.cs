using BookLibrary.ConsoleApp.Entities;
using System;
using System.Collections.Generic;

namespace BookLibrary.ConsoleApp.Services.Library
{
    public class LibraryService : ILibraryService
    {
        public readonly Dictionary<string, LibraryBook> LibraryBooks;

        public LibraryService()
        {
            LibraryBooks = new Dictionary<string, LibraryBook>();
        }

        /// <summary>
        /// If book already exists, only increases availability count
        /// </summary>
        /// <param name="book"></param>
        /// <param name="count"></param>
        public void AddBook(Book book, int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException(
                    "Count may not be equal to or less than zero",
                    paramName: nameof(count)
                    );
            }

            if (LibraryBooks.ContainsKey(book.ISBN))
            {
                if (LibraryBooks[book.ISBN].Book == book) // record value equality
                {
                    LibraryBooks[book.ISBN].Inventory.AvailableCount += count;
                }
                else
                {
                    throw new ArgumentException(
                        "Trying to add a different book with existing ISBN",
                        paramName: nameof(book));
                }
            }
            else
            {
                LibraryBooks.Add(
                    book.ISBN,
                    new LibraryBook(
                        book,
                        new BookInventory
                        {
                            TakenCount = 0,
                            AvailableCount = count
                        })
                    );
            }
        }
    }
}
