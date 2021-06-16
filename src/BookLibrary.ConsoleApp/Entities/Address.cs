namespace BookLibrary.ConsoleApp.Entities
{
    public class Address
    {
        public Address(string street, string zipcode)
        {
            Street = street;
            Zipcode = zipcode;
        }

        public string Street { get; set; }

        public string Zipcode { get; set; }
    }
}
