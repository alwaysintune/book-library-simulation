using BookLibrary.ConsoleApp.Entities;
using BookLibrary.ConsoleApp.Services.Exceptions;
using System;
using System.Collections.Generic;

namespace BookLibrary.ConsoleApp.Services.Library
{
    public class LibraryService : ILibraryService
    {
        public readonly Dictionary<string, LibraryBook> LibraryBooks;
        public readonly Dictionary<Guid, LibraryMember> LibraryMembers;

        public int MaximumBorrowMonths { get; private set; } = 2;
        public int MaximumBorrowedBooks { get; private set; } = 3;

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

        public void BorrowBook(BookRecord bookRecord)
        {
            if (bookRecord.TakenOn > DateTime.Now ||
                bookRecord.ReturnBy <= bookRecord.TakenOn)
            {
                throw new ArgumentException("Either return by or taken on date is erroneous");
            }

            if (bookRecord.ReturnBy >
                bookRecord.TakenOn.AddMonths(MaximumBorrowMonths))
            {
                throw new BusinessRuleException($"Return by date may not be longer than {MaximumBorrowMonths} month(-s)");
            }

            if (!LibraryMembers.ContainsKey(bookRecord.LibraryCardId))
            {
                throw new BusinessRuleException("Before borrowing, please register as a library member");
            }

            if (!LibraryBooks.ContainsKey(bookRecord.ISBN))
            {
                throw new BookNotFoundException("Book to be borrowed is not owned by the library");
            }

            List<BookRecord> borrowedBooks = LibraryMembers[bookRecord.LibraryCardId].Records
                .FindAll(x => x.IsReturned == false);
            if (borrowedBooks.Count >= MaximumBorrowedBooks)
            {
                throw new BusinessRuleException($"No more than {MaximumBorrowedBooks} book(-s) may be borrowed at once");
            }

            var bookInventory = LibraryBooks[bookRecord.ISBN].Inventory;
            if (bookInventory.AvailableCount <= 0)
            {
                throw new BusinessRuleException("Book to be borrowed is not available at the moment");
            }

            bookInventory.AvailableCount--;
            bookInventory.TakenCount++;
            bookRecord.IsReturned = false;

            borrowedBooks = LibraryMembers[bookRecord.LibraryCardId].Records;
            borrowedBooks.Add(bookRecord);
        }

        public void ReturnBook(string ISBN, Guid libraryCardId)
        {
            var bookRecord = LibraryMembers[libraryCardId].Records
                .Find(x => x.IsReturned == false &&
                           x.ISBN == ISBN);

            if (bookRecord == null)
            {
                throw new BookRecordNotFoundException("No record of book being borrowed was found");
            }

            if (!LibraryBooks.ContainsKey(bookRecord.ISBN))
            {
                throw new BookNotFoundException("Book to be returned was removed from the library. Consider adding it");
            }

            var bookInventory = LibraryBooks[bookRecord.ISBN].Inventory;
            bookInventory.AvailableCount++;
            bookInventory.TakenCount--;
            bookRecord.IsReturned = true;

            if (DateTime.Now > bookRecord.ReturnBy)
            {
                int overdueDays = DateTime.Now
                    .Subtract(bookRecord.ReturnBy).Days;

                throw new BusinessRuleException($"Book's return is {overdueDays} day(-s) overdue past the period");
            }
        }
    }
}
