using BookLibrary.ConsoleApp.Entities;
using BookLibrary.ConsoleApp.Enums;
using BookLibrary.ConsoleApp.Extensions;
using BookLibrary.ConsoleApp.Services.JsonBuilder;
using BookLibrary.ConsoleApp.Services.Library;
using System;
using System.Collections.Generic;

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
                nameof(RegisterLibraryMember).SplitCamelCase(),
                nameof(ListAllBooks).SplitCamelCase(),
                nameof(TryPrintAvailableActions).SplitCamelCase(),
                "To Quit press Z"
            };
        }

        public void Start()
        {
            Console.WriteLine("Welcome to the Library!");

            TryLoadFromJson();

            if (!TryPrintAvailableActions(_actionArray)) { return; }

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
                        case '4':
                            RegisterLibraryMember();
                            break;
                        case '5':
                            ListAllBooks();
                            break;
                        case '6':
                            TryPrintAvailableActions(_actionArray);
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

                Console.WriteLine("Action Completed.. To print available actions press 6");
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

        private void RegisterLibraryMember()
        {
            string fullName = ReadLineWithMessage($"{nameof(LibraryCard.FullName).SplitCamelCase()}:");
            string street = ReadLineWithMessage($"{nameof(Address.Street)}:");
            string zipcode = ReadLineWithMessage($"{nameof(Address.Zipcode)}:");

            var address = new Address(street, zipcode);
            var libraryCard = new LibraryCard(fullName, address);

            Guid id = _libraryService.RegisterLibraryMember(libraryCard);
            Console.WriteLine($"Your library card ID is:\n{id}");
        }

        private void ListAllBooks()
        {
            List<LibraryBook> libraryBooks;
            ConsoleKeyInfo info = ReadChoiceWithMessage("Filter list? Y/N");
            if (info.Key == ConsoleKey.N)
            {
                libraryBooks = _libraryService.ListAllBooks();

                Console.WriteLine($"Book count: {libraryBooks.Count}");
                foreach (var libraryBook in libraryBooks)
                {
                    Console.WriteLine(libraryBook.Book);
                    Console.WriteLine(libraryBook.Inventory);
                }

                TrySaveJson(libraryBooks);

                return;
            }

            var bookFilter = new BookFilter();

            info = ReadChoiceWithMessage(
                $"Filter list by book {nameof(BookFilter.Name)}? Y/N");
            if (info.Key == ConsoleKey.Y)
            {
                bookFilter.Name = ReadLineWithMessage(
                    $"{nameof(BookFilter.Name)}:");
            }

            info = ReadChoiceWithMessage(
                $"Filter list by book {nameof(BookFilter.Author)}? Y/N");
            if (info.Key == ConsoleKey.Y)
            {
                bookFilter.Author = ReadLineWithMessage(
                    $"{nameof(BookFilter.Author)}:");
            }

            info = ReadChoiceWithMessage(
                $"Filter list by book {nameof(BookFilter.Category)}? Y/N");
            if (info.Key == ConsoleKey.Y)
            {
                bookFilter.Category = ReadLineWithMessage(
                    $"{nameof(BookFilter.Category)}:");
            }

            info = ReadChoiceWithMessage(
                $"Filter list by book {nameof(BookFilter.Language)}? Y/N");
            if (info.Key == ConsoleKey.Y)
            {
                bookFilter.Category = ReadLineWithMessage(
                    $"{nameof(BookFilter.Language)}:");
            }

            info = ReadChoiceWithMessage(
                $"Filter list by book {nameof(BookFilter.ISBN)}? Y/N");
            if (info.Key == ConsoleKey.Y)
            {
                bookFilter.Category = ReadLineWithMessage(
                    $"{nameof(BookFilter.ISBN)}:");
            }

            BookState stateFilter = BookState.Available;
            info = ReadChoiceWithMessage(
                $"Choose one of: list taken or available books? T/A",
                first: ConsoleKey.T,
                second: ConsoleKey.A);
            if (info.Key == ConsoleKey.T)
            {
                stateFilter = BookState.Taken;
            }

            libraryBooks = _libraryService.ListAllBooks(bookFilter, stateFilter);

            Console.WriteLine($"Book count: {libraryBooks.Count}");
            foreach (var libraryBook in libraryBooks)
            {
                Console.WriteLine(libraryBook.Book);
                Console.WriteLine(libraryBook.Inventory);
            }
        }

        private void TryLoadFromJson()
        {
            ConsoleKeyInfo input = ReadChoiceWithMessage("Load library books from JSON? Y/N");

            if (input.Key == ConsoleKey.Y)
            {
                string filePath = ReadLineWithMessage("File Path:");
                var libraryBooks = _jsonService.ReadFromFile<List<LibraryBook>>(filePath);
                _libraryService = new LibraryService(libraryBooks);
            }
        }

        private void TrySaveJson(List<LibraryBook> libraryBooks)
        {
            ConsoleKeyInfo input = ReadChoiceWithMessage("Save library books to JSON? Y/N");

            if (input.Key == ConsoleKey.Y)
            {
                string filePath = ReadLineWithMessage("File Path:");
                _jsonService.WriteToFile(libraryBooks, filePath);
            }
        }

        private static bool TryPrintAvailableActions(string[] actions)
        {
            if (actions == null || actions == Array.Empty<string>())
            {
                Console.WriteLine("No action is available at the moment");
                return false;
            }

            Console.WriteLine("Available actions:");
            for (int i = 0; i < actions.Length; i++)
            {
                Console.WriteLine($"[{i}]: {actions[i]}");
            }

            return true;
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

        private static ConsoleKeyInfo ReadKeyWithMessage(string msg)
        {
            Console.WriteLine(msg);
            return ReadKey();
        }

        private static ConsoleKeyInfo ReadChoiceWithMessage(
            string msg,
            ConsoleKey first = ConsoleKey.Y,
            ConsoleKey second = ConsoleKey.N
            )
        {
            ConsoleKeyInfo info = ReadKeyWithMessage(msg);

            while (info.Key != first && info.Key != second)
            {
                info = ReadKeyWithMessage($"Please enter one of the choices: {first}/{second}");
            }

            return info;
        }
    }
}
