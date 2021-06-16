using BookLibrary.ConsoleApp.Entities;
using BookLibrary.ConsoleApp.Services.Library;
using System;

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
    }
}
