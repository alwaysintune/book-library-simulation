using BookLibrary.ConsoleApp.Entities;
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
    }
}
