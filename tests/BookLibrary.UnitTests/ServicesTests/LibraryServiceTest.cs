using BookLibrary.ConsoleApp.Entities;
using BookLibrary.ConsoleApp.Services.Exceptions;
using BookLibrary.ConsoleApp.Services.Library;
using System;
using System.Collections.Generic;
using Xunit;

namespace BookLibrary.UnitTests.ServicesTests
{
    public class LibraryServiceTest
    {
        private ILibraryService _libraryService;
        private Book _book;
        private BookFilter _bookFilter;
        private BookRecord _bookRecord;
        private LibraryCard _libraryCard;
        private DateTime _dateTimeNow;

        public LibraryServiceTest()
        {
            _libraryService = new LibraryService();
            _dateTimeNow = DateTime.Now;
            _book = new Book
            {
                Name = "_name1",
                Author = "_author1",
                Category = "_cat1",
                Language = "_lang1",
                ISBN = "_isbn1",
                PublicationDate = _dateTimeNow
            };
            _bookFilter = new BookFilter
            {
                Name = "_name1",
                Author = "_author1",
                Category = "_cat1",
                Language = "_lang1",
                ISBN = "_isbn1"
            };
            _bookRecord = new BookRecord(
                "_isbn1",
                Guid.Empty,
                _dateTimeNow
                );

            var address = new Address("_street", "_zipcode");
            _libraryCard = new LibraryCard("_fullname", address);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AddBook_ThrowsArgumentException_WhenCountBelowOne(int count)
        {
            void act() => _libraryService.AddBook(_book, count);

            ArgumentException exception = Assert.Throws<ArgumentException>(act);
            Assert.Equal("count", exception.ParamName);
        }

        [Fact]
        public void AddBook_ThrowsArgumentException_WhenDifferentBookWithExistingISBN()
        {
            var book2 = _book with { Name = "_name2" };

            _libraryService.AddBook(_book, 1);
            void act() => _libraryService.AddBook(book2, 1);

            ArgumentException exception = Assert.Throws<ArgumentException>(act);
            Assert.Equal("book", exception.ParamName);
        }

        [Fact]
        public void AddBook_IncreasesAvailableCount_WhenBookAlreadyExists()
        {
            int count = 1;
            int repeat = 2;

            for (int i = 0; i < repeat; i++)
            {
                _libraryService.AddBook(_book, count);
            }
            List<LibraryBook> books = _libraryService.ListAllBooks();

            Assert.Single(books);
            Assert.Equal(repeat * count, books[0].Inventory.AvailableCount);
        }

        [Fact]
        public void BorrowBook_ThrowsArgumentException_WhenTakenOnDateIsAheadOfReturnByDate()
        {
            _bookRecord.ReturnBy = _dateTimeNow.AddMonths(-1);

            void act() => _libraryService.BorrowBook(_bookRecord);

            ArgumentException exception = Assert.Throws<ArgumentException>(act);
            Assert.Equal("Either return by or taken on date is erroneous", exception.Message);
        }

        [Fact]
        public void BorrowBook_ThrowsBusinessRuleException_WhenReturnByDateIsAheadOfMaximumBorrowMonths()
        {
            _bookRecord.ReturnBy = _dateTimeNow.AddMonths(_libraryService.MaximumBorrowMonths + 1);

            void act() => _libraryService.BorrowBook(_bookRecord);

            BusinessRuleException exception = Assert.Throws<BusinessRuleException>(act);
            Assert.Equal(
                $"Return by date may not be longer than {_libraryService.MaximumBorrowMonths} month(-s)",
                exception.Message
                );
        }

        [Fact]
        public void BorrowBook_ThrowsBusinessRuleException_WhenLibraryMemberNotFound()
        {
            _bookRecord.ReturnBy = _dateTimeNow.AddMonths(_libraryService.MaximumBorrowMonths);

            void act() => _libraryService.BorrowBook(_bookRecord);

            BusinessRuleException exception = Assert.Throws<BusinessRuleException>(act);
            Assert.Equal("Before borrowing, please register as a library member", exception.Message);
        }

        [Fact]
        public void BorrowBook_ThrowsBookNotFoundException_WhenBookWithGivenISBNDoesNotExist()
        {
            _bookRecord.ReturnBy = _dateTimeNow.AddMonths(_libraryService.MaximumBorrowMonths);

            Guid libraryCardId = _libraryService.RegisterLibraryMember(_libraryCard);
            _bookRecord.LibraryCardId = libraryCardId;
            void act() => _libraryService.BorrowBook(_bookRecord);

            BookNotFoundException exception = Assert.Throws<BookNotFoundException>(act);
            Assert.Equal("Book to be borrowed is not owned by the library", exception.Message);
        }

        [Fact]
        public void BorrowBook_ThrowsBusinessRuleException_WhenMoreThanAllowedMaximumBorrowedBooks()
        {
            _bookRecord.ReturnBy = _dateTimeNow.AddMonths(_libraryService.MaximumBorrowMonths);

            _libraryService.AddBook(_book, 4);
            Guid libraryCardId = _libraryService.RegisterLibraryMember(_libraryCard);
            _bookRecord.LibraryCardId = libraryCardId;

            for (int i = 0; i < _libraryService.MaximumBorrowedBooks; i++)
            {
                _libraryService.BorrowBook(_bookRecord);
            }

            void act() => _libraryService.BorrowBook(_bookRecord);

            BusinessRuleException exception = Assert.Throws<BusinessRuleException>(act);
            Assert.Equal(
                $"No more than {_libraryService.MaximumBorrowedBooks} book(-s) may be borrowed at once",
                exception.Message
                );
        }

        [Fact]
        public void BorrowBook_ThrowsBusinessRuleException_WhenBookIsNotAvailable()
        {
            _bookRecord.ReturnBy = _dateTimeNow.AddMonths(_libraryService.MaximumBorrowMonths);

            _libraryService.AddBook(_book, 1);
            Guid libraryCardId = _libraryService.RegisterLibraryMember(_libraryCard);
            _bookRecord.LibraryCardId = libraryCardId;
            _libraryService.BorrowBook(_bookRecord);
            void act() => _libraryService.BorrowBook(_bookRecord);

            BusinessRuleException exception = Assert.Throws<BusinessRuleException>(act);
            Assert.Equal("Book to be borrowed is not available at the moment", exception.Message);
        }

        [Fact]
        public void ReturnBook_ThrowsBookRecordNotFoundException_WhenRecordWithLibraryCardIdDoesNotExist()
        {
            Guid libraryCardId = Guid.Empty;

            libraryCardId = _libraryService.RegisterLibraryMember(_libraryCard);
            void act() => _libraryService.ReturnBook("_isbn1", libraryCardId);

            BookRecordNotFoundException exception = Assert.Throws<BookRecordNotFoundException>(act);
            Assert.Equal("No record of book being borrowed was found", exception.Message);
        }

        [Fact]
        public void ReturnBook_ThrowsBookNotFoundException_WhenBookWithGivenISBNDoesNotExist()
        {
            Guid libraryCardId = Guid.Empty;
            _bookRecord.ReturnBy = _dateTimeNow.AddMonths(_libraryService.MaximumBorrowMonths);

            _libraryService.AddBook(_book, 2);
            libraryCardId = _libraryService.RegisterLibraryMember(_libraryCard);
            _bookRecord.LibraryCardId = libraryCardId;
            _libraryService.BorrowBook(_bookRecord);
            _libraryService.DeleteBook(_book.ISBN);
            void act() => _libraryService.ReturnBook(_book.ISBN, libraryCardId);

            BookNotFoundException exception = Assert.Throws<BookNotFoundException>(act);
            Assert.Equal("Book to be returned was removed from the library. Consider adding it", exception.Message);
        }

        [Fact]
        public void ReturnBook_ThrowsBusinessRuleException_WhenReturnOfBookIsOverdue()
        {
            LibraryService libraryService = new();
            Guid libraryCardId = Guid.Empty;
            _bookRecord.ReturnBy = _dateTimeNow.AddDays(1);
            int overdueDays = 5;

            libraryService.AddBook(_book, 2);
            libraryCardId = libraryService.RegisterLibraryMember(_libraryCard);
            _bookRecord.LibraryCardId = libraryCardId;
            libraryService.BorrowBook(_bookRecord);
            _bookRecord = libraryService.LibraryMembers[libraryCardId].Records[0];
            _bookRecord.ReturnBy = _bookRecord.ReturnBy.AddDays(-overdueDays - 1);
            void act() => libraryService.ReturnBook(this._book.ISBN, libraryCardId);

            BusinessRuleException exception = Assert.Throws<BusinessRuleException>(act);
            Assert.Equal($"Book's return is {overdueDays} day(-s) overdue past the period", exception.Message);
        }
    }
}
