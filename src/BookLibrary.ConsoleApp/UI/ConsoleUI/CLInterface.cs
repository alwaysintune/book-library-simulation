using BookLibrary.ConsoleApp.Entities;
using BookLibrary.ConsoleApp.Extensions;
using BookLibrary.ConsoleApp.Services.JsonBuilder;
using BookLibrary.ConsoleApp.Services.Library;
using System;

namespace BookLibrary.ConsoleApp.UI.ConsoleUI
{
    public class CLInterface : ICLInterface
    {
        private ILibraryService _libraryService;
        private readonly IJsonService _jsonService;
        private readonly string[] _actionArray;

        public CLInterface()
        {
            _libraryService = new LibraryService();
            _jsonService = new JsonService();

            _actionArray = new string[] {
                nameof(AddBook).SplitCamelCase(),
                nameof(BorrowBook).SplitCamelCase(),
                nameof(ReturnBook).SplitCamelCase(),
                nameof(DeleteBook).SplitCamelCase(),
            };
        }

        public void Start()
        {
            Console.WriteLine("Welcome to the Library!");

            int inputCursorLeft = 0;
            ConsoleKeyInfo info;
            while ((info = ReadKey()).Key != ConsoleKey.Z)
            {
                int inputCursorTop = Console.CursorTop - 1;
                try
                {
                    switch (info.KeyChar)
                    {
                        case '0':
                            AddBook();
                            break;
                        case '1':
                            BorrowBook();
                            break;
                        case '2':
                            ReturnBook();
                            break;
                        case '3':
                            DeleteBook();
                            break;
                        default:
                            Console.SetCursorPosition(inputCursorLeft, inputCursorTop);
                            Console.Write(' ');
                            Console.SetCursorPosition(inputCursorLeft, inputCursorTop);
                            continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }

                Console.WriteLine("Action Completed..");
            }
        }

        private void AddBook()
        {
            Console.WriteLine("Enter book info:");
            string name = ReadLineWithMessage($"{nameof(Book.Name)}:");
            string author = ReadLineWithMessage($"{nameof(Book.Author)}:");
            string category = ReadLineWithMessage($"{nameof(Book.Category)}:");
            string language = ReadLineWithMessage($"{nameof(Book.Language)}:");

            string input = ReadLineWithMessage($"{nameof(Book.PublicationDate).SplitCamelCase()}:");
            DateTime publicationDateUtc;
            while (!DateTime.TryParse(input, out publicationDateUtc))
            {
                input = ReadLineWithMessage($"Please enter a valid date. For example, {DateTime.Now:yyyy/MM//dd}");
            }

            string isbn = ReadLineWithMessage($"{nameof(Book.ISBN)}:");

            input = ReadLineWithMessage("Book count:");
            int count;
            while (!int.TryParse(input, out count))
            {
                input = ReadLineWithMessage("Please enter a whole number");
            }

            var book = new Book()
            {
                Name = name,
                Author = author,
                Category = category,
                Language = language,
                PublicationDate = publicationDateUtc,
                ISBN = isbn
            };

            _libraryService.AddBook(book, count);
        }

        private void BorrowBook()
        {
            string isbn = ReadLineWithMessage($"{nameof(BookRecord.ISBN)}:");

            string input = ReadLineWithMessage($"{nameof(BookRecord.LibraryCardId).SplitCamelCase()}:");
            Guid libraryCardId;
            while (!Guid.TryParse(input, out libraryCardId))
            {
                input = ReadLineWithMessage($"Please enter a valid ID. For example, {Guid.Empty}");
            }

            input = ReadLineWithMessage($"{nameof(BookRecord.ReturnBy).SplitCamelCase()}:");
            DateTime returnByUtc;
            while (!DateTime.TryParse(input, out returnByUtc))
            {
                input = ReadLineWithMessage($"Please enter a valid date. For example, {DateTime.Now:yyyy/MM/dd}");
            }

            var bookRecord = new BookRecord(isbn, libraryCardId, returnByUtc);
            _libraryService.BorrowBook(bookRecord);
        }

        private void ReturnBook()
        {
            string isbn = ReadLineWithMessage($"{nameof(BookRecord.ISBN)}:");

            string input = ReadLineWithMessage($"{nameof(BookRecord.LibraryCardId).SplitCamelCase()}:");
            Guid libraryCardId;
            while (!Guid.TryParse(input, out libraryCardId))
            {
                input = ReadLineWithMessage($"Please enter a valid ID. For example, {Guid.Empty}");
            }

            _libraryService.ReturnBook(isbn, libraryCardId);
        }

        private void DeleteBook()
        {
            string isbn = ReadLineWithMessage($"{nameof(Book.ISBN)}:");

            _libraryService.DeleteBook(isbn);
        }

        private static string ReadLine()
        {
            string result = Console.ReadLine();
            result = result.Trim();

            return result;
        }

        private static ConsoleKeyInfo ReadKey()
        {
            ConsoleKeyInfo result = Console.ReadKey();

            Console.WriteLine();

            return result;
        }

        private static string ReadLineWithMessage(string msg)
        {
            Console.WriteLine(msg);
            return ReadLine();
        }
    }
}
