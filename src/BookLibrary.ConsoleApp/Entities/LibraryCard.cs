using System;

namespace BookLibrary.ConsoleApp.Entities
{
    public class LibraryCard
    {
        public LibraryCard(string fullName, Address address)
        {
            FullName = fullName;
            Address = address;
        }

        public Guid Id { get; init; } = Guid.NewGuid();

        public string FullName { get; set; }

        public DateTime IssueDate { get; init; } = DateTime.Now;

        public Address Address { get; set; }
    }
}
